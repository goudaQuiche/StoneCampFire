using Verse;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using RimWorld;

namespace StoneCampFire
{
	public class CompVariousGlow : ThingComp
    {
		private List<ThingComp> comps = new List<ThingComp>();
		public CompGlower glowComp;
		public CompGlower oldGlower;
		public float glowRadius;

		private ColorInt newcolor;
		//private ColorInt plain = new ColorInt(217, 217, 208, 0);

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

        private CompProperties_VariousGlow Props
        {
            get
            {
                return (CompProperties_VariousGlow)this.props;
            }
        }

        public override void PostExposeData()
		{
			base.PostExposeData();
			Scribe_Values.Look(ref curIndex, "curIndex", 0);
		}

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);

            glowComp = parent.GetComp<CompGlower>();
			glowRadius = glowComp.Props.glowRadius;
			//colors.Add(plain);
			colors.Add(red);
			colors.Add(orange);
			colors.Add(yellow);
			colors.Add(green);
			colors.Add(blue);
            colors.Add(cyan);
            colors.Add(indigo);
			colors.Add(violet);
		}

		//public override IEnumerable<Gizmo> GetGizmos()
        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            foreach (Gizmo c in base.CompGetGizmosExtra())
            {
                yield return c;
            }
            //if (Faction == Faction.OfColony)
            if (this.parent.Faction == Faction.OfPlayer)
            {
				{
					Command_Action light = new Command_Action();
					light.defaultLabel = "Change Colour";
					light.defaultDesc = "Change colour of light";
					light.icon = ContentFinder<Texture2D>.Get("UI/Commands/Halt");
					light.action = changeColour;
					light.activateSound = SoundDef.Named("Click");
					yield return light;
				}
			}
		}

        public override void PostDestroy(DestroyMode mode, Map previousMap)
        {
            this.glowComp.PostDestroy(mode, previousMap);
            base.PostDestroy(mode, previousMap);
        }

		public void changeColour()
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
