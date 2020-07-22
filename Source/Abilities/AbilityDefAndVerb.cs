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

    public class Verb_CastOccultMagic : Verb_CastAbility 
    {
    }

}