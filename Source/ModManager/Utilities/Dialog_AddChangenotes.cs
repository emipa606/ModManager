using System;
using UnityEngine;
using Verse;

namespace ModManager;

public class Dialog_AddChangenotes : Dialog_MessageBox
{
    public Dialog_AddChangenotes(Action acceptAction)
        : base("Fluffy.ModManager.AddChangenote".Translate(), "Confirm".Translate(), acceptAction, "GoBack".Translate(),
            null, null, true, acceptAction)
    {
        Workshop.CurrentChangenote = "";
    }

    public override void OnAcceptKeyPressed()
    {
    }

    public override void DoWindowContents(Rect inRect)
    {
        base.DoWindowContents(inRect);
        var topLeft = new Vector2(inRect.x + 10f, inRect.y + 10f + 35f);
        Widgets.Label(new Rect(topLeft, new Vector2(inRect.width - 20f, 35f)),
            "Fluffy.ModManager.AddChangenoteInfo".Translate());
        topLeft.y += 35f;
        Workshop.CurrentChangenote =
            Widgets.TextArea(new Rect(topLeft, new Vector2(inRect.width - 20, inRect.height / 2)),
                Workshop.CurrentChangenote);
    }
}