// Extensions.cs
// Copyright Karel Kroeze, 2018-2018

using UnityEngine;
using Verse;

namespace FluffyUIReduced;

public static class Extensions
{
    extension(Rect rect)
    {
        public Rect ContractedBy(Vector2 vector)
        {
            return new Rect(
                rect.xMin + vector.x,
                rect.yMin + vector.y,
                rect.width - (vector.x * 2),
                rect.height - (vector.y * 2));
        }

        public Rect CenteredIn(Rect parent)
        {
            return rect.CenteredOnXIn(parent).CenteredOnYIn(parent);
        }
    }
}