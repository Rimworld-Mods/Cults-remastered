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
    public class SpellDef : IncidentDef
    {
        public int tier = 0;
        public float difficultyFactor = 1.0f;
        public List<string> requiredChoices;

        public bool hasChoice(Building_BaseAltar.Choice choice)
        {
            switch(choice)
            {
                case Building_BaseAltar.Choice.Food:   if(this.requiredChoices.Contains("Food"))   return true; break;
                case Building_BaseAltar.Choice.Item:   if(this.requiredChoices.Contains("Item"))   return true; break;
                case Building_BaseAltar.Choice.Animal: if(this.requiredChoices.Contains("Animal")) return true; break;
                case Building_BaseAltar.Choice.Human:  if(this.requiredChoices.Contains("Human"))  return true; break;
            }
            return false;
        }

        public void CastSpell() // Pawn caster
        {
            IncidentParms parms = new IncidentParms();
            this.Worker.TryExecute(parms);
        }
    }   
    
}


