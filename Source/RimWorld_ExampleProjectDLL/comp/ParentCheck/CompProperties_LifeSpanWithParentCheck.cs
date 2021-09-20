using System;
using Verse;
using RimWorld;

namespace StoneCampFire
{
    public class CompProperties_LifeSpanWithParentCheck : CompProperties
    {
        public int lifespanTicks = 100;
        public bool debug = false;

        public CompProperties_LifeSpanWithParentCheck()
        {
            compClass = typeof(CompLifeSpanWithParentCheck);
        }
    }
}