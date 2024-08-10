// LoadOrder.cs
// Copyright Karel Kroeze, -2020

using UnityEngine;
using Verse;

namespace ModManager;

public abstract class LoadOrder : Dependency
{
    protected LoadOrder(Manifest parent, string packageId) : base(parent, packageId)
    {
    }

    protected LoadOrder(Manifest parent, ModDependency _depend) : base(parent, _depend)
    {
    }

    public override bool IsApplicable => (parent?.Mod?.Active ?? false) && (Target?.Active ?? false);

    public override Color Color => IsSatisfied ? Color.white : Color.red;

    public override int Severity => IsSatisfied ? 0 : 3;
}