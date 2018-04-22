using System;
using UnityEngine;
using Verse;
using RimWorld;

namespace StoneCampFire
{
	[StaticConstructorOnStartup]
	public class CompLightableFireOverlay : ThingComp
	{
		private static readonly Graphic FireGraphic = GraphicDatabase.Get<StoneCampFire.Graphic_Flicker>("Things/Special/Fire", ShaderDatabase.TransparentPostLight, Vector2.one, Color.white);
        bool burning = true;

		public CompProperties_LightableFireOverlay Props
		{
			get
			{
				return (CompProperties_LightableFireOverlay)this.props;
			}
		}

        public void Toggle()
        {
            burning = !burning;
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look<bool>(ref this.burning, "burn", true, false);
        }

        public override void PostDraw()
		{
			base.PostDraw();
            if (burning)
            {
                Vector3 drawPos = this.parent.DrawPos;
                drawPos.y += 0.046875f;
                FireGraphic.Draw(drawPos, Rot4.North, this.parent, 0f);
            }
		}
	}
}