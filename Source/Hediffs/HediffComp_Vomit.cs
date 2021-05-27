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
    
    public class HediffComp_Vomit : Verse.HediffComp
    {
        HediffCompProperties_PerformJob Props => (HediffCompProperties_PerformJob)props;
        public override void CompPostTick(ref float severityAdjustment)
		{
            Pawn pawn = parent.pawn;
            // parent.CurStage;
            // TODO: make with stages
			if (Props.jobMtbDays > 0f && pawn.IsHashIntervalTick(600) && Rand.MTBEventOccurs(Props.jobMtbDays, 60000f, 600f) && parent.pawn.Spawned && pawn.Awake() && pawn.RaceProps.IsFlesh)
			{
				pawn.jobs.StartJob(JobMaker.MakeJob(Props.jobDef), JobCondition.InterruptForced, null, resumeCurJobAfterwards: true);
			}
		}
    }

}