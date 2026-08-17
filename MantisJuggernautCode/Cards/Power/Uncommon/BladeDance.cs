using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MantisJuggernaut.Characters;
using MantisJuggernaut.HoverTips;
using STS2RitsuLib.Interop.AutoRegistration;
using MantisJuggernaut.Powers;
using MegaCrit.Sts2.Core.Models.Powers;

namespace MantisJuggernaut.Cards;

[RegisterCard(typeof(MantisJuggernautCardPool))]
public sealed class BladeDance() : MantisJuggernautPowerCard(BaseEnergyCost, CardRarityValue, CardTarget)
{
    // 基础耗能。
    private const int BaseEnergyCost = 1;

    // 卡牌稀有度。
    private const CardRarity CardRarityValue = CardRarity.Uncommon;

    // 目标类型（Self 表示自己）。
    private const TargetType CardTarget = TargetType.Self;

    protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
    [
        InstinctSlash.MakeCardHoverTip()
    ];

    // 卡牌基础数值。
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<BladeDancePower>(1m)
    ];

    // 打出时的效果逻辑。
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "PowerUp", Owner.Character.PowerUpAnimDelay);
        BladeDancePower? power = await PowerCmd.Apply<BladeDancePower>(
            choiceContext,
            Owner.Creature,
            DynamicVars[nameof(BladeDancePower)].BaseValue,
            Owner.Creature,
            this
        );
        if (power is not null)
        {
            power.IsUpgraded = IsUpgraded;
        }
    }

    // 升级后的效果逻辑。
    protected override void OnUpgrade()
    {
    }
}