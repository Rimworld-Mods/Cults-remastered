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
	public class CompProperties_AbilityGiveHediff : CompProperties_AbilityEffect
	{
        public List<BodyPartDef> partsToAffect;
        public int partsCount = 1;
		public JobDef jobToStart;
        public HediffDef hediffDef; 
        public bool canAffectAnyLivePart = false;
		public bool affectRandomPart = false;

        public CompProperties_AbilityGiveHediff()
        {
            compClass = typeof(CompAbilityEffect_GiveHediff);
        }
	}


	public class CompAbilityEffect_GiveHediff : CompAbilityEffect
	{
        public new CompProperties_AbilityGiveHediff Props => (CompProperties_AbilityGiveHediff)props;
		public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
		{
			base.Apply(target, dest);
            if(Props.hediffDef == null) return;

			Hediff hediff = target.Pawn.health.hediffSet.GetFirstHediffOfDef(Props.hediffDef);
			if(hediff != null)
			{
				hediff.Severity += hediff.def.initialSeverity;
			}
			else
			{
				HediffGiverUtility.TryApply(target.Pawn, Props.hediffDef, Props.partsToAffect, Props.affectRandomPart, Props.partsCount, null);
                if(Props.jobToStart != null)
                {
                    Job job = JobMaker.MakeJob(Props.jobToStart);
                    target.Pawn.jobs.StartJob(job, JobCondition.InterruptForced, null, resumeCurJobAfterwards: true);
                }
			}
		}

	}


}