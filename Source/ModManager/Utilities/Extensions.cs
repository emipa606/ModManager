// Extensions.cs
// Copyright Karel Kroeze, 2018-2018

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Verse;

namespace ModManager;

public static class Extensions
{
    private static readonly Dictionary<ModMetaData, Mod>
        _modClassWithSettingsCache = new();

    public static string SanitizeYaml(this string msg)
    {
        // TODO: conditionally escape only when needed
        return $"'{msg.Replace("'", "''")}'";
    }

    public static string StringJoin(this IEnumerable<string> list, string glue)
    {
        return string.Join(glue, list.ToArray());
    }

    public static string VersionList(this IEnumerable<Version> versions)
    {
        return versions.Select(v => $"{v.Major}.{v.Minor}").StringJoin(", ");
    }

    public static Color Desaturate(this Color color, float saturation = 0.5f)
    {
        Color.RGBToHSV(color, out var h, out var s, out var v);
        s *= saturation;
        v *= saturation;
        return Color.HSVToRGB(h, s, v);
    }

    public static string StripSpaces(this string str)
    {
        return str?.Replace(" ", "");
    }

    public static bool IsValidSteamWorkshopIdentifier(this ulong identifier)
    {
        return identifier != 0;
    }

    // https://stackoverflow.com/a/4975942/2604271
    public static string ToStringSize(this long bytes)
    {
        string[] suf = ["B", "KB", "MB", "GB", "TB", "PB", "EB"]; //Longs run out around EB
        if (bytes == 0)
        {
            return $"0{suf[0]}";
        }

        var place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
        var num = Math.Round(bytes / Math.Pow(1024, place), 1);
        return (Math.Sign(bytes) * num) + suf[place];
    }

    public static ModAttributes UserData(this ModMetaData mod)
    {
        return ModManager.UserData[mod];
    }

    extension(ModMetaData mod)
    {
        public int LoadOrder()
        {
            if (mod == null)
            {
                Log.Error("Tried to get load order for NULL mod");
                return -1;
            }

            var activeMods = ModsConfig.ActiveModsInLoadOrder;
            if (activeMods.All(am => am?.PackageId != mod.PackageId))
            {
                return -1;
            }

            return activeMods.FirstIndexOf(am => am.SamePackageId(mod.PackageId));
        }

        public bool HasSettings()
        {
            return mod.SettingsCategory() != null;
        }

        private string SettingsCategory()
        {
            return mod.ModClassWithSettings()?.SettingsCategory();
        }

        public Mod ModClassWithSettings()
        {
            if (_modClassWithSettingsCache.TryGetValue(mod, out var modClass))
            {
                return modClass;
            }

            modClass = LoadedModManager.ModHandles.FirstOrDefault(m =>
                mod.SamePackageId(m.Content.PackageId) &&
                !m.SettingsCategory().NullOrEmpty());
            _modClassWithSettingsCache.Add(mod, modClass);
            return modClass;
        }

        public int Compatibility(bool careAboutBuild = false)
        {
            return mod.VersionCompatible ? 1 : 0;
        }
    }

    extension<T>(List<T> list)
    {
        public bool TryRemove(T element)
        {
            if (element != null && list.Contains(element))
            {
                return list.Remove(element);
            }

            return false;
        }

        public void TryAdd(T element)
        {
            if (element == null || list.Contains(element))
            {
                return;
            }

            list.Add(element);
        }
    }

    extension(ModMetaData mod)
    {
        public string AboutDir()
        {
            return Path.Combine(mod.RootDir.FullName, "About");
        }

        public bool IsLocalCopy()
        {
            return mod.Source == ContentSource.ModsFolder &&
                   mod.PackageId.StartsWith(IO.LocalCopyPrefix);
        }
    }
}