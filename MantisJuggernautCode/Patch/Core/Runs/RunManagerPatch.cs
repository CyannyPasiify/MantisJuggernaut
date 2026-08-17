using HarmonyLib;
using MantisJuggernaut.Characters;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Platform;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.TestSupport;

namespace MantisJuggernaut.Patch.Core.Runs;

[HarmonyPatch(typeof(RunManager), nameof(RunManager.UpdateRichPresence))]
public static class RunManagerUpdateRichPresencePatch
{
    static void Postfix(RunManager __instance)
    {
        if (!TestMode.IsOn && __instance.State != null)
        {
            if (LocalContext.GetMe(__instance.State)?.Character is MantisJuggernautCharacter zoltan)
            {
                PlatformUtil.SetRichPresenceValue(
                    "Character",
                    StringHelper.Slugify(zoltan.PlaceholderCharacterId ?? "")
                );
                PlatformUtil.SetRichPresenceValue(
                    "Ascension",
                    $"{__instance.State.AscensionLevel.ToString()}（曼提斯武装猎手）"
                );
            }
        }
    }
}