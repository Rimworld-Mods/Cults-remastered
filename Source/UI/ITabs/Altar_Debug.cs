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
    class ITab_AltarDebug : ITab
    {
        public override bool IsVisible => Prefs.DevMode;

        public ITab_AltarDebug()
        {
            this.size = WorshipCardUtility.cardSize;
            this.labelKey = "Debug";
        }

        
        protected override void FillTab()
        {
            DrawButtons();
            DrawSelections();
        }

        private void DrawButtons(){
            Rect rect;
            rect = new Rect(17f, 25f, 120f, 25f);
            if (Widgets.ButtonText(rect, "Discover", true, false, true))
            {
                CultKnowledge.DiscoverDeity();
            }

            rect = new Rect(17f, 50f, 120f, 25f);
            if (Widgets.ButtonText(rect, " Finish research ", true, false, true))
            {
                Find.ResearchManager.FinishProject(CultsDefOf.Cults_ForbiddenSculptures, true, null);
            }
            
            rect = new Rect(17f, 75f, 120f, 25f);
            if (Widgets.ButtonText(rect, " - ", true, false, true))
            {

            }
        }

        private void DrawSelections(){

        }
    }
}