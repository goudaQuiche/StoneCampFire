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
                if( lightableRefuelableComp == null || !lightableRefuelableComp.HasFuel)
                {
                    Log.Warning("firecamp has no fuel");
                }

                if (extinguishComp == null || !extinguishComp.SwitchIsOn)
                {
                    Log.Warning("firecamp is off");
                    return false;
                }

                //Log.Warning("campfire is usable for bills");
                return true;
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
        /*
        public Building_CampFire()
        {
            billStack = new BillStack(this);
        }
        */

    }
}