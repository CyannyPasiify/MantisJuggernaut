using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Nodes.Cards.Holders;
using MegaCrit.Sts2.Core.Nodes.Combat;
using HarmonyLib;

namespace MantisJuggernaut.Patch.Core.Nodes.Combat;


[HarmonyPatch(typeof(NPlayerHand), nameof(NPlayerHand.AfterCardsSelected))]
public static class NPlayerHandAfterCardsSelectedPrefix
{
    static void Prefix(NPlayerHand __instance)
    {
        Log.Info($"[{Entry.ModId}] AfterCardsSelected");
        // 在 _selectedCards.Clear() 之前执行
        foreach (var card in __instance._selectedCards)
        {
            Log.Info($"[{Entry.ModId}] AfterCardsSelected: {card}");
            var holder = (NHandCardHolder?)__instance.GetCardHolder(card);
            if (holder is not null)
            {
                holder.Reparent(__instance.CardHolderContainer);
                if (holder.CardNode?.Model is not null)
                {
                    int handInsertIndex = __instance.GetHandInsertIndex(holder.CardNode.Model);
                    if (handInsertIndex >= 0)
                        __instance.CardHolderContainer.MoveChildSafely(holder, handInsertIndex);
                }

                holder.SetDefaultTargets();
            }
        }
    }
}