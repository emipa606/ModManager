// Patch_WorkshopItems_NotifySubscribed.cs
// Copyright Karel Kroeze, 2018-2018

using System.Collections.Generic;
using HarmonyLib;
using Verse;
using Verse.Steam;

namespace ModManager;

public class Private_Fields
{
    /**
     * RimWorld rebuilds the entire mod list whenever an item is installed, uninstalled or even subscribed to.
     * 
     * This is patently rediculous, as we know exactly what items were manipulated.
     */

    public static List<ModMetaData> modlister => Traverse.CreateWithType("ModLister")
        .Field("mods")
        .GetValue<List<ModMetaData>>();

    public static List<WorkshopItem> workshopitems => Traverse.CreateWithType("WorkshopItems")
        .Field("subbedItems")
        .GetValue<List<WorkshopItem>>();
}