// Copyright Karel Kroeze, 2020-2021.
// ModManager/ModManager/IO.cs

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using HarmonyLib;
using RimWorld;
using Verse;

namespace ModManager;

public static class IO
{
    private const string LocalCopyPostfix = "_copy";

    private static readonly Regex _postfixRegex =
        new($@"(?:{ModMetaData.SteamModPostfix}|{LocalCopyPostfix}(?:_\d+)?)$");

    private static readonly List<Pair<ModButton_Installed, ModMetaData>> _batchCreatedCopies =
        [];

    private static readonly Dictionary<string, string> _hashCache = new();


    private static readonly char[] invalidChars =
        Path.GetInvalidPathChars().Concat(Path.GetInvalidFileNameChars()).ToArray();

    private static readonly Regex invalidFileNameCharsRegex =
        new($"[{Regex.Escape(new string(invalidChars))}]",
            RegexOptions.Compiled & RegexOptions.CultureInvariant);

    private static readonly string[] reservedWords =
    [
        "CON", "PRN", "AUX", "CLOCK$", "NUL", "COM0", "COM1", "COM2", "COM3", "COM4",
        "COM5", "COM6", "COM7", "COM8", "COM9", "LPT0", "LPT1", "LPT2", "LPT3", "LPT4",
        "LPT5", "LPT6", "LPT7", "LPT8", "LPT9"
    ];

    private static string DateStamp => $"-{DateTime.Now.Day}-{DateTime.Now.Month}";
    public static string LocalCopyPrefix => "__LocalCopy";
    private static string ModsDir => GenFilePaths.ModsFolderPath;


    private static void Copy(this DirectoryInfo source, string destination, bool recursive)
    {
        // Get the subdirectories for the specified directory.
        var dirs = source.GetDirectories();

        // If the destination directory doesn't exist, create it.
        if (!Directory.Exists(destination))
        {
            Directory.CreateDirectory(destination);
        }

        // Get the files in the directory and copy them to the new location.
        var files = source.GetFiles();
        foreach (var file in files)
        {
            var temppath = Path.Combine(destination, file.Name);
            file.CopyTo(temppath, false);
        }

        // If copying subdirectories, copy them and their contents to new location.
        if (!recursive)
        {
            return;
        }

        foreach (var subdir in dirs)
        {
            var temppath = Path.Combine(destination, subdir.Name);
            subdir.Copy(temppath, true);
        }
    }

    internal static void CreateLocalCopies(IEnumerable<ModMetaData> mods, bool force = false)
    {
        var steamMods = mods.Where(m => m.Source == ContentSource.SteamWorkshop);
        if (!force && steamMods.Count() > 5)
        {
            Find.WindowStack.Add(Dialog_MessageBox.CreateConfirmation(
                I18n.CreateLocalCopiesConfirmation(steamMods.Count()),
                () => CreateLocalCopies(steamMods, true)));
            return;
        }

        foreach (var mod in steamMods)
        {
            CreateLocalCopy(mod, true);
        }

        FinishBatchCreate();
    }

    internal static void CreateLocalCopy(ModMetaData mod, bool batch = false)
    {
        LongEventHandler.QueueLongEvent(() =>
        {
            LongEventHandler.SetCurrentEventText(I18n.CreatingLocal(mod.Name));
            if (TryCreateLocalCopy(mod, out var copy))
            {
                var button = ModButton_Installed.For(copy);
                if (batch)
                {
                    _batchCreatedCopies.Add(new Pair<ModButton_Installed, ModMetaData>(button, copy));
                }
                else
                {
                    Messages.Message(I18n.CreateLocalSucceeded(mod.Name), MessageTypeDefOf.NeutralEvent, false);
                    LongEventHandler.QueueLongEvent(() => button.Notify_VersionAdded(copy, true), "", true, null);
                }
            }
            else
            {
                Messages.Message(I18n.CreateLocalFailed(mod.Name), MessageTypeDefOf.RejectInput, false);
            }
        }, null, true, null);
    }

