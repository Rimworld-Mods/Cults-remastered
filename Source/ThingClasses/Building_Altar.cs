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

        public Choice congregationChoice; // what ITab option is open
        private Dictionary<Choice, CongregationParms> parms; // default parms come from [exposeData]

        public class CongregationParms : IExposable
        {
            public CosmicEntityDef deity;
            public Pawn preacher;
            public CongregationRecipeDef recipe; // recipe def to find what Things to offer/sacrifice
            public Pawn sacrifice; // with sacrifices extra option to select specific Thing/Pawn
            public SpellDef reward;

            public void ExposeData()
            {
                Scribe_Defs.Look(ref this.deity, "deity");
                Scribe_Defs.Look(ref this.recipe, "recipe");
                Scribe_Defs.Look(ref this.reward, "reward");
                Scribe_References.Look(ref this.preacher, "preacher");
                Scribe_References.Look(ref this.sacrifice, "sacrifice");
            }
        }

        

        public CongregationParms congregationParms 
        {
            get
            {
                return this.parms[this.congregationChoice];
            }
        }

        public void setPreacherForAllParms(Pawn thing)
        {
            foreach(KeyValuePair<Choice, CongregationParms> kvp in this.parms)
            {
                kvp.Value.preacher = thing;
            }
        }

        public void setDeityForAllParms(CosmicEntityDef def)
        {
            foreach(KeyValuePair<Choice, CongregationParms> kvp in this.parms)
            {
                kvp.Value.deity = def;
            }
        }

        public void setRewardForAllParms(SpellDef def)
        {
            foreach(KeyValuePair<Choice, CongregationParms> kvp in this.parms)
            {
                kvp.Value.reward = def;
            }
        }

        //------------------------------------------------------------------------------
        // Other

        private void FoodCongregation(Pawn pawn)
        {
            // get required things based on bill's recipe
            List<ThingCount> chosen_things = new List<ThingCount>();
            Bill_Spell bill = new Bill_Spell(CultsDefOf.Cults_OfferMeatRaw_Worthy);
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

        private void HumanCongregation(Pawn pawn)
        {
            // prepare bill
            Bill_Spell bill = new Bill_Spell(CultsDefOf.Cults_SacrificeHuman);
            this.billStack.Clear();
            this.billStack.AddBill(bill);
            bill.billStack = this.billStack;
            bill.ingredientSearchRadius = 5f;

            // prepare job   
            Job job = JobMaker.MakeJob(CultsDefOf.Cults_DoBill); // JobDefOf.DoBill); //  
            job.targetQueueB = new List<LocalTargetInfo>() { this.congregationParms.sacrifice };
            job.countQueue = new List<int>() { 1 };
            job.playerForced = true;
            job.targetA = this;
            job.targetC = PositionHeld;
            job.haulMode = HaulMode.ToCellNonStorage;
            job.locomotionUrgency = LocomotionUrgency.Sprint;
            job.bill = this.billStack.FirstShouldDoNow;

            // start job
            pawn.jobs.TryTakeOrderedJob(job);

        }

        public override void ExposeData()
        {
            base.ExposeData();
            if(this.parms == null)
            {
                Log.Message("Null parms condition");
                this.parms = new Dictionary<Choice, CongregationParms>() {
                    { Choice.Food, new CongregationParms() },
                    { Choice.Item, new CongregationParms() },
                    { Choice.Animal, new CongregationParms() },
                    { Choice.Human, new CongregationParms() },
                };
            }
            Scribe_Collections.Look<Choice, CongregationParms>(ref this.parms, "congregationParms", LookMode.Value, LookMode.Deep);
            Scribe_Values.Look(ref this.congregationChoice, "congregationChoice", defaultValue: Choice.Food);
        }
        
        // [StartOffering()]
        public override IEnumerable<FloatMenuOption> GetFloatMenuOptions(Pawn pawn)
		{
            yield return new FloatMenuOption(
                "Offer meat", delegate
                {
                    FoodCongregation(pawn); 
                }
            );

            yield return new FloatMenuOption(
                "Offer human", delegate
                {
                    HumanCongregation(pawn); 
                }
            );
		}
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
