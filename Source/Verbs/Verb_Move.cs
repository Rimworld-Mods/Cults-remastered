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
    public class Verb_Move : Verb_CastAbility 
    {
		public override void OrderForceTarget(LocalTargetInfo target)
		{
			Map map = CasterPawn.Map;
			IntVec3 intVec = RCellFinder.BestOrderedGotoDestNear_NewTemp(target.Cell, CasterPawn, AcceptableDestination);
			Job job = JobMaker.MakeJob(JobDefOf.CastJump, intVec);
			job.verbToUse = this;
			if (CasterPawn.jobs.TryTakeOrderedJob(job))
			{
				MoteMaker.MakeStaticMote(intVec, map, ThingDefOf.Mote_FeedbackGoto);
			}
			bool AcceptableDestination(IntVec3 c)
			{
				if (ValidJumpTarget(map, c))
				{
					return CanHitTargetFrom(caster.Position, c);
				}
				return false;
			}
		}

		public override bool ValidateTarget(LocalTargetInfo target)
		{
			if (caster == null)
			{
				return false;
			}
			if (!CanHitTarget(target) || !ValidJumpTarget(caster.Map, target.Cell))
			{
				return false;
			}
			if (!ReloadableUtility.CanUseConsideringQueuedJobs(CasterPawn, base.EquipmentSource))
			{
				return false;
			}
			return true;
		}

		public override bool CanHitTargetFrom(IntVec3 root, LocalTargetInfo targ)
		{
			float num = EffectiveRange * EffectiveRange;
			IntVec3 cell = targ.Cell;
			if ((float)caster.Position.DistanceToSquared(cell) <= num)
			{
				return GenSight.LineOfSight(root, cell, caster.Map);
			}
			return false;
		}

		public override void OnGUI(LocalTargetInfo target)
		{
			if (CanHitTarget(target) && ValidJumpTarget(caster.Map, target.Cell))
			{
				base.OnGUI(target);
			}
			else
			{
				GenUI.DrawMouseAttachment(TexCommand.CannotShoot);
			}
		}

		public override void DrawHighlight(LocalTargetInfo target)
		{
			if (target.IsValid && ValidJumpTarget(caster.Map, target.Cell))
			{
				GenDraw.DrawTargetHighlightWithLayer(target.CenterVector3, AltitudeLayer.MetaOverlays);
			}
			GenDraw.DrawRadiusRing(caster.Position, EffectiveRange, Color.white, (IntVec3 c) => GenSight.LineOfSight(caster.Position, c, caster.Map) && ValidJumpTarget(caster.Map, c));
		}

		public static bool ValidJumpTarget(Map map, IntVec3 cell)
		{
			if (!cell.IsValid || !cell.InBounds(map))
			{
				return false;
			}
			if (cell.Impassable(map) || !cell.Walkable(map) || cell.Fogged(map))
			{
				return false;
			}
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