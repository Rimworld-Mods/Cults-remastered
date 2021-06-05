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
    public class Hediff_SpectralShield : Verse.HediffWithComps
    {
		public override void PostAdd(DamageInfo? dinfo)
		{
			base.PostAdd(dinfo);
			AddPawnComp();
		}

		public override void ExposeData()
        {
            base.ExposeData();
            if (Scribe.mode == LoadSaveMode.PostLoadInit) 
            { // after game load, add [ThingComp] to [Pawn] for damage absorbtion
				AddPawnComp();
            }
        }

		public override void PostRemoved()
		{
			base.PostRemoved();
			RemovePawnComp();
		}

		public override string SeverityLabel
		{
			get
			{
				return Severity.ToStringPercent();
			}
		}

		private void AddPawnComp(){
			if(pawn != null && pawn.GetComp<CompSpectralShield>() == null)
			{
				CompSpectralShield comp = new CompSpectralShield();
				comp.parent = pawn;
				pawn.AllComps.Add(comp);
			}
		}

		private void RemovePawnComp(){
			pawn.AllComps.RemoveAll((ThingComp comp) => comp is CompSpectralShield);
		}

    }

}