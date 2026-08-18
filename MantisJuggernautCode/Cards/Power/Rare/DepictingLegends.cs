using MantisJuggernaut.Characters;
using MantisJuggernaut.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using STS2RitsuLib.Interop.AutoRegistration;

namespace MantisJuggernaut.Cards;

[RegisterCard(typeof(MantisJuggernautCardPool))]
public sealed class DepictingLegends() : MantisJuggernautPowerCard(BaseEnergyCost, CardRarityValue, CardTarget)
{
    // 基础耗能。
    private const int BaseEnergyCost = 3;

    // 卡牌稀有度。
    private const CardRarity CardRarityValue = CardRarity.Rare;

    // 目标类型（Self 表示自己）。
    private const TargetType CardTarget = TargetType.Self;

    protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
    [
        HoverTipFactory.FromCard<Omnislash>(IsUpgraded),
        HoverTipFactory.FromCard<Guillotine>(IsUpgraded),
        HoverTipFactory.FromCard<TurboWhirl>(IsUpgraded),
        HoverTipFactory.FromCard<DominateForce>(IsUpgraded),
        HoverTipFactory.FromCard<MazeStep>(IsUpgraded)
    ];

    // 卡牌基础数值。
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<DepictingLegendsPower>(1m)
    ];

    // 打出时的效果逻辑。
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "PowerUp", Owner.Character.PowerUpAnimDelay);
        var power = await PowerCmd.Apply<DepictingLegendsPower>(
            choiceContext,
            Owner.Creature,
            DynamicVars[nameof(DepictingLegendsPower)].BaseValue,
            Owner.Creature,
            this
        );
        if (power is not null) power.IsUpgraded = IsUpgraded;
    }

    // 升级后的效果逻辑。
    protected override void OnUpgrade()
    {
    }
}