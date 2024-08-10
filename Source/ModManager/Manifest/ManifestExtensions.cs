using Verse;

namespace ModManager;

public static class ManifestExtensions
{
    public static Manifest GetManifest(this ModMetaData mod)
    {
        return Manifest.For(mod);
    }
}