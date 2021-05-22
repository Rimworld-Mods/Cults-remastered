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
    public class CompProperties_AbilityOccult : CompProperties_AbilityEffect
	{
		public float sanityLoss = 0;
		public float occultismGain = 0;
		public FloatRange residualEnergyGain = new FloatRange(0.02f, 0.04f);
		public CompProperties_AbilityOccult()
		{
			compClass = typeof(CompAbilityEffect_Occult);
		}
	}

	public class CompAbilityEffect_Occult : CompAbilityEffect{
		public new CompProperties_AbilityOccult Props => (CompProperties_AbilityOccult)props;
		public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
		{
			base.Apply(target, dest);
            Pawn caster = parent.pawn;  
            caster.skills.Learn(CultsDefOf.Cults_Skill_Occultism, Props.occultismGain);
            GiveInsanity(caster);
            releaseResidualEnergy();              
		}

        public void GiveInsanity(Pawn target)
        {
            Hediff sanityLossHediff = target.health.hediffSet.GetFirstHediffOfDef(CultsDefOf.Cults_SanityLoss);
            if(sanityLossHediff != null)
            {
                sanityLossHediff.Severity += Props.sanityLoss;
            }
            else
            {
                sanityLossHediff = HediffMaker.MakeHediff(CultsDefOf.Cults_SanityLoss, target, target.health.hediffSet.GetBrain());
                sanityLossHediff.Severity = Props.sanityLoss;
                target.health.AddHediff(sanityLossHediff); // sanity loss to pawn
            }
        }

        public void releaseResidualEnergy()
        {
            FloatRange amount =  Props.residualEnergyGain;
            ResidualEnergy residualEnergy = parent.pawn.Map.GetComponent<ResidualEnergy>(); 
            float add = Rand.Range(amount.min, amount.max);
            Log.Message("Energy: " + amount.min + " - " + amount.max + " - " + add);
            residualEnergy.Severity += add;
        }
	}
}