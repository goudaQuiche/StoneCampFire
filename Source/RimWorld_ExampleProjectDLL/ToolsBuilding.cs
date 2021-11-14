using RimWorld;
using Verse;   


namespace StoneCampFire
{

    public class ToolsBuilding
    {

        public static bool CheckBuilding(Building building)
        {
            if (building == null || building.Map == null || building.Position == null)
                return false;
            return true;
        }
        

        public static void UnsetCompHeatPusher(Thing t, Map map)
        {
            CompHeatPusher comp = t.TryGetComp<CompHeatPusher>();
            if (comp is CompHeatPusher)
                comp.PostDestroy(DestroyMode.Vanish, map);
            //comp.PostDeSpawn(map);
            else
                Log.Warning("should have found a CompHeatPusher but no CompHeatPusher found");
        }

        public static void ToggleFireOverlay(Thing t, Map map)
        {
            CompLightableFireOverlay comp = t.TryGetComp<CompLightableFireOverlay>();
            if (comp is CompLightableFireOverlay)
            {
                    comp.Toggle();
                return;
            }
            
            Log.Warning("should have found a CompFireOverlay but no CompFireOverlay found");
        }
        public static void SetFireOverlay(Thing t, Map map, bool value = true)
        {
            CompLightableFireOverlay comp = t.TryGetComp<CompLightableFireOverlay>();
            if (comp is CompLightableFireOverlay)
            {
                //Log.Warning("Trying to set fire overlay:" + value, true);
                comp.SetBurn(value);
                return;
            }
            
            Log.Warning("should have found a CompFireOverlay but no CompFireOverlay found");
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

                return;
            }
            
            Log.Warning("should have found a CompGatherSpot but no CompGatherSpot found");
        }
    }
}