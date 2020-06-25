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

// https://github.com/roxxploxx/RimWorldModGuide/wiki

namespace Cults
{
    [DefOf]
    public static class CultsDefOf
    {
        //-------------------------------------------------------
        // General
        public static LetterDef Cults_Letter_Success;
        public static TimeAssignmentDef Cults_TimeAssignment_Worship;

        //-------------------------------------------------------
        // Buildings
        //public static ThingDef Cults_Building_StandardAltar;

        //-------------------------------------------------------
        // AI, think, work
        public static ThinkTreeDef Cults_InsertHookTest;
        public static JobDef Cults_Job_Worship;

        //-------------------------------------------------------
        // Items
        
        //-------------------------------------------------------
        // Cosmic entities
        public static CosmicEntityDef Cults_CosmicEntity_Cthulhu;
        public static CosmicEntityDef Cults_CosmicEntity_Bast;
        public static CosmicEntityDef Cults_CosmicEntity_Hastur;

        //-------------------------------------------------------
        // ...
    }
}
