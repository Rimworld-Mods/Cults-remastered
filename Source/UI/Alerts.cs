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
    public class Alert_NoAltar : Alert
	{            
        private bool isAltarAvailable = false; // if altar available alert only shown when deity is not selected
        private bool isPawnWorship = false; // if any pawn has Worship in their schedule
		public override string GetLabel()
		{
            return isAltarAvailable? "Select Deity" : "Need altar";
        }

		public override TaggedString GetExplanation()
		{
            // string builder
            /*
			StringBuilder stringBuilder = new StringBuilder();
			foreach (Caravan idleCaravan in IdleCaravans)
			{
				stringBuilder.AppendLine("  - " + idleCaravan.Label);
			}
			return "CaravanIdleDesc".Translate(stringBuilder.ToString());
            */

            return isAltarAvailable? "Worshipers doesn't have the Diety to worship" : "Colonists have Worship job in their schedule but no built altar";
		}

		public override AlertReport GetReport()
		{        
            isAltarAvailable = false;
            isPawnWorship = false;

            foreach (Pawn item in PawnsFinder.HomeMaps_FreeColonistsSpawned)
            {
                for (int i = 0; i < item.timetable.times.Count; i++)
                {
                    if (item.timetable.times[i] == CultsDefOf.Cults_TimeAssignment_Worship)
                    {
                        isPawnWorship = true;
                        break;
                    }
                }
            }

            if(!isPawnWorship) return false;

            // check if there's an altar on a map 
            List<Map> maps = Find.Maps;
			for (int i = 0; i < maps.Count; i++)
			{   
                IEnumerable<Thing> altars = maps[i].listerBuildings.AllBuildingsColonistOfClass<Building_BaseAltar>();
                if(!altars.EnumerableNullOrEmpty())
                {
                    // altar exists, check if deity is selected
                    isAltarAvailable = true;
                    if(CultKnowledge.selectedDeity == null) // if null show alert
                    { 
                        return AlertReport.CulpritsAre(altars.ToList());
                    }
                    else
                    {
                        return false;
                    }; 
                };
			}

            // no altar, but some pawns have worship in their schedule
			return true;
		}
	}
}