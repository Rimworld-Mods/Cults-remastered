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
    public class SpellDef : IncidentDef
    {
        public int tier = 0;
        public float difficultyFactor = 1.0f;
        public List<string> requirements;

        public void CastSpell() // Pawn caster
        {
            IncidentParms parms = new IncidentParms();
            this.Worker.TryExecute(parms);
        }
    }   

    class SpellWorker_Favor : IncidentWorker
    {
        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            return true;
        }
    }
}