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
	public class CompAbilityEffect_PsionicBlast : CompAbilityEffect
	{
		private int blastRange = 20;

		public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
		{
			base.Apply(target, dest);
			Pawn caster = parent.pawn;
			Map map = caster.Map;

			IntVec3 loc = getPushDestination(caster.TrueCenter(), target.Pawn.TrueCenter(), caster.Map);

			ThingMover mover = ThingMover.MakeMover(CultsDefOf.Cults_BlastThing, target.Pawn, loc);
			if (mover != null)
			{
				GenSpawn.Spawn(mover, loc, map);
			}                     
		}

		private IntVec3 getPushDestination(Vector3 startLoc, Vector3 endLoc, Map map)
		{
			Vector3 direction = (endLoc - startLoc).normalized;
			Vector3 currentLoc = endLoc;

			for(int check = 1; check <= blastRange; check++)
			{
				IntVec3 cell = (currentLoc + direction).ToIntVec3();
				if(cell.Impassable(map) || !cell.Walkable(map) || !isDoorPassable(cell, map))
				{
					return currentLoc.ToIntVec3();
				}
				else
				{
					currentLoc += direction;
				}
			}
			return currentLoc.ToIntVec3();
		}

		private bool isDoorPassable(IntVec3 cell, Map map)
		{
			Building edifice = cell.GetEdifice(map);
			Building_Door building_Door;
			if (edifice != null && (building_Door = (edifice as Building_Door)) != null && !building_Door.Open)
			{
				return false;
			}
			return true;
		}
	}

}