    internal static void DeleteLocal(ModMetaData mod, bool force = false)
    {
        if (force)
        {
            LongEventHandler.QueueLongEvent(() =>
            {
                LongEventHandler.SetCurrentEventText(I18n.RemovingLocal(mod.Name));
                if (TryRemoveLocalCopy(mod))
                {
                    Messages.Message(I18n.RemoveLocalSucceeded(mod.Name),
                        MessageTypeDefOf.NeutralEvent, false);
                }
                else
                {
                    Messages.Message(I18n.RemoveLocalFailed(mod.Name),
                        MessageTypeDefOf.RejectInput, false);
                }

                // remove this version either way, as it's likely to be borked.
                ModButton_Installed.For(mod).Notify_VersionRemoved(mod);
            }, null, true, null);
            return;
        }

        Find.WindowStack.Add(Dialog_MessageBox.CreateConfirmation(
            I18n.ConfirmRemoveLocal(mod.Name), () => DeleteLocal(mod, true), true));
    }

    private static void DeleteLocalCopies(IEnumerable<ModMetaData> mods)
    {
        var modList = mods
            .Select(m =>
                $"{m.Name} ({m.SupportedVersionsReadOnly.Select(v => v.ToString()).StringJoin(", ")})")
            .ToLineList();
        var dialog = Dialog_MessageBox.CreateConfirmation(
            I18n.MassUnSubscribeConfirm(mods.Count(), modList),
            () =>
            {
                foreach (var mod in new List<ModMetaData>(mods))
                {
                    DeleteLocal(mod, true);
                }
            },
            true);
        Find.WindowStack.Add(dialog);
    }

    private static void FinishBatchCreate()
    {
        LongEventHandler.QueueLongEvent(() =>
        {
            foreach (var localCopy in _batchCreatedCopies)
            {
                var button = localCopy.First;
                var copy = localCopy.Second;
                button.Notify_VersionAdded(copy, true);
            }

            _batchCreatedCopies.Clear();
        }, string.Empty, true, null);
    }

    internal static string GetFolderHash(this DirectoryInfo folder)
    {
        if (_hashCache.TryGetValue(folder.FullName, out var hash))
        {
            return hash;
        }

        var files = folder.GetFiles("*", SearchOption.AllDirectories)
            .OrderBy(p => p.FullName);

        using var md5 = MD5.Create();
        foreach (var file in files)
        {
            // hash path
            var pathBytes = Encoding.UTF8.GetBytes(file.FullName[folder.FullName.Length..]);
            md5.TransformBlock(pathBytes, 0, pathBytes.Length, pathBytes, 0);

            // hash contents
            var contentBytes = File.ReadAllBytes(file.FullName);

            md5.TransformBlock(contentBytes, 0, contentBytes.Length, contentBytes, 0);
        }

        //Handles empty filePaths case
        md5.TransformFinalBlock([], 0, 0);

        hash = BitConverter.ToString(md5.Hash).Replace("-", "").ToLower();
        _hashCache.Add(folder.FullName, hash);
        return hash;
    }

    private static string GetLocalCopyFolder(this ModMetaData mod)
    {
        return Path.Combine(ModsDir, $"{LocalCopyPrefix}_{mod.Name}_{DateStamp}".SanitizeFileName());
    }

    private static string GetUniquePackageId(ModMetaData mod)
    {
        var baseId = mod.PackageIdPlayerFacing + LocalCopyPostfix;
        var id = baseId;
        var i = 1;
        while (ModLister.GetModWithIdentifier(id) != null)
        {
            id = $"{baseId}_{i++}";
        }

        return id;
    }

