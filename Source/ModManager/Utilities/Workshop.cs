// Workshop.cs
// Copyright Karel Kroeze, 2018-2018

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using HarmonyLib;
using RimWorld;
using Steamworks;
using Verse;
using Verse.Sound;

namespace ModManager;

public static class Workshop
{
    public static string CurrentChangenote;
    private static CallResult<SteamUGCQueryCompleted_t> OnSteamUGCQueryCompletedCallResult;
    private static SteamUGCQueryCompleted_t collectionQueryResult;

    public static void Unsubscribe(ModMetaData mod, bool force = false)
    {
        if (force)
        {
            mod.enabled = false;
            AccessTools.Method(typeof(Verse.Steam.Workshop), "Unsubscribe", [typeof(PublishedFileId_t)])
                .Invoke(null, [mod.GetPublishedFileId()]);
            return;
        }

        Find.WindowStack.Add(Dialog_MessageBox.CreateConfirmation(I18n.ConfirmUnsubscribe(mod.Name),
            delegate { Unsubscribe(mod, true); }, true));
    }

    private static void Unsubscribe(IEnumerable<ModMetaData> mods)
    {
        var modList = mods
            .Select(m => $"{m.Name} ({m.SupportedVersionsReadOnly.Select(v => v.ToString()).StringJoin(", ")})")
            .ToLineList();
        var dialog = Dialog_MessageBox.CreateConfirmation(
            I18n.MassUnSubscribeConfirm(mods.Count(), modList),
            () =>
            {
                foreach (var mod in mods)
                {
                    Unsubscribe(mod, true);
                }
            },
            true);
        Find.WindowStack.Add(dialog);
    }

    public static void Subscribe(PublishedFileId_t fileId)
    {
        SteamUGC.SubscribeItem(fileId);
    }

    public static void Subscribe(string identifier)
    {
        Subscribe(new PublishedFileId_t(ulong.Parse(identifier)));
    }

    public static void Subscribe(IEnumerable<string> identifiers)
    {
        foreach (var identifier in identifiers)
        {
            Subscribe(identifier);
        }
    }

    public static void Upload(ModMetaData mod)
    {
        var publishedFileid = mod.GetPublishedFileId();
        try
        {
            if (!mod.translationMod && publishedFileid != PublishedFileId_t.Invalid)
            {
                OnSteamUGCQueryCompletedCallResult =
                    CallResult<SteamUGCQueryCompleted_t>.Create(OnSteamUGCQueryCompleted);
                collectionQueryResult = new SteamUGCQueryCompleted_t();
                var UGCDetailsRequest = SteamUGC.CreateQueryUGCDetailsRequest([publishedFileid], 1);
                SteamUGC.SetReturnLongDescription(UGCDetailsRequest, true);
                SteamUGC.SetReturnChildren(UGCDetailsRequest, true);
                var createQueryUGCDetailsRequest = SteamUGC.SendQueryUGCRequest(UGCDetailsRequest);
                OnSteamUGCQueryCompletedCallResult.Set(createQueryUGCDetailsRequest);
                while (collectionQueryResult.m_eResult == EResult.k_EResultNone)
                {
                    Thread.Sleep(50);
                    SteamAPI.RunCallbacks();
                }

                if (collectionQueryResult.m_eResult == EResult.k_EResultOK &&
                    SteamUGC.GetQueryUGCResult(UGCDetailsRequest, 0, out var details))
                {
                    mod.translationMod = details.m_rgchTags.Split(',').Contains("Translation");
                }
            }
        }
        catch
        {
            // ignored
        }

        Find.WindowStack.Add(new Dialog_ConfirmModUpload(mod, delegate
        {
            if (publishedFileid != PublishedFileId_t.Invalid)
            {
                Find.WindowStack.Add(new Dialog_AddChangenotes(AcceptAction));
                return;
            }

            AcceptAction();
            return;

            void AcceptAction()
            {
                SoundDefOf.Tick_High.PlayOneShotOnCamera();
                var dialogMessageBox = Dialog_MessageBox.CreateConfirmation("ConfirmContentAuthor".Translate(), delegate
                {
                    SoundDefOf.Tick_High.PlayOneShotOnCamera();
                    AccessTools.Method(typeof(Verse.Steam.Workshop), "Upload").Invoke(null, [mod]);
                }, true);
                dialogMessageBox.buttonAText = "Yes".Translate();
                dialogMessageBox.buttonBText = "No".Translate();
                if (!ModManager.Settings.SkipPublishingCountdown)
                {
                    dialogMessageBox.interactionDelay = 6f;
                }

                Find.WindowStack.Add(dialogMessageBox);
            }
        }));
    }

    public static void MassUnsubscribeFloatMenu()
    {
        var options = Utilities.NewOptionsList;
        var steamMods = ModButtonManager.AllMods.Where(m => m.Source == ContentSource.SteamWorkshop);
        var outdated = steamMods.Where(m => !m.VersionCompatible && !m.MadeForNewerVersion);
        var inactive = ModButtonManager.AvailableMods.Where(m => m.Source == ContentSource.SteamWorkshop);

        options.Add(new FloatMenuOption(I18n.MassUnSubscribeAll, () => Unsubscribe(steamMods)));
        if (outdated.Any())
        {
            options.Add(new FloatMenuOption(I18n.MassUnSubscribeOutdated, () => Unsubscribe(outdated)));
        }

        if (inactive.Any())
        {
            options.Add(new FloatMenuOption(I18n.MassUnSubscribeInactive, () => Unsubscribe(inactive)));
        }

        Utilities.FloatMenu(options);
    }

    private static void OnSteamUGCQueryCompleted(SteamUGCQueryCompleted_t pCallback, bool bIOFailure)
    {
        collectionQueryResult = pCallback;
    }
}