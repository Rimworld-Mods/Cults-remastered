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


    public class CultKnowledge : GameComponent
    {
        public CultKnowledge(Game game){}

        public static bool is_exposed;
        public static CosmicEntityDef selected_deity;
        public List<CosmicEntity> deities;

		public override void LoadedGame()
		{
            Log.Message("Game is loaded! filling deity list");
            deities = new List<CosmicEntity>();
            deities.Add(new CosmicEntity(CultsDefOf.Cults_CosmicEntity_Cthulhu));
            deities.Add(new CosmicEntity(CultsDefOf.Cults_CosmicEntity_Bast));
            deities.Add(new CosmicEntity(CultsDefOf.Cults_CosmicEntity_Hastur));
		}

        public override void FinalizeInit()
		{
            Log.Message("Game init!");
		}

        public override void ExposeData()
        {
            Scribe_Values.Look(ref is_exposed, "isExposed", false, true);
            Scribe_Defs.Look(ref selected_deity, "selectedDeity");
            Scribe_Collections.Look(ref deities, "deities", LookMode.Deep, deities);

            /*
            Dictionary<string, int> test = new Dictionary<string, int>();
            test.Add("key1",23);
            test.Add("key2",13);


            Scribe_Collections.Look(ref test, "dict", LookMode.Value);
            */



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
    public static class MyMod
    {
        static MyMod() //our constructor
        {

            Log.Message(ModsConfig.IsActive(ModContentPack.RoyaltyModPackageId).ToString());
            Log.Message("Cults mod success"); //Outputs "Hello World!" to the dev console.
            //

            //CosmicEntity deity = new CosmicEntity(CultsDefOf.Cults_CosmicEntity_Cthulhu);
            //Log.Message("list: " + deity.def.tiers[0]);
        }
    }


    public class Building_StandardAltar : Building
    {
        public override void SpawnSetup(Map map, bool ral)
        {
            base.SpawnSetup(map, ral);
            Log.Message("Built altar " + ral.ToString() + " !");
            Log.Message("   " + this.def.label);
        }
    }
}
