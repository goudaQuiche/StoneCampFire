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
                return (this.stoneComp == null || this.stoneComp.SwitchIsOn) 
                    && (this.refuelableComp == null || this.refuelableComp.HasFuel)
                    && (this.breakdownableComp == null || !this.breakdownableComp.BrokenDown);
            }
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            this.stoneComp = this.parent.GetComp<CompExtinguishable>();
            this.refuelableComp = this.parent.GetComp<CompRefuelable>();
            this.breakdownableComp = this.parent.GetComp<CompBreakdownable>();
        }
    }
}