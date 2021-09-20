using System;
using Verse;
using RimWorld;

namespace StoneCampFire
{
	public class CompProperties_LightableGlower : CompProperties
	{
		//public float overlightRadius;

		public float glowRadius = 14f;
		public ColorInt glowColor = new ColorInt(255, 255, 255, 0) * 1.45f;
        public bool debug = false;

		public CompProperties_LightableGlower()
		{
			compClass = typeof(CompLightableGlower);
		}
	}
}