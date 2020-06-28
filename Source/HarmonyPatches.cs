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

using HarmonyLib;

namespace Cults
{
    public class ForbiddenResearchExtension : DefModExtension
    {
    }

    // Overrides ResearchProjectDef.CanBeResearchedAt;
    
    [HarmonyPatch(typeof(ResearchProjectDef), "CanBeResearchedAt")]
    public class ExtraResearchCheck
    {
        private static void Prefix(ref bool __state, ResearchProjectDef __instance, ref Building_ResearchBench bench)
        {   
            //if(!Find.ResearchManager.currentProj.HasModExtension<ForbiddenResearchExtension>())
                /*
                if(bench.def.defName == "Cults_OccultResearchBench"){
                    return false;
                    //__instance.requiredResearchFacilities = new List<ThingDef>();
                    //__instance.requiredResearchFacilities.Add(CultsDefOf.Cults_Building_StandardAltar);
                }else{
                    return true;
                }
                */
                // if(__instance.requiredResearchFacilities == null) __instance.requiredResearchFacilities = new List<ThingDef>();
                // __instance.requiredResearchFacilities.Add(CultsDefOf.Cults_Building_StandardAltar);
            
            
            /*
            if (__instance.requiredResearchBuilding == null){

                __instance.requiredResearchFacilities
                __state = bench.def.defName;
                // GenDefDatabase.GetDef(typeof(ThingDef), "SimpleResearchBench", true);
                // public static ThingDef SimpleResearchBench;
                // public static HiTechResearchBench;
                if(bench.def.defName !=  "Cults_OccultResearchBench") __instance.requiredResearchBuilding = bench.def;
                // if(bench.def.defName ==  "HiTechResearchBench") __instance.requiredResearchBuilding = bench.def;
                //if(bench.def.defName ==  "SimpleResearchBench") __instance.requiredResearchBuilding = bench.def;

            }
            */
        }
        
        private static void Postfix(bool __result, ref bool __state)
        {   
            /*
            if(!__state){
                return __state;
            }
            return __result;
            */
        }
        
        
    }
    
    /*
    [HarmonyPatch(typeof(ResearchManager), "FinishProject")]
    public class ExtraFinishProjectFunction
    {
        private static void Prefix()
        {
            // Discover deity here
            Log.Message("Finished research");
        }
    }
    */
}