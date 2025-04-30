using RimWorld;
using UnityEngine;
using Verse;

namespace ModManager;

public class Dialog_Import_FromString : Dialog_ImExport_String
{
    public override void DoWindowContents(Rect inRect)
    {
        var textAreaRect = inRect.TopPartPixels(inRect.height - Margin - 32);
        var importButtonRect = inRect.BottomPartPixels(32).RightPartPixels(200);

        GUI.SetNextControlName("content");
        Content = GUI.TextArea(textAreaRect, Content ?? "");

        if (GUI.changed)
        {
            Notify_ModListChanged();
        }

        if (!Widgets.ButtonText(importButtonRect, I18n.Import))
        {
            return;
        }

        ModList.Import(Event.current.shift);
        Messages.Message(I18n.XModsImportedFromString(ModList.Mods.Count), MessageTypeDefOf.TaskCompletion, false);
        Close();
    }

    public void Notify_ModListChanged()
    {
        try
        {
            ModList = ModList.FromXml(Content);
        }
        catch
        {
            ModList = null;
        }
    }
}