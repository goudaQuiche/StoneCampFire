using System;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.AI;
using RimWorld;

namespace StoneCampFire
{
    public class WorkGiverExtinguish : WorkGiver_Scanner//, WorkGiver
    {
        public override ThingRequest PotentialWorkThingRequest => ThingRequest.ForDef(MyDefs.MyBuilding);
        public override PathEndMode PathEndMode => PathEndMode.InteractionCell;
        public override Danger MaxPathDanger(Pawn pawn) => Danger.Deadly;

        public override IEnumerable<Thing> PotentialWorkThingsGlobal(Pawn pawn)
        {
            //return pawn.Map.listerBuildings.AllBuildingsColonistOfDef(ThingDef.Named("LTF_MindcontrolBench")).Cast<Thing>();
            return pawn.Map.listerBuildings.AllBuildingsColonistOfDef(ThingDefOf.DeepDrill).Cast<Thing>();
        }

        public override bool ShouldSkip(Pawn pawn, bool forced = false)
        {
            List<Building> allBuildingsColonist = pawn.Map.listerBuildings.allBuildingsColonist;
            for (int i = 0; i < allBuildingsColonist.Count; i++)
            {
                if (allBuildingsColonist[i].def == MyDefs.MyBuilding)
                {
                    CompPowerTrader comp = allBuildingsColonist[i].GetComp<CompPowerTrader>();
                    if (comp == null || comp.PowerOn)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            if (t.Faction != pawn.Faction)
            {
                //Log.Warning("strike! bench not mine");
                return false;
            }
            if (!(t is Building building))
            {
                //Log.Warning("strike! building null");
                return false;
            }
            // HERE IF IT FAILS
            if (building.IsForbidden(pawn))
            {
                // Log.Warning("strike! forbiden");
                return false;
            }
            LocalTargetInfo target = building;
            if (!pawn.CanReserve(target, 1, -1, null, forced))
            {
                //Log.Warning("strike! cant reserver");
                return false;
            }
            CompExtinguishable comp = null;
            comp = building.TryGetComp<CompExtinguishable>();
            if (comp == null)
            {
                //Log.Warning("extinguish comp null",true);
                return false;
            }

            if (building.IsBurning())
            {
                //Log.Warning("builiding on fire", true);
                return false;
            }

            if (!comp.WantsFlick())
            {
                //Log.Warning("no work needed", true);
                return false;
            }

            return true;
        }

        public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            //return new Job(JobDefOf.OperateDeepDrill, t, 1500, true);
            //JobDriver_OperateShieldBench whatever = null;
            //            ThingDef.Named("JobDriver_LTF_MindMine").;
            //whatever = JobDef("JobDriver_LTF_MindMine");
            //whatever.job.def
            //return new Job(JobDef.Named("JobDriver_LTF_MindMine"), t, 1500, true);
            return new Job(MyDefs.ExtinguishJobDef, t, 1500, true);
        }
    }
}
