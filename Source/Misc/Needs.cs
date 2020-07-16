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

namespace Cults{

	// apprehension, 
	// perception
	// cult affinity
	// mental touhness
	// (mind, mental) fortitude
	// resilience
	// !!! Extrasensory perception (clairvoyance, telepathy, precognition)
    public class Spirituality : Need 
    {
		private float baseGainPerHour = 0.001f;
        private int lastGainTick = 0;

        public Spirituality(Pawn pawn) : base(pawn)
        {
            this.threshPercents = new List<float>();
            this.threshPercents.Add(0.2f);
			this.threshPercents.Add(0.5f);
            this.threshPercents.Add(0.7f);
        }

        public override void SetInitialLevel()
        {
            this.CurLevel = 0.0f;
        }

        public override void NeedInterval()
        {
        }

		public void Gain()
		{
			this.CurLevel += this.baseGainPerHour / 2500; // 1 hour = 2500 ticks, 1000 hours to full
			this.lastGainTick = Find.TickManager.TicksGame;
		}

        private bool isGaining
        {
            get
            {
                return Find.TickManager.TicksGame < this.lastGainTick + 2;
            }
        }
        
        //--------------------------------------------
        // UI
        
		public override int GUIChangeArrow
		{
			get
			{
				if(this.isGaining) return 1;
				return 0;
				//if(pawn.jobs.curJob.def == CultsDefOf.Cults_Job_Worship) return 1;
			}
		}

        public override void DrawOnGUI(Rect rect, int maxThresholdMarkers = int.MaxValue, float customMargin = -1f, bool drawArrows = true, bool doTooltip = true)
		{
            if(!CultKnowledge.isExposed) return;

			if (rect.height > 70f)
			{
				float num = (rect.height - 70f) / 2f;
				rect.height = 70f;
				rect.y += num;
			}
			if (Mouse.IsOver(rect))
			{
				Widgets.DrawHighlight(rect);
			}
			if (doTooltip && Mouse.IsOver(rect))
			{
				TooltipHandler.TipRegion(rect, new TipSignal(() => GetTipString(), rect.GetHashCode()));
			}
			float num2 = 14f;
			float num3 = (customMargin >= 0f) ? customMargin : (num2 + 15f);
			if (rect.height < 50f)
			{
				num2 *= Mathf.InverseLerp(0f, 50f, rect.height);
			}
			Text.Font = ((rect.height > 55f) ? GameFont.Small : GameFont.Tiny);
			Text.Anchor = TextAnchor.LowerLeft;
			Widgets.Label(new Rect(rect.x + num3 + rect.width * 0.1f, rect.y, rect.width - num3 - rect.width * 0.1f, rect.height / 2f), LabelCap);
			Text.Anchor = TextAnchor.UpperLeft;
			Rect rect2 = new Rect(rect.x, rect.y + rect.height / 2f, rect.width, rect.height / 2f);
			rect2 = new Rect(rect2.x + num3, rect2.y, rect2.width - num3 * 2f, rect2.height - num2);
			Rect rect3 = rect2;
			float num4 = 1f;
			if (def.scaleBar && MaxLevel < 1f)
			{
				num4 = MaxLevel;
			}
			rect3.width *= num4;
			Rect barRect = Widgets.FillableBar(rect3, CurLevelPercentage, Textures.RedColorTex);
			if (drawArrows)
			{
				Widgets.FillableBarChangeArrows(rect3, GUIChangeArrow);
			}
			if (threshPercents != null)
			{
				for (int i = 0; i < Mathf.Min(threshPercents.Count, maxThresholdMarkers); i++)
				{
					DrawBarThreshold(barRect, threshPercents[i] * num4);
				}
			}

			float curInstantLevelPercentage = CurInstantLevelPercentage;
			if (curInstantLevelPercentage >= 0f)
			{
				DrawBarInstantMarkerAt(rect2, curInstantLevelPercentage * num4);
			}
			if (!def.tutorHighlightTag.NullOrEmpty())
			{
				UIHighlighter.HighlightOpportunity(rect, def.tutorHighlightTag);
			}
			Text.Font = GameFont.Small;
		}

        private void DrawBarThreshold(Rect barRect, float threshPct)
		{
			float num = (!(barRect.width > 60f)) ? 1 : 2;
			Rect position = new Rect(barRect.x + barRect.width * threshPct - (num - 1f), barRect.y + barRect.height / 2f, num, barRect.height / 2f);
			Texture2D image;
			if (threshPct < CurLevelPercentage)
			{
				image = BaseContent.BlackTex;
				GUI.color = new Color(1f, 1f, 1f, 0.9f);
			}
			else
			{
				image = BaseContent.GreyTex;
				GUI.color = new Color(1f, 1f, 1f, 0.5f);
			}
			GUI.DrawTexture(position, image);
			GUI.color = Color.white;
		}
        
    }

}