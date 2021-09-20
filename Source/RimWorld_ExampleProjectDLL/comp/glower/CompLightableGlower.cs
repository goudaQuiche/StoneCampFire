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
        
        private new CompProperties_LightableGlower Props => (CompProperties_LightableGlower)props;
        private bool MyDebug => Props.debug;

        private bool ShouldBeLitNow
        {
            get
            {
                if ( (!parent.Spawned) || (stoneComp == null))
                    return false;
                
                return (fuelComp == null || fuelComp.HasFuel) && (stoneComp == null || stoneComp.SwitchIsOn);
            }
        }
        
        
        public new void UpdateLit(Map map)
        {
            bool shouldBeLitNowVar = this.ShouldBeLitNow;
            if (glowOnInt == shouldBeLitNowVar)
            {
                return;
            }
            glowOnInt = shouldBeLitNowVar;
            if (!glowOnInt)
            {
                map.mapDrawer.MapMeshDirty(parent.Position, MapMeshFlag.Things);
                map.glowGrid.DeRegisterGlower(this);
            }
            else
            {
                map.mapDrawer.MapMeshDirty(parent.Position, MapMeshFlag.Things);
                map.glowGrid.RegisterGlower(this);
            }
        }
        
        public override void ReceiveCompSignal(string signal)
        {
            if (MyDebug) Log.Warning("recieved " + signal + " signal");

            switch (signal)
            {
                case "FlickedOn":
                case "FlickedOff":
                case "Refueled":
                case "RanOutOfFuel":
                    UpdateLit(parent.Map);
                    break;
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
        private ColorInt CurrentColor => ShouldBeLitNow ? LitColor : ExtinguishedColor;
        private float CurrentRadius => ShouldBeLitNow ? LitRadius : ExtinguishedRadius;

        
        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            stoneComp = this.parent.GetComp<CompExtinguishable>();
            if (stoneComp == null && MyDebug)
                Log.Warning("could not find extinguish comp");

            fuelComp = parent.GetComp<CompLightableRefuelable>();
            if (fuelComp == null && MyDebug)
                Log.Warning("could not find fuel comp");

            updateProps();

            base.PostSpawnSetup(respawningAfterLoad);
        }
        
    }
}