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

//using System.Reflection;
//using HarmonyLib;

/*
First deity is discovered via research, rest of them via worship
New scheduled work type - Worship
Sermons now occur (with a small chance) when 3 or more colonists worship at the same time
No more occult research center forbid, pawns will use this bench only if current research project is occult one.

Finish forship AI job
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
        public static bool isExposed;
        public static bool performSermons;
        public static CosmicEntityDef selectedDeity;

        // functions
        public static void RenameCult(string n)
        {
            CultKnowledge.cultName = n;
        }
        public static string GetCultName() => CultKnowledge.cultName;


        public override void FinalizeInit() // first
        {
        }
        public override void LoadedGame() // second
        {
        }

        public override void ExposeData()
        {
            Scribe_Values.Look(ref isExposed, "isExposed", false, true);
            Scribe_Values.Look(ref cultName, "cultName", "Unnamed cult", true);
            Scribe_Values.Look(ref performSermons, "performSermons", true, true);
            Scribe_Defs.Look(ref selectedDeity, "selectedDeity");
            Scribe_Collections.Look(ref deities, "deities", LookMode.Deep);
        }

        public static void DiscoverDeity()
        {
            List<CosmicEntityDef> available_defs = new List<CosmicEntityDef>{
                Cults.CultsDefOf.Cults_CosmicEntity_Cthulhu,
                Cults.CultsDefOf.Cults_CosmicEntity_Nyarlathotep,
                Cults.CultsDefOf.Cults_CosmicEntity_Dagon,
                Cults.CultsDefOf.Cults_CosmicEntity_Hastur,
                Cults.CultsDefOf.Cults_CosmicEntity_Shub,
                Cults.CultsDefOf.Cults_CosmicEntity_Tsathoggua,
                Cults.CultsDefOf.Cults_CosmicEntity_Bast,
            };

            // remove duplicated
            if(deities == null) deities = new List<CosmicEntity>();
            foreach(CosmicEntity deity in deities)
            {
                for(int i = 0; i < available_defs.Count; i++)
                {
                    if(deity.def == available_defs[i]) {
                        available_defs.RemoveAt(i);
                        i -= 1;
                    }
                }
            }

            if(available_defs.Count == 0){
                Log.Message("Discovered all deities");
                return;
            }

            // select random and discover
            System.Random rand = new System.Random();
            int index = rand.Next() % available_defs.Count;
            deities.Add(new CosmicEntity(available_defs[index]));
            Messages.Message("Discovered " + available_defs[index].label, null, MessageTypeDefOf.PositiveEvent);

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
            /*
            Harmony harmony = new Harmony( "Arvkus.Cults" );
            harmony.PatchAll( Assembly.GetExecutingAssembly() );
            */

            Log.Message(ModsConfig.IsActive(ModContentPack.RoyaltyModPackageId).ToString());
            Log.Message("Cults mod success");
        }
    }

}
