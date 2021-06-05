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
        public static DamageDef Cults_Damage_Miasma;

        //public static ThingDef Cults_OccultFire;
        public static ThingDef Cults_OccultSpark;
        public static ThingDef Cults_AbnormalShift;
        public static ThingDef Cults_BlastThing;

        //-------------------------------------------------------
        // AI, think, work
        public static JobDef Cults_Job_Worship;
        public static JobDef Cults_DoBill;
        public static JobDef Cults_WaitDemise;

        public static JobDef Cults_Job_VomitWater;
        public static JobDef Cults_Job_VomitBile;

        //-------------------------------------------------------
        // Recipes
        public static RecipeDef Cults_OfferMeatRaw;
        public static RecipeDef Cults_OfferMeatRaw_Worthy;
        public static RecipeDef Cults_SacrificeHuman;

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
        public static CosmicEntityDef Cults_CosmicEntity_Yog;

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
        // Filth
        public static ThingDef Cults_Filth_WaterVomit;
        public static ThingDef Cults_Filth_BileVomit;
        public static ThingDef Cults_Gas_Miasma;

        //-------------------------------------------------------
        // Conditions
        public static GameConditionDef Cults_GameCondition_StarsAreWrong;
        public static GameConditionDef Cults_GameCondition_StarsAreRight;
        public static GameConditionDef Cults_GameCondition_BloodMoon;
        public static GameConditionDef Cults_GameCondition_ResidualEnergy;
        
        //-------------------------------------------------------
        // Thoughts
        public static ThoughtDef Cults_Thought_SawBloodMoonSad;
        public static ThoughtDef Cults_Thought_SawBloodMoonHappy;

        //-------------------------------------------------------
        // Abilities
        public static AbilityDef Cults_Ability_PsionicBlast; 
        public static AbilityDef Cults_Ability_EerieScorch; 
        public static AbilityDef Cults_Ability_Castigation; 

        public static AbilityDef Cults_Ability_BreathOfTheSea;
        public static AbilityDef Cults_Ability_GraspFromBellow; 
        public static AbilityDef Cults_Ability_SwellingTorment; 

        public static AbilityDef Cults_Ability_UtteranceOfBile;
        public static AbilityDef Cults_Ability_MiasmaField; 
        public static AbilityDef Cults_Ability_Zymoticism; 

        public static AbilityDef Cults_Ability_ObscureVision;
        public static AbilityDef Cults_Ability_Madness; 
        public static AbilityDef Cults_Ability_MindDance; 

        public static AbilityDef Cults_Ability_AbnormalShift;
        public static AbilityDef Cults_Ability_Shriveling; 
        public static AbilityDef Cults_Ability_SpectralShield; 

        //-------------------------------------------------------
        // Heddifs
        public static HediffDef Cults_SanityLoss;
        public static HediffDef Cults_Hediff_WateryLungs;
        public static HediffDef Cults_Hediff_FoulBile;
        public static HediffDef Cults_Hediff_MiasmaSickness;
        public static HediffDef Cults_Hediff_FungalInfection;
        public static HediffDef Cults_Hediff_SwellingBrain;
        public static HediffDef Cults_Hediff_UnspeakableOath;
        public static HediffDef Cults_Hediff_SpectralShield;
        
        //-------------------------------------------------------
        // Effecters
        public static EffecterDef Cults_Effecter_VomitWater;
        public static EffecterDef Cults_Effecter_VomitBile;

        //-------------------------------------------------------
        // Motes
        public static ThingDef Cults_Mote_Miasma;
        public static ThingDef Cults_Mote_Slash;

    }

}


