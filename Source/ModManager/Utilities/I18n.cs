// I18n.cs
// Copyright Karel Kroeze, 2018-2018

using System;
using System.Collections.Generic;
using RimWorld;
using Verse;
using static ModManager.Utilities;
using Version = System.Version;

namespace ModManager;

public static class I18n
{
    private const string PREFIX = "Fluffy.ModManager";

    public static readonly string YesAsTranslation = Key("YesAsTranslation").Translate();
    public static readonly string AvailableMods = Key("AvailableMods").Translate();
    public static readonly string ActiveMods = Key("ActiveMods").Translate();
    public static readonly string NoModSelected = Key("NoModSelected").Translate();

    public static readonly string Preview = Key("Preview").Translate();
    public static readonly string Title = "Title".Translate(); // core
    public static readonly string Author = "Author".Translate(); // core

    public static string Unknown = Key("Unknown").Translate();
    public static readonly string ModsChanged = "ModsChanged".Translate();
    public static readonly string Later = Key("Later").Translate();
    public static readonly string Dependencies = Key("Dependencies").Translate();
    public static readonly string Details = Key("Details").Translate();

    public static readonly string OK = "OK".Translate(); // core
    public static readonly string Yes = "Yes".Translate(); // core
    public static readonly string No = "No".Translate(); // core
    public static readonly string Cancel = Key("Cancel").Translate(); // you'd think this was in core...

    public static readonly string CurrentVersion = Key("CurrentVersion").Translate();

    public static string CoreNotFirst = Key("CoreNotFirst").Translate();

    public static readonly string GetMoreMods_SteamWorkshop = Key("GetMoreMods_SteamWorkshop").Translate();
    public static readonly string GetMoreMods_LudeonForums = Key("GetMoreMods_LudeonForums").Translate();
    public static readonly string UnSubscribe = Key("SteamWorkshop.UnSubscribe").Translate();
    public static readonly string ConfirmSteamWorkshopUpload = "ConfirmSteamWorkshopUpload".Translate(); // core
    public static readonly string ConfirmContentAuthor = "ConfirmContentAuthor".Translate(); // core
    public static string RebuildingModList_Key = Key("RebuildingModList");
    [Obsolete] public static string ExportModList = Key("ExportModList").Translate();
    [Obsolete] public static string ImportModListFromClipboard = Key("ImportModList").Translate();
    [Obsolete] public static string LoadModList = Key("LoadModList").Translate();
    [Obsolete] public static string AddModList = Key("AddModList").Translate();
    [Obsolete] public static string SaveModList = Key("SaveModList").Translate();
    public static readonly string DeleteModList = Key("DeleteModList").Translate();
    public static readonly string RenameModList = Key("RenameModList").Translate();

    // mass unsub
    public static readonly string MassUnSubscribe = Key("SteamWorkshop.MassUnSubscribe").Translate();
    public static readonly string MassUnSubscribeAll = Key("SteamWorkshop.MassUnSubscribeAll").Translate();
    public static readonly string MassUnSubscribeInactive = Key("SteamWorkshop.MassUnSubscribeInactive").Translate();
    public static readonly string MassUnSubscribeOutdated = Key("SteamWorkshop.MassUnSubscribeOutdated").Translate();

    // mass remove local
    public static readonly string MassRemoveLocal = Key("IO.MassRemoveLocal").Translate();
    public static readonly string MassRemoveLocalAll = Key("IO.MassRemoveLocalAll").Translate();
    public static readonly string MassRemoveLocalInactive = Key("IO.MassRemoveLocalInactive").Translate();
    public static readonly string MassRemoveLocalOutdated = Key("IO.MassRemoveLocalOutdated").Translate();

    public static readonly string CreateLocalCopies = Key("CreateLocalCopies").Translate();

    public static string NeedsWellFormattedTargetVersion = "MessageModNeedsWellFormattedTargetVersion".Translate(
        VersionControl.CurrentVersionString);

    // Modlists
    public static readonly string ModListsTip = Key("ModListsTip").Translate();

    public static readonly string Import = Key("Import").Translate();
    public static readonly string Export = Key("Export").Translate();
    public static readonly string Edit = Key("Edit").Translate();

    public static readonly string Import_FromModList = Key("Import.FromModList").Translate();
    public static readonly string Import_FromString = Key("Import.FromString").Translate();
    public static readonly string Import_FromSaveGame = Key("Import.FromSaveGame").Translate();
    public static readonly string Export_ToModList = Key("Export.ToModList").Translate();
    public static readonly string Export_ToString = Key("Export.ToString").Translate();
    public static readonly string CopyToClipboard = Key("CopyToClipboard").Translate();

