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

using System.Reflection;
using HarmonyLib;

/*
First deity is discovered via research, rest of them via worship
New scheduled work type - Worship
Sermons now occur (with a small chance) when 3 or more colonists worship at the same time
No more occult research center forbid, pawns will use this bench only if current research project is occult one.

// Finish evaluations
// Finish UI and job giving
// Implement Things (pawns / items)
// Implement Spells
// Implement 

Bugs:
    When schedule worship and pawn sleeps - error
*/

namespace Cults
{
    public static class Tick{
        public static int minute = 60;
        public static int hour = 2_500;
        public static int day = 60_000;
        public static int quadrum = 900_000;
        public static int year = 3_600_000;

        public static int rareTick = 250;
        public static int longTick = 2000;
    }

    [StaticConstructorOnStartup]
    public static class Start
    {
        static Start() // quick debug
        {
            Harmony harmony = new Harmony( "Arvkus.Cults" );
            harmony.PatchAll( Assembly.GetExecutingAssembly() );
        }
    }
}
