using MantisJuggernaut.Characters;
using MantisJuggernaut.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;

namespace MantisJuggernaut.Cards;

[RegisterCard(typeof(MantisJuggernautCardPool))]
public sealed class Gnaw()
    : MantisJuggernautAttackCard(BaseEnergyCost, CardRarityValue, CardTarget)
{
    // 基础耗能。
    private const int BaseEnergyCost = 1;

    // 卡牌稀有度。
    private const CardRarity CardRarityValue = CardRarity.Uncommon;

    // 目标类型。
    private const TargetType CardTarget = TargetType.AnyEnemy;

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => base.AdditionalHoverTips.Concat(
    [
        HoverTipFactory.FromPower<InstinctPower>(),
        HoverTipFactory.FromPower<HuntMarkPower>()
    ]);

    // 卡牌基础数值。
    protected override IEnumerable<DynamicVar> CanonicalVars => base.CanonicalVars.Concat(
    [
        new DamageVar(6m, ValueProp.Move),
        new PowerVar<InstinctPower>(2),
        new PowerVar<HuntMarkPower>(4)
    ]);

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Exhaust
    ];

    // 打出时的效果逻辑。
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this, cardPlay)
            .Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_bite")
            .Execute(choiceContext);

        await PowerCmd.Apply<InstinctPower>(
            choiceContext,
            Owner.Creature,
            DynamicVars[nameof(InstinctPower)].BaseValue,
            Owner.Creature,
            this
        );

        await PowerCmd.Apply<HuntMarkPower>(
            choiceContext,
            cardPlay.Target,
            DynamicVars[nameof(HuntMarkPower)].BaseValue,
            Owner.Creature,
            this
        );
    }

    // 升级后的效果逻辑。
    protected override void OnUpgrade()
    {
        DynamicVars[nameof(InstinctPower)].UpgradeValueBy(1m);
        DynamicVars[nameof(HuntMarkPower)].UpgradeValueBy(2m);
    }
}