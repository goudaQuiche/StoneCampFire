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

        public bool debug = false;

        public FloatRange mediumFuelFireRange = new FloatRange(.07f, .25f);
        public FloatRange lowFuelFireRange = new FloatRange(0, .07f);

        public CompProperties_Extinguishable()
        {
            compClass = typeof(CompExtinguishable);
        }
    }
}