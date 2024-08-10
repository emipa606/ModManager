using HarmonyLib;
using Steamworks;

namespace ModManager;

[HarmonyPatch(typeof(SteamUGC), nameof(SteamUGC.SubmitItemUpdate))]
internal class SteamUGC_SubmitItemUpdate
{
    private static void Prefix(ref string pchChangeNote)
    {
        if (!pchChangeNote.StartsWith("[Auto-generated text]") || string.IsNullOrEmpty(Workshop.CurrentChangenote))
        {
            return;
        }

        pchChangeNote = Workshop.CurrentChangenote;
    }
}