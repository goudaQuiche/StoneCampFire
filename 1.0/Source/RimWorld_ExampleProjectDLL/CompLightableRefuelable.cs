using RimWorld;
using System;
using Verse;

using LTF_Lanius;

namespace StoneCampFire
{
    public class CompLightableRefuelable : CompRefuelable
    {
        protected CompExtinguishable stoneComp;
        //protected CompLightableRefuelable refuelableComp;
        //protected CompRefuelable refuelableComp;
        protected CompVariousGlow variousGlowComp;

        public const int CheckEvery5Second = 300;
        protected bool LaniusMod = false;

        CompVariousGlow.GlowStatus lastStatus;

        private float ConsumptionRatePerTick
        {
            get
            {
                return this.Props.fuelConsumptionRate / 60000f;
            }
        }

        private bool RainThreshold
        {
            get
            {
                return parent.Map.weatherManager.RainRate > 0.4f;
            }
        }

        private bool UnroofedBuilding
        {
            get
            {
                return !parent.Map.roofGrid.Roofed(this.parent.Position);
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
            if (this.Props.fuelConsumptionPerTickInRain > 0f && parent.Spawned && RainThreshold && UnroofedBuilding)
            {
                this.ConsumeFuel(this.Props.fuelConsumptionPerTickInRain);
            }


            // Varying glow if threshold has been reached
            
            CompVariousGlow.GlowStatus myStatus = variousGlowComp.MyGlowWay(FuelPercentOfMax);
            if (myStatus != lastStatus)
            {
                Tools.Warn("now: " + myStatus + " ; b4:" + lastStatus, true);
                variousGlowComp.ChangeGlow(FuelPercentOfMax);
                Tools.Warn("Did changeGlow", true);
            }

            lastStatus = myStatus;
            

            // Trying to extinguish bc rain / lanius
            // no more fuel, extinguishing
            if (Fuel <= 0)
            {
                stoneComp.DoFlick(false);
                stoneComp.ResetToOff();
            }

            //Tools.Warn("every1sec", true);

            if (!stoneComp.RainVulnerable && !stoneComp.OxygenVulnerable)
                return;

            if (!every5Sec)
                return;

            Tools.Warn("every5sec", true);

            // raining, chances to extinguish
            if (stoneComp.RainVulnerable)
                RollForRainFire();

            // lanius mod active, checking oxygen ratio
            if (stoneComp.OxygenVulnerable)
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

        private bool RollForRainFire()
        {
            if ((!RainThreshold) ||
                (!UnroofedBuilding))
                return false;

            // propsChance * isItRaining
            float chance = stoneComp.ExtinguishInRainChance * this.parent.Map.weatherManager.RainRate;
            if (!Rand.Chance(chance))
                return false;

            // unroofed
            if (UnroofedBuilding)
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

            stoneComp = parent.GetComp<CompExtinguishable>();
            variousGlowComp = parent.GetComp<CompVariousGlow>();
            //refuelableComp = this.parent.GetComp<CompRefuelable>();

            //refuelableComp = this.parent.GetComp<CompLightableRefuelable>();
            //lastStatus = variousGlowComp.MyGlowWay(FuelPercentOfMax);
            
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

            stoneComp = parent.GetComp<CompExtinguishable>();
            variousGlowComp = this.parent.GetComp<CompVariousGlow>();
            //refuelableComp = this.parent.GetComp<CompLightableRefuelable>();
            //refuelableComp = this.parent.GetComp<CompRefuelable>();
            //lastStatus = variousGlowComp.MyGlowWay(FuelPercentOfMax);
            LaniusMod = ModCompatibilityCheck.LaniusIsActive;
            
            //this.breakdownableComp = this.parent.GetComp<CompBreakdownable>();
        }
    }
}