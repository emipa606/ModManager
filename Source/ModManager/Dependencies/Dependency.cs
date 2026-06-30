// Dependency.cs
// Copyright Karel Kroeze, 2020-2020

using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml;
using RimWorld;
using UnityEngine;
using Verse;

namespace ModManager;

public abstract class Dependency : ModDependency
{
    private const string InvalidPackageId = "invalid.package.id";

    private static readonly Regex packageIdFormatRegex =
        new(@"(?=.{1,60}$)^(?:[a-z0-9]+\.)+[a-z0-9]+$", RegexOptions.IgnoreCase);

    protected ModMetaData _target;
    protected bool _targetResolved;
    public Manifest parent;

    private bool? satisfied;

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
            _target = ModLister.GetActiveModWithIdentifier(packageId, true) ??
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
            satisfied ??= CheckSatisfied();

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

    private static bool TryGetPackageIdFromIdentifier(string identifier, out string packageId)
    {
        // Enumerate directly instead of calling .ToList() on every invocation.
        // Folder-name match has priority over display-name match (same as before).
        string nameMatch = null;
        foreach (var m in ModLister.AllInstalledMods)
        {
            if (m.FolderName.StripSpaces() == identifier)
            {
                packageId = m.PackageId.StripPostfixes();
                return true;
            }

            if (nameMatch == null && m.Name.StripSpaces() == identifier)
            {
                nameMatch = m.PackageId.StripPostfixes();
            }
        }

        if (nameMatch != null)
        {
            packageId = nameMatch;
            return true;
        }

        packageId = InvalidPackageId;
        return false;
    }

    protected void TryParseIdentifier(string text, XmlNode node)
    {
        if (packageIdFormatRegex.IsMatch(text))
        {
            packageId = text;
            return;
        }

        if (TryGetPackageIdFromIdentifier(text, out packageId))
        {
            if (Prefs.DevMode)
            {
                Log.Message($"Invalid packageId '{text}' resolved to '{packageId}'");
            }

            return;
        }

        // Could not resolve – leave packageId as InvalidPackageId (set by TryGetPackageIdFromIdentifier).
#if DEBUG
        Log.Message($"Failed to parse dependency: {node.OuterXml}.\nInner exception: Invalid packageId: '{text}'");
#else
        if (Prefs.DevMode)
        {
            Log.Warning($"Failed to parse dependency: {node.OuterXml}.\nInner exception: Invalid packageId: '{text}'");
        }
#endif
    }
}