    public static readonly string NameTooShort = Key("NameTooShort").Translate();
    public static string ImportModListFromSave = Key("LoadModListFromSave").Translate();
    public static string Problems = Key("Problems").Translate();

    public static readonly string AddToModList = Key("AddToModList").Translate();
    public static readonly string RemoveFromModList = Key("RemoveFromModList").Translate();


    // resolvers
    public static string MoveCoreToFirst = Key("MoveCoreToFirst").Translate();


    public static readonly string ChangeColour = Key("ChangeColour").Translate();

    public static readonly string ChangeListColour = Key("ChangeListColour").Translate();

    public static readonly string DialogConfirmIssuesCritical = Key("DialogConfirmIssuesCritical").Translate();

    // manifest
    public static string ManifestNotImplemented = Key("ManifestNotImplemented").Translate();
    public static string FetchingOnlineManifest = Key("FetchingOnlineManifest").Translate();

    public static readonly string LatestVersion = Key("LatestVersion").Translate();

    // workshop
    public static readonly string DownloadPending = Key("DownloadPending").Translate();

    public static readonly string SubscribeAllMissing = Key("SubscribeAllMissing").Translate();
    public static readonly string ResetMods = Key("ResetMods").Translate();
    public static readonly string ConfirmResetMods = Key("ConfirmResetMods").Translate();
    public static readonly string SortMods = Key("SortMods").Translate();

    // settings
    public static readonly string ModSettings = Key("ModSettings").Translate();
    public static readonly string ShowAllRequirements = Key("ShowAllRequirements").Translate();
    public static readonly string ShowAllRequirementsTip = Key("ShowAllRequirementsTip").Translate();
    public static readonly string AddModManagerToNewModList = Key("AddModManagerToNewModList").Translate();
    public static readonly string AddModManagerToNewModListTip = Key("AddModManagerToNewModListTip").Translate();
    public static readonly string AddHugsLibToNewModList = Key("AddHugsLibToNewModList").Translate();
    public static readonly string AddHugsLibToNewModListTip = Key("AddHugsLibToNewModListTip").Translate();
    public static readonly string AddExpansionsToNewModList = Key("AddExpansionsToNewModList").Translate();
    public static readonly string AddExpansionsToNewModListTip = Key("AddExpansionsToNewModListTip").Translate();
    public static readonly string ShowVersionChecksForSteamMods = Key("ShowVersionChecksForSteamMods").Translate();

    public static readonly string ShowVersionChecksForSteamModsTip =
        Key("ShowVersionChecksForSteamModsTip").Translate();

    public static readonly string UseTempFolderForCrossPromotionCache =
        Key("UseTempFolderForCrossPromotionCache").Translate();

    public static readonly string UseTempFolderForCrossPromotionCacheTip =
        Key("UseTempFolderForCrossPromotionCacheTip").Translate();

    public static readonly string DeleteCrossPromotionCache = Key("DeleteCrossPromotionCache").Translate();

    public static readonly string NoDownloadUri = Key("NoDownloadUri").Translate();

    // options
    public static string SettingsCategory => Key("SettingsCategory").Translate();
    public static string ShowPromotions => Key("ShowPromotions").Translate();
    public static string ShowPromotionsTip => Key("ShowPromotionsTip").Translate();
    public static string ShowPromotions_NotSubscribed => Key("ShowPromotions_NotSubscribed").Translate();
    public static string ShowPromotions_NotActive => Key("ShowPromotions_NotActive").Translate();
    public static string TrimTags => Key("TrimTags").Translate();
    public static string TrimTagsTip => Key("TrimTagsTip").Translate();
    public static string TrimVersionStrings => Key("TrimVersionStrings").Translate();
    public static string TrimVersionStringsTip => Key("TrimVersionStringsTip").Translate();

    private static string Key(string key)
    {
        return $"{PREFIX}.{key}";
    }

    private static string Key(params string[] keys)
    {
        return $"{PREFIX}.{string.Join(".", keys)}";
    }

    public static string TargetVersions(string versions)
    {
        return Key("TargetVersions").Translate(versions);
    }

    public static string InvalidVersion(List<Version> versions)
    {
        return Key("InvalidVersion").Translate(versions.VersionList());
    }

    public static string DifferentVersion(ModMetaData mod)
    {
        return Key("DifferentVersion").Translate(mod.Name, mod.SupportedVersionsReadOnly.VersionList(),
            $"{VersionControl.CurrentMajor}.{VersionControl.CurrentMinor}");
    }

