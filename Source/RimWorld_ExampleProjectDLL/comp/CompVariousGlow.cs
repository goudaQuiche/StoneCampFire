using Verse;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using RimWorld;

namespace StoneCampFire
{
	public class CompVariousGlow : ThingComp
    {
		//private List<ThingComp> comps = new List<ThingComp>();

		public CompGlower glowComp;
		public CompGlower oldGlower;
		public float glowRadius;

        
		private ColorInt newcolor;

		private ColorInt red = new ColorInt(255, 0, 0, 0);
		private ColorInt orange = new ColorInt(255, 127, 0, 0);
		private ColorInt yellow = new ColorInt(255, 255, 0, 0);
		private ColorInt green = new ColorInt(0, 255, 0, 0);
        private ColorInt cyan = new ColorInt(0, 255, 255, 0);
        private ColorInt blue = new ColorInt(0, 0, 255, 0);
		private ColorInt indigo = new ColorInt(75, 0, 130, 0);
		private ColorInt violet = new ColorInt(143, 0, 255, 0);
		public List<ColorInt> colors = new List<ColorInt>();

		private int curIndex;
        

        public enum GlowStatus
        {
            extinct = 0,
            dark = 1,
            light = 2
        };

        //private GlowStatus PreviousStatus;

        private CompProperties_VariousGlow Props => (CompProperties_VariousGlow)props;
        private bool MyDebug => Props.debug;

        public override void PostExposeData()
		{
			base.PostExposeData();
			//Scribe_Values.Look(ref curIndex, "curIndex", 0);
		}

        public GlowStatus MyGlowWay(float perc)
        {
            if (glowComp == null)
                return GlowStatus.extinct;

            if (perc > Props.threshold)
                return GlowStatus.light;
            else
                return GlowStatus.dark;
        }


        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);

            glowComp = parent.GetComp<CompGlower>();
			glowRadius = glowComp.Props.glowRadius;
			
			colors.Add(red);
			colors.Add(orange);
			colors.Add(yellow);
			colors.Add(green);
			colors.Add(blue);
            colors.Add(cyan);
            colors.Add(indigo);
			colors.Add(violet);
            
		}

		
        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            foreach (Gizmo c in base.CompGetGizmosExtra())
            {
                yield return c;
            }
            //if (Faction == Faction.OfColony)
            if (parent.Faction == Faction.OfPlayer)
            {
				{
					Command_Action light = new Command_Action();
					light.defaultLabel = "Change Colour";
					light.defaultDesc = "Change colour of light";
					light.icon = ContentFinder<Texture2D>.Get("UI/Commands/Halt");
					light.action = changeColourTest;
					light.activateSound = SoundDef.Named("Click");
					yield return light;
				}
			}
		}
        

        public override void PostDestroy(DestroyMode mode, Map previousMap)
        {
            UnsetGlow(parent.Map);
            glowComp.PostDestroy(mode, previousMap);
            base.PostDestroy(mode, previousMap);
        }

        private void SetGlow(ColorInt glowColor, float glowRadius)
        {
            // new glower
            CompGlower myThingComp = new CompGlower();
            myThingComp.parent = parent;
            CompProperties_Glower compProps = new CompProperties_Glower
            {
                // setting props
                glowColor = glowColor,
                glowRadius = glowRadius
            };

            // init
            myThingComp.Initialize(compProps);

            // applying to map
            Map myMap = parent.Map;
            IntVec3 myPos = parent.Position;

            myMap.mapDrawer.MapMeshDirty(myPos, MapMeshFlag.Things);
            myMap.glowGrid.RegisterGlower(myThingComp);

            // removing old glow
            // AZAAAAARG
            UnsetGlow();

            // remembering what you did last summer
            glowComp = myThingComp;
        }

        public void UnsetGlow(Map forcedMap=null)
        {
            if (glowComp == null)
            {
                if(MyDebug)Log.Warning("cant unset null glow");
                return;
            }
            Map myMap;
            
            if (forcedMap != null)
            {
                myMap = parent.Map;    
                if (myMap == null)
                {
                    if (MyDebug) Log.Warning("cant unset null map glow");
                    return;
                }
            }
            else
            {
                myMap = forcedMap;
            }

            // removing old glow
            parent.AllComps.Remove(glowComp);
            myMap.glowGrid.DeRegisterGlower(glowComp);

            glowComp = null;
        }

        public void ChangeGlow(float value)
        {
            if (MyDebug) Log.Warning("Fuel : " + value + ";Threshold : " + Props.threshold);

            if (value > Props.threshold)
            {
                SetLightGlow();
                if (MyDebug) Log.Warning("Light glow set");
            }
            else
            {
                SetDarkGlow();
                if (MyDebug) Log.Warning("Dark glow set");
            }
                
            
        }

        public void SetLightGlow()
        {
            SetGlow(Props.lightColor, Props.lightRadius);
        }

        public void SetDarkGlow()
        {
            SetGlow(Props.darkColor, Props.darkRadius);
        }
        
        public void changeColourTest()
		{
            // new glower
			CompGlower myThingComp = new CompGlower();
			myThingComp.parent = this.parent;
            CompProperties_Glower compProps = new CompProperties_Glower();

            // color init
            curIndex = ((curIndex + 1) > 7) ? (0) : (curIndex + 1);
            newcolor = colors[curIndex];

            // setting props
            compProps.glowColor = newcolor;
			compProps.glowRadius = glowRadius;
            // init
			myThingComp.Initialize(compProps);

            // applying to map
            Map myMap = parent.Map;
            IntVec3 myPos = parent.Position;

            myMap.mapDrawer.MapMeshDirty(myPos, MapMeshFlag.Things);
            myMap.glowGrid.RegisterGlower(myThingComp);

            // removing old glow
            parent.AllComps.Remove(glowComp);
            myMap.glowGrid.DeRegisterGlower(glowComp);

            // remembering what you did last summer
            glowComp = myThingComp;
        }
        
	}
}
