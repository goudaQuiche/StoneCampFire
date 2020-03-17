using System;
using Verse;

namespace StoneCampFire
{
    public class CompProperties_VariousGlow : CompProperties
    {
        public float lightRadius = 10;
        public ColorInt lightColor = new ColorInt(252, 187, 113, 0) * 1.45f;
        public float darkRadius = 4;
        public ColorInt darkColor = new ColorInt(226, 59, 16, 0) * 1.45f;

        public float threshold = 10;

        public string commandTextureOn = "UI/Commands/Extinguishable/Light";
        public string commandTextureOff = "UI/Commands/Extinguishable/Extinguish";

        public string commandLabelKey = "CommandDesignateTogglePowerLabel";
        public string commandDescKey = "CommandDesignateTogglePowerDesc";

        public CompProperties_VariousGlow()
        {
            this.compClass = typeof(CompVariousGlow);
        }
    }
}