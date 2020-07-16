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
    public class IncidentWorker_StarsAlignment : IncidentWorker_MakeGameCondition
    {
        protected override bool CanFireNowSub(IncidentParms parms)
        {   
            bool isActive = 
                Find.World.GameConditionManager.ConditionIsActive(CultsDefOf.Cults_GameCondition_StarsAreWrong) ||
                Find.World.GameConditionManager.ConditionIsActive(CultsDefOf.Cults_GameCondition_StarsAreRight);

            return CultKnowledge.isExposed && !isActive && base.CanFireNowSub(parms);
        }
        
    }

    //---------------------------------------------------------------

    public class GameCondition_StarsAreWrong : GameCondition
    {
    }

    public class GameCondition_StarsAreRight : GameCondition
    {
    }

    public class GameCondition_BloodMoon : GameCondition
    {
        private const int lerpTicks = 200;
        private readonly SkyColorSet bloodSkyColors = new SkyColorSet(new Color(0.9f, 0.103f, 0.182f), Color.white, new Color(0.7f, 0.1f, 0.1f), 1f);
        public override float SkyTargetLerpFactor(Map map)
        {
            return GameConditionUtility.LerpInOutValue((float)base.TicksPassed, (float)base.TicksLeft, (float)lerpTicks, 1f);
        }

        public override void Init()
        {
            base.Init();
            foreach (Map map in this.AffectedMaps)
            {
                affectColonists(map);
                spawnWolves(map);
            }
        }

        private void spawnWolves(Map map)
        {
            PawnKindDef wolfType = (map.mapTemperature.OutdoorTemp > 0f) ? PawnKindDef.Named("Wolf_Timber") : PawnKindDef.Named("Wolf_Arctic");

            RCellFinder.TryFindRandomPawnEntryCell(out IntVec3 loc, map, CellFinder.EdgeRoadChance_Animal, false, null);

            int numberOfWolves = Rand.Range(3, 6);
            List<Thing> wolves = new List<Thing>();
            
            for (int i = 0; i < numberOfWolves; i++)
            {
                Pawn newWolf = PawnGenerator.GeneratePawn(wolfType, null);
                wolves.Add(GenSpawn.Spawn(newWolf, loc, map));
            }
        }

        private void affectColonists(Map map)
        {
            foreach (Pawn pawn in map.mapPawns.FreeColonistsAndPrisoners.ToList())
            {
                pawn.needs.mood.thoughts.memories.TryGainMemory(CultsDefOf.Cults_Thought_SawBloodMoonSad);
                pawn.needs.mood.thoughts.memories.TryGainMemory(CultsDefOf.Cults_Thought_SawBloodMoonHappy);
            }  
        }

        public override SkyTarget? SkyTarget(Map map)
        {
            return new SkyTarget?(new SkyTarget(0f, this.bloodSkyColors, 1f, 0f));
        }

    }
}