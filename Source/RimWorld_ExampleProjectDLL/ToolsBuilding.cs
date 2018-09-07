using RimWorld;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

using Verse;               // RimWorld universal objects are here (like 'Building')
using Verse.Sound;

using UnityEngine;

namespace StoneCampFire
{

    public class ToolsBuilding
    {

        public enum Link { Orphan = 0, Linked = 1 };
        public static string[] LinkLabel = { "Orphan", "Linked" };

        public static bool ValidLink(Link val)
        {

            int cast = (int)val;
            int min = (int)Link.Orphan;
            int max = (int)Link.Linked;

            return ((cast >= min) && (cast <= max));

        }

        //Building dependencies
        public static bool CheckPower(Building building)
        {
            CompPowerTrader comp = null;
            comp = building?.TryGetComp<CompPowerTrader>();

            if (comp == null || !comp.PowerOn)
                return false;

            return true;
        }
        public static bool CheckPower(CompPowerTrader comp)
        {
            if (comp == null || !comp.PowerOn)
                return false;

            return true;
        }
        public static bool CheckBuilding(Building building)
        {
            if (building == null || building.Map == null || building.Position == null)
                return false;
            return true;
        }
        public static CompAffectedByFacilities GetAffectedComp(Building building, bool debug = false)
        {
            if (!CheckBuilding(building))
            {
                Tools.Warn("//bad building, wont check facility", debug);
                return null;
            }

            CompAffectedByFacilities affectedComp;
            affectedComp = building.TryGetComp<CompAffectedByFacilities>();

            if (affectedComp == null)
            {
                Tools.Warn("//no affected by facility comp found", debug);
                return null;
            }

            return affectedComp;
        }
        public static Building GetFacility(CompAffectedByFacilities buildingFacilityComp, bool debug = false)
        {
            if (buildingFacilityComp == null)
            {
                Tools.Warn("//no comp", debug);
                return null;
            }

            //Building.CompFacility legit
            if (buildingFacilityComp.LinkedFacilitiesListForReading.NullOrEmpty())
            {
                Tools.Warn("//no linked facility found", debug);
                return null;
            }
            Tools.Warn("Found: " + buildingFacilityComp.LinkedFacilitiesListForReading.Count + " facilities", debug);

            Thing thing = null;
            thing = buildingFacilityComp.LinkedFacilitiesListForReading.RandomElement();
            if (thing == null)
            {
                // will happen on load
                Tools.Warn("no facility found; ok on load", debug);
                return null;
            }
            Building newFacility = thing as Building;

            return newFacility;
        }

        // Stone camp fire
        public static void SetCompGlower(Thing t, Map map, bool burning=true)
        {
            Tools.Warn("Trying to set glow:"+burning, true);
            CompGlower comp = t.TryGetComp<CompGlower>();
            if (comp is CompGlower) {
                /*
                CompGlower myThingComp = new CompGlower();
                myThingComp.parent = (ThingWithComps)t;

                CompProperties compProps = new CompProperties();
                compProps.compClass = typeof(CompGlower);

                ColorInt newcolor = (burning)?(new ColorInt(252, 187, 113, 0) * 1.45f):( new ColorInt(255, 50, 0, 0) * 1.45f);
                compProps.compClass.GetProperties.
                compProps.glow = newcolor;
                compProps.glowRadius = glowRadius;
                */
                //myThingComp.Initialize(compProps);
                comp.Props.glowColor = (burning) ? (new ColorInt(252, 187, 113, 0) * 1.45f) : (new ColorInt(255, 50, 0, 0) * 1.45f);
                comp.Props.glowRadius = (burning) ? 10f : 2f;
                
                Tools.Warn(" glow rad:" + comp.Props.glowRadius, true);
                comp.Initialize(comp.Props);
                comp.UpdateLit(map);
                map.glowGrid.RegisterGlower(comp);
                comp.UpdateLit(map);

            } else
                Tools.Warn("should have found a CompGlower but no CompGlower found", true);
        }
        // Stone camp fire
        public static void SetFlickableGlower(Thing t, Map map, bool burning = true)
        {
            Tools.Warn("Trying to set glow:" + burning, true);
            CompLightableGlower comp = t.TryGetComp<CompLightableGlower>();
            comp.updateDisplay();
        }

            public static void UnsetCompHeatPusher(Thing t, Map map)
        {
            CompHeatPusher comp = t.TryGetComp<CompHeatPusher>();
            if (comp is CompHeatPusher)
                comp.PostDestroy(DestroyMode.Vanish, map);
            //comp.PostDeSpawn(map);
            else
                Tools.Warn("should have found a CompHeatPusher but no CompHeatPusher found", true);
        }
        public static void ToggleFireOverlay(Thing t, Map map)
        {
            CompLightableFireOverlay comp = t.TryGetComp<CompLightableFireOverlay>();
            if (comp is CompLightableFireOverlay)
            {
                    comp.Toggle();
            }
            else
                Tools.Warn("should have found a CompFireOverlay but no CompFireOverlay found", true);
        }
        public static void SetFireOverlay(Thing t, Map map, bool value = true)
        {
            CompLightableFireOverlay comp = t.TryGetComp<CompLightableFireOverlay>();
            if (comp is CompLightableFireOverlay)
            {
                Tools.Warn("Trying to set fire overlay:" + value, true);
                comp.SetBurn(value);
            }
            else
                Tools.Warn("should have found a CompFireOverlay but no CompFireOverlay found", true);
        }
        public static void ToggleCompGatherSpot(Thing t, bool force = false, bool value = false)
        {
            CompGatherSpot comp = t.TryGetComp<CompGatherSpot>();
            if (comp is CompGatherSpot)
            {
                if(!force)
                    comp.Active = false;
                else
                    comp.Active = value;
            }
            else
                Tools.Warn("should have found a CompGatherSpot but no CompGatherSpot found", true);
        }
    }
}