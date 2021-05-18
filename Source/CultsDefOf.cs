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
    [DefOf]
    public static class CultsDefOf
    {
        //-------------------------------------------------------
        // General
        public static LetterDef Cults_Letter_Success;
        public static TimeAssignmentDef Cults_TimeAssignment_Worship;
        public static NeedDef Cults_Need_Spirituality;
        public static SkillDef Cults_Skill_Occultism;

        
        //public static ThingDef Cults_OccultFire;
        public static ThingDef Cults_OccultSpark;
        

        //-------------------------------------------------------
        // Buildings
        //public static ThingDef Cults_Building_StandardAltar;

        //-------------------------------------------------------
        // AI, think, work
        public static JobDef Cults_Job_Worship;
        public static JobDef Cults_DoBill;

        public static RecipeDef Cults_OfferMeatRaw;
        public static RecipeDef Cults_OfferMeatRaw_Worthy;

        //-------------------------------------------------------
        // Items
        
        //-------------------------------------------------------
        // Cosmic entities
        public static CosmicEntityDef Cults_CosmicEntity_Cthulhu;
        public static CosmicEntityDef Cults_CosmicEntity_Nyarlathotep;
        public static CosmicEntityDef Cults_CosmicEntity_Dagon;
        public static CosmicEntityDef Cults_CosmicEntity_Hastur;
        public static CosmicEntityDef Cults_CosmicEntity_Shub;
        public static CosmicEntityDef Cults_CosmicEntity_Tsathoggua;
        public static CosmicEntityDef Cults_CosmicEntity_Bast;

        //-------------------------------------------------------
        // Research
        public static ResearchProjectDef Cults_ForbiddenStudies;
        public static ResearchProjectDef Cults_ForbiddenDeities;
        public static ResearchProjectDef Cults_ForbiddenAttire;
        public static ResearchProjectDef Cults_ForbiddenSculptures;
        public static ResearchProjectDef Cults_ForbiddenObelisk;
        public static ResearchProjectDef Cults_ForbiddenAltarI;
        public static ResearchProjectDef Cults_ForbiddenAltarII;
        public static ResearchProjectDef Cults_ForbiddenAltarIII;

        //-------------------------------------------------------
        // Buildings
        public static ThingDef Cults_Building_StandardAltar;
        public static ThingDef Cults_Building_SacrificialAltar;
        public static ThingDef Cults_Building_BloodAltar;
        public static ThingDef Cults_Building_NightmareAltar;

        //-------------------------------------------------------
        // Conditions
        public static GameConditionDef Cults_GameCondition_StarsAreWrong;
        public static GameConditionDef Cults_GameCondition_StarsAreRight;
        public static GameConditionDef Cults_GameCondition_BloodMoon;
        
        //-------------------------------------------------------
        // Thoughts
        public static ThoughtDef Cults_Thought_SawBloodMoonSad;
        public static ThoughtDef Cults_Thought_SawBloodMoonHappy;

        //-------------------------------------------------------
        // Abilities
        public static AbilityDef Cults_Ability_PsionicBlast; 
        public static AbilityDef Cults_Ability_PsionicBurn; 
        public static AbilityDef Cults_Ability_WrathOfCthulhu; 

    }

}


