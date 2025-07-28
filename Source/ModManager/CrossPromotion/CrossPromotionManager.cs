// Copyright Karel Kroeze, 2020-2021.
// ModManager/ModManager/CrossPromotionManager.cs

using System.Collections.Generic;
using System.IO;
using System.Linq;
using RimWorld;
using Steamworks;
using UnityEngine;
using Verse;
using Verse.Steam;
using static ModManager.Constants;
using static ModManager.Resources;

namespace ModManager;

public static class CrossPromotionManager
{
    private static AppId_t appId = AppId_t.Invalid;

    private static readonly Dictionary<PublishedFileId_t, AccountID_t> authorForMod = new();

    private static int? cacheCount;


    private static string cachePath;

    private static long? cacheSize;
    private static readonly HashSet<PublishedFileId_t> currentlyFetchingFiles = [];
    private static readonly bool enabled;
    private static readonly CallResult<SteamUGCQueryCompleted_t> modDetailsCallResult;

    private static readonly Dictionary<AccountID_t, List<CrossPromotion>> modsForAuthor = new();

    private static Vector2 scrollPosition = Vector2.zero;
    private static readonly CallResult<SteamUGCQueryCompleted_t> userModsCallResult;
    internal static bool CachePathOverriden;

    static CrossPromotionManager()
    {
        if (!SteamManager.Initialized)
        {
            return;
        }

        enabled = true;
        userModsCallResult = CallResult<SteamUGCQueryCompleted_t>.Create(OnUserModsReceived);
        modDetailsCallResult = CallResult<SteamUGCQueryCompleted_t>.Create(OnModDetailsReceived);
    }

    private static AppId_t AppID
    {
        get
        {
            if (enabled && appId == AppId_t.Invalid)
            {
                appId = SteamUtils.GetAppID();
            }

            return appId;
        }
    }

    public static int CacheCount => cacheCount ??= new DirectoryInfo(CachePath).GetFiles().Length;

    internal static string CachePath
    {
        get
        {
            if (cachePath != null)
            {
                return cachePath;
            }

            if (GenCommandLine.TryGetCommandLineArg("cross-promotions-path", out var path))
            {
                path = path.TrimEnd('\\', '/');
                if (path == "")
                {
                    path = Path.DirectorySeparatorChar.ToString();
                }

                CachePathOverriden = true;
                Log.Message($"CrossPromotion preview images location overriden: {path}");
            }
            else
            {
                path = ModManager.Settings.UseTempFolderForCrossPromotions
                    ? Path.Combine(Path.GetTempPath(), "CrossPromotions")
                    : Path.Combine(GenFilePaths.SaveDataFolderPath, "CrossPromotions");
            }

            var dir = new DirectoryInfo(path);
            if (!dir.Exists)
            {
                dir.Create();
            }

            return path;
        }
    }

    public static long CacheSize => cacheSize ??= new DirectoryInfo(CachePath).GetFiles().Sum(f => f.Length);

    private static List<CrossPromotion> RelevantPromotions { get; set; }

    private static AccountID_t? AuthorForMod(PublishedFileId_t fileId)
    {
        if (authorForMod.TryGetValue(fileId, out var author))
        {
            return author;
        }

        if (!currentlyFetchingFiles.Contains(fileId))
        {
            fetchModDetails(fileId);
        }

        return null;
    }

    internal static void DeleteCache()
    {
        Find.WindowStack.Add(new Dialog_MessageBox(
            I18n.ConfirmDeletingCrossPromotionCache(
                CachePath, CacheCount, CacheSize),
            "Confirm".Translate(),
            () =>
            {
                var dir = new DirectoryInfo(CachePath);
                dir.Delete(true);
                Notify_CrossPromotionPathChanged();
            }, "Cancel".Translate(), buttonADestructive: true));
    }

    private static void drawCrossPromotions(ref Rect canvas, IEnumerable<CrossPromotion> promotions)
    {
        var backgroundRect = new Rect(
            canvas.xMin,
            canvas.yMin,
            canvas.width,
            PromotionsHeight);
        var outRect = backgroundRect.ContractedBy(SmallMargin / 2f);
        var height = (int)outRect.height;
        var width = promotions.Sum(p => p.NormalizedWidth(height)) + ((promotions.Count() - 1) * SmallMargin);
        if (width > outRect.width)
        {
            height -= 16;
            // recalculate total width
            width = promotions.Sum(p => p.NormalizedWidth(height)) + ((promotions.Count() - 1) * SmallMargin);
        }

        var viewRect = new Rect(
            canvas.xMin,
            canvas.yMin,
            width,
            height);
        var pos = viewRect.min;
        canvas.yMin += PromotionsHeight + SmallMargin;

        Widgets.DrawBoxSolid(backgroundRect, SlightlyDarkBackground);
        if (Mouse.IsOver(outRect) && Event.current.type == EventType.ScrollWheel)
        {
            scrollPosition.x += Event.current.delta.y * ScrollSpeed;
        }

        Widgets.BeginScrollView(outRect, ref scrollPosition, viewRect);
        foreach (var promotion in promotions)
        {
            var normalizedWidth = promotion.NormalizedWidth(height);
            var rect = new Rect(pos.x, pos.y, normalizedWidth, height);
            if (Widgets.ButtonImage(rect, promotion.Preview, new Color(.9f, .9f, .9f), Color.white))
            {
                if (!promotion.Installed)
                {
                    var options = Utilities.NewOptionsList;
                    options.Add(new FloatMenuOption(I18n.Subscribe(promotion.Name),
                        () => Workshop.Subscribe(promotion.FileId)));
                    options.Add(new FloatMenuOption(I18n.WorkshopPage(promotion.Name),
                        () => SteamUtility.OpenWorkshopPage(promotion.FileId)));
                    Utilities.FloatMenu(options);
                }
                else
                {
                    var button = ModButtonManager.AllButtons.FirstOrDefault(b => b.Name == promotion.Name);
                    if (button != null)
                    {
                        Page_BetterModConfig.Instance.Selected = button;
                    }
                }
            }

            TooltipHandler.TipRegion(rect, $"{promotion.Name}\n\n{promotion.Description}");
            pos.x += normalizedWidth + SmallMargin;
        }

        Widgets.EndScrollView();
    }

