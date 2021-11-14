using RimWorld;
using System;
using UnityEngine;
using Verse;

namespace StoneCampFire
{
    public static class ToolsFire
    {
        public static void TrySpread(IntVec3 srcPos, Map map)
        {
            IntVec3 position = srcPos;
            bool flag;
            if (Rand.Chance(0.8f))
            {
                position = srcPos + GenRadial.ManualRadialPattern[Rand.RangeInclusive(1, 8)];
                flag = true;
            }
            else
            {
                position = srcPos + GenRadial.ManualRadialPattern[Rand.RangeInclusive(10, 20)];
                flag = false;
            }
            if (!position.InBounds(map) || !Rand.Chance(FireUtility.ChanceToStartFireIn(position, map)))
            {
                return;
            }
            if (!flag)
            {
                CellRect startRect = CellRect.SingleCell(srcPos);
                CellRect endRect = CellRect.SingleCell(position);
                if (GenSight.LineOfSight(srcPos, position, map, startRect, endRect))
                {
                    ((Spark)GenSpawn.Spawn(ThingDefOf.Spark, srcPos, map)).Launch(null, position, position, ProjectileHitFlags.All);
                }
            }
            else
            {
                FireUtility.TryStartFireIn(position, map, 0.1f);
            }
        }
    }
}