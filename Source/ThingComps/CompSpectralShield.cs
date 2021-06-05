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
    public class CompSpectralShield : ThingComp
    {
        public Pawn Pawn => this.parent as Pawn;
        public Hediff_SpectralShield Hediff => Pawn.health.hediffSet.GetFirstHediffOfDef(CultsDefOf.Cults_Hediff_SpectralShield) as Hediff_SpectralShield;

		private Vector3 impactAngleVect;

		private int lastAbsorbDamageTick = -9999;

		private const float MinDrawSize = 1.3f;

		private const float MaxDrawSize = 1.55f;

		private const float MaxDamagedJitterDist = 0.05f;

		private const int JitterDurationTicks = 8;

		private float EnergyLossPerDamage = 0.005f;

		private static readonly Material BubbleMat = MaterialPool.MatFrom("Other/ShieldBubble", ShaderDatabase.Transparent);

		public float Energy
        {
            get
            {
                return Hediff.Severity;
            }
            set
            {
                Hediff.Severity = value;
            }

        }

		private bool ShouldDisplay
		{
			get
			{
				if (!Pawn.Spawned || Pawn.Dead)
				{
					return false;
				}
                return true;
			}
		}


		public override void CompTick()
		{
			base.CompTick();
			if (Pawn == null)
			{
				Energy = 0f;
			}
		}


        public override void PostPreApplyDamage(DamageInfo dinfo, out bool absorbed)
        {
            absorbed = this.checkDamageAbsorbtion(dinfo);
        }

		private bool checkDamageAbsorbtion(DamageInfo dinfo)
		{
            Energy -= dinfo.Amount * EnergyLossPerDamage;
            if (Energy < 0f)
            {
                Break();
            }
            else
            {
                AbsorbedDamage(dinfo);
            }
            return true;
		}

		private void AbsorbedDamage(DamageInfo dinfo)
		{
			SoundDefOf.EnergyShield_AbsorbDamage.PlayOneShot(new TargetInfo(Pawn.Position, Pawn.Map));
			impactAngleVect = Vector3Utility.HorizontalVectorFromAngle(dinfo.Angle);
			Vector3 loc = Pawn.TrueCenter() + impactAngleVect.RotatedBy(180f) * 0.5f;
			float num = Mathf.Min(10f, 2f + dinfo.Amount / 10f);
			MoteMaker.MakeStaticMote(loc, Pawn.Map, ThingDefOf.Mote_ExplosionFlash, num);
			int num2 = (int)num;
			for (int i = 0; i < num2; i++)
			{
				MoteMaker.ThrowDustPuff(loc, Pawn.Map, Rand.Range(0.8f, 1.2f));
			}
			lastAbsorbDamageTick = Find.TickManager.TicksGame;
		}

		private void Break()
		{
			SoundDefOf.EnergyShield_Broken.PlayOneShot(new TargetInfo(Pawn.Position, Pawn.Map));
			MoteMaker.MakeStaticMote(Pawn.TrueCenter(), Pawn.Map, ThingDefOf.Mote_ExplosionFlash, 12f);
			for (int i = 0; i < 6; i++)
			{
				MoteMaker.ThrowDustPuff(Pawn.TrueCenter() + Vector3Utility.HorizontalVectorFromAngle(Rand.Range(0, 360)) * Rand.Range(0.3f, 0.6f), Pawn.Map, Rand.Range(0.8f, 1.2f));
			}
			Energy = 0f;
		}

		public void DrawShield(Vector3 drawPos)
		{
			if (ShouldDisplay)
			{
				float num = Mathf.Lerp(1.2f, 1.55f, Energy);
				drawPos.y = AltitudeLayer.MoteOverhead.AltitudeFor();
				int num2 = Find.TickManager.TicksGame - lastAbsorbDamageTick;
				if (num2 < 8)
				{
					float num3 = (float)(8 - num2) / 8f * 0.05f;
					drawPos += impactAngleVect * num3;
					num -= num3;
				}
				float angle = Rand.Range(0, 360);
				Vector3 s = new Vector3(num, 1f, num);
				Matrix4x4 matrix = default(Matrix4x4);
                
				matrix.SetTRS(drawPos, Quaternion.AngleAxis(angle, Vector3.up), s);
				Graphics.DrawMesh(MeshPool.plane10, matrix, BubbleMat, 0);
			}
		}


    }
}
