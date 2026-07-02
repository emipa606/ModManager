// VersionedDependency.cs
// Copyright Karel Kroeze, 2020-2020

using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using SemanticVersioning;
using UnityEngine;
using Verse;

namespace ModManager;

public class VersionedDependency : Dependency
{
    private static readonly Regex SteamIdRegex = new(@"(\d*)$");
    protected bool versioned;

    public VersionedDependency() : base(null, string.Empty)
    {
    }

    public VersionedDependency(Manifest parent, ModDependency depend) : base(parent, depend)
    {
    }

    protected VersionedDependency(Manifest parent, string packageId) : base(parent, packageId)
    {
    }

    public Range Range
    {
        get;
        private set
        {
            versioned = true;
            field = value;
        }
    } = new(">= 0.0.0");

    public override int Severity
    {
        get
        {
            if (IsSatisfied)
            {
                return 0;
            }

            if (IsActive && !IsInRange)
            {
                return 2;
            }

            return 3;
        }
    }

    public override Color Color => IsSatisfied ? Color.white : Color.red;

    public override List<FloatMenuOption> Resolvers
    {
        get
        {
            var options = Utilities.NewOptionsList;
            // if available, activate
            // else
            // if it has steam id, subscribe + link
            // if it has download location, link
            // else 
            // search forum
            // search steam

            if (IsAvailable && IsInRange)
            {
                var manifest = Target?.GetManifest();
                var button = manifest?.Button;
                if (button != null)
                {
                    options.Add(new FloatMenuOption(I18n.ActivateMod(Target),
                        () => button.Active = true));
                }
            }
            else if (!downloadUrl.NullOrEmpty() || !steamWorkshopUrl.NullOrEmpty())
            {
                if (!downloadUrl.NullOrEmpty())
                {
                    options.Add(new FloatMenuOption(I18n.OpenDownloadUri(downloadUrl),
                        () => SteamUtility.OpenUrl(downloadUrl)));
                }

                if (steamWorkshopUrl.NullOrEmpty())
                {
                    return options;
                }

                var steamId = SteamIdRegex.Match(steamWorkshopUrl).Groups[1].Value;
                Debug.Log($"steamUrl: {steamWorkshopUrl}, id: {steamId}");
                options.Add(new FloatMenuOption(I18n.WorkshopPage(displayName ?? packageId),
                    () => SteamUtility.OpenUrl(downloadUrl)));
                options.Add(new FloatMenuOption(I18n.Subscribe(displayName ?? packageId),
                    () => Workshop.Subscribe(steamId)));
            }
            else
            {
                options.Add(new FloatMenuOption(I18n.SearchForum(displayName ?? packageId),
                    () => SteamUtility.OpenUrl("http://rimworldgame.com/getmods")));
                options.Add(new FloatMenuOption(I18n.SearchSteamWorkshop(displayName ?? packageId),
                    () => SteamUtility.OpenUrl(
                        $"https://steamcommunity.com/workshop/browse/?appid=294100&searchtext={displayName ?? packageId}")));
            }

            return options;
        }
    }

    public override string Tooltip
    {
        get
        {
            if (!IsAvailable)
            {
                return I18n.DependencyNotFound(displayName ?? packageId);
            }

            if (!IsActive)
            {
                return I18n.DependencyNotActive(Target);
            }

            if (!IsInRange)
            {
                return I18n.DependencyWrongVersion(Target, this);
            }

            return IsSatisfied ? I18n.DependencyMet(Target) : "Something weird happened.";
        }
    }

    private bool IsAvailable => Target != null;
    protected bool IsActive => Target?.GetManifest()?.Button?.Active ?? false;
    public override bool IsApplicable => parent?.Mod?.Active ?? false;

    protected bool IsInRange
    {
        get
        {
            var v = Target?.GetManifest()?.Version;
            return v != null && Range.IsSatisfied($"{v.Major}.{v.Minor}.{v.Build}", true);
        }
    }

    public override string RequirementTypeLabel => "dependsOn".Translate();

    protected override bool CheckSatisfied()
    {
        return IsAvailable && IsActive && IsInRange;
    }

    public void LoadDataFromXmlCustom(XmlNode root)
    {
        var parts = root.InnerText.Split(' ');
        string _packageId;

        Debug.TraceDependencies($"Trying to parse '{root.OuterXml}'");

        // can have 1, 2 or 3 parts
        // 1 part: packageId only.
        // 2 parts: packageId op:version     || where version is attached to the op, e.g. >1.0.0
        // 3 parts: packageId op version
        // Multi-word mod names (e.g. "Colored Mood Bars") would split into 3 parts with the
        // last two misinterpreted as a version range. Catch any Range parse failure and fall back
        // to treating the whole inner text as the package identifier.
        switch (parts.Length)
        {
            case 1:
                _packageId = parts[0];
                break;
            case 2:
                try
                {
                    _packageId = parts[0];
                    Range = new Range(parts[1], true);
                }
                catch
                {
                    _packageId = root.InnerText;
                }

                break;
            case 3:
                try
                {
                    _packageId = parts[0];
                    Range = new Range(parts.Skip(1).StringJoin(""));
                }
                catch
                {
                    _packageId = root.InnerText;
                }

                break;
            default:
                _packageId = root.InnerText;
                break;
        }

        TryParseIdentifier(_packageId, root);
    }
}