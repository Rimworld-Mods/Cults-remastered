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
	public class Miasma : Gas 
	{
		private const int applyEffectInterval = 52;

		public override void Tick()
		{
			if(Find.TickManager.TicksGame % applyEffectInterval == 0)
			{
				List<Thing> things = base.Position.GetThingList(base.Map).ToList();
				foreach(Thing thing in things) 
				{
					if(thing is Pawn) this.ApplyEffect((Pawn)thing);
				}
			}
			base.Tick();
		}

		private void ApplyEffect(Pawn pawn)
		{
			Hediff hediff = pawn.health?.hediffSet?.GetFirstHediffOfDef(CultsDefOf.Cults_Hediff_MiasmaSickness);
			if(hediff != null)
			{
				hediff.Severity += 0.01f;
			}
			else
			{
				HediffGiverUtility.TryApply(pawn, CultsDefOf.Cults_Hediff_MiasmaSickness, null, false, 1, null);
			}
		}

	}


}