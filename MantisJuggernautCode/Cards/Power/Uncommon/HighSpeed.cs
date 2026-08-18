using MantisJuggernaut.Characters;
using MantisJuggernaut.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2RitsuLib.Interop.AutoRegistration;

namespace MantisJuggernaut.Cards;

[RegisterCard(typeof(MantisJuggernautCardPool))]
public sealed class HighSpeed() : MantisJuggernautPowerCard(BaseEnergyCost, CardRarityValue, CardTarget)
{
    // 基础耗能。
    private const int BaseEnergyCost = 1;

    // 卡牌稀有度。
    private const CardRarity CardRarityValue = CardRarity.Uncommon;

    // 目标类型（Self 表示自己）。
    private const TargetType CardTarget = TargetType.Self;

    protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
    [
        HoverTipFactory.FromPower<DexterityPower>(),
        HoverTipFactory.FromPower<InstinctPower>()
    ];

    // 卡牌基础数值。
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<DexterityPower>(1m),
        new PowerVar<HighSpeedPower>(1m)
    ];

    // 打出时的效果逻辑。
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "PowerUp", Owner.Character.PowerUpAnimDelay);
        await PowerCmd.Apply<DexterityPower>(
            choiceContext,
            Owner.Creature,
            DynamicVars[nameof(DexterityPower)].BaseValue,
            Owner.Creature,
            this
        );
        await PowerCmd.Apply<HighSpeedPower>(
            choiceContext,
            Owner.Creature,
            DynamicVars[nameof(HighSpeedPower)].BaseValue,
            Owner.Creature,
            this
        );
    }

    // 升级后的效果逻辑。
    protected override void OnUpgrade()
    {
        DynamicVars[nameof(DexterityPower)].UpgradeValueBy(1m);
    }
}