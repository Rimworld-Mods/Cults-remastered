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
	// Override research job WorkGiver and WorkDriver
	// Adds new condition that checks if project is occult and correct workbench is selected

    public class WorkGiver_CultResearcher : WorkGiver_Researcher
	{
		public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
		{
			ResearchProjectDef currentProj = Find.ResearchManager.currentProj;
			if (currentProj == null)
			{
				return false;
			}

			// --- new code ---
			if(!Find.ResearchManager.currentProj.HasModExtension<ForbiddenResearchExtension>() && t.def.defName == "Cults_OccultResearchBench")
			{
				return false;
			}
			// --- -------- ---

			Building_ResearchBench building_ResearchBench = t as Building_ResearchBench;
			if (building_ResearchBench == null)
			{
				return false;
			}
			if (!currentProj.CanBeResearchedAt(building_ResearchBench, ignoreResearchBenchPowerStatus: false))
			{
				return false;
			}
			if (!pawn.CanReserve(t, 1, -1, null, forced))
			{
				return false;
			}
			return true;
		}
	}



  	public class JobDriver_CultResearch : JobDriver // JobDriver_Research 
	{
		private const int JobEndInterval = 4000;

		private ResearchProjectDef Project => Find.ResearchManager.currentProj;

		private Building_ResearchBench ResearchBench => (Building_ResearchBench)base.TargetThingA;

		public override bool TryMakePreToilReservations(bool errorOnFailed)
		{
			return pawn.Reserve(ResearchBench, job, 1, -1, null, errorOnFailed);
		}

		protected override IEnumerable<Toil> MakeNewToils()
		{
			JobDriver_CultResearch jobDriver_Research = this;
			this.FailOnDespawnedNullOrForbidden(TargetIndex.A);
			yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.InteractionCell);
			Toil research = new Toil();
			research.tickAction = delegate
			{
				Pawn actor = research.actor;
				float statValue = actor.GetStatValue(StatDefOf.ResearchSpeed);
				statValue *= jobDriver_Research.TargetThingA.GetStatValue(StatDefOf.ResearchSpeedFactor);
				Find.ResearchManager.ResearchPerformed(statValue, actor);
				actor.skills.Learn(SkillDefOf.Intellectual, 0.1f);
				actor.GainComfortFromCellIfPossible(chairsOnly: true);
			};
			research.FailOn(() => jobDriver_Research.Project == null);
			research.FailOn(() => !jobDriver_Research.Project.CanBeResearchedAt(jobDriver_Research.ResearchBench, ignoreResearchBenchPowerStatus: false));

            // -- new code --
            research.FailOn(() => 
				!Find.ResearchManager.currentProj.HasModExtension<ForbiddenResearchExtension>() && 
				jobDriver_Research.ResearchBench.def.defName == "Cults_OccultResearchBench"
			);
            // --- ------ ---

			research.FailOnCannotTouch(TargetIndex.A, PathEndMode.InteractionCell);
			research.WithEffect(EffecterDefOf.Research, TargetIndex.A);
			research.WithProgressBar(TargetIndex.A, () => jobDriver_Research.Project?.ProgressPercent ?? 0f);
			research.defaultCompleteMode = ToilCompleteMode.Delay;
			research.defaultDuration = 4000;
			research.activeSkill = (() => SkillDefOf.Intellectual);
			yield return research;
			yield return Toils_General.Wait(2);
		}
	}
}