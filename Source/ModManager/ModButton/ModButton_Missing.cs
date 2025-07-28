// ModButton_Missing.cs
// Copyright Karel Kroeze, 2018-2018

using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using static ModManager.Constants;

namespace ModManager;

public sealed class ModButton_Missing(string id, string name, ulong steamWorkshopId) : ModButton
{
    public override string Name => name;

    public override string Identifier => id;
    public override ulong SteamWorkshopId => steamWorkshopId;

    public override bool Active
    {
        get => true;
        set
        {
            if (!value)
            {
                ModButtonManager.TryRemove(this);
            }
        }
    }

    private Color Color => Color.gray;

    public override IEnumerable<Dependency> Requirements => Manifest.EmptyRequirementList;


    public override bool SamePackageId(string packageId)
    {
        return false;
    }

    public override void DoModButton(Rect canvas, bool alternate = false, Action clickAction = null,
        Action doubleClickAction = null,
        bool deemphasizeFiltered = false, string filter = null)
    {
        base.DoModButton(canvas, alternate, clickAction, doubleClickAction, deemphasizeFiltered, filter);
        canvas = canvas.ContractedBy(SmallMargin / 2f);

        var nameRect = new Rect(
            canvas.xMin,
            canvas.yMin,
            canvas.width - (SmallIconSize * 2) - SmallMargin,
            canvas.height * 2 / 3f);

        var deemphasized = deemphasizeFiltered && !filter.NullOrEmpty() && MatchesFilter(filter) <= 0;

        Text.Anchor = TextAnchor.MiddleLeft;
        Text.Font = GameFont.Small;
        GUI.color = deemphasized ? Color.Desaturate() : Color;
        Widgets.Label(nameRect, Name.Truncate(nameRect.width, _modNameTruncationCache));
        GUI.color = Color.white;
        Text.Anchor = TextAnchor.UpperLeft;
        Text.Font = GameFont.Small;

        if (Mouse.IsOver(nameRect) && Name != Name.Truncate(nameRect.width, _modNameTruncationCache))
        {
            TooltipHandler.TipRegion(nameRect, Name);
        }
    }

    internal override void DoModActionButtons(Rect canvas)
    {
    }

    internal override void DoModDetails(Rect canvas)
    {
        DrawRequirements(ref canvas);
    }
    //        {
    //            get
    //            {
    //                if ( _issues == null )
    //                {
    //                    _issues = new List<ModRequirement>();
    //                    _issues.Add( ModRequirement.MissingMod( this ) );
    //                }
    //                return _issues;
    //            }
    //        }
}