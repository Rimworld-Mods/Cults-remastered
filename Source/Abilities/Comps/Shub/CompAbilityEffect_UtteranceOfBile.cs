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

    public class CompProperties_AbilityUtteranceOfBile : CompProperties_AbilityEffect
	{
		public CompProperties_AbilityUtteranceOfBile()
		{
			compClass = typeof(CompAbilityEffect_UtteranceOfBile);
		}
	}


	public class CompAbilityEffect_UtteranceOfBile : CompAbilityEffect{
		public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
		{
			base.Apply(target, dest);
			Pawn pawn = target.Pawn;
			Hediff hediff = pawn.health.hediffSet.GetFirstHediffOfDef(CultsDefOf.Cults_Hediff_FoulBile);
			IEnumerable<BodyPartRecord> liver = pawn.RaceProps.body.GetPartsWithDef(BodyPartDefOf.Liver);
			if(!liver.Any())
			{
				Messages.Message(parent.def.LabelCap + ": failed. Target does not have a liver", new LookTargets(pawn), MessageTypeDefOf.RejectInput, historical: false);
				return;
			}
			if(hediff != null)
			{
				hediff.Severity += hediff.def.initialSeverity;
				Job job = JobMaker.MakeJob(CultsDefOf.Cults_Job_VomitBile);
				pawn.jobs.StartJob(job, JobCondition.InterruptForced, null, resumeCurJobAfterwards: true);
			}
			else
			{
				HediffGiverUtility.TryApply(pawn, CultsDefOf.Cults_Hediff_FoulBile, new List<BodyPartDef>() { BodyPartDefOf.Liver }, true, 1, null);
				Job job = JobMaker.MakeJob(CultsDefOf.Cults_Job_VomitBile);
				pawn.jobs.StartJob(job, JobCondition.InterruptForced, null, resumeCurJobAfterwards: true);
			}
          
		}
	}


}