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
    // TODO: add other altars

    public abstract class Building_AbstractAltar : Building
    {
        //public CosmicEntityDef selectedDeity;
    }

    public class Building_StandardAltar : Building_AbstractAltar
    {
        public override void SpawnSetup(Map map, bool ral)
        {
            base.SpawnSetup(map, ral);
            Log.Message("Built altar " + ral.ToString() + " !");
            Log.Message("   " + this.def.label);
            //Log.Message("   " + this.selectedDeity);

            
        }
        /*
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Defs.Look(ref this.selectedDeity, "selectedDeity");
        }
        */
    }


    public class Building_OccultResearchBench : Building_ResearchBench
    {
        public Building_OccultResearchBench(){}
        private int num = 0;
        public override void Tick(){
            base.Tick();
            num +=1;
            Log.Message(num.ToString());
        }



    }


}
