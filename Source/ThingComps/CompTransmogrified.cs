using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace Cults
{
    public class CompTransmogrified : ThingComp
    {
        public Pawn Pawn => this.parent as Pawn;
        public Hediff_SpectralShield Hediff => null; //Pawn.health.hediffSet.GetFirstHediffOfDef(CultsDefOf.Cults_Hediff_SpectralShield) as Hediff_SpectralShield;

		public override void CompTickRare()
		{
            Log.Message("CompTransmogrified is ticking");
		}

        public override void PostPreApplyDamage(DamageInfo dinfo, out bool absorbed)
        {
            absorbed = true;
        }


        //public BodyPartRecord CorePart
        //{
        //    get
        //    {
        //        return Pawn?.health?.hediffSet.GetNotMissingParts().FirstOrDefault(x => x.def == Pawn?.RaceProps?.body?.corePart?.def) ?? null;
        //    }
        //}

        // private bool isTransmogrified = false;
        // public bool IsTransmogrified
        // {
        //     get => isTransmogrified;
        //     set
        //     {
        //         if (value == true && isTransmogrified == false)
        //         {
        //             Find.LetterStack.ReceiveLetter("Cults_TransmogrifiedLetter".Translate(), "Cults_TransmogrifiedLetterDesc".Translate(this.parent.LabelShort), LetterDefOf.PositiveEvent, new RimWorld.Planet.GlobalTargetInfo(this.parent), null);
        //         }
        //         //HealthUtility.AdjustSeverity(this.parent as Pawn, CultsDefOf.Cults_MonstrousBody, 1.0f);
        //         isTransmogrified = value;
        //         MakeHediff();

        //     }
        // }

        // public void MakeHediff()
        // {
        //     if (isTransmogrified && Hediff == null)
        //     {
        //         Hediff hediff = null; //HediffMaker.MakeHediff(CultsDefOf.Cults_MonstrousBody, Pawn, null);
        //         hediff.Severity = 1.0f;
        //         Pawn.health.AddHediff(hediff, null, null);
        //     }
        // }

        // public override void PostExposeData()
        // {
        //     base.PostExposeData();
        //     Scribe_Values.Look<bool>(ref this.isTransmogrified, "isTransmogrified", false);
        //     if (Scribe.mode == LoadSaveMode.PostLoadInit)
        //     {
        //         MakeHediff();
        //     }
        // }
    }
}
