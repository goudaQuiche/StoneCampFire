using RimWorld;
using System;
using Verse;

namespace StoneCampFire
{
    public class CompLightableHeatPusher : CompHeatPusher
    {
        protected CompExtinguishable stoneComp;
        protected CompRefuelable refuelableComp;
        protected CompBreakdownable breakdownableComp;

        protected override bool ShouldPushHeatNow
        {
            get
            {
                return base.ShouldPushHeatNow && stoneComp != null && stoneComp.SwitchIsOn;
                    //&& (refuelableComp == null || refuelableComp.HasFuel);
                    //&& (breakdownableComp == null || !breakdownableComp.BrokenDown);
            }
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            stoneComp = parent.GetComp<CompExtinguishable>();
            refuelableComp = parent.GetComp<CompRefuelable>();
            breakdownableComp = parent.GetComp<CompBreakdownable>();
        }
    }
}