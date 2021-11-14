using RimWorld;
using System.Collections.Generic;
using System.Diagnostics;
using Verse;
using Verse.AI;

namespace StoneCampFire
{
    public class JobDriver_SmokeSignal : JobDriver
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
                return !comp.CanSmokeSignalNow(out string _);
            });
            yield return 
                Toils_General.WaitWith(TargetIndex.A, 360, useProgressBar: true)
                .WithEffect(MyDefs.Smokejoint, TargetIndex.A);

            /*
            Toil spawnSmoke = new Toil();
            spawnSmoke.tickAction = delegate
            {
                Pawn actor = spawnSmoke.actor;
                if (actor.IsHashIntervalTick(50))
                    FleckMaker.ThrowSmoke(TargetA.CenterVector3, actor.Map, 1);
            };
            */
            yield return new Toil
            {
                initAction = () =>
                {
                    CompSmokeSignalComms comp = (pawn.jobs.curJob.GetTarget(TargetIndex.A).Thing).TryGetComp<CompSmokeSignalComms>();
                    
                    if (comp.CanSmokeSignalNow(out string _))
                    {
                        pawn.jobs.curJob.commTarget.TryOpenComms(pawn);
                    }
                }
            };
        }
    }
}