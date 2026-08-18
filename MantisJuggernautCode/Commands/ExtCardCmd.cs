using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards.Holders;
using MegaCrit.Sts2.Core.Nodes.Combat;

namespace MantisJuggernaut.Commands;

public static class ExtCardCmd
{
    public static async Task DiscardAndDraw(
        PlayerChoiceContext choiceContext,
        IEnumerable<CardModel> cardsToDiscard,
        int cardsToDraw,
        CardPilePosition position = CardPilePosition.Bottom
    )
    {
        if (!CombatManager.Instance.IsOverOrEnding)
        {
            var discardCards = cardsToDiscard.ToList();
            if (discardCards.Count > 0)
            {
                var combatState = discardCards[0].CombatState ?? discardCards[0].Owner.Creature.CombatState;
                var slyCards = new List<CardModel>();
                var discardPile = PileType.Discard.GetPile(discardCards[0].Owner);
                foreach (var card in discardCards)
                {
                    if (card.IsSlyThisTurn)
                        slyCards.Add(card);
                    await CardPileCmd.Add(card, discardPile);
                    if (combatState != null)
                    {
                        CombatManager.Instance.History.CardDiscarded(combatState, card);
                        await Hook.AfterCardDiscarded(combatState, choiceContext, card);
                    }
                }

                discardPile.InvokeContentsChanged();
                if (cardsToDraw > 0)
                    await ExtCardPileCmd.Draw(choiceContext, cardsToDraw, discardCards[0].Owner, position);

                foreach (var card in slyCards)
                    await CardCmd.AutoPlay(choiceContext, card, null, AutoPlayType.SlyDiscard);
            }
        }
    }

    public static async Task DiscardAndLeftDraw(
        PlayerChoiceContext choiceContext,
        IEnumerable<CardModel> cardsToDiscard,
        int cardsToDraw
    )
    {
        await DiscardAndDraw(choiceContext, cardsToDiscard, cardsToDraw, CardPilePosition.Top);
    }

    public static async Task Prepare(CardModel card, bool toRight = false)
    {
        await CardPileCmd.Add(card, PileType.Hand, toRight ? CardPilePosition.Bottom : CardPilePosition.Top);
        var nPlayerHand = NPlayerHand.Instance;
        if (nPlayerHand?.GetCardHolder(card) is NHandCardHolder holder)
        {
            nPlayerHand.CardHolderContainer.MoveChildSafely(holder, toRight ? -1 : 0);
            holder.SetDefaultTargets();
            nPlayerHand.ForceRefreshCardIndices();
        }
    }

    public static async Task Prepare(IEnumerable<CardModel> cards, bool toRight = false)
    {
        List<CardModel> procCards = cards.ToList();
        if (!toRight)
        {
            procCards.Reverse();
        }

        var nPlayerHand = NPlayerHand.Instance;
        foreach (var card in procCards)
        {
            await CardPileCmd.Add(card, PileType.Hand, toRight ? CardPilePosition.Bottom : CardPilePosition.Top);
            if (nPlayerHand?.GetCardHolder(card) is NHandCardHolder holder)
            {
                nPlayerHand.CardHolderContainer.MoveChildSafely(holder, toRight ? -1 : 0);
                holder.SetDefaultTargets();
            }
        }

        nPlayerHand?.ForceRefreshCardIndices();
    }
}