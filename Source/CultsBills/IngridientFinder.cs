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
	
*/

namespace Cults
{
    public class IngredientFinder
    {
        private class DefCountList
		{
			private List<ThingDef> defs = new List<ThingDef>();

			private List<float> counts = new List<float>();

			public int Count => defs.Count;

			public float this[ThingDef def]
			{
				get
				{
					int num = defs.IndexOf(def);
					if (num < 0)
					{
						return 0f;
					}
					return counts[num];
				}
				set
				{
					int num = defs.IndexOf(def);
					if (num < 0)
					{
						defs.Add(def);
						counts.Add(value);
						num = defs.Count - 1;
					}
					else
					{
						counts[num] = value;
					}
					CheckRemove(num);
				}
			}

			public float GetCount(int index)
			{
				return counts[index];
			}

			public void SetCount(int index, float val)
			{
				counts[index] = val;
				CheckRemove(index);
			}

			public ThingDef GetDef(int index)
			{
				return defs[index];
			}

			private void CheckRemove(int index)
			{
				if (counts[index] == 0f)
				{
					counts.RemoveAt(index);
					defs.RemoveAt(index);
				}
			}

			public void Clear()
			{
				defs.Clear();
				counts.Clear();
			}

			public void GenerateFrom(List<Thing> things)
			{
				Clear();
				for (int i = 0; i < things.Count; i++)
				{
					this[things[i].def] += things[i].stackCount;
				}
			}
		}


        //-------------------------------------------------------------------

        private List<ThingCount> chosenIngThings = new List<ThingCount>();
        private static List<Thing> newRelevantThings = new List<Thing>();
        private static List<Thing> relevantThings = new List<Thing>();
        private static HashSet<Thing> processedThings = new HashSet<Thing>();
        private static List<IngredientCount> ingredientsOrdered = new List<IngredientCount>();
        private static DefCountList availableCounts = new DefCountList();

        //-------------------------------------------------------------------

