using RimWorld;
using System;
using Verse;

namespace  StoneCampFire
{
    public class CompLightableGlower : CompGlower
    {
        
        protected CompExtinguishable stoneComp;
        protected CompLightableRefuelable fuelComp;

        public ColorInt LitColor =          new ColorInt(252, 187, 113, 0) * 1.45f;
        public ColorInt ExtinguishedColor = new ColorInt(255, 50, 0, 0) * 1.45f;

        public float LitRadius = 10f;
        public float ExtinguishedRadius = 2f;

        private bool glowOnInt;

        //public bool Lit = false;
        
        private new CompProperties_LightableGlower Props
        {
            get
            {
                return (CompProperties_LightableGlower)this.props;
            }
        }
        

        private bool ShouldBeLitNow
        {
            get
            {
                if ( (!this.parent.Spawned) || (stoneComp == null))
                    return false;
                
                return (fuelComp == null || fuelComp.HasFuel) &&
                (stoneComp == null || stoneComp.SwitchIsOn);
            }
        }
        
        
        public new void UpdateLit(Map map)
        {
            bool shouldBeLitNow = this.ShouldBeLitNow;
            if (this.glowOnInt == shouldBeLitNow)
            {
                return;
            }
            this.glowOnInt = shouldBeLitNow;
            if (!this.glowOnInt)
            {
                map.mapDrawer.MapMeshDirty(this.parent.Position, MapMeshFlag.Things);
                map.glowGrid.DeRegisterGlower(this);
            }
            else
            {
                map.mapDrawer.MapMeshDirty(this.parent.Position, MapMeshFlag.Things);
                map.glowGrid.RegisterGlower(this);
            }
        }
        
        public override void ReceiveCompSignal(string signal)
        {
            if (signal == "FlickedOn" || signal == "FlickedOff")
            {
                updateDisplay();
            }
        }
        public void updateDisplay()
        {
            updateProps();
            UpdateLit(parent.Map);
        }
        private void updateProps()
        {
            //CompProperties glow = props.compClass.GetProperty("glowColor").Attributes;

            Props.glowColor = CurrentColor;
            Props.glowRadius = CurrentRadius;
        }
        private ColorInt CurrentColor
        {
            get
            {
                return (ShouldBeLitNow) ? (LitColor) : (ExtinguishedColor);
            }
        }
        private float CurrentRadius
        {
            get
            {
                return (ShouldBeLitNow) ? (LitRadius) : (ExtinguishedRadius);
            }
        }

        
        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            stoneComp = this.parent.GetComp<CompExtinguishable>();
            if (stoneComp == null)
                Tools.Warn("could not find extinguish comp", true);

            fuelComp = parent.GetComp<CompLightableRefuelable>();
            if (fuelComp == null)
                Tools.Warn("could not find fuel comp", true);

            updateProps();

            base.PostSpawnSetup(respawningAfterLoad);
        }
        
    }
}