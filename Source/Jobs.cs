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


/*
ThinkTrees - hierarchy of nodes that calculate priorities based upon game conditions, to select the best current job.
JobDriver - Identifying job, specify every detail of that job in a list of "Toils".

ThinkTree (xml) -> JobGiver (cs) -> JobDriver -> Toil

	private void DoTimeAssignment(Rect rect, Pawn p, int hour)
		{
			rect = rect.ContractedBy(1f);
			bool mouseButton = Input.GetMouseButton(0);
			TimeAssignmentDef assignment = p.timetable.GetAssignment(hour);
			GUI.DrawTexture(rect, assignment.ColorTexture);
			if (!mouseButton)
			{
				MouseoverSounds.DoRegion(rect);
			}
			if (!Mouse.IsOver(rect))
			{
				return;
			}
			Widgets.DrawBox(rect, 2);
			if (mouseButton && assignment != TimeAssignmentSelector.selectedAssignment && TimeAssignmentSelector.selectedAssignment != null)
			{
				SoundDefOf.Designate_DragStandard_Changed.PlayOneShotOnCamera();
				p.timetable.SetAssignment(hour, TimeAssignmentSelector.selectedAssignment);
				PlayerKnowledgeDatabase.KnowledgeDemonstrated(ConceptDefOf.TimeAssignments, KnowledgeAmount.SmallInteraction);
				if (TimeAssignmentSelector.selectedAssignment == TimeAssignmentDefOf.Meditate)
				{
					PlayerKnowledgeDatabase.KnowledgeDemonstrated(ConceptDefOf.MeditationSchedule, KnowledgeAmount.Total);
				}
			}
		}

*/
namespace Cults
{
    class JobGiver_Worship : ThinkNode_JobGiver
    {
        protected override Job TryGiveJob(Pawn pawn)
        {
            TimeAssignmentDef assignment = pawn.timetable.CurrentAssignment;


            IntVec3 spot = new IntVec3(103,0,103);
            Job job = JobMaker.MakeJob(CultsDefOf.Cults_Job_Worship, spot);

            //  && pawn.needs.TryGetNeed<Need>() == null
            if(assignment == Cults.CultsDefOf.Cults_TimeAssignment_Worship) return job;
            return null;
        }

        public override float GetPriority(Pawn pawn)
        {
            return 9.0f;
        }
    }


    class JobDriver_Worship : JobDriver
    {
        private float worship_done;
        private const TargetIndex target_cell = TargetIndex.A;
        protected IntVec3 cell => job.GetTarget(TargetIndex.A).Cell;

        [DebuggerHidden]
        protected override IEnumerable<Toil> MakeNewToils()
        {
            Toil worship = new Toil();
            worship.defaultCompleteMode = ToilCompleteMode.Delay;
            worship.WithProgressBarToilDelay(TargetIndex.A, false, -0.5f);
            worship.defaultDuration = 600;
            worship.initAction = delegate
            {
                job.targetA = new IntVec3(110,0,103);
                this.worship_done = 0;
            };
            worship.tickAction = delegate
            {
                //Log.Message("Worship job tick action");
                this.worship_done += 1;
                /*
                if(worship_done == 300){
                    Log.Message("Worship job success");
                    this.EndJobWith(JobCondition.Succeeded);
                }
                */
            };

            worship.AddFinishAction(delegate{
                Log.Message("Worship job success");
            });

            

            //yield return Toils_Reserve.Reserve(target_cell, 1, -1);
            yield return Toils_Goto.GotoCell(TargetIndex.A, PathEndMode.OnCell);
            yield return worship;
            //yield return Toils_Goto.GotoCell(TargetIndex.A, PathEndMode.InteractionCell);
        }
        
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            //pawn.Map.pawnDestinationReservationManager.Reserve(pawn, job, job.targetA.Cell);
            return true;
        }
        /*
        public override void ExposeData()
        {
            base.ExposeData();
            //Scribe_Values.Look<Rot4>(ref this.faceDir, "faceDir", default(Rot4), false);
        }
        */
    }
}