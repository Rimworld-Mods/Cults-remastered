using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using RimWorld;

namespace Cults
{
    public class HediffComp_SanityLoss : HediffComp
    {
        // public override void CompPostTick(ref float severityAdjustment)
        // {
        //     if (base.Pawn != null)
        //     {
        //         if (base.Pawn.RaceProps != null)
        //         {
        //             if (base.Pawn.RaceProps.IsMechanoid)
        //             {
        //                 MakeSane();
        //             }
        //         }
        //     }
        // }

        
        public void RemoveSanity()
        {
            this.parent.Severity -= 1f;
            base.Pawn.health.Notify_HediffChanged(this.parent);
        }
        
    }

}
