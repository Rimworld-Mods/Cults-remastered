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
    class CompProperties_OccultThing : CompProperties 
    {
        public CompProperties_OccultThing()
		{
			this.compClass = typeof(CompOccultThing);
		}

        // gives bonus offer or sacrifice success factor to [favoredBy] deities
        public List<CosmicEntityDef> favoredBy = new List<CosmicEntityDef>(); 

        // Artefacts: reward spells are only available from [recognisedBy] deities
        // Buildings: gives double smaller success bonus than [favoredBy] bonus
        public List<CosmicEntityDef> recognisedBy = new List<CosmicEntityDef>();    
    }

    public class CompOccultThing : ThingComp
    {
        private CompProperties_OccultThing Props => (CompProperties_OccultThing)this.props;

        public override void PostSpawnSetup(bool raf) // respawn after laod
        {
        }


        public override string CompInspectStringExtra()
        {
            if(Props.favoredBy.NullOrEmpty()) return null;

            string t = "Favored by: " ;
            foreach (CosmicEntityDef e in Props.favoredBy) t += e.label + ", ";
            t = t.Substring(0,t.Length-2) + ".";
            return t;
        }
    }

}