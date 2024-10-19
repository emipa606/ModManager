// Resources.cs
// Copyright Karel Kroeze, 2018-2018

using System.Linq;
using UnityEngine;
using Verse;

namespace ModManager;

[StaticConstructorOnStartup]
public class Resources
{
    public static Color SlightlyDarkBackground;

    public static Texture2D Close;

    public static readonly Texture2D EyeOpen;

    public static readonly Texture2D EyeClosed;

    public static readonly Texture2D Search;

    public static readonly Texture2D Steam;

    public static readonly Texture2D Ludeon;

    public static readonly Texture2D Folder;

    public static readonly Texture2D File;

    public static readonly Texture2D Warning;

    public static Texture2D Question;

    public static readonly Texture2D Palette;

    public static readonly Texture2D Gear;

    public static readonly Texture2D Status_Cross;

    public static Texture2D Status_Down;

    public static readonly Texture2D Status_Up;

    public static readonly Texture2D Status_Plus;

    public static readonly Texture2D Check;

    public static readonly Texture2D Wand;

    public static readonly Texture2D[] Spinner;

    public static readonly Color WindowBGBorderColor = new ColorInt(97, 108, 122).ToColor;

    static Resources()
    {
        SlightlyDarkBackground = new Color(0f, 0f, 0f, .2f);
        Close = ContentFinder<Texture2D>.Get("UI/Icons/Close");
        EyeOpen = ContentFinder<Texture2D>.Get("UI/Icons/EyeOpen");
        EyeClosed = ContentFinder<Texture2D>.Get("UI/Icons/EyeClosed");
        Search = ContentFinder<Texture2D>.Get("UI/Icons/Search");
        Steam = ContentFinder<Texture2D>.Get("UI/Icons/ContentSources/SteamWorkshop");
        Ludeon = ContentFinder<Texture2D>.Get("UI/Icons/Ludeon");
        File = ContentFinder<Texture2D>.Get("UI/Icons/File");
        Folder = ContentFinder<Texture2D>.Get("UI/Icons/ContentSources/ModsFolder");
        Warning = ContentFinder<Texture2D>.Get("UI/Icons/Warning");
        Question = ContentFinder<Texture2D>.Get("UI/Icons/Question");
        // the joys of case-unaware file systems - I now don't know which version is out there...
        Palette = ContentFinder<Texture2D>.Get("UI/Icons/Palette", false);
        Palette ??= ContentFinder<Texture2D>.Get("UI/Icons/palette");
        Gear = ContentFinder<Texture2D>.Get("UI/Icons/Gear");
        Check = ContentFinder<Texture2D>.Get("UI/Widgets/CheckOn");
        Wand = ContentFinder<Texture2D>.Get("UI/Icons/Wand");

        Status_Cross = ContentFinder<Texture2D>.Get("UI/Icons/Status/Cross");
        Status_Down = ContentFinder<Texture2D>.Get("UI/Icons/Status/Down");
        Status_Up = ContentFinder<Texture2D>.Get("UI/Icons/Status/Up");
        Status_Plus = ContentFinder<Texture2D>.Get("UI/Icons/Status/Plus");

        Spinner = ContentFinder<Texture2D>.GetAllInFolder("UI/Icons/Spinner").ToArray();
    }

    public static void DrawWindowBackground(Rect rect, Color bgColor)
    {
        GUI.color = Widgets.WindowBGFillColor;
        GUI.DrawTexture(rect, BaseContent.WhiteTex);
        GUI.color = bgColor;
        GUI.DrawTexture(rect, BaseContent.WhiteTex);
        GUI.color = WindowBGBorderColor;
        Widgets.DrawBox(rect);
        GUI.color = Color.white;
    }
}