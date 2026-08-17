using MantisJuggernaut.Characters;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;

namespace MantisJuggernaut.Cards;

[RegisterCard(typeof(MantisJuggernautCardPool))]
public sealed class Piercing()
    : MantisJuggernautAttackCard(BaseEnergyCost, CardRarityValue, CardTarget)
{
    // 基础耗能。
    private const int BaseEnergyCost = 2;

    // 卡牌稀有度。
    private const CardRarity CardRarityValue = CardRarity.Uncommon;

    // 目标类型。
    private const TargetType CardTarget = TargetType.AnyEnemy;

    public const string DamageIncreaseVarKey = "DamageIncrease";

    private decimal _extraDamge;

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => base.AdditionalHoverTips.Concat(
    [
        HoverTipFactory.FromPower<VulnerablePower>()
    ]);

    // 卡牌基础数值。
    protected override IEnumerable<DynamicVar> CanonicalVars => base.CanonicalVars.Concat(
    [
        new DamageVar(10m, ValueProp.Move),
        new PowerVar<VulnerablePower>(2m),
        new DynamicVar(DamageIncreaseVarKey, 6m)
    ]);

    private decimal ExtraDamage
    {
        get => _extraDamge;
        set
        {
            AssertMutable();
            _extraDamge = value;
        }
    }

    // 打出时的效果逻辑。
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this, cardPlay)
            .Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_giant_horizontal_slash", null, "slash_attack.mp3")
            .Execute(choiceContext);

        await PowerCmd.Apply<VulnerablePower>(
            choiceContext,
            Owner.Creature,
            DynamicVars.Vulnerable.BaseValue,
            Owner.Creature,
            this
        );
    }

    public override Task AfterCardChangedPiles(CardModel card, PileType oldPileType, AbstractModel? source)
    {
        if (card != this || card.Pile is null) return Task.CompletedTask;

        if (card.Pile.Type == PileType.Discard)
        {
            DynamicVars.Damage.BaseValue += DynamicVars[DamageIncreaseVarKey].BaseValue;
            ExtraDamage += DynamicVars[DamageIncreaseVarKey].BaseValue;
        }

        return Task.CompletedTask;
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
}