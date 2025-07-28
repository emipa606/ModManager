// ScribeMetaHeaderUtility_TryCreateDialogsForVersionMismatchWarnings.cs
// Copyright Karel Kroeze, 2019-2019

using System;
using RimWorld;
using Verse;

namespace ModManager;

//    [HarmonyPatch(typeof( ScribeMetaHeaderUtility ), nameof( ScribeMetaHeaderUtility.TryCreateDialogsForVersionMismatchWarnings ) )]
public class ScribeMetaHeaderUtility_TryCreateDialogsForVersionMismatchWarnings
{
    public static bool Prefix(Action confirmedAction, ScribeMetaHeaderUtility.ScribeHeaderMode ___lastMode)
    {
        string message = null;
        string title = null;

        if (!BackCompatibility.IsSaveCompatibleWith(ScribeMetaHeaderUtility.loadedGameVersion) &&
            // ScribeMetaHeaderUtility.VersionsMatch is private, it's component parts are not...
            VersionControl.BuildFromVersionString(ScribeMetaHeaderUtility.loadedGameVersion) !=
            VersionControl.BuildFromVersionString(VersionControl.CurrentVersionStringWithRev))
        {
            title = "VersionMismatch".Translate();
            var loadedVersion = !ScribeMetaHeaderUtility.loadedGameVersion.NullOrEmpty()
                ? ScribeMetaHeaderUtility.loadedGameVersion
                : ("(" + "UnknownLower".Translate() + ")").ToString();
            message = ___lastMode switch
            {
                ScribeMetaHeaderUtility.ScribeHeaderMode.Map => "SaveGameIncompatibleWarningText".Translate(
                    loadedVersion, VersionControl.CurrentVersionString),
                ScribeMetaHeaderUtility.ScribeHeaderMode.World => "WorldFileVersionMismatch".Translate(loadedVersion,
                    VersionControl.CurrentVersionString),
                ScribeMetaHeaderUtility.ScribeHeaderMode.None => throw new NotImplementedException(),
                ScribeMetaHeaderUtility.ScribeHeaderMode.Scenario => throw new NotImplementedException(),
                ScribeMetaHeaderUtility.ScribeHeaderMode.Ideo => throw new NotImplementedException(),
                _ => "FileIncompatibleWarning".Translate(loadedVersion, VersionControl.CurrentVersionString)
            };
        }

        var modMismatch = false;
        if (!ScribeMetaHeaderUtility.LoadedModsMatchesActiveMods(out var loadedMods, out var activeMods))
        {
            modMismatch = true;
            string modsMismatchMessage = "We're terribly sorry this message is so useless" +
                                         "ModsMismatchWarningText".Translate(loadedMods, activeMods);
            message = message == null ? modsMismatchMessage : $"{message}\n\n{modsMismatchMessage}";
            title ??= "ModsMismatchWarningTitle".Translate();
        }

        if (message == null)
        {
            return false;
        }

        var dialog = Dialog_MessageBox.CreateConfirmation(message, confirmedAction, false, title);
        dialog.buttonAText = "LoadAnyway".Translate();

        if (modMismatch)
        {
            dialog.buttonCText = "ChangeLoadedMods".Translate();
            dialog.buttonCAction = delegate
            {
                // TODO: load mods from save game, mod manager style.
                // Probably want to open the mod menu?
                // ModsConfig.RestartFromChangedMods();
            };
        }

        Find.WindowStack.Add(dialog);

        return false;
    }
}