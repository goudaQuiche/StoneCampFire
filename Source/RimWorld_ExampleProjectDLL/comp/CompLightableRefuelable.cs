using RimWorld;
using System;
using Verse;
using System.Collections;
using System.Collections.Generic;

using LTF_Lanius;

namespace StoneCampFire
{
    public class CompLightableRefuelable : CompRefuelable
    {
        protected CompExtinguishable extinguishableComp;
        //protected CompLightableRefuelable refuelableComp;
        //protected CompRefuelable refuelableComp;
        //protected CompVariousGlow variousGlowComp;

        public const int CheckEvery5Second = 300;
        protected bool LaniusMod = false;

        //CompVariousGlow.GlowStatus LastStatus;
        //public const string RanOutOfFuelSignal = "RanOutOfFuel";

        private float ConsumptionRatePerTick => Props.fuelConsumptionRate / 60000f;
        private bool RainThreshold => parent.Map.weatherManager.RainRate > 0.4f;
        private bool UnroofedBuilding => !parent.Map.roofGrid.Roofed(parent.Position);

        private bool MyDebug => extinguishableComp?.MyDebug == true;

        private bool IsRegular => !extinguishableComp.IsMediumFire && !extinguishableComp.IsLowFire && extinguishableComp.SwitchIsOn;
        private bool IsMedium => extinguishableComp.IsMediumFire && extinguishableComp.SwitchIsOn;
        private bool IsLow => extinguishableComp.IsLowFire && extinguishableComp.SwitchIsOn;

        private bool WasRegular;
        private bool WasMedium;
        private bool WasLow;

        void UpdateGlowStatus()
        {
            bool RegularNeeded = IsRegular != WasRegular;
            bool MediumNeeded = IsMedium != WasMedium;
            bool LowNeeded = IsLow != WasLow;
            bool NeedToSpawn = RegularNeeded || MediumNeeded || LowNeeded;

            if(NeedToSpawn)
            {
                if(extinguishableComp.GetBuildingGlower() is Building b)
                {
                    b.Destroy(DestroyMode.KillFinalize);
                }
                extinguishableComp.SpawnIfNoGlow();
            }

            if (IsRegular)
            {
                WasRegular = true;
                WasMedium = false;
                WasLow = false;
            }
            else if (IsMedium)
            {
                WasRegular = false;
                WasMedium = true;
                WasLow = false;
            }
            else if (IsMedium)
            {
                WasRegular = false;
                WasMedium = false;
                WasLow = true;
            }
        }

        public override void CompTick()
        {
            //base.CompTick();
            //Log.Warning("0", true);

            if (extinguishableComp == null)
            {
                Log.Warning("Cant find extinguishableComp: " + parent.Label);
                return;
            }
            //Log.Warning("1", true);
            if (!extinguishableComp.SwitchIsOn)
                return;

            //Log.Warning("2", true);
            // regular consumption
            if (!Props.consumeFuelOnlyWhenUsed)
            {
                ConsumeFuel(ConsumptionRatePerTick);
            }

            // additionnal rain consumption
            if (Props.fuelConsumptionPerTickInRain > 0f && parent.Spawned && RainThreshold && UnroofedBuilding)
            {
                ConsumeFuel(Props.fuelConsumptionPerTickInRain);
            }

            // Trying to extinguish bc rain / lanius
            // no more fuel, extinguishing
            if (Fuel <= 0)
            {
                parent.BroadcastCompSignal(RanOutOfFuelSignal);
                extinguishableComp.DoFlick(false);
                extinguishableComp.ResetToOff();
            }

            UpdateGlowStatus();

            //Log.Warning("6", true);
            //Log.Warning("every1sec", true);

            if (!extinguishableComp.RainVulnerable && !extinguishableComp.OxygenVulnerable)
                return;

            if (!parent.IsHashIntervalTick(300))
                return;

            //Log.Warning("7", true);
            if(MyDebug) Log.Warning("every5sec");

            // raining, chances to extinguish
            if (extinguishableComp.RainVulnerable)
                RollForRainFire();

            //Log.Warning("8", true);
            // lanius mod active, checking oxygen ratio
            if (extinguishableComp.OxygenVulnerable)
                if (LaniusMod)
                {
                    Room room = parent.GetRoom();
                    if (room.PsychologicallyOutdoors)
                        return;
                    float breathablility = parent.Map.GetComponent<RoomBreathabilityManager>().RoomBreathability(room);

                    if (breathablility < 50f)
                    {
                        extinguishableComp.DoFlick(false);
                        extinguishableComp.ResetToOff();
                    }
                }
        }

        private bool RollForRainFire()
        {
            if ((!RainThreshold) ||
                (!UnroofedBuilding))
                return false;

            // propsChance * isItRaining
            float chance = extinguishableComp.ExtinguishInRainChance * parent.Map.weatherManager.RainRate;
            if (!Rand.Chance(chance))
                return false;

            // unroofed
            if (UnroofedBuilding)
            {
                extinguishableComp.DoFlick(false);
                extinguishableComp.ResetToOff();
                return true;
            }
            return false;
        }

        //public float MyFuelPercentOfMax => FuelPercentOfMax;

        public override void Initialize(CompProperties props)
        {
            base.Initialize(props);
            Refuel(Props.fuelCapacity);
            //this.fuel = this.Props.fuelCapacity;

            extinguishableComp = parent.GetComp<CompExtinguishable>();
            //variousGlowComp = parent.GetComp<CompVariousGlow>();
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

            extinguishableComp = parent.GetComp<CompExtinguishable>();
            //variousGlowComp = parent.GetComp<CompVariousGlow>();
            //refuelableComp = this.parent.GetComp<CompLightableRefuelable>();
            //refuelableComp = this.parent.GetComp<CompRefuelable>();
            //lastStatus = variousGlowComp.MyGlowWay(FuelPercentOfMax);
            LaniusMod = ModCompatibilityCheck.LaniusIsActive;
            
            //this.breakdownableComp = this.parent.GetComp<CompBreakdownable>();
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            foreach (Gizmo item in base.CompGetGizmosExtra())
            {
                yield return item;
            }
            if (Prefs.DevMode)
            {
                Command_Action command_Action = new Command_Action();
                command_Action.defaultLabel = "Debug: Remove 1f";
                command_Action.action = delegate
                {
                    ConsumeFuel(1f);
                };
                yield return command_Action;
            }
        }

    }
}