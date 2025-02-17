using System.Collections.Generic;
using System.Xml;
using Verse;

namespace ModManager;

public class LoadOrder_After(Manifest parent, string packageId) : LoadOrder(parent, packageId)
{
    public LoadOrder_After() : this(null, string.Empty)
    {
    }

    public override List<FloatMenuOption> Resolvers
    {
        get
        {
            var options = Utilities.NewOptionsList;
            options.Add(new FloatMenuOption(I18n.MoveAfter(parent.Button, Target.GetManifest().Button),
                () => ModButtonManager.MoveAfter(
                    parent.Button, Target.GetManifest().Button)));
            options.Add(new FloatMenuOption(I18n.MoveBefore(Target.GetManifest().Button, parent.Button),
                () => ModButtonManager.MoveBefore(
                    Target.GetManifest().Button, parent.Button)));
            return options;
        }
    }

    public override string Tooltip
    {
        get
        {
            if (!IsApplicable)
            {
                return "Not applicable";
            }

            return IsSatisfied
                ? I18n.LoadedAfter(Target.Name)
                : I18n.ShouldBeLoadedAfter(Target.Name);
        }
    }

    public override string RequirementTypeLabel => "loadOrder".Translate();

    protected override bool CheckSatisfied()
    {
        var mods = ModButtonManager.ActiveMods;
        if (!Target.Active)
        {
            return true;
        }

        return Target is { Active: true } &&
               parent.Mod.Active &&
               mods.IndexOf(Target) < mods.IndexOf(parent.Mod);
    }

    public void LoadDataFromXmlCustom(XmlNode root)
    {
        var text = root.InnerText.Trim();
        TryParseIdentifier(text, root);
    }
}