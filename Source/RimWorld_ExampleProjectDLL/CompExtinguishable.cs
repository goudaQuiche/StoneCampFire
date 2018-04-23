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
				if (switchOnInt)
				{
					parent.BroadcastCompSignal("FlickedOn");
				}
				else
				{
					parent.BroadcastCompSignal("FlickedOff");
				}
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
					this.offGraphic = GraphicDatabase.Get(this.parent.def.graphicData.graphicClass, this.parent.def.graphicData.texPath + "_Off", ShaderDatabase.ShaderFromType(this.parent.def.graphicData.shaderType), this.parent.def.graphicData.drawSize, this.parent.DrawColor, this.parent.DrawColorTwo);
				}
				return this.offGraphic;
			}
		}
        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            //Building
            building = (Building)parent;
            myDefName = building?.def?.label;
            buildingPos = building.DrawPos;
            buildingName = building?.LabelShort;

            myMap = building?.Map;

            compFuel = this.parent.GetComp<CompLightableRefuelable>();
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

        void MyUnsetCompGlower()
        {
            ToolsBuilding.UnsetCompGlower(parent, myMap);
        }
        void ToggleFireOverlay()
        {
            ToolsBuilding.ToggleFireOverlay(parent, myMap);
        }

        void MyUnsetCompHeatPusher()
        {
            ToolsBuilding.UnsetCompHeatPusher(parent, myMap);
        }

        void ChangeComps()
        {
            //Tools.Warn("Trying to change comp: "+Tools.OkStr(SwitchIsOn), true);

            if (SwitchIsOn)
            {
                //Glower
                MyUnsetCompGlower();
                //Gather
                ToolsBuilding.ToggleCompGatherSpot(parent);
                //heat
                MyUnsetCompHeatPusher();
                //fire overlay
                
            }
            ToggleFireOverlay();

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

            ChangeComps();

            SwitchIsOn = !SwitchIsOn;
        }

		public void ResetToOn()
		{
			switchOnInt = true;
			wantSwitchOn = true;
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
                    hotKey = KeyBindingDefOf.CommandTogglePower,
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