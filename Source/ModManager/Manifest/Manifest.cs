// Manifest.cs
// Copyright Karel Kroeze, 2018-2018

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Verse;

namespace ModManager;

public class Manifest
{
    private const string ManifestFileName = "Manifest.xml";
    private const string AssembliesFolder = "Assemblies/";

    private static readonly Version _zero = new(0, 0);

    private static readonly Dictionary<ModMetaData, Manifest> _manifestCache = new();

    public static readonly List<Dependency> EmptyRequirementList = [];

    // dependencies and incompatibilities are still relevant in the manifest because versioning is not a thing in vanilla
    private readonly List<VersionedDependency> dependencies = [];

    public readonly List<Dependency> Dependencies = [];
    private readonly List<Dependency> Incompatibilities = [];
    private readonly List<Incompatible> incompatibleWith = [];

    [Obsolete("dependency management has been implemented in RW from 1.1 onwards.")]
    private readonly List<LoadOrder_After> loadAfter = [];

    public readonly List<Dependency> LoadAfter = [];

    [Obsolete("dependency management has been implemented in RW from 1.1 onwards.")]
    private readonly List<LoadOrder_Before> loadBefore = [];

    public readonly List<Dependency> LoadBefore = [];
    public readonly bool showCrossPromotions = true;

    // idem for version itself, also version checking requires this
    private readonly string version;
    private ModContentPack _pack;
    private List<Dependency> _requirements;
    private Version _version;
    public string downloadUri;

    [Obsolete("mods should implement a packageId in About.xml")]
    public string identifier;

    // version checking
#pragma warning disable 649
    internal string manifestUri;
#pragma warning restore 649
    public ModMetaData Mod;

    // source mod
    public SourceSync sourceSync;

    // suggestions
    public List<string> suggests = [];

    [Obsolete("Multiple target versions have been implemented in RW since 1.0")]
    protected List<string> targetVersions;

    private VersionCheck versionCheck;

    public Manifest()
    {
    }

    private Manifest(ModMetaData mod)
    {
        Mod = mod;
    }

    public Manifest(ModMetaData mod, string version) : this(mod)
    {
        this.version = version;
    }

    private ModContentPack Pack => _pack ??=
        LoadedModManager.RunningModsListForReading.Find(mcp => Mod.SamePackageId(mcp.PackageId)) ??
        new ModContentPack(Mod.RootDir, Mod.PackageId, Mod.PackageIdPlayerFacing, int.MaxValue, Mod.Name, Mod.Official);

    public Version Version
    {
        get
        {
            if (_version == null)
            {
                SetVersion(false);
            }

            return _version;
        }
        private set => _version = value;
    }

    public bool HasVersion => Version > _zero;

    public ModButton_Installed Button => ModButton_Installed.For(Mod);

    public IEnumerable<Dependency> Requirements
    {
        get
        {
            _requirements ??= Dependencies
                .Concat(Incompatibilities)
                .Concat(LoadBefore)
                .Concat(LoadAfter)
                .Concat(versionCheck)
                .Concat(sourceSync)
                .Where(d => d != null)
                .ToList();

            return _requirements.Where(r => r.IsApplicable);
        }
    }

