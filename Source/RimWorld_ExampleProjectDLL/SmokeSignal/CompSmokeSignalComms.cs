using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace  StoneCampFire
{
    public class CompSmokeSignalComms : CompGlower
    {
        
        protected CompExtinguishable stoneComp;
        protected CompRefuelable regularComp;

        private new CompProperties_SmokeSignalComms Props => (CompProperties_SmokeSignalComms)props;
        private bool MyDebug => Props.debug;

        public bool IsOutside => parent.GetRoom().OutdoorsForWork;
        private bool HasStoneCampfireComp => stoneComp != null;
        private bool HasRegularCampfireComp => regularComp != null;

        public bool CanSmokeSignalNow
        {
            get
            {
                if (!parent.Spawned) 
                    return false;
                
                if (HasStoneCampfireComp)
                {
                    return stoneComp.SwitchIsOn && stoneComp.IsHighFire && IsOutside;
                }
                else if(HasRegularCampfireComp)
                {
                    return regularComp.HasFuel && IsOutside;
                }

                return false;
                    
            }
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

        private void UseAct(Pawn myPawn, ICommunicable commTarget)
        {
            var job = new Job(MyDefs.SmokeSignalJobDef, parent)
            {
                commTarget = commTarget
            };
            myPawn.jobs.TryTakeOrderedJob(job, 0);

            PlayerKnowledgeDatabase.KnowledgeDemonstrated(ConceptDefOf.OpeningComms, KnowledgeAmount.Total);
        }

        public IEnumerable<FloatMenuOption> GetFloatMenuOptions(Pawn myPawn)
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

            if (!CanSmokeSignalNow)
            {
                Log.Error(myPawn + " could not use signal fire for unknown reason.");
                return new List<FloatMenuOption>
                {
                    new FloatMenuOption("Cannot use now", null)
                };
            }

            var list = new List<FloatMenuOption>();
            foreach (ICommunicable commTarget in Find.FactionManager.AllFactionsVisibleInViewOrder)
            {
                var localCommTarget = commTarget;
                var text = "CallOnRadio".Translate(localCommTarget.GetCallLabel());

                if (localCommTarget is Faction faction)
                {
                    if (faction.IsPlayer)
                    {
                        continue;
                    }

                    /*
                    if (ModStuff.Settings.LimitContacts && faction.def.categoryTag != "Tribal")
                    {
                        continue;
                    }
                    */

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

                    var job = new Job(MyDefs.SmokeSignalJobDef, parent)
                    {
                        commTarget = localCommTarget
                    };
                    myPawn.jobs.TryTakeOrderedJob(job, 0);
                    PlayerKnowledgeDatabase.KnowledgeDemonstrated(ConceptDefOf.OpeningComms, (KnowledgeAmount)6);
                }

                list.Add(FloatMenuUtility.DecoratePrioritizedTask(
                    new FloatMenuOption(text, action, (MenuOptionPriority)7), myPawn, parent));
            }

            return list;
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