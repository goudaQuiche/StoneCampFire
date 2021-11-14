using RimWorld;
using System.Collections.Generic;
using System.Diagnostics;
using Verse;
using Verse.AI;

namespace StoneCampFire
{
    public class JobDriver_ApparelSmokeSignal : JobDriver
    {
        private Apparel Apparel => (Apparel)job.GetTarget(TargetIndex.B).Thing;
        private Building CampfireBuilding => (Building)job.GetTarget(TargetIndex.A).Thing;

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            //Log.Warning("try make pre toil reservations", true);
            return pawn.Reserve(job.targetA, job, 1, -1, null);
        }

        [DebuggerHidden]
        protected override IEnumerable<Toil> MakeNewToils()
        {
            //Log.Warning("make new toil", true);

            this.FailOnDespawnedOrNull(TargetIndex.A);
            this.FailOnAggroMentalState(TargetIndex.A);

            this.FailOnDespawnedOrNull(TargetIndex.B);

            yield return Toils_Goto.GotoThing(TargetIndex.B, PathEndMode.ClosestTouch).FailOnDespawnedNullOrForbidden(TargetIndex.B).FailOnSomeonePhysicallyInteracting(TargetIndex.B);
            yield return Toils_Haul.StartCarryThing(TargetIndex.B, putRemainderInQueue: false, subtractNumTakenFromJobCount: true).FailOnDestroyedNullOrForbidden(TargetIndex.B);
            //yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);

            yield return Toils_Goto.GotoCell(TargetIndex.A, PathEndMode.InteractionCell).FailOn(delegate (Toil MovingPawn)
            {
                CompSmokeSignalComms comp = CampfireBuilding.TryGetComp<CompSmokeSignalComms>();
                return !comp.CanSmokeSignalNow(out string _);
            });
            yield return 
                Toils_General.WaitWith(TargetIndex.A, 360, useProgressBar: true)
                .WithEffect(MyDefs.Smokejoint, TargetIndex.A);

            yield return new Toil
            {
                initAction = () =>
                {
                    if( (CampfireBuilding.TryGetComp<CompSmokeSignalComms>() is CompSmokeSignalComms comp) && comp.CanSmokeSignalNow(out string _))
                        pawn.jobs.curJob.commTarget.TryOpenComms(pawn);
                }
            };

            yield return Toils_General.Do(delegate
            {
                if (pawn.inventory.Contains(Apparel))
                {
                    if (pawn.apparel.TryDrop(Apparel, out var resultingAp))
                    {
                        job.targetA = resultingAp;
                        if (job.haulDroppedApparel)
                        {
                            resultingAp.SetForbidden(value: false, warnOnFail: false);
                            StoragePriority currentPriority = StoreUtility.CurrentStoragePriorityOf(resultingAp);
                            if (StoreUtility.TryFindBestBetterStoreCellFor(resultingAp, pawn, base.Map, currentPriority, pawn.Faction, out var foundCell))
                            {
                                job.count = resultingAp.stackCount;
                                job.targetB = foundCell;
                            }
                            else
                            {
                                EndJobWith(JobCondition.Incompletable);
                            }
                        }
                        else
                        {
                            EndJobWith(JobCondition.Succeeded);
                        }
                    }
                    else
                    {
                        EndJobWith(JobCondition.Incompletable);
                    }
                }
                else
                {
                    EndJobWith(JobCondition.Incompletable);
                }
            });
        }
    }
}