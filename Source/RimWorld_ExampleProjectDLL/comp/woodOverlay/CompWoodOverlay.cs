using RimWorld;
using System;
using System.Linq;
using UnityEngine;
using Verse;

namespace StoneCampFire
{
    // Main
    public class CompWoodOverlay : ThingComp
    {
        public CompProperties_WoodOverlay Props => (CompProperties_WoodOverlay)props;
        public bool myDebug => Props.debug;
        public Building MyBuilding => (Building)parent;

        public float DrawSize => Math.Max(MyBuilding.Graphic.drawSize.x, MyBuilding.Graphic.drawSize.y);
        public CompExtinguishable comp;

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            comp = MyBuilding.TryGetComp<CompExtinguishable>();
        }

        public override void PostDraw()
        {
            base.PostDraw();
            //Tools.Warn("Comp_LTF_Hydra_PostDraw drawSize:"+ DrawSize, myDebug);

            if (!comp.compFuel.HasFuel && !comp.HasEverBurnt)
                return;

            Vector3 BuildingPos = parent.DrawPos;

            // Copying mech attributes
            Vector3 BuildingSize = new Vector3(DrawSize, 1f, DrawSize);
            Matrix4x4 BuildingMatrix = default(Matrix4x4);
            Vector3 WoodPos = BuildingPos;

            //Adding vertical offset
            WoodPos.y = BuildingPos.y + .1f;

            // finalize matrix
            BuildingMatrix.SetTRS(WoodPos, Quaternion.identity, BuildingSize);

            // retrieving material
            Material WoodMaterial = null;

            if (!comp.compFuel.HasFuel && !comp.SwitchIsOn && comp.HasEverBurnt)
                WoodMaterial = Props.coalMaterial.DrawMatSingle;
            else if (comp.compFuel.HasFuel)
                WoodMaterial = Props.woodLogMaterial.DrawMatSingle;

            if (WoodMaterial != null)
                // drawing
                Graphics.DrawMesh(MeshPool.plane10, BuildingMatrix, WoodMaterial, 0);
        }
    }
}