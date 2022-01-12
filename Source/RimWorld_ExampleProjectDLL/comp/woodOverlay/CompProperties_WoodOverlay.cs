using Verse;
using System.Collections.Generic;
using UnityEngine;

namespace StoneCampFire
{
    public class CompProperties_WoodOverlay : CompProperties
    {
        public ThingDef coalMaterial;
        public ThingDef woodLogMaterial;

        public bool debug;

        public CompProperties_WoodOverlay()
        {
            compClass = typeof(CompWoodOverlay);
        }
    }
}