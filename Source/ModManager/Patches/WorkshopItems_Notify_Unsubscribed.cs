using HarmonyLib;
using RimWorld;
using Steamworks;
using Verse;
using Verse.Steam;

namespace ModManager;

[HarmonyPatch(typeof(WorkshopItems), "Notify_Unsubscribed")]
public class WorkshopItems_Notify_Unsubscribed
{
    public static bool Prefix(PublishedFileId_t pfid)
    {
        Debug.Log("Notify_Unsubscribed");

        // deregister item in WorkshopItems
        var item = Private_Fields.workshopitems.FirstOrDefault(i => i.PublishedFileId == pfid);
        Private_Fields.workshopitems.TryRemove(item);

        // deregister item in ModLister
        var mod = Private_Fields.modlister.FirstOrDefault(m => m.Source == ContentSource.SteamWorkshop &&
                                                               m.PackageId == pfid.ToString());
        Private_Fields.modlister.TryRemove(mod);

        // remove button
        ModButtonManager.Notify_Unsubscribed(pfid.ToString());

        ScenarioLister.MarkDirty();
        return false;
    }
}