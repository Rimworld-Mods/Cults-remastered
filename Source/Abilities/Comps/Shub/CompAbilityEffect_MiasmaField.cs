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

    public class CompProperties_AbilityMiasmaField : CompProperties_AbilityEffect
	{
		public CompProperties_AbilityMiasmaField()
		{
			compClass = typeof(CompAbilityEffect_MiasmaField);
		}
	}

	// PawnObserver.cs

	public class CompAbilityEffect_MiasmaField : CompAbilityEffect{
		public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
		{
			base.Apply(target, dest);
			Pawn pawn = parent.pawn;
			Log.Message("Casted spell: " + parent.def.label);   
			// GenExplosion.DoExplosion(pawn.Position, pawn.Map, 8, DamageDefOf.Smoke, null, -1, -1f, null, null, null, null, ThingDefOf.Gas_Smoke, 1f);          
			GenExplosion.DoExplosion(target.Cell, pawn.Map, 8, CultsDefOf.Cults_Damage_Miasma, null, -1, -1f, null, null, null, null, CultsDefOf.Cults_Gas_Miasma, 1f);          
		}
	}


}