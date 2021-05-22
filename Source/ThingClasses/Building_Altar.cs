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

    public class Building_BaseAltar : Building_WorkTable//, IBillGiver
    {
        public Building_BaseAltar() : base() 
        {

        }


        //------------------------------------------------------------------------------
        public enum Level { None = 0, Standard = 1, Sacrificial = 2, Blood = 3, Nightmare = 4 }
        public enum State { None = 0, Sermon = 1, Congregation = 2 }
        public enum Choice { Food = 0, Item = 1, Animal = 2, Human = 3}

        private Level lvl
        {
            get
            {
                if(this.def == CultsDefOf.Cults_Building_StandardAltar) return Level.Standard;
                if(this.def == CultsDefOf.Cults_Building_SacrificialAltar) return Level.Sacrificial;
                if(this.def == CultsDefOf.Cults_Building_BloodAltar) return Level.Blood;
                if(this.def == CultsDefOf.Cults_Building_NightmareAltar) return Level.Nightmare;
                return Level.None;
            }
        }

        //------------------------------------------------------------------------------

        private bool canUpgrade
        {
            get
            {
                if(lvl == Level.Standard && Cults.CultsDefOf.Cults_ForbiddenAltarII.IsFinished) return true;
                if(lvl == Level.Sacrificial && Cults.CultsDefOf.Cults_ForbiddenAltarIII.IsFinished) return true;
                return false;  
            }
        }

        //------------------------------------------------------------------------------
        // Gizmos

        public override IEnumerable<Gizmo> GetGizmos()
        {
            // get default gizmos:
            foreach (Gizmo g in base.GetGizmos()) yield return g;

            if(lvl != Level.Blood)
            {
                // additional gizmos:
                Command_Action gizmo = new Command_Action();

                gizmo.defaultLabel = "Upgrade";
                gizmo.defaultDesc = "This is desc";
                gizmo.disabled = !canUpgrade || lvl == Level.Nightmare;
                gizmo.action = delegate
                {
                    if(lvl == Level.Standard)
                    {
                        this.def = CultsDefOf.Cults_Building_SacrificialAltar;
                        this.Notify_ColorChanged(); // [Notify_ColorChanged] changes texture
                        return;
                    }

                    if(lvl == Level.Sacrificial)
                    {
                        this.def = CultsDefOf.Cults_Building_BloodAltar;
                        this.Notify_ColorChanged();
                        return;
                    }
                };

                if(lvl == Level.Standard)    gizmo.icon = ContentFinder<Texture2D>.Get(canUpgrade?"Commands/Altar/Upgrade2":"Commands/Altar/Upgrade2Disabled");
                if(lvl == Level.Sacrificial) gizmo.icon = ContentFinder<Texture2D>.Get(canUpgrade?"Commands/Altar/Upgrade3":"Commands/Altar/Upgrade3Disabled");
                
                yield return gizmo;
            }

            yield break;
  
        }

        //------------------------------------------------------------------------------
        // Congregation options

        public class CongregationParms // exposable
        {
            public CongregationRecipeDef recipe;
            public Pawn sacrifice;
            public SpellDef reward;
        }

        // Common
        public CosmicEntityDef congregationDeity;
        public Pawn congregationPreacher;
        public Choice congregationChoice;

        // Different
        public CongregationParms congregationParmsFood = new CongregationParms();
        public CongregationParms congregationParmsItem = new CongregationParms();
        public CongregationParms congregationParmsAnimal = new CongregationParms();
        public CongregationParms congregationParmsHuman = new CongregationParms();

        //------------------------------------------------------------------------------
        // Other

        private void giveJob(Pawn pawn)
        {
            // get required things based on bill's recipe
            List<ThingCount> chosen_things = new List<ThingCount>();
            Bill_Production bill = new Bill_Production(CultsDefOf.Cults_OfferMeatRaw_Worthy);
            if(!IngredientFinder.TryFindBestBillIngredients(bill, pawn, this, chosen_things))
            {
                Messages.Message("Ingredients not available", null, MessageTypeDefOf.RejectInput, null);
                return;
            };

            // prepare bill
            this.billStack.Clear();
            this.billStack.AddBill(bill);
            bill.billStack = this.billStack;
            bill.ingredientSearchRadius = 5f;

            // prepare job   
            Job job = JobMaker.MakeJob(CultsDefOf.Cults_DoBill); // JobDefOf.DoBill); //  
            List<LocalTargetInfo> target_list = new List<LocalTargetInfo>();
            List<int> thing_count = new List<int>();
        
            job.targetQueueB = new List<LocalTargetInfo>(chosen_things.Count);
            job.countQueue = new List<int>(chosen_things.Count);

            for(int i = 0; i < chosen_things.Count; i++)
            {
				job.targetQueueB.Add(chosen_things[i].Thing);
				job.countQueue.Add(chosen_things[i].Count);
            }
            
            job.playerForced = true;
            job.targetA = this;
            job.targetC = PositionHeld;
            job.haulMode = HaulMode.ToCellNonStorage;
            job.locomotionUrgency = LocomotionUrgency.Sprint;
            job.bill = this.billStack.FirstShouldDoNow;

            // start job
            pawn.jobs.TryTakeOrderedJob(job);
        }
        
        // [StartOffering()]
        // public override IEnumerable<FloatMenuOption> GetFloatMenuOptions(Pawn pawn)
		// {
        //     yield return new FloatMenuOption(
        //         "Give ability", delegate
        //         {
        //             // giveJob(pawn); 
        //             pawn.abilities.GainAbility(CultsDefOf.Cults_Ability_PsionicBlast);
        //             pawn.abilities.GainAbility(CultsDefOf.Cults_Ability_PsionicBurn);
        //             pawn.abilities.GainAbility(CultsDefOf.Cults_Ability_WrathOfCthulhu);
        //         }
        //     );
		// }

    }





}

/*
		public static void DoExecutionByCut(Pawn executioner, Pawn victim)
		{
			_ = victim.Map;
			_ = victim.Position;
			int num = Mathf.Max(GenMath.RoundRandom(victim.BodySize * 8f), 1);
			for (int i = 0; i < num; i++)
			{
				victim.health.DropBloodFilth();
			}
			BodyPartRecord bodyPartRecord = ExecuteCutPart(victim);
			int num2 = Mathf.Clamp((int)victim.health.hediffSet.GetPartHealth(bodyPartRecord) - 1, 1, 20);
			DamageInfo damageInfo = new DamageInfo(DamageDefOf.ExecutionCut, num2, 999f, -1f, executioner, bodyPartRecord);
			victim.TakeDamage(damageInfo);
			if (!victim.Dead)
			{
				victim.Kill(damageInfo);
			}
		}

*/
