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
    public class CultKnowledge : GameComponent
    {
        public CultKnowledge(Game game)
        {
        }
        public static List<CosmicEntity> deities; // discovered deities

        // Save
        private static string cultName = "Unnamed cult";
        public static bool isExposed => CultsDefOf.Cults_ForbiddenStudies.IsFinished;
        public static bool performSermons;
        public static CosmicEntityDef selectedDeity;

        // functions
        public static void RenameCult(string n)
        {
            CultKnowledge.cultName = n;
        }
        public static string GetCultName() => CultKnowledge.cultName;

        /*
        public static void ExposeToHorror()
        {
            Find.LetterStack.ReceiveLetter("Cult beginning", CultsDefOf.Cults_Letter_Success.description, CultsDefOf.Cults_Letter_Success, null);
            isExposed = true;
        }
        */

        public static void GiveFavor(CosmicEntityDef def, float f)
        {
            for(int i = 0; i < deities.Count; i++)
            {
                if(deities[i].def == def) deities[i].GiveFavor(f);
            }
        }

        public override void FinalizeInit() // first
        {
        }
        public override void LoadedGame() // second
        {
        }

        public override void ExposeData()
        {
            if(deities == null)
            {
                deities = new List<CosmicEntity>();
                List<CosmicEntityDef> available_defs = DefDatabase<CosmicEntityDef>.AllDefs.ToList();
                foreach(CosmicEntityDef d in available_defs)
                {
                    deities.Add(new CosmicEntity(){ def = d });
                }
            }
            //Scribe_Values.Look(ref isExposed, "isExposed", false, true);
            Scribe_Values.Look(ref cultName, "cultName", "Unnamed cult", true);
            Scribe_Values.Look(ref performSermons, "performSermons", true, true);
            Scribe_Defs.Look(ref selectedDeity, "selectedDeity");
            Scribe_Collections.Look(ref deities, "deities", LookMode.Deep);
        }

        public static void DiscoverRandomDeity()
        {
            List<CosmicEntity> discoverable = new List<CosmicEntity>();
            if(deities == null) return;
            foreach(CosmicEntity deity in deities)
            {
                if(!deity.isDiscovered) discoverable.Add(deity);
            }

            

            if(discoverable.Count == 0){
                Log.Message("Discovered all deities");
                return;
            }

            // select random and discover
            int index = Math.Abs(Rand.Int) % discoverable.Count;
            //Log.Message(Rand.Int.ToString() + " % " + discoverable.Count + " = " + index);
            Messages.Message("Discovered " + discoverable[index].def.label, null, MessageTypeDefOf.PositiveEvent);
            discoverable[index].Discover(); //.Add(new CosmicEntity(){ def = available_defs[index] });

            // sort list
            deities.SortBy(d => d.def.index);
        }
    }

    /*
    public class MyMapComponent : MapComponent
    {
        public MyMapComponent(Map map) : base(map){}
        public override void FinalizeInit()
        {
            Messages.Message("Success", null, MessageTypeDefOf.PositiveEvent);
            Find.LetterStack.ReceiveLetter("Success", CultsDefOf.Cults_Letter_Success.description, CultsDefOf.Cults_Letter_Success, null);
        }
    }
    */

    [StaticConstructorOnStartup]
    public static class Start
    {
        static Start() // quick debug
        {
            Harmony harmony = new Harmony( "Arvkus.Cults" );
            harmony.PatchAll( Assembly.GetExecutingAssembly() );
        }
    }

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
