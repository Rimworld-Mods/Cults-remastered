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
	public class Miasma : Thing // Same as gas but with [ApplyEffect]
	{
		public int destroyTick;

		public float graphicRotation;

		public float graphicRotationSpeed;

		private const int applyEffectInterval = 52;

		public override void SpawnSetup(Map map, bool respawningAfterLoad)
		{
			while (true)
			{
				Thing gas = base.Position.GetGas(map);
				if (gas == null)
				{
					break;
				}
				gas.Destroy();
			}
			base.SpawnSetup(map, respawningAfterLoad);
			if (!respawningAfterLoad)
			{
				destroyTick = Find.TickManager.TicksGame + def.gas.expireSeconds.RandomInRange.SecondsToTicks();
			}
			graphicRotationSpeed = Rand.Range(0f - def.gas.rotationSpeed, def.gas.rotationSpeed) / 60f;
		}

		public override void Tick()
		{
			if(Find.TickManager.TicksGame % applyEffectInterval == 0)
			{
				List<Thing> things = base.Position.GetThingList(base.Map);
				foreach(Thing thing in things) 
				{
					if(thing is Pawn) this.ApplyEffect((Pawn)thing);
				}
			}
			if (destroyTick <= Find.TickManager.TicksGame)
			{
				Destroy();
			}
			graphicRotation += graphicRotationSpeed;
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref destroyTick, "destroyTick", 0);
		}

		private void ApplyEffect(Pawn pawn)
		{
			List<BodyPartDef> breathing_sources = pawn.RaceProps.body.GetPartsWithTag(BodyPartTagDefOf.BreathingSource).Select(part => part.def).ToList();
			Hediff hediff = pawn.health.hediffSet.GetFirstHediffOfDef(CultsDefOf.Cults_Hediff_WateryLungs);
			if(hediff != null)
			{
				hediff.Severity += 0.01f;
			}
			else
			{
				HediffGiverUtility.TryApply(pawn, CultsDefOf.Cults_Hediff_WateryLungs, breathing_sources, true, 2, null);
				Job job = JobMaker.MakeJob(CultsDefOf.Cults_Job_VomitWater);
			}
		}
	}



}