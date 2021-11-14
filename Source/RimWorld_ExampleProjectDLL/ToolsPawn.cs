using RimWorld;
using System;
using UnityEngine;
using Verse;

namespace StoneCampFire
{
    public static class ToolsPawn
    {
        public static bool IsPawnTribal(this Pawn pawn)
        {
            return pawn.kindDef.defaultFactionType.techLevel == TechLevel.Neolithic;
        }
       
    }
}