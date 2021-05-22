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
using Verse.Grammar;
using RimWorld;            // RimWorld specific functions are found here (like 'Building_Battery')
using RimWorld.Planet;     // RimWorld specific functions for world creation

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

        public static CosmicEntity getDeityFromDef(CosmicEntityDef def)
        {
            for(int i = 0; i < deities.Count; i++) if(deities[i].def == def) return deities[i];
            return null;
        }
    }

}