    private static void fetchModDetails(PublishedFileId_t fileId)
    {
        Debug.TracePromotions($"Fetching details for {fileId}...");
        currentlyFetchingFiles.Add(fileId);
        var query = SteamUGC.CreateQueryUGCDetailsRequest([fileId], 1);
        var request = SteamUGC.SendQueryUGCRequest(query);
        modDetailsCallResult.Set(request);
    }

    private static void fetchModsForAuthor(AccountID_t author)
    {
        Debug.TracePromotions($"Fetching mods for {author}...");
        var query = SteamUGC.CreateQueryUserUGCRequest(
            author,
            EUserUGCList.k_EUserUGCList_Published,
            EUGCMatchingUGCType.k_EUGCMatchingUGCType_UsableInGame,
            EUserUGCListSortOrder.k_EUserUGCListSortOrder_VoteScoreDesc,
            AppID, AppID, 1);
        SteamUGC.AddRequiredTag(query, $"{VersionControl.CurrentMajor}.{VersionControl.CurrentMinor}");
        var request = SteamUGC.SendQueryUGCRequest(query);
        userModsCallResult.Set(request);
    }

    public static void HandleCrossPromotions(ref Rect canvas, ModMetaData mod)
    {
        if (!enabled)
        {
            return;
        }

        if (!ModManager.Settings.ShowPromotions || !Manifest.For(mod).showCrossPromotions)
        {
            return;
        }

        if (mod.GetPublishedFileId() == PublishedFileId_t.Invalid)
        {
            return;
        }

        var author = AuthorForMod(mod.GetPublishedFileId());
        if (author == null)
        {
            return;
        }

        RelevantPromotions ??= promotionsForAuthor(author.Value)?.Where(p => p.ShouldShow).ToList();
        if (RelevantPromotions.NullOrEmpty())
        {
            return;
        }

        if (Widgets.ButtonImage(
                new Rect(canvas.xMax - SmallIconSize, canvas.yMin, SmallIconSize, SmallIconSize), Gear, Color.grey,
                GenUI.MouseoverColor))
        {
            Utilities.OpenSettingsFor(ModManager.Instance);
        }

        Utilities.DoLabel(ref canvas, I18n.PromotionsFor(mod.AuthorsString));
        drawCrossPromotions(ref canvas, RelevantPromotions);
    }

    public static void Notify_CrossPromotionPathChanged()
    {
        cachePath = null;
        cacheCount = null;
        cacheSize = null;
    }

    public static void Notify_UpdateRelevantMods()
    {
        RelevantPromotions = null;
    }

    private static void OnModDetailsReceived(SteamUGCQueryCompleted_t result, bool failure)
    {
        Debug.Log(
            $"Received mod details: failure: {failure}, result: {result.m_eResult}, count: {result.m_unNumResultsReturned}");
        for (uint i = 0; i < result.m_unNumResultsReturned; i++)
        {
            if (!SteamUGC.GetQueryUGCResult(result.m_handle, i, out var details))
            {
                continue;
            }

            Debug.TracePromotions($" - {details.m_rgchTitle} ({details.m_ulSteamIDOwner}");
            var author = new CSteamID(details.m_ulSteamIDOwner).GetAccountID();
            authorForMod.Add(details.m_nPublishedFileId, author);
            currentlyFetchingFiles.Remove(details.m_nPublishedFileId);
        }

        SteamUGC.ReleaseQueryUGCRequest(result.m_handle);
    }

    private static void OnUserModsReceived(SteamUGCQueryCompleted_t result, bool failure)
    {
        Debug.Log(
            $"Received user mods: failure: {failure}, result: {result.m_eResult}, count: {result.m_unNumResultsReturned}");
        var author = CSteamID.Nil;
        var promotions = new List<CrossPromotion>();
        for (uint i = 0; i < result.m_unNumResultsReturned; i++)
        {
            if (!SteamUGC.GetQueryUGCResult(result.m_handle, i, out var details))
            {
                continue;
            }

            Debug.TracePromotions($" - {details.m_rgchTitle} ({details.m_ulSteamIDOwner}");
            author = new CSteamID(details.m_ulSteamIDOwner);
            promotions.Add(new CrossPromotion(details));
        }

        if (author != CSteamID.Nil)
        {
            modsForAuthor[author.GetAccountID()] = promotions;
            Notify_UpdateRelevantMods();
        }

        SteamUGC.ReleaseQueryUGCRequest(result.m_handle);
    }

    private static List<CrossPromotion> promotionsForAuthor(AccountID_t author)
    {
        if (modsForAuthor.TryGetValue(author, out var mods))
        {
            return mods;
        }

        mods = [];
        modsForAuthor.Add(author, mods);
        fetchModsForAuthor(author);
        return mods;
    }

    public static void Update()
    {
        if (enabled)
        {
            SteamAPI.RunCallbacks();
        }
    }
}