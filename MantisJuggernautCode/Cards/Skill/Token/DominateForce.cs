using MantisJuggernaut.Characters;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2RitsuLib.Interop.AutoRegistration;

namespace MantisJuggernaut.Cards;

[RegisterCard(typeof(TokenCardPool))]
public sealed class DominateForce()
    : MantisJuggernautSkillCard(BaseEnergyCost, CardRarityValue, CardTarget)
{
    // 基础耗能。
    private const int BaseEnergyCost = 3;

    // 卡牌稀有度。
    private const CardRarity CardRarityValue = CardRarity.Token;

    // 目标类型。
    private const TargetType CardTarget = TargetType.Self;

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => base.AdditionalHoverTips.Concat(
    [
        HoverTipFactory.FromPower<SlipperyPower>()
    ]);

    // 卡牌基础数值。
    protected override IEnumerable<DynamicVar> CanonicalVars => base.CanonicalVars.Concat(
    [
        new PowerVar<SlipperyPower>(1m),
        new CardsVar(6)
    ]);

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Sly,
        CardKeyword.Exhaust
    ];

    // 打出时的效果逻辑。
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await PowerCmd.Apply<SlipperyPower>(
            choiceContext,
            Owner.Creature,
            DynamicVars[nameof(SlipperyPower)].BaseValue,
            Owner.Creature,
            this
        );
        await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.BaseValue, Owner);
    }

    // 升级后的效果逻辑。
    protected override void OnUpgrade()
    {
        DynamicVars.Cards.UpgradeValueBy(3m);
    }
}