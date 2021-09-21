using Verse;
using Verse.AI;
using RimWorld;

namespace StoneCampFire
{
	public class WorkGiver_DoStonyBills : WorkGiver_DoBill
    {
        public override Job JobOnThing(Pawn pawn, Thing thing, bool forced = false)
		{
            //Log.Warning("ok JobOnThing");

            if (thing.def != MyDefs.MyBuilding)
                return null;
            //Log.Warning("ok def");

            if (!(thing is Building_CampFire StonyCampFire))
                return null;
            //Log.Warning("ok campfire");

            if (!StonyCampFire.CurrentlyUsableForBills)
                return null;
            //Log.Warning("ok usable");

            //Log.Warning("base.JobOnThing: " + base.JobOnThing(pawn, thing, forced)?.def.defName);

            IBillGiver billGiver = thing as IBillGiver;
            if (billGiver == null || !ThingIsUsableBillGiver(thing) || !billGiver.BillStack.AnyShouldDoNow || !billGiver.UsableForBillsAfterFueling() || !pawn.CanReserve(thing, 1, -1, null, forced) || thing.IsBurning() || thing.IsForbidden(pawn))
            {
                Log.Warning("billGiver == null" + (billGiver == null).ToString());
                Log.Warning("!ThingIsUsableBillGiver(thing)" + !ThingIsUsableBillGiver(thing));

                Log.Warning("!billGiver.BillStack.AnyShouldDoNow" + !billGiver.BillStack.AnyShouldDoNow);
                Log.Warning("!billGiver.UsableForBillsAfterFueling()" + !billGiver.UsableForBillsAfterFueling());

                Log.Warning("billGiver.BillStack.Bills:" + billGiver.BillStack.Bills.Count);
                foreach(Bill bill in billGiver.BillStack.Bills)
                {
                    Log.Warning("bill:" + bill.ToString());
                }

                Log.Warning("Arg null");
            }
            billGiver.BillStack.RemoveIncompletableBills();
            
            //return StartOrResumeBillJob(pawn, billGiver);

            return base.JobOnThing(pawn, thing, forced);
            
        }

        
    }
}