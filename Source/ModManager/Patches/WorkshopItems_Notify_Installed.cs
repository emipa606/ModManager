using HarmonyLib;
using RimWorld;
using Steamworks;
using Verse;
using Verse.Steam;

namespace ModManager;

[HarmonyPatch(typeof(WorkshopItems), "Notify_Installed")]
public class WorkshopItems_Notify_Installed
{
    public static bool Prefix(PublishedFileId_t pfid)
    {
        Debug.Log("Notify_Installed");

        // register item in WorkshopItems
        var item = WorkshopItem.MakeFrom(pfid);
        Private_Fields.workshopitems.Add(item);

        // register item in ModLister
        var mod = new ModMetaData(item);
        Private_Fields.modlister.Add(mod);

        // show a message
        Messages.Message(I18n.ModInstalled(mod.Name), MessageTypeDefOf.PositiveEvent, false);

        // notify button manager that we're done stuff.
        ModButtonManager.Notify_DownloadCompleted(mod);

        ScenarioLister.MarkDirty();
        return false;
    }
}