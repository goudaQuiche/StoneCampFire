using RimWorld;
using System;
using UnityEngine;
using Verse;

namespace StoneCampFire
{
    public static class MyDefs
    {
        public static ThingDef MyBuilding = ThingDef.Named("StoneCampfire");

        public static ThingDef RegularGlower = ThingDef.Named("StonyCampFire_GlowerRegular");
        public static ThingDef MediumGlower = ThingDef.Named("StonyCampFire_GlowerMedium");
        public static ThingDef LowGlower = ThingDef.Named("StonyCampFire_GlowerLow");

        public static JobDef ExtinguishJobDef => DefDatabase<JobDef>.GetNamed("ExtinguishCampFire");
        public static JobDef SmokeSignalJobDef => DefDatabase<JobDef>.GetNamed("UseSmokeSignal");

        public static SoundDef CampFireExtinguishSound = SoundDef.Named("CampFireExtinguish");
        public static SoundDef CampFireLightSound= SoundDef.Named("CampFireLight");
    }
}