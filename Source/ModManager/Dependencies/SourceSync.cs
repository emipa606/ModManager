// SourceSync.cs
// Copyright Karel Kroeze, 2020-2020

using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace ModManager;

public class SourceSync : Dependency
{
    private string _sourceHash;

    public SourceSync(Manifest parent, string packageId) : base(parent, packageId)
    {
    }

    public SourceSync(Manifest parent, ModDependency depend) : base(parent, depend)
    {
    }

    public override ModMetaData Target
    {
        get
        {
            if (_targetResolved)
            {
                return _target;
            }

            _target = ModLister.GetModWithIdentifier(packageId);
            _targetResolved = true;
            return _target;
        }
    }

    private string SourceHash => _sourceHash ??= Target.RootDir.GetFolderHash();

    public override Color Color => IsSatisfied ? Color.white : GenUI.MouseoverColor;

    public override string Tooltip => IsSatisfied ? I18n.XIsUpToDate(parent.Mod) : I18n.YHasUpdated(Target);

    public override List<FloatMenuOption> Resolvers
    {
        get
        {
            var options = Utilities.NewOptionsList;
            options.Add(new FloatMenuOption(I18n.UpdateLocalCopy(parent.Mod),
                () => IO.TryUpdateLocalCopy(Target, parent.Mod)));
            return options;
        }
    }

    protected override bool CheckSatisfied()
    {
        return parent.Mod.UserData().SourceHash == SourceHash;
    }
}