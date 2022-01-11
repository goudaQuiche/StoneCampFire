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
        public bool IsHighFire => compFuel == null ? false : !IsLowFire && !IsMediumFire;

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

        public Thing GetGlowMote()
        {
            if (building.Position.GetFirstThing(myMap, MyDefs.RegularGlower) is ThingWithComps regularGlower)
                return regularGlower;

            if (building.Position.GetFirstThing(myMap, MyDefs.MediumGlower) is ThingWithComps mediumGlower)
                return mediumGlower;

            if (building.Position.GetFirstThing(myMap, MyDefs.LowGlower) is ThingWithComps lowGlower)
                return lowGlower;

            return null;
        }

        public void SpawnIfNoGlow()
        {
            if (GetGlowMote() != null)
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
            //if(MyDebug) Log.Warning("wantSwitchOn: " + Tools.OkStr(wantSwitchOn) + "!=" + Tools.OkStr(switchOnInt));
            return wantSwitchOn != switchOnInt;
		}

        void ToggleFireOverlay()
        {
            ToolsBuilding.ToggleFireOverlay(parent, myMap);
        }
        void SetFireOverlay(bool value = true)
        {
            ToolsBuilding.SetFireOverlay(parent, myMap, value);
        }

        void UpdateComps()
        {
            ToolsBuilding.ToggleCompGatherSpot(parent, true, SwitchIsOn);
            SetFireOverlay(SwitchIsOn);
        }

        public void DoFlick(bool active = true)
        {
            //Log.Warning("Trying DoFlick ", true);

            if (active)
            {
                SoundDef mySound = SwitchIsOn ? MyDefs.CampFireExtinguishSound : MyDefs.CampFireLightSound;
                mySound.PlayOneShot(new TargetInfo(parent.Position, parent.Map, false));
            }

            Vector3 slightlyAbove = buildingPos + new Vector3(0, 0, .6f);

            // turning off
            if (SwitchIsOn)
            {
                FleckMaker.ThrowSmoke(slightlyAbove, myMap, compFuel.FuelPercentOfMax);
                if (GetGlowMote() is ThingWithComps glower)
                {
                    glower.BroadcastCompSignal(SentSignal);
                }
            }
            // turning on
            else
            {
                FleckMaker.ThrowMicroSparks(slightlyAbove, myMap);
                SpawnIfNoGlow();
            }
            if (MyDebug) Log.Warning("Sending " + SentSignal + " signal");


            //parent.BroadcastCompSignal(SentSignal);

            SwitchIsOn = !SwitchIsOn;
            UpdateComps();

        }

        public override void PostDestroy(DestroyMode mode, Map previousMap)
        {
            if (SwitchIsOn && mode == DestroyMode.KillFinalize)
            {
                int spreadNum;

                if (IsMediumFire)
                    spreadNum = 2;
                else if (IsLowFire)
                    spreadNum = 1;
                else
                    spreadNum = 3;

                for (int i = 0; i < spreadNum; i++)
                    ToolsFire.TrySpread(buildingPos.ToIntVec3(), previousMap);
            }

            base.PostDestroy(mode, previousMap);
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