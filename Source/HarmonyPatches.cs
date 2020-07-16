using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

using UnityEngine;         // Always needed
using Verse;               // RimWorld universal objects are here (like 'Building')
using Verse.AI;            // Needed when you do something with the AI
using Verse.AI.Group;
using Verse.Sound;         // Needed when you do something with Sound
using Verse.Noise;         // Needed when you do something with Noises
using RimWorld;            // RimWorld specific functions are found here (like 'Building_Battery')
using RimWorld.Planet;     // RimWorld specific functions for world creation

using HarmonyLib;

namespace Cults
{
    [HarmonyPatch(typeof(ResearchManager), "FinishProject")]
    public class ExtraFinishProjectFunction
    {
        private static void Prefix(ref ResearchProjectDef proj) // on research finish "event"
        {
            CultKnowledge.DiscoverRandomDeity();
        }
    }

    [HarmonyPatch(typeof(Pawn_TimetableTracker), "CurrentAssignment", MethodType.Getter)]
    public class WorshipAssignment
    {
        private static TimeAssignmentDef Postfix(TimeAssignmentDef __result) 
        {
            // custom TimeAssignment def breaks vanilla pawn AI functions, do not return it
            return (__result == CultsDefOf.Cults_TimeAssignment_Worship)? TimeAssignmentDefOf.Anything : __result;
        }
    }

    [HarmonyPatch(typeof(InspectPaneFiller), "DrawTimetableSetting")]
    public class FixTimeAssignmentInspectPane
    {
        private static bool Prefix(ref WidgetRow row, ref Pawn pawn)
        {
            // override [InspectPanelFiller] UI current (schedule) assignment draw function
            TimeAssignmentDef realDef = pawn.timetable.times[GenLocalDate.HourOfDay(pawn)];

            if(realDef == CultsDefOf.Cults_TimeAssignment_Worship)
            {
                row.Gap(6f);
                row.FillableBar(93f, 16f, 1f, realDef.LabelCap, realDef.ColorTexture);
                return false; // skip original method
            }
            else
            {
                return true; // normal behavior
            }
        }
    }

}