using System;
using Verse;

namespace StoneCampFire
{
    public class CompProperties_Extinguishable : CompProperties
    {
        public string commandTextureOn = "UI/Commands/Extinguishable/Light";
        public string commandTextureOff = "UI/Commands/Extinguishable/Extinguish";

        public string commandLabelKey = "CommandDesignateTogglePowerLabel";
        public string commandDescKey = "CommandDesignateTogglePowerDesc";

        public CompProperties_Extinguishable()
        {
            this.compClass = typeof(CompExtinguishable);
        }
    }
}