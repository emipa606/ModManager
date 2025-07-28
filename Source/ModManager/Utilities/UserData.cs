// UserData.cs
// Copyright Karel Kroeze, 2020-2020

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using Verse;

namespace ModManager;

public class UserData
{
    private const string UserDataFolder = "ModManager_UserData";
    private const string ModsFolder = "Mods";
    private const string ButtonFolder = "Buttons";

    private static readonly Dictionary<ModMetaData, ModAttributes> ModAttributes = new();

    private static readonly Dictionary<ModButton, ButtonAttributes> ButtonAttributes = new();

    public ModAttributes this[ModMetaData mod]
    {
        get
        {
            if (ModAttributes.TryGetValue(mod, out var attributes))
            {
                return attributes;
            }

            var path = GetModAttributesPath(mod);
            if (File.Exists(path))
            {
                attributes = Read<ModAttributes>(path);
                attributes.Mod = mod;
            }
            else
            {
                attributes = new ModAttributes(mod);
            }

            ModAttributes.Add(mod, attributes);
            return attributes;
        }
    }

    public ButtonAttributes this[ModButton button]
    {
        get
        {
            if (ButtonAttributes.TryGetValue(button, out var attributes))
            {
                return attributes;
            }

            var path = GetButtonAttributesPath(button);
            if (File.Exists(path))
            {
                attributes = Read<ButtonAttributes>(path);
                attributes.Button = button;
            }
            else
            {
                attributes = new ButtonAttributes(button);
            }

            ButtonAttributes.Add(button, attributes);
            return attributes;
        }
    }

    public static string GetModAttributesPath(ModMetaData mod)
    {
        return Path.Combine(GenFilePaths.SaveDataFolderPath, UserDataFolder, ModsFolder,
            $"{mod.PackageId}.xml");
    }

    public static string GetButtonAttributesPath(ModButton button)
    {
        try
        {
            return Path.Combine(GenFilePaths.SaveDataFolderPath, UserDataFolder, ButtonFolder,
                $"{button.Name.SanitizeFileName()}.xml");
        }
        catch (ArgumentException err)
        {
            Debug.Error(
                $"Error getting path for {button.Name}:\n\tSystem: {Environment.OSVersion} :: {RuntimeInformation.OSDescription}\n\tException: {err}");
            throw;
        }
    }

    public static void Write(IUserData data)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(data.FilePath));
        Scribe.saver.InitSaving(data.FilePath, "UserData");
        data.ExposeData();
        Scribe.saver.FinalizeSaving();
    }

    private T Read<T>(string path) where T : IUserData
    {
        Scribe.loader.InitLoading(path);
        Scribe.loader.EnterNode("UserData");
        var userData = Activator.CreateInstance<T>();
        userData.ExposeData();
        Scribe.loader.FinalizeLoading();

        return userData;
    }
}