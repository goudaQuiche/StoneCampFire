using RimWorld;
using System;
using System.Collections.Generic;
using System.Diagnostics;

using Verse;
using Verse.Sound;

using UnityEngine;


namespace StoneCampFire
{
	public class CompExtinguishable : ThingComp
	{
        // Work base
        Building building = null;
        Vector3 buildingPos;
        ThingDef myDef = null;
        string buildingName = string.Empty;

        Map myMap = null;

        CompLightableRefuelable compFuel = null;
        //CompLightableGlower compglow = null;
        //CompGlower compGlow = null;

        private bool switchOnInt = true;
		private bool wantSwitchOn = true;

		private Graphic offGraphic;
		private Texture2D cachedCommandTex;

		private const string OffGraphicSuffix = "_Off";
		public const string FlickedOnSignal = "FlickedOn";
		public const string FlickedOffSignal = "FlickedOff";

        public string SentSignal => SwitchIsOn ? FlickedOffSignal : FlickedOnSignal;

        private CompProperties_Extinguishable Props => (CompProperties_Extinguishable)props;
        public bool MyDebug => Props.debug;

        public float ExtinguishInRainChance => Props.extinguishInRainChance;
        public bool RainProof => Props.rainProof;
        public bool RainVulnerable => !RainProof;
        public bool OxygenLackProof => Props.oxygenLackProof;
        public bool OxygenVulnerable => !OxygenLackProof;

        public bool IsMediumFire => compFuel == null ? false : Props.mediumFuelFireRange.Includes(compFuel.FuelPercentOfMax);
        public bool IsLowFire => compFuel == null ? false : Props.lowFuelFireRange.Includes(compFuel.FuelPercentOfMax);

        public ThingDef GetGlowerDef()
        {
            if (IsMediumFire)
                return MyDefs.MediumGlower;
            else if(IsLowFire)
                return MyDefs.LowGlower;

            return MyDefs.RegularGlower;
        }

        private Texture2D CommandTex
		{
			get
			{
				if (cachedCommandTex == null)
				{
                    string cmdTexture = SwitchIsOn ? Props.commandTextureOff : Props.commandTextureOn;
                    cachedCommandTex = ContentFinder<Texture2D>.Get(cmdTexture, true);
				}
				return cachedCommandTex;
			}
		}

        private Texture2D MyCommandTex => ContentFinder<Texture2D>.Get(SwitchIsOn ? Props.commandTextureOff : Props.commandTextureOn, true);

        public bool SwitchIsOn
        {
            get
            {
                return switchOnInt;
            }
            set
            {
                if (switchOnInt == value)
                {
                    return;
                }
                switchOnInt = value;

                parent.BroadcastCompSignal(switchOnInt ? FlickedOnSignal : FlickedOffSignal);
                if (parent.Spawned)
                {
                    parent.Map.mapDrawer.MapMeshDirty(parent.Position, MapMeshFlag.Things | MapMeshFlag.Buildings);
                }
            }
        }

		public Graphic CurrentGraphic
		{
			get
			{
				if (SwitchIsOn)
				{
					return parent.DefaultGraphic;
				}
				if (offGraphic == null)
				{
                    offGraphic = GraphicDatabase.Get(parent.def.graphicData.graphicClass, parent.def.graphicData.texPath + "_Off", ShaderDatabase.LoadShader(parent.def.graphicData.texPath), parent.def.graphicData.drawSize, parent.DrawColor, parent.DrawColorTwo);
                }
				return offGraphic;
			}
		}
        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            //Building
            building = (Building)parent;
            myDef = parent.def;

            buildingPos = building.DrawPos;
            buildingName = building?.LabelShort;

            myMap = building?.Map;

            compFuel = parent.TryGetComp<CompLightableRefuelable>();

            SpawnIfNoGlow();
            //compGlow = parent.TryGetComp<CompGlower>();
            //ChangeComps();
        }

        public Thing GetBuildingGlower()
        {
            if (building.Position.GetFirstThing(myMap, MyDefs.RegularGlower) is Building regularGlower)
                return regularGlower;

            if (building.Position.GetFirstThing(myMap, MyDefs.MediumGlower) is Building mediumGlower)
                return mediumGlower;

            if (building.Position.GetFirstThing(myMap, MyDefs.LowGlower) is Building lowGlower)
                return lowGlower;

            return null;
        }

        public void SpawnIfNoGlow()
        {
            if (GetBuildingGlower() != null)
                return;

            Thing t = ThingMaker.MakeThing(GetGlowerDef());
            if( t.TryGetComp<CompLifeSpanWithParentCheck>() is CompLifeSpanWithParentCheck LSWPC)
                LSWPC.SetParentBuilding(building);

            GenPlace.TryPlaceThing(t, parent.Position, myMap, ThingPlaceMode.Direct, out Thing lastResultingThing);
        }

