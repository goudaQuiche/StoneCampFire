using RimWorld;
using Verse;

namespace StoneCampFire
{
    public class CompLifeSpanWithParentCheck : ThingComp
    {
        public int age = -1;
        public int RareInc = 250;

        public CompProperties_LifeSpanWithParentCheck Props => (CompProperties_LifeSpanWithParentCheck)props;
        public bool MyDebug => Props.debug;

        private Building parentBuilding = null;
        private CompExtinguishable compExtinguishable = null;
        private CompLightableRefuelable compLightableRefuelable = null;

        public bool IsLegit => parentBuilding != null && compExtinguishable != null && compLightableRefuelable != null;

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref age, "age", 0);
            Scribe_References.Look(ref parentBuilding, "StoneCampfireParent");
        }

        public void SetParentBuilding(Building b)
        {
            parentBuilding = b;
            //parent.SetFaction(parentBuilding.Faction);
            SetComp();
        }

        public void SetComp()
        {
            if (parentBuilding == null)
                return;

            compExtinguishable = parentBuilding.TryGetComp<CompExtinguishable>();
            compLightableRefuelable = parentBuilding.TryGetComp<CompLightableRefuelable>();
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            SetComp();
            //if (MyDebug) Log.Warning(parent.Label + " spawned");
        }

        public void MyTick(int tickNum)
        {
            age += tickNum;

            if (!IsLegit)
            {
                if (MyDebug) Log.Warning(parent.Label + " is not legit, expiring");
                Expire();
                return;
            }

            //if (MyDebug) Log.Warning(parent.Label + " is legit");

            if (age >= Props.lifespanTicks)
            {
                //if (MyDebug) Log.Warning(parent.Label + " may die there");

                if (compExtinguishable.SwitchIsOn && compLightableRefuelable.HasFuel)
                {
                    if (MyDebug) Log.Warning(parentBuilding.Label + " is still on fire. " + parent.Label + " shall live longer");
                    age = 0;
                    return;
                }

                Expire();
            }
        }

        public override void ReceiveCompSignal(string signal)
        {
            switch (signal)
            {
                case "FlickedOff":
                case "RanOutOfFuel":
                    Expire();
                    break;
            }
        }

        public override void CompTick()
        {
            MyTick(1);
        }

        public override void CompTickRare()
        {
            MyTick(250);
        }


        protected void Expire()
        {
            parent.DeSpawn(DestroyMode.KillFinalize);
            //parent.Destroy(DestroyMode.KillFinalize);
        }
    }
}
