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

    public class CompProperties_AbilityZymoticism : CompProperties_AbilityEffect
	{
		public CompProperties_AbilityZymoticism()
		{
			compClass = typeof(CompAbilityEffect_Zymoticism);
		}
	}


	public class CompAbilityEffect_Zymoticism : CompAbilityEffect{
		public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
		{
			base.Apply(target, dest);
			Log.Message("Casted spell: " + parent.def.label);     
			Pawn pawn = target.Pawn;
			// List<BodyPartRecord> parts = pawn.RaceProps.body.AllParts;

			Hediff hediff = pawn?.health?.hediffSet?.GetFirstHediffOfDef(CultsDefOf.Cults_Hediff_FoulBile);
			if(hediff == null)
			{
				HediffGiverUtility.TryApply(pawn, CultsDefOf.Cults_Hediff_FoulBile, null, false, 1, null);
			}
			else
			{
				Messages.Message("Target already has an infection", new LookTargets(pawn), MessageTypeDefOf.RejectInput, historical: false);
			}        
		}
	}


}