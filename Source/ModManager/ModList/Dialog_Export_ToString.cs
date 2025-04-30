using RimWorld;
using UnityEngine;
using Verse;

namespace ModManager;

public class Dialog_Export_ToString(ModList modlist) : Dialog_ImExport_String
{
    private readonly string _content = modlist.ToXml();

    protected override ModList ModList => modlist;

    protected override string Content => _content;

    public override void DoWindowContents(Rect inRect)
    {
        var textAreaRect = inRect.TopPartPixels(inRect.height - Margin - 32);
        var importButtonRect = inRect.BottomPartPixels(32).RightPartPixels(200);

        GUI.SetNextControlName("content");
        GUI.TextArea(textAreaRect, Content ?? "");

        if (!Widgets.ButtonText(importButtonRect, I18n.CopyToClipboard))
        {
            return;
        }

        GUIUtility.systemCopyBuffer = Content;
        Messages.Message(I18n.XModsExportedToString(ModList.Mods.Count), MessageTypeDefOf.TaskCompletion, false);
        Close();
    }
}