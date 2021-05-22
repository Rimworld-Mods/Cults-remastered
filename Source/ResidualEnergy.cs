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
    public class ResidualEnergy : MapComponent
    {
        public ResidualEnergy(Map map) : base(map){ }
        // public override void FinalizeInit()
        // {
        //     // Messages.Message("Success", null, MessageTypeDefOf.PositiveEvent);
        //     // Find.LetterStack.ReceiveLetter("Success", CultsDefOf.Cults_Letter_Success.description, CultsDefOf.Cults_Letter_Success, null);
        // }

        private float counter = 0;
        private float severity;
        private float severityMin = 0f;
        private float severityMax = 1f;
        private float dispersePerHour = 0.01f;

        public float Severity
		{
			get
			{
				return severity;
			}
			set
			{
				severity = Mathf.Clamp(value, severityMin, severityMax);
			}
		}


        public override void MapGenerated()
		{
		}

        public override void MapComponentTick()
		{
            if(Severity > 0.1) // TODO: optimize
            {
                if(!map.GameConditionManager.ConditionIsActive(CultsDefOf.Cults_GameCondition_ResidualEnergy)){
                     map.GameConditionManager.RegisterCondition(GameConditionMaker.MakeCondition(CultsDefOf.Cults_GameCondition_ResidualEnergy));
                }
            }
            else
            {
                GameCondition_ResidualEnergy condition = map.GameConditionManager.GetActiveCondition<GameCondition_ResidualEnergy>();
                condition?.End();
            }

            if(counter++ % Tick.hour == 0)
            {
                severity -= dispersePerHour;
            }
		}
    }

}