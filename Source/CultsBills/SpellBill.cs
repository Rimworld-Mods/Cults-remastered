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

// With vanilla code ingridients can only be found with [WorkGiver_DoBill] class.
// ingridient finder function is private
// Extracted from [WorkGiver_DoBill] class finder to use it for spell ingridients checker

namespace Cults{

    public class Bill_Spell : Bill_Production 
    {
        public Bill_Spell() : base() {}
        public Bill_Spell(RecipeDef recipe) : base(recipe) {}

		public override void Notify_IterationCompleted(Pawn billDoer, List<Thing> ingredients)
		{

            Building_BaseAltar altar = (Building_BaseAltar)this.billStack.billGiver;
            base.Notify_IterationCompleted(billDoer, ingredients);
            Log.Message("start spell incident");
            Log.Message("giver: " + altar.LabelShort.ToString());

            // altar has required info
            // roll dice
            // do spell incident here?

        }
    }
}