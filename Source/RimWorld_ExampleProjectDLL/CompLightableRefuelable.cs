using RimWorld;
using System;
using Verse;

using LTF_Lanius;

namespace StoneCampFire
{
    public class CompLightableRefuelable : CompRefuelable
    {
        protected CompExtinguishable stoneComp;
        //protected CompRefuelable refuelableComp;
        protected CompBreakdownable breakdownableComp;

        public const int CheckEvery5Second = 300;
        protected bool LaniusMod = false;

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

            bool every5Sec = (Find.TickManager.TicksGame % CheckEvery5Second == 0);

            if (stoneComp != null && !stoneComp.SwitchIsOn)
                return;

            // regular consumption
            if (!this.Props.consumeFuelOnlyWhenUsed)
            {
                this.ConsumeFuel(this.ConsumptionRatePerTick);
            }
            // additionnal rain consumption
            if (this.Props.fuelConsumptionPerTickInRain > 0f && this.parent.Spawned && this.parent.Map.weatherManager.RainRate > 0.4f && !this.parent.Map.roofGrid.Roofed(this.parent.Position))
            {
                this.ConsumeFuel(this.Props.fuelConsumptionPerTickInRain);
            }

            // if on
            if (stoneComp.SwitchIsOn) {
                // no more fuel, extinguishing
                if (Fuel <= 0)
                {
                    stoneComp.DoFlick(false);
                    stoneComp.ResetToOff();
                }

                if (every5Sec)
                {
                    // raining, chances to extinguish
                    if (RollForRainFire()) return;

                    if (LaniusMod)
                    {
                        Room room = this.parent.GetRoom();
                        float breathablility = this.parent.Map.GetComponent<RoomBreathabilityManager>().RoomBreathability(room);

                        if (breathablility < 50f)
                        {
                            stoneComp.DoFlick(false);
                            stoneComp.ResetToOff();
                        }
                    }
                }

            }
        }

        private bool RollForRainFire()
        {
            float chance = stoneComp.ExtinguishInRainChance * this.parent.Map.weatherManager.RainRate;
            if (!Rand.Chance(chance))
            {
                return false;
            }

            Building building = this.parent as Building;
            if (!building.Map.roofGrid.Roofed(building.Position))
            {
                stoneComp.DoFlick(false);
                stoneComp.ResetToOff();
                return true;
            }
            return false;
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
            LaniusMod = ModCompatibilityCheck.LaniusIsActive;
            //this.refuelableComp = this.parent.GetComp<CompRefuelable>();
            //this.breakdownableComp = this.parent.GetComp<CompBreakdownable>();
        }
    }
}