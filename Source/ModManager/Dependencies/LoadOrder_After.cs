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
            var targetManifest = Target?.GetManifest();
            var targetButton = targetManifest?.Button;
            if (targetButton == null)
            {
                return options;
            }

            options.Add(new FloatMenuOption(I18n.MoveAfter(parent.Button, targetButton),
                () => ModButtonManager.MoveAfter(parent.Button, targetButton)));
            options.Add(new FloatMenuOption(I18n.MoveBefore(targetButton, parent.Button),
                () => ModButtonManager.MoveBefore(targetButton, parent.Button)));

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