using RimWorld;
using System.Collections.Generic;
using System.Diagnostics;
using Verse;
using Verse.AI;

namespace StoneCampFire
{
    public class JobDriver_SmokeSignal : JobDriver
    //public class JobDriver_Extinguish : JobDriver_SingleInteraction
    {
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

            
            yield return Toils_Goto.GotoCell(TargetIndex.A, PathEndMode.InteractionCell).FailOn(delegate (Toil MovingPawn)
            {
                CompSmokeSignalComms comp = (MovingPawn.actor.jobs.curJob.GetTarget(TargetIndex.A).Thing).TryGetComp<CompSmokeSignalComms>();
                return !comp.CanSmokeSignalNow;
            });
            yield return Toils_General.WaitWith(TargetIndex.A, 180, useProgressBar: true);
            yield return new Toil
            {
                initAction = () =>
                {
                    CompSmokeSignalComms comp = (pawn.jobs.curJob.GetTarget(TargetIndex.A).Thing).TryGetComp<CompSmokeSignalComms>();
                    
                    if (comp.CanSmokeSignalNow)
                    {
                        pawn.jobs.curJob.commTarget.TryOpenComms(pawn);
                    }
                }
            };
        }
    }
}