    public static string UpdateAvailable(Version current, Version latest)
    {
        return Key("UpdateAvailable").Translate(current.ToString(), latest.ToString());
    }

    public static string MissingMod(string name, string id)
    {
        return Key("MissingMod").Translate(name, id);
    }

    public static string DependencyNotFound(string name)
    {
        return Key("DependencyNotFound").Translate(name);
    }

    public static string DependencyUnknownVersion(ModMetaData tgt)
    {
        return Key("DependencyUnknownVersion").Translate(tgt.Name);
    }

    public static string DependencyWrongVersion(ModMetaData tgt, VersionedDependency depend)
    {
        return Key("DependencyWrongVersion")
            .Translate(tgt.Name, depend.Range.ToString(), tgt.GetManifest().Version.ToString());
    }

    public static string DependencyNotActive(ModMetaData tgt)
    {
        return Key("DependencyNotActive").Translate(tgt.Name);
    }

    public static string DependencyMet(ModMetaData tgt)
    {
        return Key("DependencyMet").Translate(tgt.Name, tgt.GetManifest()?.Version.ToString() ?? "[?]");
    }

    public static string IncompatibleMod(string name)
    {
        return Key("IncompatibleMod").Translate(name);
    }

    public static string LoadedBefore(string name)
    {
        return Key("LoadedBefore").Translate(name);
    }

    public static string ShouldBeLoadedBefore(string identifier)
    {
        return Key("ShouldBeLoadedBefore").Translate(identifier);
    }

    public static string LoadedAfter(string name)
    {
        return Key("LoadedAfter").Translate(name);
    }

    public static string ShouldBeLoadedAfter(string identifier)
    {
        return Key("ShouldBeLoadedAfter").Translate(identifier);
    }

    public static string MassUnSubscribeConfirm(int count, string list)
    {
        return Key("SteamWorkshop.MassUnSubscribeConfirm").Translate(count, list);
    }

    public static string MassRemoveLocalConfirm(int count, string list)
    {
        return Key("IO.MassRemoveLocalConfirm").Translate(count, list);
    }

    public static string CreateLocalCopiesConfirmation(int count)
    {
        return Key("CreateLocalCopiesConfirmation").Translate(count);
    }

    public static string CreateLocalCopy(string name)
    {
        return Key("CreateLocalCopy").Translate(name);
    }

    public static string CreateLocalSucceeded(string name)
    {
        return Key("CreateLocalSucceeded").Translate(name);
    }

    public static string CreateLocalFailed(string name)
    {
        return Key("CreateLocalFailed").Translate(name);
    }

    public static string CreatingLocal(string name)
    {
        return Key("CreatingLocal").Translate(name);
    }

    public static string RemovingLocal(string name)
    {
        return Key("RemovingLocal").Translate(name);
    }

    public static string DeleteLocalCopy(string name)
    {
        return Key("DeleteLocalCopy").Translate(name);
    }

    public static string ConfirmRemoveLocal(string name)
    {
        return Key("ConfirmRemoveLocal").Translate(name);
    }

    public static string RemoveLocalSucceeded(string name)
    {
        return Key("RemoveLocalSucceeded").Translate(name);
    }

    public static string RemoveLocalFailed(string name)
    {
        return Key("RemoveLocalFailed").Translate(name);
    }

    public static string ConfirmUnsubscribe(string name)
    {
        return "ConfirmUnsubscribe".Translate(name);
    } // core 

    public static string InvalidName(string name, string invalidChars)
    {
        return Key("InvalidName").Translate(name, invalidChars);
    }

    public static string ConfirmOverwriteModList(string name)
    {
        return Key("ConfirmOverwriteModList").Translate(name);
    }

    public static string ModListRenamed(string oldName, string newName)
    {
        return Key("ModListRenamed").Translate(oldName, newName);
    }

    public static string ModListCreated(string name)
    {
        return Key("ModListCreated").Translate(name);
    }

    public static string ModListLoaded(string name)
    {
        return Key("ModListLoaded").Translate(name);
    }

    public static string ModListDeleted(string name)
    {
        return Key("ModListDeleted").Translate(name);
    }

    public static string ModListCopiedToClipboard(string name)
    {
        return Key("ModListCopiedToClipboard").Translate(name);
    }

    public static string ModListCreatedFromClipboard(string name)
    {
        return Key("ModListCreatedFromClipboard").Translate(name);
    }