        public static bool TryFindBestBillIngredients(Bill bill, Pawn pawn, Thing billGiver, List<ThingCount> chosen)
		{
			chosen.Clear();
			newRelevantThings.Clear();
			if (bill.recipe.ingredients.Count == 0)
			{
				return true;
			}
			IntVec3 rootCell = GetBillGiverRootCell(billGiver, pawn);
			Region rootReg = rootCell.GetRegion(pawn.Map);
			if (rootReg == null)
			{
				return false;
			}
			MakeIngredientsListInProcessingOrder(ingredientsOrdered, bill);
			relevantThings.Clear();
			processedThings.Clear();
			bool foundAll = false;
			Predicate<Thing> baseValidator = (Thing t) => t.Spawned && !t.IsForbidden(pawn) && (float)(t.Position - billGiver.Position).LengthHorizontalSquared < bill.ingredientSearchRadius * bill.ingredientSearchRadius && bill.IsFixedOrAllowedIngredient(t) && bill.recipe.ingredients.Any((IngredientCount ingNeed) => ingNeed.filter.Allows(t)) && pawn.CanReserve(t);
			bool billGiverIsPawn = billGiver is Pawn;
            /*
			if (billGiverIsPawn)
			{
				AddEveryMedicineToRelevantThings(pawn, billGiver, relevantThings, baseValidator, pawn.Map);
				if (TryFindBestBillIngredientsInSet(relevantThings, bill, chosen, rootCell, billGiverIsPawn))
				{
					relevantThings.Clear();
					ingredientsOrdered.Clear();
					return true;
				}
			}
            */

			TraverseParms traverseParams = TraverseParms.For(pawn);
			RegionEntryPredicate entryCondition = null;
			if (Math.Abs(999f - bill.ingredientSearchRadius) >= 1f)
			{
				float radiusSq = bill.ingredientSearchRadius * bill.ingredientSearchRadius;
				entryCondition = delegate(Region from, Region r)
				{
					if (!r.Allows(traverseParams, isDestination: false))
					{
						return false;
					}
					CellRect extentsClose = r.extentsClose;
					int num = Math.Abs(billGiver.Position.x - Math.Max(extentsClose.minX, Math.Min(billGiver.Position.x, extentsClose.maxX)));
					if ((float)num > bill.ingredientSearchRadius)
					{
						return false;
					}
					int num2 = Math.Abs(billGiver.Position.z - Math.Max(extentsClose.minZ, Math.Min(billGiver.Position.z, extentsClose.maxZ)));
					return !((float)num2 > bill.ingredientSearchRadius) && (float)(num * num + num2 * num2) <= radiusSq;
				};
			}
			else
			{
				entryCondition = ((Region from, Region r) => r.Allows(traverseParams, isDestination: false));
			}
			int adjacentRegionsAvailable = rootReg.Neighbors.Count((Region region) => entryCondition(rootReg, region));
			int regionsProcessed = 0;
			processedThings.AddRange(relevantThings);
			RegionProcessor regionProcessor = delegate(Region r)
			{
				List<Thing> list = r.ListerThings.ThingsMatching(ThingRequest.ForGroup(ThingRequestGroup.HaulableEver));
				for (int i = 0; i < list.Count; i++)
				{
					Thing thing = list[i];
					if (!processedThings.Contains(thing) && ReachabilityWithinRegion.ThingFromRegionListerReachable(thing, r, PathEndMode.ClosestTouch, pawn) && baseValidator(thing) && !(thing.def.IsMedicine && billGiverIsPawn))
					{
						newRelevantThings.Add(thing);
						processedThings.Add(thing);
					}
				}
				regionsProcessed++;
				if (newRelevantThings.Count > 0 && regionsProcessed > adjacentRegionsAvailable)
				{
					relevantThings.AddRange(newRelevantThings);
					newRelevantThings.Clear();
					if (TryFindBestBillIngredientsInSet(relevantThings, bill, chosen, rootCell, billGiverIsPawn))
					{
						foundAll = true;
						return true;
					}
				}
				return false;
			};
			RegionTraverser.BreadthFirstTraverse(rootReg, entryCondition, regionProcessor, 99999);
			relevantThings.Clear();
			newRelevantThings.Clear();
			processedThings.Clear();
			ingredientsOrdered.Clear();
			return foundAll;
		}

        private static IntVec3 GetBillGiverRootCell(Thing billGiver, Pawn forPawn)
		{
			Building building = billGiver as Building;
			if (building != null)
			{
				if (building.def.hasInteractionCell)
				{
					return building.InteractionCell;
				}
				Log.Error(string.Concat("Tried to find bill ingredients for ", billGiver, " which has no interaction cell."));
				return forPawn.Position;
			}
			return billGiver.Position;
		}

        private static void MakeIngredientsListInProcessingOrder(List<IngredientCount> ingredientsOrdered, Bill bill)
		{
			ingredientsOrdered.Clear();
			if (bill.recipe.productHasIngredientStuff)
			{
				ingredientsOrdered.Add(bill.recipe.ingredients[0]);
			}
			for (int i = 0; i < bill.recipe.ingredients.Count; i++)
			{
				if (!bill.recipe.productHasIngredientStuff || i != 0)
				{
					IngredientCount ingredientCount = bill.recipe.ingredients[i];
					if (ingredientCount.IsFixedIngredient)
					{
						ingredientsOrdered.Add(ingredientCount);
					}
				}
			}
			for (int j = 0; j < bill.recipe.ingredients.Count; j++)
			{
				IngredientCount item = bill.recipe.ingredients[j];
				if (!ingredientsOrdered.Contains(item))
				{
					ingredientsOrdered.Add(item);
				}
			}
		}

        private static bool TryFindBestBillIngredientsInSet(List<Thing> availableThings, Bill bill, List<ThingCount> chosen, IntVec3 rootCell, bool alreadySorted)
		{
			if (bill.recipe.allowMixingIngredients)
			{
				return TryFindBestBillIngredientsInSet_AllowMix(availableThings, bill, chosen);
			}
			return TryFindBestBillIngredientsInSet_NoMix(availableThings, bill, chosen, rootCell, alreadySorted);
		}

