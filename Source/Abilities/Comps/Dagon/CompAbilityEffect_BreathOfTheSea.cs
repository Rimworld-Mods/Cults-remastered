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

	public class CompAbilityEffect_BreathOfTheSea : CompAbilityEffect
	{
		public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
		{
			base.Apply(target, dest);
			List<BodyPartDef> breathing_sources = target.Pawn.RaceProps.body.GetPartsWithTag(BodyPartTagDefOf.BreathingSource).Select(part => part.def).ToList();
			Hediff hediff = target.Pawn.health.hediffSet.GetFirstHediffOfDef(CultsDefOf.Cults_Hediff_WateryLungs);
			if(hediff != null)
			{
				hediff.Severity += hediff.def.initialSeverity;
			}
			else
			{
				HediffGiverUtility.TryApply(target.Pawn, CultsDefOf.Cults_Hediff_WateryLungs, breathing_sources, true, 2, null);
				Job job = JobMaker.MakeJob(CultsDefOf.Cults_Job_VomitWater);
				target.Pawn.jobs.StartJob(job, JobCondition.InterruptForced, null, resumeCurJobAfterwards: true);
			}

			// target.Pawn.jobs.TryTakeOrderedJob(job);
		}

	}


}