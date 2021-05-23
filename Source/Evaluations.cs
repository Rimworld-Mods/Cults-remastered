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
    using CongregationParms = Building_BaseAltar.CongregationParms;
    using Choice = Building_BaseAltar.Choice;


    //---------------------------------------------------------------------
    // Def
    
    public class EvaluationDef : Def
    {
        private EvaluationWorker workerInt;

        public Type workerClass;
        public List<Evaluation> stages;
        public EvaluationWorker worker
		{
			get
			{
				if (workerInt == null && workerClass != null)
				{
					workerInt = (EvaluationWorker)Activator.CreateInstance(workerClass);
					workerInt.def = this;
				}
				return workerInt;
			}
		}
    }

    public class Evaluation
    {
        public string label = "null label";
        public string description = "null description";
        public float score = 0;

        private static readonly Color NegativeColor = new Color(0.8f, 0.4f, 0.4f);
		private static readonly Color PositiveColor = new Color(0.1f, 1f, 0.1f); 
		private static readonly Color NeutralColor = new Color(0.5f, 0.5f, 0.5f, 0.75f);
        public Color color => score > 0? PositiveColor : (score < 0? NegativeColor : NeutralColor);
    }

    public static class EvaluationManager
    {
        public static List<EvaluationDef> defs;
        static EvaluationManager() => defs = DefDatabase<EvaluationDef>.AllDefs.Where(e => e.worker != null).ToList();

        public static List<Evaluation> EvaluateAltar(Building_BaseAltar altar)
        {
            List<Evaluation> stages = new List<Evaluation>();
            foreach(EvaluationDef def in defs)
            {
                try
                {   
                    Evaluation stage = def.worker.GetStage(altar);
                    if(stage != null) stages.Add(stage); //  && stage.score != 0
                }
                catch(Exception e)
                {
                    Log.Warning("Evaluation exception: " + e.ToString());
                }
            }

            stages.Sort((e1, e2) => e2.score.CompareTo(e1.score));
            return stages;
        }
    }

    public abstract class EvaluationWorker
    {
        public EvaluationDef def;
        public List<Evaluation> evaluations => this.def.stages;
        public virtual Evaluation GetStage(Building_BaseAltar altar)
        {
            return def.stages.NullOrEmpty()? new Evaluation() : def.stages.First();
        }

        protected Pawn getSacrificePawn(Building_BaseAltar altar)
        {
            CongregationParms parms = new CongregationParms();
            if(altar.congregationChoice == Choice.Animal || altar.congregationChoice == Choice.Human) parms = altar.congregationParms;
            if(parms.sacrifice == null) return null;
            return parms.sacrifice;
        }
    }


    //return new Evaluation(this, CultsDefOf.Cults_Evaluation_Apparel);
    //---------------------------------------------------------------------
    // Workers


    // ---------- Preacher ------------
    public class EvaluationWorker_Apparel : EvaluationWorker
    {
        public override Evaluation GetStage(Building_BaseAltar altar)
        {
            CongregationParms parms = altar.congregationParms;
            if(parms.deity == null || parms.preacher == null) return null;

            int stage = 0;
            foreach(Apparel apparel in parms.preacher.apparel.WornApparel)
            {
                for(int i = 0; i < parms.deity.favoredApparel.Count; i++)
                {
                    if(parms.deity.favoredApparel[i].def == apparel.def)
                    {
                        stage++;
                        break;
                    }
                }
            }

            return evaluations[Mathf.Clamp(stage, 0, evaluations.Count-1)];
        }
    }

    public class EvaluationWorker_Talking : EvaluationWorker
    {
        public override Evaluation GetStage(Building_BaseAltar altar)
        {
            CongregationParms parms = altar.congregationParms;
            
            if(parms.preacher == null) return null;

            float talking = parms.preacher.health.capacities.GetLevel(PawnCapacityDefOf.Talking);
            if(talking > .90f)
            {
                return evaluations[1];
            }
            else if(talking > .70f)
            {
                return evaluations[0];
            }
            else
            {
                return null;
            }
        }
    }

    public class EvaluationWorker_Spirituality : EvaluationWorker
    {
        public override Evaluation GetStage(Building_BaseAltar altar)
        {
            CongregationParms parms = altar.congregationParms;

            if(parms.preacher == null) return null;

            Spirituality need = parms.preacher.needs.TryGetNeed<Spirituality>();

            float skill = 0.0f;
            if(need != null) skill = need.CurLevelPercentage;

            if( skill > .7f)
            {
                return evaluations[3];
            }
            else if(skill > .5f)
            {
                return evaluations[2];
            }
            else if(skill > .3f)
            {
                return evaluations[1];
            }
            else
            {
                return evaluations[0];
            }
        }
    }

    // ---------- Sacrifice ------------
    public class EvaluationWorker_Health : EvaluationWorker
    {
        public override Evaluation GetStage(Building_BaseAltar altar)
        {
            CongregationParms parms = new CongregationParms();
            if(altar.congregationChoice == Choice.Animal || altar.congregationChoice == Choice.Human) parms = altar.congregationParms;
            if(parms.sacrifice == null || !(parms.sacrifice is Pawn)) return null;
            
            float health = (parms.sacrifice as Pawn).health.summaryHealth.SummaryHealthPercent;
            

            if( health < .4f)
            {
                return evaluations[0];
            }
            else if(health < .6f)
            {
                return evaluations[1];
            }
            else if(health < .8f)
            {
                return evaluations[2];
            }
            return null;
        }
    }

    public class EvaluationWorker_Parts : EvaluationWorker
    {     
        public override Evaluation GetStage(Building_BaseAltar altar)
        {
            Pawn pawn = this.getSacrificePawn(altar);
            if(pawn == null) return null;

            List<Hediff_MissingPart> parts = pawn.health.hediffSet.GetMissingPartsCommonAncestors();

            if(parts.Count >= 4) return evaluations[0];
            if(parts.Count >= 2) return evaluations[1];
            if(parts.Count >= 1) return evaluations[2];
            return null;
        }

    }

    

    // ---------- Map conditions ------------

    public class EvaluationWorker_Eclipse : EvaluationWorker
    {
        public override Evaluation GetStage(Building_BaseAltar altar)
        {
            return altar.Map.GameConditionManager.GetActiveCondition(GameConditionDefOf.Eclipse) != null? evaluations.First() : null;
        }
    }
    public class EvaluationWorker_Aurora : EvaluationWorker
    {
        public override Evaluation GetStage(Building_BaseAltar altar)
        {
            return altar.Map.GameConditionManager.GetActiveCondition(GameConditionDefOf.Aurora) != null? evaluations.First() : null;
        }
    }
    public class EvaluationWorker_BloodMoon : EvaluationWorker
    {
        public override Evaluation GetStage(Building_BaseAltar altar)
        {
            return altar.Map.GameConditionManager.GetActiveCondition(CultsDefOf.Cults_GameCondition_BloodMoon) != null? evaluations.First() : null;
        }
    }
    public class EvaluationWorker_StarsAreRight : EvaluationWorker
    {
        public override Evaluation GetStage(Building_BaseAltar altar)
        {
            return altar.Map.GameConditionManager.GetActiveCondition(CultsDefOf.Cults_GameCondition_StarsAreRight) != null? evaluations.First() : null;
        }
    }
    public class EvaluationWorker_StarsAreWrong : EvaluationWorker
    {
        public override Evaluation GetStage(Building_BaseAltar altar)
        {
            return altar.Map.GameConditionManager.GetActiveCondition(CultsDefOf.Cults_GameCondition_StarsAreWrong) != null? evaluations.First() : null;
        }
    }

    // ---------- Temple ------------

    public class EvaluationWorker_Temple : EvaluationWorker
    {

        public override Evaluation GetStage(Building_BaseAltar altar)
        {
            Room temple = altar.GetRoom();
            if(temple == null) return null;
            
            float score_impressivness = temple.GetStat(RoomStatDefOf.Impressiveness);

            //RoomStatScoreStage stage = RoomStatDefOf.Impressiveness.GetScoreStage(score_impressivness);
            int i = RoomStatDefOf.Impressiveness.GetScoreStageIndex(score_impressivness);
            return evaluations[Mathf.Clamp(i, 0, evaluations.Count-1)];
        }
    }
    public class EvaluationWorker_Statues : EvaluationWorker
    {
        public override Evaluation GetStage(Building_BaseAltar altar)
        {
            return null;
            /*
            Room temple = altar.GetRoom();
            if(temple == null) return null;

            List<Thing> sculptures = temple.ContainedAndAdjacentThings.FindAll(x => x is ThingWithComps y && y.TryGetComp<CompFavoredObject>() != null);
            */
        }
    }

}
/*
        private static int SpellCalc_StatuesNearby(Building_SacrificialAltar altar, StringBuilder s,
            StringBuilder reportFavorables)
        {
            int modifier = 0;
            bool statueOfDeityExists = false;
            bool qualityExists = false;
            Room temple = altar.GetRoom();
            CosmicEntity deity = altar.SacrificeData.Entity;

            if (temple != null)
            {
                List<Thing> sculptures = temple.ContainedAndAdjacentThings.FindAll(x => x is ThingWithComps y &&
                    y.TryGetComp<CompFavoredObject>() != null);
                if (sculptures != null && sculptures.Count > 0)
                {
                    foreach (Thing sculpture in sculptures)
                    {
                        CompFavoredObject compFavoredObject = sculpture.TryGetComp<CompFavoredObject>();
                        if (compFavoredObject != null)
                        {
                            if (compFavoredObject.Deities.FirstOrDefault(y => y.deityDef == deity.def.defName) != null)
                            {
                                statueOfDeityExists = true;
                            }
                        }

                        QualityCategory qc;
                        if (sculpture.TryGetQuality(out qc))
                        {
                            if (qc >= QualityCategory.Normal)
                            {
                                qualityExists = true;
                            }
                        }
                    }
                }
            }
            */















        /*
        private List<string> stacks = new List<string>();
        public EvaluationDef def;
        public Evaluation(EvaluationDef d) => def = d;

        public int score => def.isStackable? this.stacks.Count * def.score : def.score;
        public string getDescription()
        {
            if(!def.isStackable) return def.description;

            StringBuilder builder = new StringBuilder(def.description);
            builder.AppendLine();
            foreach(string s in this.stacks)
            {
                builder.AppendInNewLine(" - " + s);
            }
            
            return builder.ToString();
        }

        public void addStack(string s) => this.stacks.Add(s);
        public void clearStacks() => this.stacks.Clear();
        */



