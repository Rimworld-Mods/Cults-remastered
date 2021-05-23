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
	public class JobDriver_WaitDemise : JobDriver_Wait
	{
		// public override void DecorateWaitToil(Toil wait)
		// {
		// 	base.DecorateWaitToil(wait);
		// 	wait.AddFailCondition(() => true);
		// }

		protected override IEnumerable<Toil> MakeNewToils()
        {

            yield return new Toil
            {
                initAction = delegate
                {
                    this.pawn.Reserve(this.pawn.Position, this.job);// De ReserveDestinationFor(this.pawn, this.pawn.Position);
                    this.pawn.pather.StopDead();
                    JobDriver curDriver = this.pawn.jobs.curDriver;
                    pawn.jobs.posture = PawnPosture.LayingOnGroundFaceUp;
                    curDriver.asleep = false;
                },
                tickAction = delegate
                {
                    if (this.job.expiryInterval == -1 && this.job.def == JobDefOf.Wait_Combat && !this.pawn.Drafted)
                    {
                        Log.Error(this.pawn + " in eternal WaitCombat without being drafted.");
                        this.ReadyForNextToil();
                        return;
                    }
                    if ((Find.TickManager.TicksGame + this.pawn.thingIDNumber) % 4 == 0)
                    {
                        //base.CheckForAutoAttack();
                    }
                    
                },
                defaultCompleteMode = ToilCompleteMode.Never
            };
        }

	}
}