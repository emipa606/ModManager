using UnityEngine;
using Verse;

namespace ModManager;

public abstract class Dialog_ImExport_String : Window
{
    protected Dialog_ImExport_String()
    {
        closeOnClickedOutside = true;
        absorbInputAroundWindow = true;
        closeOnAccept = false;
    }

    public override Vector2 InitialSize => new(820, 506); // golden ratio-ish
    protected override float Margin => 8f;

    protected virtual string Content { get; set; }

    protected virtual ModList ModList { get; set; }

    public override void PostOpen()
    {
        base.PostOpen();

        GUI.FocusControl("content");
    }
}