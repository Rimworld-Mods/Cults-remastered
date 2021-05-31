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
    class MiasmaWorker : DamageWorker 
    {
		public override void ExplosionStart(Explosion explosion, List<IntVec3> cellsToAffect)
		{
			// if (def.explosionHeatEnergyPerCell > float.Epsilon)
			// {
			// 	GenTemperature.PushHeat(explosion.Position, explosion.Map, def.explosionHeatEnergyPerCell * (float)cellsToAffect.Count);
			// }
			// MoteMaker.MakeStaticMote(explosion.Position, explosion.Map, ThingDefOf.Mote_ExplosionFlash, explosion.radius * 6f);
			// if (explosion.Map == Find.CurrentMap)
			// {
			// 	float magnitude = (explosion.Position.ToVector3Shifted() - Find.Camera.transform.position).magnitude;
			// 	Find.CameraDriver.shaker.DoShake(4f * explosion.radius / magnitude);
			// }
			ExplosionVisualEffectCenter(explosion);
		}

        protected override void ExplosionVisualEffectCenter(Explosion explosion)
		{
			for (int i = 0; i < 6; i++)
			{
				this.ThrowSmoke(explosion.Position.ToVector3Shifted() + Gen.RandomHorizontalVector(explosion.radius * 0.7f), explosion.Map, explosion.radius * 0.4f);
			}
        }

        private void ThrowSmoke(Vector3 loc, Map map, float size) // from [MoteMaker.ThrowSmoke()]
        {
			if (loc.ShouldSpawnMotesAt(map) && !map.moteCounter.SaturatedLowPriority)
			{
				MoteThrown obj = (MoteThrown)ThingMaker.MakeThing(CultsDefOf.Cults_Mote_Miasma);
				obj.Scale = Rand.Range(1.5f, 2.5f) * size;
				obj.rotationRate = Rand.Range(-30f, 30f);
				obj.exactPosition = loc;
				obj.SetVelocity(Rand.Range(30, 40), Rand.Range(0.5f, 0.7f));
				GenSpawn.Spawn(obj, loc.ToIntVec3(), map);
			}
        }


    }

}