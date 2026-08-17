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
public sealed class WindFlow() : MantisJuggernautSkillBackswingCard(BaseEnergyCost, CardRarityValue, CardTarget)
{
    // 基础耗能。
    private const int BaseEnergyCost = 1;

    // 卡牌稀有度。
    private const CardRarity CardRarityValue = CardRarity.Uncommon;

    // 目标类型（Self 表示自己）。
    private const TargetType CardTarget = TargetType.Self;

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => base.AdditionalHoverTips.Concat(
    [
        HoverTipFactory.FromPower<SwiftPower>()
    ]);

    // 卡牌基础数值。
    protected override IEnumerable<DynamicVar> CanonicalVars => base.CanonicalVars.Concat(
    [
        new CardsVar(1),
        new PowerVar<SwiftPower>(2m)
    ]);

    // 打出时的效果逻辑。
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.BaseValue, Owner);
        await PowerCmd.Apply<SwiftPower>(
            choiceContext,
            Owner.Creature,
            DynamicVars[nameof(SwiftPower)].BaseValue,
            Owner.Creature,
            this
        );
        await base.OnPlay(choiceContext, cardPlay);
    }

    // 升级后的效果逻辑。
    protected override void OnUpgrade()
    {
        DynamicVars[nameof(SwiftPower)].UpgradeValueBy(2m);
    }
}