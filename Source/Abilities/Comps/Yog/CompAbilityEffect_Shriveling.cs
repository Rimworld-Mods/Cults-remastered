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

    public class CompProperties_AbilityShriveling : CompProperties_AbilityEffect
	{
		public CompProperties_AbilityShriveling()
		{
			compClass = typeof(CompAbilityEffect_Shriveling);
		}
	}


	public class CompAbilityEffect_Shriveling : CompAbilityEffect{
		public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
		{
			base.Apply(target, dest);
			Log.Message("Casted spell: " + parent.def.label);   

			Pawn pawn = target.Pawn;
			if(pawn != null && pawn.stances != null)
			{
				DamageInfo dinfo = new DamageInfo(DamageDefOf.Cut, 20, 1.0f);
				this.MakeMote(target.Pawn);
				pawn.stances.StaggerFor(45);
				pawn.TakeDamage(dinfo); // todo: associate with combat log: .AssociateWithLog(battleLogEntry_RangedImpact);
			}          
		}
		

		private void MakeMote(Pawn pawn){
			Vector3 loc = pawn.TrueCenter();
			Mote obj = (Mote)ThingMaker.MakeThing(CultsDefOf.Cults_Mote_Slash);
			obj.Scale = 1.9f;
			obj.rotationRate = Rand.Range(-60, 60);
			obj.exactPosition = loc;
			GenSpawn.Spawn(obj, loc.ToIntVec3(), pawn.Map);
		}
	}


}