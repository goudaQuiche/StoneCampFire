using System;
using UnityEngine;
using Verse;
using RimWorld;

namespace StoneCampFire
{
	[StaticConstructorOnStartup]
	public class CompLightableFireOverlay : ThingComp
	{
		private static readonly Graphic FireGraphic = GraphicDatabase.Get<StoneCampFire.MyGraphic_Flicker>("Things/Special/Fire", ShaderDatabase.TransparentPostLight, Vector2.one, Color.white);

        private static readonly Graphic FireMediumGraphic = GraphicDatabase.Get<StoneCampFire.MyGraphic_Flicker>("Things/Special/MediumFire", ShaderDatabase.TransparentPostLight, Vector2.one, Color.white);
        private static readonly Graphic FireLowGraphic = GraphicDatabase.Get<StoneCampFire.MyGraphic_Flicker>("Things/Special/LowFire", ShaderDatabase.TransparentPostLight, Vector2.one, Color.white);

        bool burning = true;

		public CompProperties_LightableFireOverlay Props => (CompProperties_LightableFireOverlay)props;

        public CompExtinguishable compExtinguishable;

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            compExtinguishable = parent.TryGetComp<CompExtinguishable>();
        }

            public void Toggle()
        {
            burning = !burning;
        }

        public void SetBurn(bool value=true)
        {
            burning = value;
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look<bool>(ref burning, "burn", true, false);
        }

        public override void PostDraw()
        {
            base.PostDraw();
            if (!burning)
                return;

            Vector3 drawPos = parent.DrawPos;
            drawPos.y += 0.046875f;

            if (compExtinguishable.IsMediumFire)
            {
                if (compExtinguishable.MyDebug) Log.Warning("IsMediumFire");
                FireMediumGraphic.Draw(drawPos, Rot4.North, parent, .8f);
                return;
            }
            else if(compExtinguishable.IsLowFire)
            {
                if (compExtinguishable.MyDebug) Log.Warning("IsLowFire");
                FireLowGraphic.Draw(drawPos, Rot4.North, parent, .6f);
                return;
            }

            FireGraphic.Draw(drawPos, Rot4.North, parent, 1f);

        }
	}
}