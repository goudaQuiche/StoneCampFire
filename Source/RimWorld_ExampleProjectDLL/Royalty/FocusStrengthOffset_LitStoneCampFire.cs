using Verse;
using RimWorld;

namespace StoneCampFire
{
    public class FocusStrengthOffset_LitStoneCampFire : FocusStrengthOffset_Lit
    {
        public override bool CanApply(Thing parent, Pawn user = null)
        {
            if (parent.TryGetComp<CompExtinguishable>() is CompExtinguishable comp)
                return comp.SwitchIsOn;

            return base.CanApply(parent, user);
        }
    }
}