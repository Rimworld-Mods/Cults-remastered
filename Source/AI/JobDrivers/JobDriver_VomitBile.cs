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
	public class JobDriver_VomitBile : RimWorld.JobDriver_Vomit
	{
		private int ticksLeft;

		protected override IEnumerable<Toil> MakeNewToils()
		{
			Toil toil = new Toil();
			toil.initAction = delegate
			{
				ticksLeft = Rand.Range(300, 900);
				int num = 0;
				IntVec3 c;
				do
				{
					c = pawn.Position + GenAdj.AdjacentCellsAndInside[Rand.Range(0, 9)];
					num++;
					if (num > 12)
					{
						c = pawn.Position;
						break;
					}
				}
				while (!c.InBounds(pawn.Map) || !c.Standable(pawn.Map));
				job.targetA = c;
				pawn.pather.StopDead();
			};
			toil.tickAction = delegate
			{
				if (ticksLeft % 150 == 149)
				{
					FilthMaker.TryMakeFilth(job.targetA.Cell, base.Map, CultsDefOf.Cults_Filth_BileVomit, pawn.LabelIndefinite());
					if (pawn.needs.food.CurLevelPercentage > 0.1f) // TODO: do other harm?
					{
						pawn.needs.food.CurLevel -= pawn.needs.food.MaxLevel * 0.04f;
					}
				}
				ticksLeft--;
				if (ticksLeft <= 0)
				{
					ReadyForNextToil();
					TaleRecorder.RecordTale(TaleDefOf.Vomited, pawn);
				}
			};
			toil.defaultCompleteMode = ToilCompleteMode.Never;
			toil.WithEffect(CultsDefOf.Cults_Effecter_VomitBile, TargetIndex.A);
			toil.PlaySustainerOrSound(() => SoundDefOf.Vomit);
			yield return toil;
		}
	}
}
