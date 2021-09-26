using System.Collections.Generic;
using Verse;
using RimWorld;
using HarmonyLib;
using System.Reflection;

namespace StoneCampFire
{
    public class HarmonyPatch_Mote
    {
        [HarmonyPatch(typeof(Building_WorkTable), "GetFloatMenuOptions")]
        static class Mote_HarmonyPatch
        {
            static IEnumerable<FloatMenuOption> Postfix(IEnumerable<FloatMenuOption> __result, Building __instance, Pawn myPawn)
            {
                foreach(FloatMenuOption FMO in __result)
                {
                    yield return FMO;
                }

                if(__instance.TryGetComp<CompSmokeSignalComms>() is CompSmokeSignalComms CSSC)
                {
                    foreach(FloatMenuOption FMO in CSSC.CompFloatMenuOptions(myPawn))
                    {
                        yield return FMO;
                    }
                }
            }
        }
    }

    [StaticConstructorOnStartup]
    static class HarmonyPatchAll
    {
        static HarmonyPatchAll()
        {
            Harmony StoneCampfireHarmonyPatch = new Harmony("goudaQuiche.StoneCampfire");

            StoneCampfireHarmonyPatch.PatchAll(Assembly.GetExecutingAssembly());
        }
    }
}