using MantisJuggernaut.Characters;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Interop.AutoRegistration;

namespace MantisJuggernaut.Cards;

[RegisterCard(typeof(MantisJuggernautCardPool))]
public sealed class SituationReversed() : MantisJuggernautSkillCard(BaseEnergyCost, CardRarityValue, CardTarget)
{
    // 基础耗能。
    private const int BaseEnergyCost = 0;

    // 卡牌稀有度。
    private const CardRarity CardRarityValue = CardRarity.Common;

    // 目标类型（Self 表示自己）。
    private const TargetType CardTarget = TargetType.Self;

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => base.AdditionalHoverTips.Concat(
    [
    ]);

    // 卡牌基础数值。
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(1)
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
    ];

    // 打出时的效果逻辑。
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        var disCard = (await CardSelectCmd.FromHandForDiscard(
            choiceContext,
            Owner,
            new CardSelectorPrefs(CardSelectorPrefs.DiscardSelectionPrompt, 1),
            null,
            this
        )).FirstOrDefault();
        if (disCard is null) return;

        await CardCmd.Discard(choiceContext, disCard);
        var ctype = disCard.Type;

        var drawCards = PileType.Draw.GetPile(Owner).Cards.Where(c => c.Type == ctype).ToList();
        if (drawCards.Count == 0) return;

        var nonXCards = drawCards.Where(e => !e.EnergyCost.CostsX).ToList();
        var xCards = drawCards.Where(e => e.EnergyCost.CostsX).ToList();
        List<CardModel> cardsToDraw = [];
        for (var i = 0; i < DynamicVars.Cards.IntValue; i++)
        {
            var card = nonXCards.MaxBy(e => e.EnergyCost.GetResolved());
            if (card != null)
            {
                cardsToDraw.Add(card);
                nonXCards.Remove(card);
                continue;
            }

            card = xCards.FirstOrDefault();
            if (card != null)
            {
                cardsToDraw.Add(card);
                xCards.Remove(card);
                continue;
            }

            break;
        }

        await CardPileCmd.Add(cardsToDraw, PileType.Hand);
    }

    // 升级后的效果逻辑。
    protected override void OnUpgrade()
    {
        DynamicVars.Cards.UpgradeValueBy(1m);
    }
}