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

        public string commandLitLabelKey = "StoneCampfire_lightLabel";
        public string commandExtinguishLabelKey = "StoneCampfire_extinguishLabel";

        public string commandLitDescKey = "StoneCampfire_lightDesc";
        public string commandExtinguishDescKey = "StoneCampfire_extinguishDesc";

        public string commandColonistWillDoItDescKey = "StoneCampfire_doIt";

        public bool debug = false;

        public FloatRange mediumFuelFireRange = new FloatRange(.2f, .5f);
        public FloatRange lowFuelFireRange = new FloatRange(0, .2f);

        public bool maxFuelTargetLevel = true;

        public CompProperties_Extinguishable()
        {
            compClass = typeof(CompExtinguishable);
        }
    }
}