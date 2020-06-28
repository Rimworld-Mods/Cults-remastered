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


// dialog // Find.WindowStack.Add(new Dialog_NamePawn(pawn));

namespace Cults
{
    public class Dialog_NamePlayerCult : Dialog_GiveName 
    {
		public Dialog_NamePlayerCult()
		{
            /*
			if (settlement.HasMap && settlement.Map.mapPawns.FreeColonistsSpawnedCount != 0)
			{
				suggestingPawn = settlement.Map.mapPawns.FreeColonistsSpawned.RandomElement();
			}
            */
			nameGenerator = () => "another RNG function here";
			curName = "RNG function here";
			nameMessageKey = "text to show here"; // top description
			gainedNameMessageKey = "renamed cult";
			invalidNameMessageKey = "invalid cult name";
		}
        public override void PostOpen()
		{
			base.PostOpen();
            Log.Message("POst open dialog event");
		}

        protected override bool IsValidName(string s)
		{
			return NamePlayerSettlementDialogUtility.IsValidName(s);
		}

		protected override void Named(string s)
		{
            Log.Message("POst named dialog event");
			Cults.CultKnowledge.RenameCult(s);
		}
    }

}