using RimWorld;
using System.Collections.Generic;
using Verse;
//StoneCampFire.Building_CampFire
namespace StoneCampFire
{
	public class Building_CampFire : Building_WorkTable
	{
		private CompLightableRefuelable lightableRefuelableComp;
        private CompExtinguishable extinguishComp;

        //public override bool UsableNow
        public new bool CurrentlyUsableForBills
//        public bool CurrentlyUsableForBills
        {
            get
            {
                return
                    (extinguishComp != null && extinguishComp.SwitchIsOn) &&
                    (lightableRefuelableComp != null && lightableRefuelableComp.HasFuel);
            }
        }

		public override void SpawnSetup(Map map, bool respawningAfterLoad)
		{
			base.SpawnSetup(map, respawningAfterLoad);
            extinguishComp = GetComp<CompExtinguishable>();
            lightableRefuelableComp = GetComp<CompLightableRefuelable>();
        }

		public override void UsedThisTick()
		{
            if (lightableRefuelableComp == null)
                return;
			
			lightableRefuelableComp.Notify_UsedThisTick();
		}

        public Building_CampFire()
        {
            this.billStack = new BillStack(this);
        }

    }
}