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
    
    public class CompProperties_AbilityEerieScorch : CompProperties_AbilityEffect
	{
		public CompProperties_AbilityEerieScorch()
		{
			compClass = typeof(CompAbilityEffect_EerieScorch);
		}
	}

    public class CompAbilityEffect_EerieScorch : CompAbilityEffect
    {
        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
		{
			base.Apply(target, dest);
			if(target.Thing != null)
			{
				OccultFireUtility.TryAttachOccultFire(target.Thing, 0.5f);
				Log.Message("Casted spell: " + parent.def.label);  
			}
		}
        
    }

}