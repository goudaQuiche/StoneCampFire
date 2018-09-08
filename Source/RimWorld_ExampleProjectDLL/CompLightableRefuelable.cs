using RimWorld;
using System;
using Verse;

namespace StoneCampFire
{
    public class CompLightableRefuelable : CompRefuelable
    {
        protected CompExtinguishable stoneComp;
        //protected CompRefuelable refuelableComp;
        protected CompBreakdownable breakdownableComp;

        //private float fuel;

        private float ConsumptionRatePerTick
        {
            get
            {
                return this.Props.fuelConsumptionRate / 60000f;
            }
        }

        public override void CompTick()
        {
            //base.CompTick();

            if (stoneComp != null && !stoneComp.SwitchIsOn)
                return;

            if (!this.Props.consumeFuelOnlyWhenUsed)
            {
                this.ConsumeFuel(this.ConsumptionRatePerTick);
            }

            if (this.Props.fuelConsumptionPerTickInRain > 0f && this.parent.Spawned && this.parent.Map.weatherManager.RainRate > 0.4f && !this.parent.Map.roofGrid.Roofed(this.parent.Position))
            {
                this.ConsumeFuel(this.Props.fuelConsumptionPerTickInRain);
            }

            if(stoneComp.SwitchIsOn && (Fuel <= 0)) 
            {
                stoneComp.DoFlick(false);
                stoneComp.ResetToOff();
            }
        }

        public float MyFuelPercentOfMax
        {
            get
            {
                return this.FuelPercentOfMax;
            }
        }

        public override void Initialize(CompProperties props)
        {
            base.Initialize(props);
            this.Refuel(this.Props.fuelCapacity);
            //this.fuel = this.Props.fuelCapacity;
            stoneComp = this.parent.GetComp<CompExtinguishable>();
        }

        /*
        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look<float>(ref this.fuel, "fuel", 0f, false);
            //Scribe_Values.Look<float>(ref configuredTargetFuelLevel, "configuredTargetFuelLevel", -1f, false);
        }
        */

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            this.stoneComp = this.parent.GetComp<CompExtinguishable>();
            //this.refuelableComp = this.parent.GetComp<CompRefuelable>();
            //this.breakdownableComp = this.parent.GetComp<CompBreakdownable>();
        }
    }
}