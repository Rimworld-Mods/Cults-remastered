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
	public class MainTabWindow_Restrict : MainTabWindow_PawnTable
	{
		private const int TimeAssignmentSelectorWidth = 191;

		private const int TimeAssignmentSelectorHeight = 65;

		protected override PawnTableDef PawnTableDef => PawnTableDefOf.Restrict;

		public override void PostOpen()
		{
			base.PostOpen();
			Find.World.renderer.wantedMode = WorldRenderMode.None;
		}

		public override void DoWindowContents(Rect fillRect)
		{
			base.DoWindowContents(fillRect);
            TimeAssignmentSelector.DrawTimeAssignmentSelectorGrid(new Rect(0f, 0f, 191f, 65f));

            // TODO: check if Royalty active (ModsConfig.RoyaltyActive)
            // new code:
            Rect rect = new Rect(191f + 96f, 0f, 191f, 65f);
            rect.xMax = rect.center.x;
			rect.yMax = rect.center.y - 1f;
            DrawTimeAssignmentSelectorFor(rect, CultsDefOf.Cults_TimeAssignment_Worship);
		}

        // Copied function from source
        private void DrawTimeAssignmentSelectorFor(Rect rect, TimeAssignmentDef ta)
		{
			rect = rect.ContractedBy(2f);
			GUI.DrawTexture(rect, ta.ColorTexture);
			if (Widgets.ButtonInvisible(rect))
			{
				TimeAssignmentSelector.selectedAssignment = ta;
				SoundDefOf.Tick_High.PlayOneShotOnCamera();
			}
			GUI.color = Color.white;
			if (Mouse.IsOver(rect))
			{
				Widgets.DrawHighlight(rect);
			}
			Text.Font = GameFont.Small;
			Text.Anchor = TextAnchor.MiddleCenter;
			GUI.color = Color.white;
			Widgets.Label(rect, ta.LabelCap);
			Text.Anchor = TextAnchor.UpperLeft;
			if (TimeAssignmentSelector.selectedAssignment == ta)
			{
				Widgets.DrawBox(rect, 2);
			}
			else
			{
				UIHighlighter.HighlightOpportunity(rect, ta.cachedHighlightNotSelectedTag);
			}
		}
	}
}