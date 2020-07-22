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
				// -- new code --
				if(Find.ResearchManager.currentProj != null && Find.ResearchManager.currentProj.HasModExtension<ForbiddenResearchExtension>())
				{
					Spirituality need = pawn.needs.TryGetNeed<Spirituality>();
					if(need != null) need.Gain();
				}
				// --- ------ ---
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
	
	/*
	public class JobDriver_BeatFire : RimWorld.JobDriver_BeatFire
	{
		//protected Fire TargetFire => (Fire)job.targetA.Thing;
		public override bool TryMakePreToilReservations(bool errorOnFailed)
		{
			return true;
		}

		protected override IEnumerable<Toil> MakeNewToils()
		{
			JobDriver_BeatFire jobDriver_BeatFire = this;
			this.FailOnDespawnedOrNull(TargetIndex.A);
			Toil beat = new Toil();
			Toil approach = new Toil();
			approach.initAction = delegate
			{
				if (jobDriver_BeatFire.Map.reservationManager.CanReserve(jobDriver_BeatFire.pawn, jobDriver_BeatFire.TargetFire))
				{
					jobDriver_BeatFire.pawn.Reserve(jobDriver_BeatFire.TargetFire, jobDriver_BeatFire.job);
				}
				jobDriver_BeatFire.pawn.pather.StartPath(jobDriver_BeatFire.TargetFire, PathEndMode.Touch);
			};
			approach.tickAction = delegate
			{
				if (jobDriver_BeatFire.pawn.pather.Moving && jobDriver_BeatFire.pawn.pather.nextCell != jobDriver_BeatFire.TargetFire.Position)
				{
					jobDriver_BeatFire.StartBeatingFireIfAnyAt(jobDriver_BeatFire.pawn.pather.nextCell, beat);
				}
				if (jobDriver_BeatFire.pawn.Position != jobDriver_BeatFire.TargetFire.Position)
				{
					jobDriver_BeatFire.StartBeatingFireIfAnyAt(jobDriver_BeatFire.pawn.Position, beat);
				}
			};
			approach.FailOnDespawnedOrNull(TargetIndex.A);
			approach.defaultCompleteMode = ToilCompleteMode.PatherArrival;
			approach.atomicWithPrevious = true;
			yield return approach;
			beat.tickAction = delegate
			{
				if (!jobDriver_BeatFire.pawn.CanReachImmediate(jobDriver_BeatFire.TargetFire, PathEndMode.Touch))
				{
					jobDriver_BeatFire.JumpToToil(approach);
				}
				else if (!(jobDriver_BeatFire.pawn.Position != jobDriver_BeatFire.TargetFire.Position) || !jobDriver_BeatFire.StartBeatingFireIfAnyAt(jobDriver_BeatFire.pawn.Position, beat))
				{
					jobDriver_BeatFire.pawn.natives.TryBeatFire(jobDriver_BeatFire.TargetFire as OccultFire);
					

					if (jobDriver_BeatFire.TargetFire.Destroyed)
					{
						jobDriver_BeatFire.pawn.records.Increment(RecordDefOf.FiresExtinguished);
						jobDriver_BeatFire.pawn.jobs.EndCurrentJob(JobCondition.Succeeded);
					}
				}
			};
			beat.FailOnDespawnedOrNull(TargetIndex.A);
			beat.defaultCompleteMode = ToilCompleteMode.Never;
			yield return beat;
		}

		private bool StartBeatingFireIfAnyAt(IntVec3 cell, Toil nextToil)
		{
			List<Thing> thingList = cell.GetThingList(base.Map);
			for (int i = 0; i < thingList.Count; i++)
			{
				Fire fire = thingList[i] as Fire;

				if (fire != null && fire.parent == null)
				{
					job.targetA = fire;
					pawn.pather.StopDead();
					JumpToToil(nextToil);
					return true;
				}
			}
			return false;
		}

	}
	*/
	
	
}