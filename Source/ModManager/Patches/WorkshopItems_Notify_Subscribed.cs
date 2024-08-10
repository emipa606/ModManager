using HarmonyLib;
using RimWorld;
using Steamworks;
using Verse;
using Verse.Steam;

namespace ModManager;

[HarmonyPatch(typeof(WorkshopItems), "Notify_Subscribed")]
public class WorkshopItems_Notify_Subscribed
{
    public static bool Prefix(PublishedFileId_t pfid)
    {
        Debug.Log("Notify_Subscribed");

        // check if item was already present.
        var item = WorkshopItem.MakeFrom(pfid);

        if (item is WorkshopItem_Mod item_installed)
        {
            // register item in WorkshopItems
            Private_Fields.workshopitems.Add(item_installed);

            // register item in ModLister
            var mod = new ModMetaData(item_installed);
            Private_Fields.modlister.Add(mod);

            // show a message
            Messages.Message(I18n.ModInstalled(mod.Name), MessageTypeDefOf.PositiveEvent, false);

            // notify button manager that we're done stuff.
            ModButtonManager.Notify_DownloadCompleted(mod);
        }
        else
        {
            // add dowloading item to MBM
            var button = new ModButton_Downloading(pfid);
            ModButtonManager.TryAdd(button);
            Page_BetterModConfig.Instance.Selected = button;
        }

        // do whatever needs doing for ScenarioLister.
        ScenarioLister.MarkDirty();
        return false;
    }
}