    public static string FailedToCreateModListFromClipboard(string reason)
    {
        return Key("FailedToCreateModListFromClipboard").Translate(reason);
    }

    public static string AddToModListX(string name)
    {
        return Key("AddToModListX").Translate(name);
    }

    public static string RemoveFromModListX(string name)
    {
        return Key("RemoveFromModListX").Translate(name);
    }

    public static string SearchSteamWorkshop(string name)
    {
        return Key("SearchSteamWorkshop").Translate(name);
    }

    public static string SearchForum(string name)
    {
        return Key("SearchForum").Translate(name);
    }

    public static string ActivateMod(ModMetaData mod)
    {
        return Key("ActivateMod").Translate(mod.Name, Manifest.For(mod)?.Version.ToString(),
            Key("ContentSource", mod.Source.ToString()).Translate());
    }

    //        public static string NoMatchingModInstalled(string name, Version desired, Dependency.EqualityOperator op )
    //        {
    //            return Key( "NoMatchingModInstalled_Version" )
    //                .Translate( name, VersionControl.CurrentMajor + "." + VersionControl.CurrentMinor, desired,
    //                    Dependency.OperatorToString( op ) );
    //        }
    public static string NoMatchingModInstalled(string name)
    {
        return Key("NoMatchingModInstalled")
            .Translate(name, $"{VersionControl.CurrentMajor}.{VersionControl.CurrentMinor}");
    }

    public static string DeactivateMod(ModButton_Installed mod)
    {
        return Key("DeactivateMod").Translate(TrimModName(mod.Name));
    }

    public static string MoveBefore(ModButton_Installed from, ModButton_Installed to)
    {
        return Key("MoveBefore").Translate(TrimModName(from.Name), TrimModName(to.Name));
    }

    public static string MoveAfter(ModButton_Installed from, ModButton_Installed to)
    {
        return Key("MoveAfter").Translate(TrimModName(from.Name), TrimModName(to.Name));
    }

    public static string ChangeModColour(string name)
    {
        return Key("ChangeModColour").Translate(name);
    }

    public static string ChangeButtonColour(string name)
    {
        return Key("ChangeButtonColour").Translate(name);
    }

    public static string ModHomePage(string url)
    {
        return Key("ModHomePage").Translate(url);
    }

    public static string WorkshopPage(string subject)
    {
        return Key("WorkshopPage").Translate(subject);
    }

    public static string DialogConfirmIssues(string warning, string description)
    {
        return Key("DialogConfirmIssues").Translate(warning, description).Resolve();
    }

    public static string DialogConfirmIssuesTitle(int count)
    {
        return Key("DialogConfirmIssuesTitle").Translate(count);
    }

    public static string FetchingOnlineManifestFailed(string error)
    {
        return Key("FetchingOnlineManifestFailed").Translate(error);
    }

    public static string NewVersionAvailable(Version current, Version latest)
    {
        return Key("NewVersionAvailable").Translate(current.ToString(), latest.ToString());
    }

    public static string ModInstalled(string name)
    {
        return Key("ModInstalled").Translate(name);
    }

    public static string Subscribe(string name)
    {
        return Key("Subscribe").Translate(name);
    }

    public static string SortFailed_Cyclic(string a, string b)
    {
        return Key("SortFailed.CyclicDependency").Translate(a, b);
    }

    // promotions
    public static string PromotionsFor(string author)
    {
        return Key("PromotionsFor").Translate(author);
    }

    public static string CrossPromotionCacheFolderSize(long size)
    {
        return Key("CrossPromotionCacheFolderSize").Translate(size.ToStringSize());
    }

    public static string ConfirmDeletingCrossPromotionCache(string path, int count, long size)
    {
        return Key("ConfirmDeletingCrossPromotionCache").Translate(path, count, size.ToStringSize());
    }

    public static string OpenDownloadUri(string downloadUri)
    {
        return Key("OpenDownloadUri").Translate(downloadUri).Resolve();
    }

    public static string XIsUpToDate(ModMetaData mod)
    {
        return Key("XIsUpToDate").Translate(mod.Name);
    }

    public static string YHasUpdated(ModMetaData target)
    {
        return Key("YHasUpdated").Translate(target.Name);
    }

    public static string UpdateLocalCopy(ModMetaData mod)
    {
        return Key("UpdateLocalCopy").Translate(mod.Name);
    }

    public static string XModsImportedFromString(int count)
    {
        return Key("XModsImportedFromString").Translate(count);
    }

    public static string XModsExportedToString(int count)
    {
        return Key("XModsExportedToString").Translate(count);
    }
}