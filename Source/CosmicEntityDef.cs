
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
    public class CosmicEntityDef : Def
    {
        public string domain;
        public float max_favor;
        public List<string> tiers; 

    }



    public class CosmicEntity : IExposable
    {
        public CosmicEntityDef def;
        public bool is_discovered; // save
        public float current_favor; // save
        public float max_favor => this.def.max_favor;

        public CosmicEntity(CosmicEntityDef def)
        {
            this.def = def;
        }

        public void ExposeData()
        {
            Scribe_Defs.Look(ref def, "def");
            Scribe_Values.Look(ref this.current_favor, "currentFavor", 0.0f, true);
            Scribe_Values.Look(ref this.is_discovered, "isDiscovered", false, true);
        }
    }

    /*
    <cultsKnowledge>
        <isExposed>True</isExposed>
        ...
        <deities>
            <li>
                <label>Cthulhu</label>
                <isDiscovered>True</isDiscovered>
                <favor>65.324</favor>
            </li>
            <li>
                <label>Bast</label>
                <isDiscovered>False</isDiscovered>
                <favor>65.324</favor>
            </li>
            ...
        </deities>
    </cultsKnowledge>

    */

    /*
            public void ExposeData()
        {
            Scribe_Values.Look(ref name, "name", null, true);
            Scribe_Values.Look(ref isDiscovered, "isDiscovered", false, true);
            Scribe_Values.Look(ref level, "level", 0, true);
        }
        */


}