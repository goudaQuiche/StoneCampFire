using RimWorld;
using Verse;

namespace StoneCampFire
{
    /*
    [DefOf]
    public static class DesignationDefOf
    {
        public static DesignationDef Extinguish;
    }
    */
    public static class ExtinguishUtility
    {
        public static void UpdateExtinguishDesignation(Thing t)
        {
 
            bool flag = false;
            if (t is ThingWithComps thingWithComps)
            {
                for (int i = 0; i < thingWithComps.AllComps.Count; i++)
                {
                    if (thingWithComps.AllComps[i] is CompExtinguishable compExtinguishable && compExtinguishable.WantsFlick())
                    {
                        Tools.Warn("wants to be extinguished", true);
                        flag = true;
                        break;
                    }
                }
            }
            Designation designation = t.Map.designationManager.DesignationOn(t, DesignationDefOf.Flick);
            if (flag && designation == null)
            {
                t.Map.designationManager.AddDesignation(new Designation(t, DesignationDefOf.Flick));
            }
            else if (!flag && designation != null)
            {
                designation.Delete();
            }
            TutorUtility.DoModalDialogIfNotKnown(ConceptDefOf.SwitchFlickingDesignation);
        }

        public static bool WantsToBeOn(Thing t)
        {
            CompExtinguishable compExtinguishable = t.TryGetComp<CompExtinguishable>();
            if (compExtinguishable != null && !compExtinguishable.SwitchIsOn)
            {
                return false;
            }
            CompSchedule compSchedule = t.TryGetComp<CompSchedule>();
            return compSchedule == null || compSchedule.Allowed;
        }

    }
}