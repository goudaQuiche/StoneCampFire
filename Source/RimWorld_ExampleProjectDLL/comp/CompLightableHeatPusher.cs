using RimWorld;
using System;
using Verse;

namespace StoneCampFire
{
    public class CompLightableHeatPusher : CompHeatPusher
    {
        protected CompExtinguishable stoneComp;
        protected override bool ShouldPushHeatNow => base.ShouldPushHeatNow && stoneComp != null && stoneComp.SwitchIsOn;

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            stoneComp = parent.GetComp<CompExtinguishable>();
        }
    }
}