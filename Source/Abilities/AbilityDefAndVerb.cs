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
    public class OccultAbility : Ability
    {
        public OccultAbility(Pawn pawn) : base(pawn)
        {
            //comps = new List<AbilityComp>(); // By default [comps] are null and base class tries to use this var without checking it
        }
        public OccultAbility(Pawn pawn, AbilityDef def) : base(pawn, def)
        {
            //comps = new List<AbilityComp>();
        }

		public override bool GizmoDisabled(out string reason)
		{
            // extra reasons to disable ability
            return base.GizmoDisabled(out reason);
		}

        public override bool Activate(LocalTargetInfo target, LocalTargetInfo dest)
        {
            return base.Activate(target, dest);
        }
    }



    // Methods not required, they only add proper field of view highlight
    public class Verb_CastOccultMagic : Verb_CastAbility 
    {

    }

    public class Verb_MoveOccultMagic : Verb_CastAbility 
    {
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