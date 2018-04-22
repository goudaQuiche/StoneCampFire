using RimWorld;
using System.Collections.Generic;
using Verse;
//StoneCampFire.Building_CampFire
namespace StoneCampFire
{
	public class Building_CampFire : Building_WorkTable
	{
		private CompLightableRefuelable lightableRefuelableComp;
		private CompBreakdownable breakdownableComp;
        private CompExtinguishable extinguishComp;

        public override bool UsableNow
        {
            get
            {
                return
                    (extinguishComp == null || extinguishComp.SwitchIsOn) &&
                    (lightableRefuelableComp == null || lightableRefuelableComp.HasFuel) &&
                    (this.breakdownableComp == null || !breakdownableComp.BrokenDown);
            }
        }

		public override void SpawnSetup(Map map, bool respawningAfterLoad)
		{
			base.SpawnSetup(map, respawningAfterLoad);
            extinguishComp = base.GetComp<CompExtinguishable>();
            lightableRefuelableComp = base.GetComp<CompLightableRefuelable>();
            this.breakdownableComp = base.GetComp<CompBreakdownable>();
        }

		public override void UsedThisTick()
		{
			if (this.lightableRefuelableComp != null)
			{
				this.lightableRefuelableComp.Notify_UsedThisTick();
			}
		}

        public Building_CampFire()
        {
            this.billStack = new BillStack(this);
        }

    }
}