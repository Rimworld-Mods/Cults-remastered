using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using RimWorld;

namespace Cults
{
    // [Cults.HediffWithComps] - shows severity level and severity factor in % without [def.lethalSeverity] from [Hediff] class

    public class HediffWithComps : Verse.HediffWithComps
    {
		public override string SeverityLabel
		{
			get
			{
				// return (Severity / def.maxSeverity).ToStringPercent();
				return Severity.ToStringPercent();
			}
		}
    }

}