using System;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.AI;
using RimWorld;

namespace StoneCampFire
{
    public class WorkGiver_StonyRefuel : WorkGiver_Scanner
    {
        //StoneCampfire
        //string jobName = "StoneCampFire.JobDriver_Extinguish";
        //string jobName = "ExtinguishCampFire";

        /*
        public override ThingRequest PotentialWorkThingRequest
        {
            get
            {
                return ThingRequest.ForGroup(ThingRequestGroup.Refuelable);
            }
        }
        */
        public override ThingRequest PotentialWorkThingRequest => ThingRequest.ForDef(MyDefs.MyBuilding);


        public override PathEndMode PathEndMode => PathEndMode.Touch;

        public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            CompLightableRefuelable compRefuelable = t.TryGetComp<CompLightableRefuelable>();
            if (compRefuelable == null || compRefuelable.IsFull)
            {
                return false;
            }
            bool flag = !forced;
            if (flag && !compRefuelable.ShouldAutoRefuelNow)
            {
                return false;
            }
            if (!t.IsForbidden(pawn))
            {
                LocalTargetInfo target = t;
                if (pawn.CanReserve(target, 1, -1, null, forced))
                {
                    if (t.Faction != pawn.Faction)
                    {
                        return false;
                    }
                    //Log.Warning("I am trying to refuel there", true);
                    /*
                    ThingWithComps thingWithComps = t as ThingWithComps;
                    if (thingWithComps != null)
                    {
                        CompExtinguishable comp = thingWithComps.GetComp<CompExtinguishable>();
                        if (comp != null && !comp.SwitchIsOn)
                        {
                            return false;
                        }
                    }
                    */
                    if (FindBestFuel(pawn, t) == null)
                    {
                        ThingFilter fuelFilter = t.TryGetComp<CompLightableRefuelable>().Props.fuelFilter;
                        JobFailReason.Is("NoFuelToRefuel".Translate(fuelFilter.Summary), null);

                        return false;
                    }
                    //Log.Warning("Found best fuel", true);
                    return true;
                }
            }
            return false;
        }

        public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            Thing t2 = this.FindBestFuel(pawn, t);
            return new Job(JobDefOf.Refuel, t, t2);
        }

        private Thing FindBestFuel(Pawn pawn, Thing refuelable)
        {
            ThingFilter filter = refuelable.TryGetComp<CompLightableRefuelable>().Props.fuelFilter;
            Predicate<Thing> predicate = (Thing x) => !x.IsForbidden(pawn) && pawn.CanReserve(x, 1, -1, null, false) && filter.Allows(x);
            IntVec3 position = pawn.Position;
            Map map = pawn.Map;
            ThingRequest bestThingRequest = filter.BestThingRequest;
            PathEndMode peMode = PathEndMode.ClosestTouch;
            TraverseParms traverseParams = TraverseParms.For(pawn, Danger.Deadly, TraverseMode.ByPawn, false);
            Predicate<Thing> validator = predicate;
            return GenClosest.ClosestThingReachable(position, map, bestThingRequest, peMode, traverseParams, 9999f, validator, null, 0, -1, false, RegionType.Set_Passable, false);
        }
    }
}
