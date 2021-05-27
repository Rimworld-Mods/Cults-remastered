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
		public new CompProperties_AbilityGiveHediff Props => (CompProperties_AbilityGiveHediff)props;
		public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
		{
			base.Apply(target, dest);
			Log.Message("Casted spell: " + parent.def.label);  

			HediffGiverUtility.TryApply(target.Pawn, CultsDefOf.Cults_Hediff_WateryLungs, Props.partsToAffect, true, Props.bodyPartCount, null);
			Job job = JobMaker.MakeJob(CultsDefOf.Cults_Job_VomitWater);
			target.Pawn.jobs.StartJob(job, JobCondition.InterruptForced, null, resumeCurJobAfterwards: true);
			// target.Pawn.jobs.TryTakeOrderedJob(job);
		}

		// protected void ApplyInner(Pawn target, Pawn other)
		// {
		// 	if (target == null)
		// 	{
		// 		return;
		// 	}
		// 	if (Props.replaceExisting)
		// 	{
		// 		Hediff firstHediffOfDef = target.health.hediffSet.GetFirstHediffOfDef(Props.hediffDef);
		// 		if (firstHediffOfDef != null)
		// 		{
		// 			target.health.RemoveHediff(firstHediffOfDef);
		// 		}
		// 	}
		// 	Hediff hediff = HediffMaker.MakeHediff(Props.hediffDef, target, Props.onlyBrain ? target.health.hediffSet.GetBrain() : null);
		// 	HediffComp_Disappears hediffComp_Disappears = hediff.TryGetComp<HediffComp_Disappears>();
		// 	if (hediffComp_Disappears != null)
		// 	{
		// 		hediffComp_Disappears.ticksToDisappear = GetDurationSeconds(target).SecondsToTicks();
		// 	}
		// 	HediffComp_Link hediffComp_Link = hediff.TryGetComp<HediffComp_Link>();
		// 	if (hediffComp_Link != null)
		// 	{
		// 		hediffComp_Link.other = other;
		// 		hediffComp_Link.drawConnection = (target == parent.pawn);
		// 	}
		// 	target.health.AddHediff(hediff);
		// }

	}


}