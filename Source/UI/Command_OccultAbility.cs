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
    [StaticConstructorOnStartup]
	public class Command_OccultAbility : Command
	{
		protected Ability ability;

		public new static readonly Texture2D BGTex = ContentFinder<Texture2D>.Get("UI/Widgets/CultsButBG");

		private static readonly Texture2D cooldownBarTex = SolidColorMaterials.NewSolidColorTexture(new Color32(200, 11, 132, 102));

		public override Texture2D BGTexture => BGTex;

		public virtual string Tooltip => ability.def.GetTooltip(ability.pawn);

		public Command_OccultAbility(Ability ability)
		{
			this.ability = ability;
			order = 5f;
			defaultLabel = ability.def.LabelCap;
			hotKey = ability.def.hotKey;
			icon = ability.def.uiIcon;
		}

		public override GizmoResult GizmoOnGUI(Vector2 topLeft, float maxWidth)
		{
			defaultDesc = Tooltip;
			disabled = ability.GizmoDisabled(out string reason);
			if (disabled)
			{
				DisableWithReason(reason.CapitalizeFirst());
			}
			GizmoResult result = base.GizmoOnGUI(topLeft, maxWidth);
			if (ability.CooldownTicksRemaining > 0)
			{
				float num = Mathf.InverseLerp(ability.CooldownTicksTotal, 0f, ability.CooldownTicksRemaining);
				Rect rect = new Rect(topLeft.x, topLeft.y, GetWidth(maxWidth), 75f);
				Widgets.FillableBar(rect, Mathf.Clamp01(num), cooldownBarTex, null, doBorder: false);
				if (ability.CooldownTicksRemaining > 0)
				{
					Text.Font = GameFont.Tiny;
					Text.Anchor = TextAnchor.UpperCenter;
					Widgets.Label(rect, num.ToStringPercent("F0"));
					Text.Anchor = TextAnchor.UpperLeft;
				}
			}
			if (result.State == GizmoState.Interacted && ability.CanCast)
			{
				return result;
			}
			return new GizmoResult(result.State);
		}

		public override void ProcessInput(Event ev)
		{
			base.ProcessInput(ev);
			SoundDefOf.Tick_Tiny.PlayOneShotOnCamera();
			if (ability.def.targetRequired)
			{
				Find.Targeter.BeginTargeting(ability.verb);
			}
			else
			{
				ability.verb.TryStartCastOn(ability.pawn);
			}
		}

		public override void GizmoUpdateOnMouseover()
		{
			Verb_CastAbility verb_CastAbility;
			if ((verb_CastAbility = (ability.verb as Verb_CastAbility)) != null && ability.def.targetRequired)
			{
				verb_CastAbility.DrawRadius();
			}
		}

		protected void DisableWithReason(string reason)
		{
			disabledReason = reason;
			disabled = true;
		}
	}
}