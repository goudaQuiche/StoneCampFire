using System;
using Verse;

namespace StoneCampFire
{
    public class CompProperties_VariousGlow : CompProperties
    {
        public float extinguishInRainChance = 0.2f;

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