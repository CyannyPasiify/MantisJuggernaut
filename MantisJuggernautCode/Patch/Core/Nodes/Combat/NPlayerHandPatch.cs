using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Nodes.Cards.Holders;
using MegaCrit.Sts2.Core.Nodes.Combat;
using HarmonyLib;

namespace MantisJuggernaut.Patch.Core.Nodes.Combat;

[HarmonyPatch(typeof(NPlayerHand), nameof(NPlayerHand.OnSelectModeSourceFinished))]
public static class NPlayerHandOnSelectModeSourceFinishedPostfix
{
    static void Postfix(NPlayerHand __instance)
    {
        Log.Info($"[{Entry.ModId}] OnSelectModeSourceFinished");
        foreach (var holder in __instance.Holders)
        {
            if (holder.CardNode?.Model is null) continue;
            int handInsertIndex = __instance.GetHandInsertIndex(holder.CardNode.Model);
            Log.Info($"[{Entry.ModId}] handInsertIndex = {handInsertIndex}");
            if (handInsertIndex >= 0)
            {
                __instance.CardHolderContainer.MoveChildSafely(holder, handInsertIndex);
            }

            holder.SetDefaultTargets();
        }

        __instance.RefreshLayout();
        // foreach (var card in __instance._selectedCards)
        // {
        //     Log.Info($"[{Entry.ModId}] AfterCardsSelected: {card}");
        //     var holder = (NHandCardHolder?)__instance.GetCardHolder(card);
        //     if (holder is not null)
        //     {
        //         holder.Reparent(__instance.CardHolderContainer);
        //         if (holder.CardNode?.Model is not null)
        //         {
        //             int handInsertIndex = __instance.GetHandInsertIndex(holder.CardNode.Model);
        //             if (handInsertIndex >= 0)
        //                 __instance.CardHolderContainer.MoveChildSafely(holder, handInsertIndex);
        //         }
        //
        //         holder.SetDefaultTargets();
        //     }
        // }
    }
}