using Verse;
using Verse.AI;
using RimWorld;

namespace StoneCampFire
{
	public class WorkGiver_DoStonyBills : WorkGiver_DoBill
    {
        public override Job JobOnThing(Pawn pawn, Thing thing, bool forced = false)
		{
            if (thing.def != MyDefs.MyBuilding)
                return null;

            if (!(thing is Building_CampFire StonyCampFire))
                return null;
            
            if(!StonyCampFire.CurrentlyUsableForBills)
                return null;

            return base.JobOnThing(pawn, thing, forced);
            
        }
	}
}