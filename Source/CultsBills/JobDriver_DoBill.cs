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
	This folder contains vanilla bill related classes with slight modifications
	Used in [BaseAltar] class for cult bills
*/

namespace Cults
{
	public class JobDriver_DoBill : Verse.AI.JobDriver_DoBill //: JobDriver
	{
		protected override IEnumerable<Toil> MakeNewToils()
		{
			AddEndCondition(delegate
			{
				Thing thing = GetActor().jobs.curJob.GetTarget(TargetIndex.A).Thing;
				return (!(thing is Building) || thing.Spawned) ? JobCondition.Ongoing : JobCondition.Incompletable;
			});
			this.FailOnBurningImmobile(TargetIndex.A);
			this.FailOn(delegate
			{
				IBillGiver billGiver = job.GetTarget(TargetIndex.A).Thing as IBillGiver;
				if (billGiver != null)
				{
					if (job.bill.DeletedOrDereferenced)
					{
						return true;
					}
					if (!billGiver.CurrentlyUsableForBills())
					{
						return true;
					}
				}
				return false;
			});

			Toil gotoBillGiver = Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.InteractionCell);
			Toil toil = new Toil();
			toil.initAction = delegate
			{
				if (job.targetQueueB != null && job.targetQueueB.Count == 1)
				{
					UnfinishedThing unfinishedThing = job.targetQueueB[0].Thing as UnfinishedThing;
					if (unfinishedThing != null)
					{
						unfinishedThing.BoundBill = (Bill_ProductionWithUft)job.bill;
					}
				}
			};

			

			yield return toil;
			yield return Toils_Jump.JumpIf(gotoBillGiver, () => job.GetTargetQueue(TargetIndex.B).NullOrEmpty());
			Toil extract = Toils_JobTransforms.ExtractNextTargetFromQueue(TargetIndex.B);
			yield return extract;
			Toil getToHaulTarget = Toils_Goto.GotoThing(TargetIndex.B, PathEndMode.ClosestTouch).FailOnDespawnedNullOrForbidden(TargetIndex.B).FailOnSomeonePhysicallyInteracting(TargetIndex.B);
			yield return getToHaulTarget;
			yield return Toils_Haul.StartCarryThing(TargetIndex.B, putRemainderInQueue: true, subtractNumTakenFromJobCount: false, failIfStackCountLessThanJobCount: true);
			yield return JumpToCollectNextIntoHandsForBill(getToHaulTarget, TargetIndex.B);
			yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.InteractionCell).FailOnDestroyedOrNull(TargetIndex.B);
			Toil findPlaceTarget2 = Toils_JobTransforms.SetTargetToIngredientPlaceCell(TargetIndex.A, TargetIndex.B, TargetIndex.C);
			yield return findPlaceTarget2;
			yield return Cults.Toils_Haul.PlaceHauledThingInCell(TargetIndex.C, findPlaceTarget2, storageMode: false);
			yield return Toils_Jump.JumpIfHaveTargetInQueue(TargetIndex.B, extract);
			yield return gotoBillGiver;

			// bill!
			
			yield return Toils_Recipe.MakeUnfinishedThingIfNeeded();

			// index 12
			yield return Toils_Recipe.DoRecipeWork().FailOnDespawnedNullOrForbiddenPlacedThings().FailOnCannotTouch(TargetIndex.A, PathEndMode.InteractionCell);
			yield return Toils_Recipe.FinishRecipeAndStartStoringProduct();
			if (job.RecipeDef.products.NullOrEmpty() && job.RecipeDef.specialProducts.NullOrEmpty())
			{
				yield break;
			}

			Cults.JobDriver_DoBill jobDriver_DoBill = this;
			yield return Toils_Reserve.Reserve(TargetIndex.B);
			findPlaceTarget2 = Toils_Haul.CarryHauledThingToCell(TargetIndex.B);
			yield return findPlaceTarget2;
			yield return Toils_Haul.PlaceHauledThingInCell(TargetIndex.B, findPlaceTarget2, storageMode: true, tryStoreInSameStorageIfSpotCantHoldWholeStack: true);
			Toil recount = new Toil();
			recount.initAction = delegate
			{
				Bill_Production bill_Production = recount.actor.jobs.curJob.bill as Bill_Production;
				if (bill_Production != null && bill_Production.repeatMode == BillRepeatModeDefOf.TargetCount)
				{
					jobDriver_DoBill.Map.resourceCounter.UpdateResourceCounts();
				}
			};
			yield return recount;
			
		}


		private static Toil JumpToCollectNextIntoHandsForBill(Toil gotoGetTargetToil, TargetIndex ind)
		{
			Toil toil = new Toil();
			toil.initAction = delegate
			{
				Pawn actor = toil.actor;
				if (actor.carryTracker.CarriedThing == null)
				{
					Log.Error(string.Concat("JumpToAlsoCollectTargetInQueue run on ", actor, " who is not carrying something."));
				}
				else if (!actor.carryTracker.Full)
				{
					Job curJob = actor.jobs.curJob;
					List<LocalTargetInfo> targetQueue = curJob.GetTargetQueue(ind);
					if (!targetQueue.NullOrEmpty())
					{
						int num = 0;
						int a;
						while (true)
						{
							if (num >= targetQueue.Count)
							{
								return;
							}
							if (GenAI.CanUseItemForWork(actor, targetQueue[num].Thing) && targetQueue[num].Thing.CanStackWith(actor.carryTracker.CarriedThing) && !((float)(actor.Position - targetQueue[num].Thing.Position).LengthHorizontalSquared > 64f))
							{
								int num2 = (actor.carryTracker.CarriedThing != null) ? actor.carryTracker.CarriedThing.stackCount : 0;
								a = curJob.countQueue[num];
								a = Mathf.Min(a, targetQueue[num].Thing.def.stackLimit - num2);
								a = Mathf.Min(a, actor.carryTracker.AvailableStackSpace(targetQueue[num].Thing.def));
								if (a > 0)
								{
									break;
								}
							}
							num++;
						}
						curJob.count = a;
						curJob.SetTarget(ind, targetQueue[num].Thing);
						curJob.countQueue[num] -= a;
						if (curJob.countQueue[num] <= 0)
						{
							curJob.countQueue.RemoveAt(num);
							targetQueue.RemoveAt(num);
						}
						actor.jobs.curDriver.JumpToToil(gotoGetTargetToil);
					}
				}
			};
			return toil;
		}


	}
}
