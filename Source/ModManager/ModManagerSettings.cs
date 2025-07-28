using ColourPicker;
using UnityEngine;
using Verse;

namespace ModManager;

public class ModManagerSettings : ModSettings
{
    public bool AddExpansionsToNewModLists = true;
    public bool AddHugsLibToNewModLists;
    public bool AddModManagerToNewModLists = true;
    public Color BackgroundColor = new(0f, 0f, 0f, 0.1f);

    public bool ShowPromotions = true;
    public bool ShowPromotions_NotActive;
    public bool ShowPromotions_NotSubscribed = true;
    public bool ShowSatisfiedRequirements;
    public bool ShowVersionChecksOnSteamMods;
    public bool SkipPublishingCountdown;

    private bool SurveyNotificationShown;
    public bool TrimTags = true;
    public bool TrimVersionStrings;
    public bool UseTempFolderForCrossPromotions;

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref ShowPromotions, "ShowPromotions", true);
        Scribe_Values.Look(ref SkipPublishingCountdown, "SkipPublishingCountdown");
        Scribe_Values.Look(ref ShowPromotions_NotSubscribed, "ShowPromotions_NotSubscribed", true);
        Scribe_Values.Look(ref ShowPromotions_NotActive, "ShowPromotions_NotActive");
        Scribe_Values.Look(ref TrimTags, "TrimTags", true);
        Scribe_Values.Look(ref TrimVersionStrings, "TrimVersionStrings");
        Scribe_Values.Look(ref AddHugsLibToNewModLists, "AddHugsLibToNewModLists", true);
        Scribe_Values.Look(ref AddModManagerToNewModLists, "AddModManagerToNewModLists", true);
        Scribe_Values.Look(ref AddExpansionsToNewModLists, "AddExpansionsToNewModLists", true);
        Scribe_Values.Look(ref ShowSatisfiedRequirements, "ShowSatisfiedRequirements");
        Scribe_Values.Look(ref ShowVersionChecksOnSteamMods, "ShowVersionChecksOnSteamMods");
        Scribe_Values.Look(ref UseTempFolderForCrossPromotions, "UseTempFolderForCrossPromotions");
        Scribe_Values.Look(ref SurveyNotificationShown, "SurveyNotificationShown");
        Scribe_Values.Look(ref BackgroundColor, "BackgroundColor", new Color(0f, 0f, 0f, 0.1f));
    }


    public void DoWindowContents(Rect canvas)
    {
        var listing = new Listing_Standard
        {
            ColumnWidth = canvas.width
        };
        listing.Begin(canvas);
        listing.CheckboxLabeled(I18n.ShowAllRequirements, ref ShowSatisfiedRequirements,
            I18n.ShowAllRequirementsTip);
        listing.CheckboxLabeled(I18n.ShowVersionChecksForSteamMods, ref ShowVersionChecksOnSteamMods,
            I18n.ShowVersionChecksForSteamModsTip);

        listing.Gap();
        listing.CheckboxLabeled(I18n.ShowPromotions, ref ShowPromotions, I18n.ShowPromotionsTip);

        if (!ShowPromotions)
        {
            GUI.color = Color.grey;
        }

        listing.CheckboxLabeled(I18n.ShowPromotions_NotSubscribed, ref ShowPromotions_NotSubscribed);
        listing.CheckboxLabeled(I18n.ShowPromotions_NotActive, ref ShowPromotions_NotActive);
        var before = UseTempFolderForCrossPromotions;
        if (CrossPromotionManager.CachePathOverriden)
        {
            GUI.color = Color.grey;
        }

        listing.CheckboxLabeled(I18n.UseTempFolderForCrossPromotionCache, ref UseTempFolderForCrossPromotions,
            I18n.UseTempFolderForCrossPromotionCacheTip);
        if (before != UseTempFolderForCrossPromotions)
        {
            CrossPromotionManager.Notify_CrossPromotionPathChanged();
        }

        if (CrossPromotionManager.CacheCount > 0)
        {
            GUI.color = Color.white;
            if (listing.ButtonTextLabeled(I18n.CrossPromotionCacheFolderSize(CrossPromotionManager.CacheSize),
                    I18n.DeleteCrossPromotionCache))
            {
                CrossPromotionManager.DeleteCache();
            }
        }
        else
        {
            GUI.color = Color.grey;
            listing.Label(I18n.CrossPromotionCacheFolderSize(CrossPromotionManager.CacheSize));
        }

        GUI.color = Color.white;
        listing.Gap();

        listing.CheckboxLabeled(I18n.TrimTags, ref TrimTags, I18n.TrimTagsTip);
        if (!TrimTags)
        {
            GUI.color = Color.grey;
        }

        listing.CheckboxLabeled(I18n.TrimVersionStrings, ref TrimVersionStrings,
            I18n.TrimVersionStringsTip);

        GUI.color = Color.white;
        listing.Gap();
        listing.CheckboxLabeled(I18n.AddModManagerToNewModList, ref AddModManagerToNewModLists,
            I18n.AddModManagerToNewModListTip);
        listing.CheckboxLabeled(I18n.AddHugsLibToNewModList, ref AddHugsLibToNewModLists,
            I18n.AddHugsLibToNewModListTip);
        listing.CheckboxLabeled(I18n.AddExpansionsToNewModList, ref AddExpansionsToNewModLists,
            I18n.AddExpansionsToNewModListTip);
        listing.Gap();
        listing.CheckboxLabeled("Fluffy.ModManager.SkipPublishingCountdown".Translate(), ref SkipPublishingCountdown);

        var colorRect = listing.GetRect(30f);
        Widgets.Label(colorRect.LeftHalf(), "Fluffy.ModManager.BackgroundColor".Translate());
        Widgets.DrawBoxSolidWithOutline(colorRect.RightHalf().RightHalf(), BackgroundColor,
            Resources.WindowBGBorderColor, 2);
        if (Widgets.ButtonInvisible(colorRect.RightHalf().RightHalf()))
        {
            Find.WindowStack.Add(new Dialog_ColourPicker(BackgroundColor,
                color => { BackgroundColor = color; }));
        }

        listing.GapLine();
        if (listing.ButtonTextLabeledPct("Fluffy.ModManager.ResetLabel".Translate(),
                "Fluffy.ModManager.Reset".Translate(), 0.75f))
        {
            Reset();
        }

        if (ModManager.CurrentVersion != null)
        {
            listing.Gap();
            GUI.contentColor = Color.gray;
            listing.Label("Fluffy.ModManager.ModVersion".Translate(ModManager.CurrentVersion));
            GUI.contentColor = Color.white;
        }

        listing.End();
    }

    private void Reset()
    {
        ShowPromotions = true;
        ShowPromotions_NotSubscribed = true;
        ShowPromotions_NotActive = false;
        ShowSatisfiedRequirements = false;
        ShowVersionChecksOnSteamMods = false;
        TrimTags = true;
        TrimVersionStrings = false;
        AddHugsLibToNewModLists = true;
        AddModManagerToNewModLists = true;
        AddExpansionsToNewModLists = true;
        UseTempFolderForCrossPromotions = false;
        BackgroundColor = new Color(0f, 0f, 0f, 0.1f);
    }
}