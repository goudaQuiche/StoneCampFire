using System;
using Verse;

namespace StoneCampFire
{
    public class CompProperties_Extinguishable : CompProperties
    {
        public float extinguishInRainChance = 0.2f;
        public bool rainProof = false;
        public bool oxygenLackProof = false;

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