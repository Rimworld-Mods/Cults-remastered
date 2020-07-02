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

public override bool HasJobOnCell(Pawn pawn, IntVec3 c, bool forced = false)
{
    if (c.IsForbidden(pawn) || pawn.Map.designationManager.DesignationAt(c, DesDef) == null || !pawn.CanReserve(c, 1, -1, ReservationLayerDefOf.Floor, forced))
    {
        return false;
    }
    return true;
}

*/
namespace Cults
{
    class JobGiver_Worship : ThinkNode_JobGiver
    {
        protected override Job TryGiveJob(Pawn pawn)
        {
            
            if(!CultKnowledge.IsExposed()) return null;
            if(CultKnowledge.selectedDeity == null) return null;

            /*
                Get [CurrentAssignment] indirectly, because [pawn.timetable.CurrentAssignment] getter has harmony patch.
                Patch overrides result from [Cults_TimeAssignment_Worship] to [TimeAssignmentDefOf.Anything]
                Vanilla functions can't check custom timetable defs otherwise they throw [NotImplementedException]
                [JobGiver_Worship] happens before core colonist behavior

                Classes that check a schedule:
                    ThinkNode_Priority_GetJoy
                    JobGiver_GetRest
                    JobGiver_Work
            */
            // TimeAssignmentDef assignment = pawn.timetable.CurrentAssignment;
            TimeAssignmentDef assignment = pawn.timetable.times[GenLocalDate.HourOfDay(pawn)]; 

            // TODO: implement reservations

            IEnumerable<Building_BaseAltar> altars = pawn.Map.listerBuildings.AllBuildingsColonistOfClass<Building_BaseAltar>();
            TraverseParms parms = TraverseParms.For(pawn, Danger.Deadly, TraverseMode.ByPawn, false);
            Thing altar = GenClosest.ClosestThing_Global_Reachable(pawn.Position, pawn.Map, altars, PathEndMode.OnCell, parms);
            
            if(altar != null)
            {
                if(!WatchBuildingUtility.TryFindBestWatchCell(altar, pawn, true, out IntVec3 spot, out Building chair))
                {
                    if(!WatchBuildingUtility.TryFindBestWatchCell(altar, pawn, false, out spot, out chair)) return null;
                };
                
                Job job = JobMaker.MakeJob(CultsDefOf.Cults_Job_Worship, spot, altar);
                //if(pawn.CurJob != null)Log.Message("info " + pawn.HasReserved(pawn.CurJob.targetA).ToString() + " " + pawn.CurJobDef.defName.ToString());
                //if(pawn.CanReserve(spot, 1, -1)) pawn.Reserve(spot, job, 1, -1);
                if(assignment == Cults.CultsDefOf.Cults_TimeAssignment_Worship) return job;
            };
            return null;
        }

        public override float GetPriority(Pawn pawn)
        {
            return 9.0f;
        }
    }


    class JobDriver_Worship : JobDriver
    {
        private TargetIndex spotIndex = TargetIndex.A;
        private TargetIndex altarIndex = TargetIndex.B;

        protected IntVec3 spotPos => job.GetTarget(TargetIndex.A).Cell;
        protected IntVec3 altarPos => job.GetTarget(TargetIndex.B).Cell;

        [DebuggerHidden]
        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.rotateToFace = altarIndex;
            this.FailOnDespawnedOrNull(TargetIndex.B);
            this.FailOn(() => CultKnowledge.selectedDeity == null);
            this.AddFinishAction(() =>
            {   
                /*
                Log.Message("Clearing reservation");
                this.pawn.ClearAllReservations();
                //if(this.TargetA.Cell.GetEdifice(this.pawn.Map) == null) return; // ??
                if (this.pawn.Map.reservationManager.ReservedBy(this.TargetA.Cell.GetEdifice(this.pawn.Map), this.pawn))
                {
                    Log.Message("Clearing reservation");
                    this.pawn.ClearAllReservations();
                }
                */
                      
                /*
                if (this.TargetC.Cell.GetEdifice(this.pawn.Map) != null)
                {
                    if (this.pawn.Map.reservationManager.ReservedBy(this.TargetC.Cell.GetEdifice(this.pawn.Map), this.pawn))
                        this.pawn.ClearAllReservations(); // this.pawn.Map.reservationManager.Release(this.TargetC.Cell.GetEdifice(this.pawn.Map), pawn);
                }
                */
            });

            Toil worship = new Toil();

            worship.defaultCompleteMode = ToilCompleteMode.Delay;
            worship.WithProgressBarToilDelay(TargetIndex.A, false, -0.5f);
            worship.defaultDuration = 500;
            worship.initAction = delegate
            {
            };

            worship.tickAction = delegate
            {
                pawn.GainComfortFromCellIfPossible();
                Spirituality need = pawn.needs.TryGetNeed<Spirituality>();
                if(need != null) need.Gain();
            };

            worship.AddFinishAction(delegate
            {
                float chance = 0.1f; //CultKnowledge.selectedDeity.discoveryChance;
                if( Rand.Range(0.0f, 1.0f) < chance)
                {
                    CultKnowledge.DiscoverRandomDeity();
                }
            });

            yield return Toils_Reserve.Reserve(spotIndex, 1, -1);
            yield return Toils_Goto.GotoCell(spotIndex, PathEndMode.OnCell);
            yield return worship;
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