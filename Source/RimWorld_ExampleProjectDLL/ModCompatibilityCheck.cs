using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace StoneCampFire
{
    [StaticConstructorOnStartup]
    public class ModCompatibilityCheck
    {
        private const string lanius_ModName = "Lanius";

        public static bool LaniusIsActive
        {
            get
            {
                return ModsConfig.ActiveModsInLoadOrder.Any(m => m.Name == lanius_ModName);
            }
        }
    }
}