    public static Manifest For(ModMetaData mod)
    {
        if (mod == null)
        {
            return null;
        }

        if (_manifestCache.TryGetValue(mod, out var manifest))
        {
            return manifest;
        }

        manifest = new Manifest(mod);

        // get from file.
        var manifestPath = Path.Combine(mod.AboutDir(), ManifestFileName);

        // manifest is first choice
        if (File.Exists(manifestPath))
        {
            try
            {
                manifest = DirectXmlLoader.ItemFromXmlFile<Manifest>(manifestPath);
                manifest.Mod = mod;

                // create them data!
                manifest.Dependencies.AddRange(manifest.dependencies);
                manifest.Incompatibilities.AddRange(manifest.incompatibleWith);
#pragma warning disable 618
                manifest.LoadBefore.AddRange(manifest.loadBefore);
                manifest.LoadAfter.AddRange(manifest.loadAfter);
#pragma warning restore 618


                if (!manifest.manifestUri.NullOrEmpty())
                {
                    manifest.versionCheck = new VersionCheck(manifest);
                }
            }
            catch (Exception e)
            {
                manifest = new Manifest(mod);
                Log.Error($"Error loading manifest for '{mod.Name}':\n{e.Message}\n\n{e.StackTrace}");
            }
        }

        // copy any information from vanilla metadata
        foreach (var before in mod.LoadBefore)
        {
            if (!manifest.LoadBefore.Any(d => d.packageId == before))
            {
                manifest.LoadBefore.Add(new LoadOrder_Before(manifest, before));
            }
        }

        foreach (var after in mod.LoadAfter)
        {
            if (!manifest.LoadAfter.Any(d => d.packageId == after))
            {
                manifest.LoadAfter.Add(new LoadOrder_After(manifest, after));
            }
        }

        foreach (var incomp in mod.IncompatibleWith)
        {
            if (!manifest.Incompatibilities.Any(d => d.packageId == incomp))
            {
                manifest.Incompatibilities.Add(new Incompatible(manifest, incomp));
            }
        }

        foreach (var depend in mod.Dependencies)
        {
            if (!manifest.Dependencies.Any(d => d.packageId == depend.packageId))
            {
                manifest.Dependencies.Add(new VersionedDependency(manifest, depend));
            }
        }

        // implicit dependencies from user data
        if (mod.UserData().Source != null)
        {
            manifest.sourceSync = new SourceSync(manifest, mod.UserData().Source.PackageId);
        }

        foreach (var dependency in manifest.Dependencies
                     .Concat(manifest.Incompatibilities)
                     .Concat(manifest.LoadBefore)
                     .Concat(manifest.LoadAfter)
                     .Concat(manifest.versionCheck)
                     .Concat(manifest.sourceSync)
                     .Where(d => d != null))
        {
            dependency.parent = manifest;
            Debug.TraceDependencies(
                $"parent={dependency.parent?.Mod.ToString() ?? "NULL"}, targetId={dependency.packageId}, target={dependency.Target}, type={dependency.GetType()}");
        }

        // resolve version - if set in manifest that takes priority,
        // otherwise try to read version from assemblies.
        manifest.SetVersion();
        _manifestCache.Add(mod, manifest);
        return manifest;
    }

    public void Notify_Recache()
    {
        if (_requirements.NullOrEmpty())
        {
            return;
        }

        foreach (var dependency in _requirements)
        {
            dependency.Notify_Recache();
        }
    }

    public void Notify_ModAddedOrRemoved(ModMetaData mod)
    {
        foreach (var dependency in _requirements)
        {
            dependency.Notify_Recache();
        }
    }

    private Version ParseVersion(string version)
    {
        return ParseVersion(version, Mod);
    }

    private static Version ParseVersion(string version, ModMetaData mod)
    {
        try
        {
            return new Version(version);
        }
        catch
        {
            try
            {
                var pattern = @"[^0-9\.]";
                return new Version(Regex.Replace(version, pattern, ""));
            }
            catch (Exception e)
            {
                Log.Warning(
                    $"Failed to parse version string '{version}' for {mod?.Name ?? "??"}: {e.Message}\n\n{e.StackTrace}");
                return new Version();
            }
        }
    }

    private void SetVersion(bool fromAssemblies = true)
    {
        if (!version.NullOrEmpty())
        {
            // if version was set, this is simple
            Version = ParseVersion(version);
        }
        else if (fromAssemblies)
        {
            // Always get Assembly FILE Version, as the actual assembly version may be intentionally kept static so as not to break references.
            var assemblies =
                ModContentPack.GetAllFilesForMod(Pack, AssembliesFolder, ext => ext.ToLower() == ".dll");

            if (assemblies.Any())
            {
                Version = ParseVersion(FileVersionInfo
                    .GetVersionInfo(assemblies.Last().Value.FullName).FileVersion);
            }
        }
        else
        {
            Version = new Version(0, 0, 0);
        }
    }
}