    public static T ItemFromXmlString<T>(string xml, bool resolveCrossRefs = true) where T : new()
    {
        if (resolveCrossRefs && DirectXmlCrossRefLoader.LoadingInProgress)
        {
            Log.Error(
                "Cannot call ItemFromXmlFile with resolveCrossRefs=true while loading is already in progress.");
        }

        if (xml.NullOrEmpty())
        {
            return Activator.CreateInstance<T>();
        }

        T result;
        try
        {
            var xmlDocument = new XmlDocument();
            // Trim should theoretically also get rid of BOMs
            xmlDocument.LoadXml(xml.Trim());
            var t = DirectXmlToObject.ObjectFromXml<T>(xmlDocument.DocumentElement, false);
            if (resolveCrossRefs)
            {
                DirectXmlCrossRefLoader.ResolveAllWantedCrossReferences(FailMode.LogErrors);
            }

            result = t;
        }
        catch (Exception ex)
        {
            Log.Error(
                $"Exception loading item from string. Loading defaults instead. \nXML: {xml}\n\nException: {ex}");
            result = Activator.CreateInstance<T>();
        }

        return result;
    }

    public static void MassRemoveLocalFloatMenu()
    {
        var options = Utilities.NewOptionsList;
        var localCopies = ModButtonManager.AllMods.Where(m => m.IsLocalCopy());
        var outdated = localCopies.Where(m => !m.VersionCompatible && !m.MadeForNewerVersion);
        var inactive = ModButtonManager.AvailableMods.Where(m => m.IsLocalCopy());
        options.Add(new FloatMenuOption(I18n.MassRemoveLocalAll, () => DeleteLocalCopies(localCopies)));
        if (outdated.Any())
        {
            options.Add(new FloatMenuOption(I18n.MassRemoveLocalOutdated, () => DeleteLocalCopies(outdated)));
        }

        if (inactive.Any())
        {
            options.Add(new FloatMenuOption(I18n.MassRemoveLocalInactive, () => DeleteLocalCopies(inactive)));
        }

        Utilities.FloatMenu(options);
    }

    private static string NewSettingsFilePath(string source, Regex mask, string targetIdentifier)
    {
        var match = mask.Match(source);
        return Path.Combine(GenFilePaths.ConfigFolderPath,
            GenText.SanitizeFilename($"Mod_{targetIdentifier}_{match.Groups[1].Value}.xml"));
    }

    public static string SanitizeFileName(this string str)
    {
        var fileSystemSanitized = invalidFileNameCharsRegex.Replace(str, "_");
        return reservedWords
            .Select(reservedWord => $@"^{reservedWord}\.")
            .Aggregate(fileSystemSanitized,
                (current, reservedWordPattern) =>
                    Regex.Replace(current, reservedWordPattern, "_", RegexOptions.IgnoreCase));
    }

    private static Regex SettingsMask(string identifier)
    {
        return new Regex($@"Mod_{GenText.SanitizeFilename(identifier)}_(.*)\.xml");
    }

    private static void SetUniquePackageId(ModMetaData mod)
    {
        var id = GetUniquePackageId(mod);
        UpdatePackageId_ModMetaData(mod, id);
        UpdatePackageId_Xml(mod, id);
    }

    public static string StripPostfixes(this string packageId)
    {
        return _postfixRegex.Replace(packageId.Trim(), "");
    }

    private static bool TryCopyMod(ModMetaData mod,
        out ModMetaData copy,
        string targetDir,
        bool copySettings = true,
        bool copyUserData = true)
    {
        try
        {
            // copy mod
            mod.RootDir.Copy(targetDir, true);
            copy = new ModMetaData(targetDir);
            SetUniquePackageId(copy);
            ModManager.UserData[copy].Source = mod;
            var manifest = copy.GetManifest();
            manifest.sourceSync = new SourceSync(manifest, mod.PackageId);

            (ModLister.AllInstalledMods as List<ModMetaData>)?.Add(copy);

            // copy settings and color attribute
            if (copySettings)
            {
                TryCopySettings(mod, copy);
            }

            if (copyUserData)
            {
                TryCopyUserData(mod, copy);
            }

            // set source attribute
            ModManager.UserData[copy].Source = mod;

            return true;
        }
        catch (Exception e)
        {
            Log.Error($"Creating local copy failed: {e.Message} \n\n{e.StackTrace}");
            copy = null;
            return false;
        }
    }

