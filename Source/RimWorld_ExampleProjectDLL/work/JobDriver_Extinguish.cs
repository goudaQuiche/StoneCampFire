using RimWorld;
using System.Collections.Generic;
using System.Diagnostics;
using Verse;
using Verse.AI;

namespace StoneCampFire
{
    public class JobDriver_Extinguish : JobDriver
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
            this.FailOn(delegate
            {
                CompExtinguishable compExtinguishable = null;
                compExtinguishable = job.targetA.Thing.TryGetComp<CompExtinguishable>();

                return !(compExtinguishable!=null  && compExtinguishable.WantsFlick());

            });
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);
            yield return Toils_General.Wait(15).FailOnCannotTouch(TargetIndex.A, PathEndMode.Touch);
            Toil finalize = new Toil();
            finalize.initAction = delegate
            {
                Pawn actor = finalize.actor;
                ThingWithComps thingWithComps = (ThingWithComps)actor.CurJob.targetA.Thing;
                for (int i = 0; i < thingWithComps.AllComps.Count; i++)
                {
                    //Log.Warning("Found "+i+" comp",true);
                    if (thingWithComps.AllComps[i] is CompExtinguishable compExtinguishable && compExtinguishable.WantsFlick())
                    {
                        //Log.Warning("found extinguish", true);
                        compExtinguishable.DoFlick();
                    }
                }
            };
            finalize.defaultCompleteMode = ToilCompleteMode.Instant;
            yield return finalize;
        }
    }
}