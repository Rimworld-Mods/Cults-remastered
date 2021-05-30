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


// TODO: remove?
namespace Cults
{
	public class CompProperties_AbilityGiveHediff : RimWorld.CompProperties_AbilityGiveHediff
	{
        public List<BodyPartDef> partsToAffect;
        public int bodyPartCount = 1;
		public JobDef jobToStart;
	}
}



