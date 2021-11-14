using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using Verse.AI;

namespace  StoneCampFire
{
    public class CompSmokeSignalComms : ThingComp
    {
        
        protected CompExtinguishable stoneComp;
        protected CompRefuelable regularComp;

        private CompProperties_SmokeSignalComms Props => (CompProperties_SmokeSignalComms)props;
        private bool MyDebug => Props.debug;

        public bool IsOutside
        {
            get
            {
                if (parent.GetRoom() is Room room)
                {
                    if (MyDebug) Log.Warning("room exists but outdoors:" + room.OutdoorsForWork);
                    return room.OutdoorsForWork;
                }

                return true;
            }
            
        }

        private bool HasStoneCampfireComp => stoneComp != null;
        private bool HasRegularCampfireComp => regularComp != null;

        public bool CanSmokeSignalNow(out string reason)
        {
            if (!parent.Spawned)
            {
                reason = "StoneCampfire_NoSpawned".Translate();
                return false;
            }
                

            if (parent.Faction != null && parent.Faction != Faction.OfPlayer)
            {
                reason = "StoneCampfire_NotPlayerFaction".Translate();
                return false;
            }
                

            if (!IsOutside)
            {
                reason = "StoneCampfire_NotOutside".Translate();
                return false;
            }
                

            if (parent.Position.Roofed(parent.Map))
            {
                reason = "StoneCampfire_Roofed".Translate();
                return false;
            }
                

            if (HasStoneCampfireComp)
            {
                if (stoneComp.parent is Building_CampFire campFire)
                {
                    reason = "StoneCampfire_Unlit".Translate();
                    return campFire.CurrentlyUsableForBills;
                }
            }
            else if (HasRegularCampfireComp)
            {
                reason = "StoneCampfire_Unlit".Translate();
                return regularComp.HasFuel;
            }

            reason = "StoneCampfire_Unmanaged".Translate();
            return false;
        }
        
        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            //protected CompExtinguishable stoneComp;
            stoneComp = parent.TryGetComp<CompExtinguishable>();
            if (!HasStoneCampfireComp) {
                if (MyDebug)
                    Log.Warning("could not find extinguish comp");

                regularComp = parent.TryGetComp<CompRefuelable>();
                if (!HasRegularCampfireComp)
                {
                    if (MyDebug)
                        Log.Warning("could not find refuelable comp");
                }
            }

            base.PostSpawnSetup(respawningAfterLoad);
        }

        private void GiveSmokeSignalJob(Pawn myPawn, ICommunicable newCommTarget)
        {
            //var job = new Job(MyDefs.ApparelSmokeSignalJobDef, parent)
            var job = new Job(MyDefs.SmokeSignalJobDef, parent)
            {
                commTarget = newCommTarget
            };
            myPawn.jobs.TryTakeOrderedJob(job, 0);

            PlayerKnowledgeDatabase.KnowledgeDemonstrated(ConceptDefOf.OpeningComms, KnowledgeAmount.Total);
        }

        public override IEnumerable<FloatMenuOption> CompFloatMenuOptions(Pawn myPawn)
        {
            if (!myPawn.CanReach(parent, PathEndMode.InteractionCell, Danger.Some))
            {
                var item = new FloatMenuOption("CannotUseNoPath".Translate(), null);
                return new List<FloatMenuOption>
                {
                    item
                };
            }

            if (!myPawn.health.capacities.CapableOf(PawnCapacityDefOf.Sight))
            {
                return new List<FloatMenuOption>
                {
                    new FloatMenuOption(
                        "CannotUseReason".Translate("IncapableOfCapacity".Translate(PawnCapacityDefOf.Sight.label)),
                        null)
                };
            }

            if (!myPawn.health.capacities.CapableOf(PawnCapacityDefOf.Manipulation))
            {
                return new List<FloatMenuOption>
                {
                    new FloatMenuOption(
                        "CannotUseReason".Translate(
                            "IncapableOfCapacity".Translate(PawnCapacityDefOf.Manipulation.label)), null)
                };
            }

            if (!myPawn.IsPawnTribal())
            {
                return new List<FloatMenuOption>
                {
                    new FloatMenuOption(
                        "StoneCampfire_PawnNoTribal".Translate(myPawn.LabelShort), null)
                };
            }



            if (!CanSmokeSignalNow(out string reason))
            {
                //Log.Error(myPawn + " could not use smoke signal.");
                return new List<FloatMenuOption>
                {
                    new FloatMenuOption(reason, null)
                };
            }

            var list = new List<FloatMenuOption>();
            foreach (ICommunicable commTarget in GetSmokeSignalTargets())
            {
                var localCommTarget = commTarget;
                var text = "CallOnRadio".Translate(localCommTarget.GetCallLabel());

                if (localCommTarget is Faction faction)
                {
                    if (!IsLeaderAvailable(faction))
                    {
                        string str = faction.leader != null
                            ? "LeaderUnavailable".Translate(faction.leader.LabelShort)
                            : "LeaderUnavailableNoLeader".Translate();

                        list.Add(new FloatMenuOption(text + " (" + str + ")", null));
                        continue;
                    }
                }

                void action()
                {
                    if (commTarget is TradeShip)
                    {
                        return;
                    }

                    GiveSmokeSignalJob(myPawn, commTarget);
                }

                list.Add(FloatMenuUtility.DecoratePrioritizedTask(
                    new FloatMenuOption(text, action, (MenuOptionPriority)7), myPawn, parent));
            }

            return list;
        }

        public IEnumerable<ICommunicable> GetSmokeSignalTargets()
        {
            return Find.FactionManager.AllFactionsVisibleInViewOrder.Where(
                (Faction f) => 
                !f.IsPlayer && 
                f.def.categoryTag == "Tribal"
            );
        }

        public static bool IsLeaderAvailable(Faction faction)
        {
            if (!(faction.leader is Pawn fLeader))
                return false;

            if (fLeader.Spawned)
                return false;
            if (fLeader.Downed)
                return false;
            if (fLeader.IsPrisoner)
                return false;
            if (!fLeader.Awake())
                return false;
            if (fLeader.InMentalState)
                return false;

            return true;
        }

    }
}