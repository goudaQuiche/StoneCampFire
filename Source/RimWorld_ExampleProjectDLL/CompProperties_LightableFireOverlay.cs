using System;
using UnityEngine;
using Verse;

namespace StoneCampFire
{
	public class CompProperties_LightableFireOverlay : CompProperties
	{
		public float fireSize = 1f;
        public Vector3 offset;

        public CompProperties_LightableFireOverlay()
		{
			this.compClass = typeof(CompLightableFireOverlay);
		}
	}
}