        private static bool TryFindBestBillIngredientsInSet_AllowMix(List<Thing> availableThings, Bill bill, List<ThingCount> chosen)
		{
			chosen.Clear();
			availableThings.Sort((Thing t, Thing t2) => bill.recipe.IngredientValueGetter.ValuePerUnitOf(t2.def).CompareTo(bill.recipe.IngredientValueGetter.ValuePerUnitOf(t.def)));
			for (int i = 0; i < bill.recipe.ingredients.Count; i++)
			{
				IngredientCount ingredientCount = bill.recipe.ingredients[i];
				float num = ingredientCount.GetBaseCount();
				for (int j = 0; j < availableThings.Count; j++)
				{
					Thing thing = availableThings[j];
					if (ingredientCount.filter.Allows(thing) && (ingredientCount.IsFixedIngredient || bill.ingredientFilter.Allows(thing)))
					{
						float num2 = bill.recipe.IngredientValueGetter.ValuePerUnitOf(thing.def);
						int num3 = Mathf.Min(Mathf.CeilToInt(num / num2), thing.stackCount);
						ThingCountUtility.AddToList(chosen, thing, num3);
						num -= (float)num3 * num2;
						if (num <= 0.0001f)
						{
							break;
						}
					}
				}
				if (num > 0.0001f)
				{
					return false;
				}
			}
			return true;
		}

        private static bool TryFindBestBillIngredientsInSet_NoMix(List<Thing> availableThings, Bill bill, List<ThingCount> chosen, IntVec3 rootCell, bool alreadySorted)
		{
			if (!alreadySorted)
			{
				Comparison<Thing> comparison = delegate(Thing t1, Thing t2)
				{
					float num4 = (t1.Position - rootCell).LengthHorizontalSquared;
					float value = (t2.Position - rootCell).LengthHorizontalSquared;
					return num4.CompareTo(value);
				};
				availableThings.Sort(comparison);
			}
			RecipeDef recipe = bill.recipe;
			chosen.Clear();
			availableCounts.Clear();
			availableCounts.GenerateFrom(availableThings);
			for (int i = 0; i < ingredientsOrdered.Count; i++)
			{
				IngredientCount ingredientCount = recipe.ingredients[i];
				bool flag = false;
				for (int j = 0; j < availableCounts.Count; j++)
				{
					float num = ingredientCount.CountRequiredOfFor(availableCounts.GetDef(j), bill.recipe);
					if ((!recipe.ignoreIngredientCountTakeEntireStacks && num > availableCounts.GetCount(j)) || !ingredientCount.filter.Allows(availableCounts.GetDef(j)) || (!ingredientCount.IsFixedIngredient && !bill.ingredientFilter.Allows(availableCounts.GetDef(j))))
					{
						continue;
					}
					for (int k = 0; k < availableThings.Count; k++)
					{
						if (availableThings[k].def != availableCounts.GetDef(j))
						{
							continue;
						}
						int num2 = availableThings[k].stackCount - ThingCountUtility.CountOf(chosen, availableThings[k]);
						if (num2 > 0)
						{
							if (recipe.ignoreIngredientCountTakeEntireStacks)
							{
								ThingCountUtility.AddToList(chosen, availableThings[k], num2);
								return true;
							}
							int num3 = Mathf.Min(Mathf.FloorToInt(num), num2);
							ThingCountUtility.AddToList(chosen, availableThings[k], num3);
							num -= (float)num3;
							if (num < 0.001f)
							{
								flag = true;
								float count = availableCounts.GetCount(j);
								count -= (float)ingredientCount.CountRequiredOfFor(availableCounts.GetDef(j), bill.recipe);
								availableCounts.SetCount(j, count);
								break;
							}
						}
					}
					if (flag)
					{
						break;
					}
				}
				if (!flag)
				{
					return false;
				}
			}
			return true;
		}

    }

}