        public override void PostExposeData()
		{
			base.PostExposeData();
			Scribe_Values.Look<bool>(ref switchOnInt, "switchOn", true, false);
			Scribe_Values.Look<bool>(ref wantSwitchOn, "wantSwitchOn", true, false);
		}

		public bool WantsFlick()
		{
            if(MyDebug) Log.Warning("wantSwitchOn: " + Tools.OkStr(wantSwitchOn) + "!=" + Tools.OkStr(switchOnInt));
            return wantSwitchOn != switchOnInt;
		}
        /*
        void MySetCompGlower()
        {
            ToolsBuilding.SetCompGlower(parent, compFuel.FuelPercentOfMax, true);
        }
        void MyUnsetCompGlower()
        {
            ToolsBuilding.SetCompGlower(parent, compFuel.FuelPercentOfMax, false);
        }
        */

        void ToggleFireOverlay()
        {
            ToolsBuilding.ToggleFireOverlay(parent, myMap);
        }
        void SetFireOverlay(bool value=true)
        {
            ToolsBuilding.SetFireOverlay(parent, myMap, value);
        }

        void MyUnsetCompHeatPusher()
        {
            ToolsBuilding.UnsetCompHeatPusher(parent, myMap);
        }

        void UpdateComps()
        {
            //Log.Warning("Trying to change comp: "+Tools.OkStr(SwitchIsOn), true);

            /*
            if (SwitchIsOn)
            {
                if(MyDebug) Log.Warning("[ON]Trying to set glow: " + Tools.OkStr(SwitchIsOn));
                //Glower
                MySetCompGlower();
            }
            else
            {
                if(MyDebug)Log.Warning("[OFF]Trying to set glow: " + Tools.OkStr(SwitchIsOn));
                //Glower
                MyUnsetCompGlower();
            }
            */

            //heat
            //MySetCompHeatPusher();
            // gather
            ToolsBuilding.ToggleCompGatherSpot(parent, true, SwitchIsOn);
            //fire overlay
            SetFireOverlay(SwitchIsOn);
            //ToggleFireOverlay();

        }

        public void DoFlick(bool active = true)
        {
            //Log.Warning("Trying DoFlick ", true);

            if (active)
            {
                SoundDef mySound = SwitchIsOn ? MyDefs.CampFireExtinguishSound : MyDefs.CampFireLightSound;
                mySound.PlayOneShot(new TargetInfo(parent.Position, parent.Map, false));
            }

            // turning off
            if (SwitchIsOn)
            {
                FleckMaker.ThrowSmoke(buildingPos, myMap, compFuel.FuelPercentOfMax);
                if (GetBuildingGlower() is ThingWithComps glower)
                {
                    glower.BroadcastCompSignal(SentSignal);
                }
            }
            // turning on
            else
            {
                FleckMaker.ThrowMicroSparks(buildingPos, myMap);
                SpawnIfNoGlow();
            }
            if (MyDebug) Log.Warning("Sending " + SentSignal + " signal");


            //parent.BroadcastCompSignal(SentSignal);

            SwitchIsOn = !SwitchIsOn;
            UpdateComps();

        }

		public void ResetToOn()
		{
			switchOnInt = true;
			wantSwitchOn = true;
		}

        public void ResetToOff()
        {
            switchOnInt = false;
            wantSwitchOn = false;
        }
        /*
        public override string CompInspectStringExtra()
        {
            string report = Tools.OkStr(SwitchIsOn);
            return report;
        }
        */


        [DebuggerHidden]
		public override IEnumerable<Gizmo> CompGetGizmosExtra()
		{
			foreach (Gizmo c in base.CompGetGizmosExtra())
			{
				yield return c;
			}
			if (parent.Faction == Faction.OfPlayer)
			{
                //Action todo = DoFlick;                    //toggleAction = todo,
                yield return new Command_Toggle
                {
                    hotKey = KeyBindingDefOf.Command_TogglePower,
                    //icon = this.CommandTex,
                    icon = this.MyCommandTex,
                    defaultLabel = this.Props.commandLabelKey.Translate(),
                    defaultDesc = this.Props.commandDescKey.Translate(),
                    isActive = (() => wantSwitchOn),
                    toggleAction = delegate
                    {
                        wantSwitchOn = !wantSwitchOn;
                        //ExtinguishUtility.UpdateExtinguishDesignation(parent);
                    }
                    /*
					isActive = (() => this.$this.wantSwitchOn),
					toggleAction = delegate
					{
						this.$this.wantSwitchOn = !this.$this.wantSwitchOn;
						FlickUtility.UpdateFlickDesignation(this.$this.parent);
					}
                    */

                };
			}
		}
	}
}