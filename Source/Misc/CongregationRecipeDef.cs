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

namespace Cults
{
    public class CongregationRecipeDef : RecipeDef
    {
        public bool isWorthy = false;
        public string requiredChoice = "None";
        
        public float additionalFavorGain = 0;
        public IntRange allowedSpellTierRange; // = new RangeInt(0,5);
        public CosmicEntityDef requiredDeity;
        public List<SpellDef> exclusiveSpells; // = new List<SpellDef>();
    }
}