    private static void TryCopySettings(ModMetaData source, ModMetaData target, bool deleteOld = false)
    {
        // find any settings files that belong to the source mod
        var mask = SettingsMask(source.FolderName);
        var settings = Directory.GetFiles(GenFilePaths.ConfigFolderPath)
            .Where(f => mask.IsMatch(f))
            .Select(f => new
            {
                source = f,
                target = NewSettingsFilePath(f, mask, target.FolderName)
            });

        // copy settings files, overwriting existing - if any.
        foreach (var setting in settings)
        {
            Debug.Log($"Copying settings :: {setting.source} => {setting.target}");
            if (deleteOld)
            {
                File.Move(setting.source, setting.target);
            }
            else
            {
                File.Copy(setting.source, setting.target, true);
            }
        }
    }

    private static void TryCopyUserData(ModMetaData source, ModMetaData target, bool deleteOld = false)
    {
        try
        {
            var sourcePath = UserData.GetModAttributesPath(source);
            if (!File.Exists(sourcePath))
            {
                return;
            }

            File.Copy(sourcePath, UserData.GetModAttributesPath(target), true);
            if (deleteOld)
            {
                File.Delete(sourcePath);
            }
        }
        catch (Exception err)
        {
            Debug.Error(
                $"Error copying user settings: \n\tsource: {source.Name}\n\ttarget: {target.Name}\n\terror: {err}");
        }
    }

    private static bool TryCreateLocalCopy(ModMetaData mod, out ModMetaData copy)
    {
        if (mod.Source != ContentSource.SteamWorkshop)
        {
            Log.Error("Can only create local copies of steam workshop mods.");
            copy = null;
            return false;
        }

        var baseTargetDir = mod.GetLocalCopyFolder();
        var targetDir = baseTargetDir;
        var i = 2;
        while (Directory.Exists(targetDir))
        {
            targetDir = $"{baseTargetDir} ({i++})";
        }

        return TryCopyMod(mod, out copy, targetDir);
    }

    private static bool TryRemoveLocalCopy(ModMetaData mod)
    {
        if (mod.Source != ContentSource.ModsFolder)
        {
            Log.Error("Can only delete locally installed non-steam workshop mods.");
            return false;
        }

        try
        {
            mod.RootDir.Delete(true);
            TryRemoveUserData(mod);
            (ModLister.AllInstalledMods as List<ModMetaData>)?.TryRemove(mod);
            return true;
        }
        catch (Exception e)
        {
            Log.Error(e.Message);
            Log.Warning("Deleting failed. Retrying may help.");
            return false;
        }
    }

    private static void TryRemoveUserData(ModMetaData mod)
    {
        var path = mod.UserData()?.FilePath;
        if (path == null)
        {
            return;
        }

        try
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
        catch (Exception err)
        {
            Debug.Error($"failed to delete {path}:\n{err}");
        }
    }

    public static void TryUpdateLocalCopy(ModMetaData source, ModMetaData local)
    {
        // delete and re-copy mod.
        var removedResult = TryRemoveLocalCopy(local);
        if (!removedResult)
        {
            return;
        }

        var updateResult = TryCopyMod(source, out var updated, local.RootDir.FullName, false);
        if (!updateResult)
        {
            return;
        }

        // update version 
        var button = ModButton_Installed.For(updated);
        button.Notify_VersionRemoved(local);
        button.Notify_VersionAdded(updated, true);
    }

    private static void UpdatePackageId_ModMetaData(ModMetaData mod, string id)
    {
        var traverse = Traverse.Create(mod);
        traverse.Field("meta").Field("traverse").SetValue(id);
        traverse.Field("packageIdLowerCase").SetValue(id.ToLower());
    }

    private static void UpdatePackageId_Xml(ModMetaData mod, string id)
    {
        var filePath = Path.Combine(mod.AboutDir(), "About.xml");
        var meta = new XmlDocument();
        meta.Load(filePath);
        var node = meta.SelectSingleNode("ModMetaData/packageId");
        if (node == null)
        {
            Debug.Error($"packageId node not found for {mod.Name}!");
        }
        else
        {
            node.InnerText = id;
            meta.Save(filePath);
        }
    }
}