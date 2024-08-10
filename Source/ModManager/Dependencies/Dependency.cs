// Dependency.cs
// Copyright Karel Kroeze, 2020-2020

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using RimWorld;
using UnityEngine;
using Verse;

namespace ModManager;

public abstract class Dependency : ModDependency
{
    public const string InvalidPackageId = "invalid.package.id";

    public static readonly Regex packageIdFormatRegex =
        new Regex(@"(?=.{1,60}$)^(?:[a-z0-9]+\.)+[a-z0-9]+$", RegexOptions.IgnoreCase);

    protected ModMetaData _target;
    protected bool _targetResolved;
    public Manifest parent;

    protected bool? satisfied;

    protected Dependency(Manifest parent, string packageId)
    {
        this.parent = parent;
        this.packageId = packageId;
    }

    protected Dependency(Manifest parent, ModDependency depend) : this(parent, depend.packageId)
    {
        displayName = depend.displayName;
        downloadUrl = depend.downloadUrl;
        steamWorkshopUrl = depend.steamWorkshopUrl;
    }

    public virtual ModMetaData Target
    {
        get
        {
            if (_targetResolved)
            {
                return _target;
            }

            // we don't want to just re-resolve _target if it's null, as we 
            // might have quite a few mods listing other dependencies that 
            // are not installed.
            _target = ModLister.GetActiveModWithIdentifier(packageId) ??
                      ModLister.GetModWithIdentifier(packageId, true);
            _targetResolved = true;
            return _target;
        }
    }

    // todo: add enum for severity
    public virtual int Severity => IsSatisfied ? 0 : 1;

    public virtual Color Color => Color.white;

    public override bool IsSatisfied
    {
        get
        {
            if (!satisfied.HasValue)
            {
                satisfied = CheckSatisfied();
            }

            return satisfied.Value;
        }
    }

    public virtual bool IsApplicable => true;

    public abstract List<FloatMenuOption> Resolvers { get; }

    public override Texture2D StatusIcon => Resources.Warning;

    public virtual void Notify_Recache()
    {
        satisfied = null;
        _targetResolved = false;
    }

    protected abstract bool CheckSatisfied();

    public override void OnClicked(Page_ModsConfig window)
    {
        if (!Resolvers.EnumerableNullOrEmpty())
        {
            Utilities.FloatMenu(Resolvers);
        }
    }

    public static bool TryGetPackageIdFromIdentifier(string identifier, out string packageId)
    {
        var allMods = ModLister.AllInstalledMods.ToList();
        var modByFolder = allMods.Find(m => m.FolderName.StripSpaces() == identifier);
        if (modByFolder != null)
        {
            packageId = modByFolder.PackageId.StripPostfixes();
            return true;
        }

        var modByName = allMods.Find(m => m.Name.StripSpaces() == identifier);
        if (modByName != null)
        {
            packageId = modByName.PackageId.StripPostfixes();
            return true;
        }

        packageId = InvalidPackageId;
        return false;
    }

    protected void TryParseIdentifier(string text, XmlNode node)
    {
        try
        {
            if (!packageIdFormatRegex.IsMatch(text))
            {
                if (TryGetPackageIdFromIdentifier(text, out packageId))
                {
                    if (Prefs.DevMode)
                    {
                        Log.Message($"Invalid packageId '{text}' resolved to '{packageId}'");
                    }
                }
                else
                {
                    throw new InvalidDataException($"Invalid packageId: '{text}'");
                }
            }
            else
            {
                packageId = text;
            }
        }
        catch (Exception ex)
        {
#if DEBUG
                Log.Message($"Failed to parse dependency: {node.OuterXml}.\nInner exception: {ex}");
#else
            if (Prefs.DevMode)
            {
                Log.Warning($"Failed to parse dependency: {node.OuterXml}.\nInner exception: {ex}");
            }
#endif
        }
    }
}