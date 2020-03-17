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
        String buildingName = string.Empty;
        String myDefName = string.Empty;
        Map myMap = null;

        CompLightableRefuelable compFuel = null;

        private bool switchOnInt = true;

		private bool wantSwitchOn = true;

		private Graphic offGraphic;

		private Texture2D cachedCommandTex;

		private const string OffGraphicSuffix = "_Off";
		public const string FlickedOnSignal = "FlickedOn";
		public const string FlickedOffSignal = "FlickedOff";

		private CompProperties_Extinguishable Props
		{
			get
			{
				return (CompProperties_Extinguishable)this.props;
			}
		}

        public float ExtinguishInRainChance
        {
            get
            {
                return Props.extinguishInRainChance;
            }
        }
        public bool RainProof
        {
            get
            {
                return Props.rainProof;
            }
        }
        public bool OxygenLackProof
        {
            get
            {
                return Props.oxygenLackProof;
            }
        }
        public bool RainVulnerable
        {
            get
            {
                return !RainProof;
            }
        }
        public bool OxygenVulnerable
        {
            get
            {
                return !OxygenLackProof;
            }
        }

        private Texture2D CommandTex
		{
			get
			{
				if (this.cachedCommandTex == null)
				{
                    if (SwitchIsOn)
                    {
                        this.cachedCommandTex = ContentFinder<Texture2D>.Get(this.Props.commandTextureOff, true);
                    }
                    else
                    {
                        this.cachedCommandTex = ContentFinder<Texture2D>.Get(this.Props.commandTextureOn, true);
                    }
				}
				return this.cachedCommandTex;
			}
		}

        private Texture2D MyCommandTex
        {
            get
            {
                return ContentFinder<Texture2D>.Get( (SwitchIsOn)?(Props.commandTextureOff) :(Props.commandTextureOn), true);
            }
        }

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

				parent.BroadcastCompSignal((switchOnInt)?"FlickedOn": "FlickedOff");
				if (parent.Spawned)
				{
					this.parent.Map.mapDrawer.MapMeshDirty(this.parent.Position, MapMeshFlag.Things | MapMeshFlag.Buildings);
				}
			}
		}

		public Graphic CurrentGraphic
		{
			get
			{
				if (this.SwitchIsOn)
				{
					return this.parent.DefaultGraphic;
				}
				if (this.offGraphic == null)
				{
                    //this.offGraphic = GraphicDatabase.Get(this.parent.def.graphicData.graphicClass, this.parent.def.graphicData.texPath + "_Off", ShaderDatabase.ShaderFromType(this.parent.def.graphicData.shaderType), this.parent.def.graphicData.drawSize, this.parent.DrawColor, this.parent.DrawColorTwo);
                    this.offGraphic = GraphicDatabase.Get(this.parent.def.graphicData.graphicClass, this.parent.def.graphicData.texPath + "_Off", ShaderDatabase.LoadShader(this.parent.def.graphicData.texPath), this.parent.def.graphicData.drawSize, this.parent.DrawColor, this.parent.DrawColorTwo);
                    //ShaderFromType(this.parent.def.graphicData.shaderType), this.parent.def.graphicData.drawSize, this.parent.DrawColor, this.parent.DrawColorTwo);
                }
				return this.offGraphic;
			}
		}
        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            //Building
            building = (Building)parent;
            
            myDefName = building?.def?.defName;
            myDef = ThingDef.Named(myDefName);

            buildingPos = building.DrawPos;
            buildingName = building?.LabelShort;

            myMap = building?.Map;

            compFuel = this.parent.GetComp<CompLightableRefuelable>();

            //ChangeComps();
        }

        public override void PostExposeData()
		{
			base.PostExposeData();
			Scribe_Values.Look<bool>(ref this.switchOnInt, "switchOn", true, false);
			Scribe_Values.Look<bool>(ref this.wantSwitchOn, "wantSwitchOn", true, false);
		}

		public bool WantsFlick()
		{
            //Tools.Warn("wantSwitchOn: " + Tools.OkStr(wantSwitchOn) + "!=" + Tools.OkStr(switchOnInt), true);
            return wantSwitchOn != switchOnInt;
		}
        void MySetCompGlower()
        {
            ToolsBuilding.SetCompGlower(parent, compFuel.FuelPercentOfMax, true);
            //ToolsBuilding.SetCompGlower(parent, myMap, true);

        }
        void MyUnsetCompGlower()
        {
            //ToolsBuilding.Un
            ToolsBuilding.SetCompGlower(parent, compFuel.FuelPercentOfMax, false);

        }

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
        /*
        void MySetCompHeatPusher()
        {
            ToolsBuilding.SetCompHeatPusher(parent, myMap, myDef);
        }
        */

        void ChangeComps()
        {
            //Tools.Warn("Trying to change comp: "+Tools.OkStr(SwitchIsOn), true);

            if (SwitchIsOn)
            {
                Tools.Warn("[ON]Trying to set glow: " + Tools.OkStr(SwitchIsOn), true);
                //Glower
                MySetCompGlower();
            }
            else
            {
                Tools.Warn("[OFF]Trying to set glow: " + Tools.OkStr(SwitchIsOn), true);
                //Glower
                MyUnsetCompGlower();
            }

            //heat
            //MySetCompHeatPusher();
            // gather
            ToolsBuilding.ToggleCompGatherSpot(parent, true, SwitchIsOn);
            //fire overlay
            SetFireOverlay(SwitchIsOn);
            //ToggleFireOverlay();

        }

		public void DoFlick(bool active=true)
		{
            //Tools.Warn("Trying DoFlick ", true);

            if (active)
            {
                SoundDef mySound = SoundDef.Named((SwitchIsOn) ? ("CampFireExtinguish") : ("CampFireLight"));
                mySound.PlayOneShot(new TargetInfo(this.parent.Position, this.parent.Map, false));
            }

            if (SwitchIsOn)
            {
                MoteMaker.ThrowSmoke(buildingPos, myMap, compFuel.FuelPercentOfMax);
                    //MakeWaterSplash(pPos.ToVector3Shifted(), pawn.Map, radius, 15f);
            }
            else
            {
                MoteMaker.ThrowMicroSparks(buildingPos, myMap);
            }

            SwitchIsOn = !SwitchIsOn;
            ChangeComps();
            
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
			if (this.parent.Faction == Faction.OfPlayer)
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