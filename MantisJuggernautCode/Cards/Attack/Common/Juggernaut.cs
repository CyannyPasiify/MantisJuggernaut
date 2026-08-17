using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using MantisJuggernaut.Characters;
using MantisJuggernaut.HoverTips;

namespace MantisJuggernaut.Cards;

[RegisterCard(typeof(MantisJuggernautCardPool))]
public sealed class Juggernaut()
    : MantisJuggernautAttackCard(BaseEnergyCost, CardRarityValue, CardTarget)
{
    // 基础耗能。
    private const int BaseEnergyCost = 2;

    // 卡牌稀有度。
    private const CardRarity CardRarityValue = CardRarity.Common;

    // 目标类型。
    private const TargetType CardTarget = TargetType.AllEnemies;

    public override bool HasTurnEndInHandEffect => true;

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => base.AdditionalHoverTips.Concat(
    [
        ExtHoverTipFactory.Static(ExtStaticHoverTip.Prepared)
    ]);

    public const string DamageIncreaseVarKey = "DamageIncrease";

    // 卡牌基础数值。
    protected override IEnumerable<DynamicVar> CanonicalVars => base.CanonicalVars.Concat(
    [
        new DamageVar(10m, ValueProp.Move),
        new DynamicVar(DamageIncreaseVarKey, 6)
    ]);

    private decimal _extraDamge;

    private decimal ExtraDamage
    {
        get => _extraDamge;
        set
        {
            AssertMutable();
            _extraDamge = value;
        }
    }

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Retain
    ];

    protected override bool ShouldGlowGoldInternal => IsPrepared;

    // 打出时的效果逻辑。
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (CombatState is null)
        {
            return;
        }

        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this, cardPlay)
            .TargetingAllOpponents(CombatState)
            .WithHitFx("vfx/vfx_heavy_blunt", null, "heavy_attack.mp3")
            .Execute(choiceContext);
    }

    protected override void AfterDowngraded()
    {
        base.AfterDowngraded();
        DynamicVars.Damage.BaseValue += ExtraDamage;
    }

    // 升级后的效果逻辑。
    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(4m);
        DynamicVars[DamageIncreaseVarKey].UpgradeValueBy(2m);
    }

    protected override Task OnTurnEndInHand(PlayerChoiceContext choiceContext)
    {
        if (IsPrepared)
        {
            DynamicVars.Damage.BaseValue += DynamicVars[DamageIncreaseVarKey].BaseValue;
            ExtraDamage += DynamicVars[DamageIncreaseVarKey].BaseValue;
        }

        return Task.CompletedTask;
    }
}