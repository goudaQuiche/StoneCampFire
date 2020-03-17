using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.AI;
using RimWorld;

namespace StoneCampFire
{
	public class WorkGiver_DoStonyBills : WorkGiver_DoBill
    {
        ThingDef myDef = ThingDef.Named("StoneCampfire");

        public override Job JobOnThing(Pawn pawn, Thing thing, bool forced = false)
		{
            if(thing.def == myDef)
            {
                CompExtinguishable comp = thing.TryGetComp<CompExtinguishable>();
                if (comp == null || !comp.SwitchIsOn)
                    return null;
            }

            return base.JobOnThing(pawn, thing, forced);
            
        }
	}
}