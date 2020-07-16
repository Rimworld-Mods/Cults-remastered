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
    // to differentiate regular research from occult one
    public class ForbiddenResearchExtension : DefModExtension
    {
    }

    public class CongregationRecipeRewardExtension : DefModExtension
    {
        public ThingDef DefGraphic;
        public float additionalFavorGain = 0;
        public bool isOfferWorthy = false;
        public RangeInt allowedTierRange; // = new RangeInt(0,5);
        public CosmicEntityDef requiredDeity;
        public List<SpellDef> exclusiveSpells; // = new List<SpellDef>();


    }
}