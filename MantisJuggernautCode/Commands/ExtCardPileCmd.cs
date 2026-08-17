using System.Diagnostics;
using MegaCrit.Sts2.Core.Audio.Debug;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards.Holders;
using MegaCrit.Sts2.Core.Nodes.Combat;

namespace MantisJuggernaut.Commands;

public static class ExtCardPileCmd
{
    public static async Task<CardModel?> Draw(
        PlayerChoiceContext choiceContext,
        Player player,
        CardPilePosition position = CardPilePosition.Bottom
    )
    {
        return (await Draw(choiceContext, 1m, player, position)).FirstOrDefault();
    }

    public static async Task<CardModel?> LeftDraw(
        PlayerChoiceContext choiceContext,
        Player player
    )
    {
        return await Draw(choiceContext, player, CardPilePosition.Top);
    }

    public static Task<IEnumerable<CardModel>> Draw(
        PlayerChoiceContext choiceContext,
        decimal count,
        Player player,
        CardPilePosition position = CardPilePosition.Bottom,
        bool fromHandDraw = false
    )
    {
        return DrawInternal(choiceContext, count, player, position, fromHandDraw);
    }

    public static Task<IEnumerable<CardModel>> LeftDraw(
        PlayerChoiceContext choiceContext,
        decimal count,
        Player player,
        bool fromHandDraw = false
    )
    {
        return Draw(choiceContext, count, player, CardPilePosition.Top, fromHandDraw);
    }

    private static async Task<IEnumerable<CardModel>> DrawInternal(
        PlayerChoiceContext choiceContext,
        decimal count,
        Player player,
        CardPilePosition position = CardPilePosition.Bottom,
        bool fromHandDraw = false
    )
    {
        if (CombatManager.Instance.IsOverOrEnding) return Array.Empty<CardModel>();

        if (player.Creature.CombatState != null &&
            !Hook.ShouldDraw(player.Creature.CombatState, player, fromHandDraw, out var modifier))
        {
            if (modifier != null) await Hook.AfterPreventingDraw(player.Creature.CombatState, modifier);
            return Array.Empty<CardModel>();
        }

        var combatState = player.Creature.CombatState;
        var result = new List<CardModel>();
        var hand = PileType.Hand.GetPile(player);
        var drawPile = PileType.Draw.GetPile(player);
        var drawsRequested = count > 0m ? (int)Math.Ceiling(count) : 0;
        if (drawsRequested == 0) return result;

        var num = Math.Max(0, CardPile.MaxCardsInHand - hand.Cards.Count);
        if (num == 0)
        {
            CheckIfDrawIsPossibleAndShowThoughtBubbleIfNot(player);
            return result;
        }

        for (var i = 0; i < drawsRequested; i++)
        {
            if (num <= 0) break;

            if (CombatManager.Instance.IsOverOrEnding) break;

            if (!CheckIfDrawIsPossibleAndShowThoughtBubbleIfNot(player)) break;

            await CardPileCmd.ShuffleIfNecessary(choiceContext, player);
            if (!CheckIfDrawIsPossibleAndShowThoughtBubbleIfNot(player)) break;

            var card = drawPile.Cards.FirstOrDefault();
            if (card == null || hand.Cards.Count >= CardPile.MaxCardsInHand) break;

            result.Add(card);
            await CardPileCmd.Add(card, hand, position);
            NPlayerHand? nPlayerHand = NPlayerHand.Instance;
            if (nPlayerHand?.GetCardHolder(card) is NHandCardHolder holder)
            {
                nPlayerHand.CardHolderContainer.MoveChildSafely(holder, hand.Cards.IndexOf(card));
                holder.SetDefaultTargets();
                nPlayerHand.ForceRefreshCardIndices();
            }
            
            if (combatState != null)
            {
                CombatManager.Instance.History.CardDrawn(combatState, card, fromHandDraw);
                await Hook.AfterCardDrawn(combatState, choiceContext, card, fromHandDraw);
            }

            card.InvokeDrawn();
            NDebugAudioManager.Instance?.Play("card_deal.mp3", 0.25f, PitchVariance.Small);
            num = Math.Max(0, CardPile.MaxCardsInHand - hand.Cards.Count);
        }

        return result;
    }

    private static bool CheckIfDrawIsPossibleAndShowThoughtBubbleIfNot(Player player)
    {
        if (PileType.Draw.GetPile(player).Cards.Count + PileType.Discard.GetPile(player).Cards.Count == 0)
        {
            ThinkCmd.Play(new LocString("combat_messages", "NO_DRAW"), player.Creature, 2.0);
            return false;
        }

        if (PileType.Hand.GetPile(player).Cards.Count >= CardPile.MaxCardsInHand)
        {
            ThinkCmd.Play(new LocString("combat_messages", "HAND_FULL"), player.Creature, 2.0);
            return false;
        }

        return true;
    }
}