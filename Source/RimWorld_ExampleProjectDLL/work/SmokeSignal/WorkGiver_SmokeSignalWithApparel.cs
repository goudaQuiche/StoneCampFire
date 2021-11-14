using System;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.AI;
using RimWorld;

namespace StoneCampFire
{
    public class WorkGiver_SmokeSignalWithApparel : WorkGiver_Scanner
    {
        public override ThingRequest PotentialWorkThingRequest => ThingRequest.ForDef(MyDefs.StoneCampfireDef);
        /*
        public override ThingRequest PotentialWorkThingRequest
        {
            get
            {
                if (ThingRequest.ForDef(MyDefs.StoneCampfireDef) is ThingRequest TR)
                    return TR;

                return (ThingRequest.ForDef(MyDefs.VanillaCampFireDef));
            }
        }
        */

        public override PathEndMode PathEndMode => PathEndMode.Touch;

        public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            if (!(t.TryGetComp<CompLightableRefuelable>() is CompLightableRefuelable compRefuelable))
                return false;

            if (!(t is Building_CampFire StonyCampFire))
                return false;

            if (!StonyCampFire.CurrentlyUsableForBills)
            {
                return false;
            }

            //if (t.IsForbidden(pawn))                return false;

            LocalTargetInfo target = t;
            if (!pawn.CanReserve(target, 1, -1, null, forced))
                return false;

            if (t.Faction != pawn.Faction)
            {
                return false;
            }

            if (FindClothOrFabricApparel(pawn) == null)
            {
                //ThingFilter fuelFilter = t.TryGetComp<CompLightableRefuelable>().Props.fuelFilter;
                JobFailReason.Is("StoneCampfire_NoApparel".Translate(), null);

                return false;
            }
            //Log.Warning("Found best fuel", true);
            return true;
        }

        public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            Thing t2 = FindClothOrFabricApparel(pawn);
            return new Job(MyDefs.ApparelSmokeSignalJobDef, t, t2);
        }

        private Thing FindClothOrFabricApparel(Pawn pawn)
        {
            //ThingFilter filter = refuelable.TryGetComp<CompLightableRefuelable>().Props.fuelFilter;
            //Predicate<Thing> predicate = (Thing x) => !x.IsForbidden(pawn) && pawn.CanReserve(x, 1, -1, null, false) && filter.Allows(x);
            bool validator(Thing x) =>
                !x.IsForbidden(pawn) && pawn.CanReserve(x, 1, -1, null, false) &&
                x.def.MadeFromStuff &&
                (x.Stuff.stuffProps.categories.Contains(StuffCategoryDefOf.Fabric) || x.Stuff.stuffProps.categories.Contains(StuffCategoryDefOf.Leathery));
            //stuffCategories == StuffCategoryDefOf.Fabric || x.Stuff == ThingDefOf.);
            IntVec3 position = pawn.Position;
            Map map = pawn.Map;
            //ThingRequest bestThingRequest = filter.BestThingRequest;
            ThingRequest apparelRequest = ThingRequest.ForGroup(ThingRequestGroup.Apparel);
            PathEndMode peMode = PathEndMode.ClosestTouch;
            TraverseParms traverseParams = TraverseParms.For(pawn, Danger.Deadly, TraverseMode.ByPawn, false);
            
            return GenClosest.ClosestThingReachable(position, map, apparelRequest, peMode, traverseParams, 9999f, validator, null, 0, -1, false, RegionType.Set_Passable, false);
        }
    }
}
