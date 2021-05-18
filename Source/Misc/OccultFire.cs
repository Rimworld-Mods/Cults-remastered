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

using System.Reflection;

namespace Cults
{
	//------------------------------------------------------------------------------------
    //------------------------------------------------------------------------------------
	// Same as [Rimworld.FireUtility]
	// OccultFireUtility starts purple fire

    public static class OccultFireUtility
    {
        public static float ChanceToStartFireIn(IntVec3 c, Map map)
        {
            List<Thing> thingList = c.GetThingList(map);
            float num = c.TerrainFlammableNow(map) ? c.GetTerrain(map).GetStatValueAbstract(StatDefOf.Flammability) : 0f;
            for (int i = 0; i < thingList.Count; i++)
            {
                Thing thing = thingList[i];
                if (thing is Fire)
                {
                    return 0f;
                }
                if (thing.def.category != ThingCategory.Pawn && thingList[i].FlammableNow)
                {
                    num = Mathf.Max(num, thing.GetStatValue(StatDefOf.Flammability));
                }
            }
            if (num > 0f)
            {
                Building edifice = c.GetEdifice(map);
                if (edifice != null && edifice.def.passability == Traversability.Impassable && edifice.OccupiedRect().ContractedBy(1).Contains(c))
                {
                    return 0f;
                }
                List<Thing> thingList2 = c.GetThingList(map);
                for (int j = 0; j < thingList2.Count; j++)
                {
                    if (thingList2[j].def.category == ThingCategory.Filth && !thingList2[j].def.filth.allowsFire)
                    {
                        return 0f;
                    }
                }
            }
            return num;
        }

        public static bool TryStartFireIn(IntVec3 c, Map map, float fireSize)
        {
            if (ChanceToStartFireIn(c, map) <= 0f)
            {
                return false;
            }
			OccultFire obj = (OccultFire)ThingMaker.MakeThing(ThingDefOf.Fire);
            obj.fireSize = fireSize;

			// Log.Message((obj is Fire).ToString());
			obj.occult = true; // make occult fire
            GenSpawn.Spawn(obj, c, map, Rot4.North);
            return true;
        }

        public static void TryAttachOccultFire(this Thing t, float fireSize)
        {
            if (t.CanEverAttachFire() && !t.HasAttachment(ThingDefOf.Fire)) // ThingDefOf.Fire
            {
                OccultFire obj = (OccultFire)ThingMaker.MakeThing(ThingDefOf.Fire);
                obj.fireSize = fireSize;
				obj.occult = true; // make occult fire
                obj.AttachTo(t);
                GenSpawn.Spawn(obj, t.Position, t.Map, Rot4.North);
                Pawn pawn = t as Pawn;
                if (pawn != null)
                {
                    pawn.jobs.StopAll();
                    pawn.records.Increment(RecordDefOf.TimesOnFire);
                }
            }
        }

    }

    //------------------------------------------------------------------------------------
    //------------------------------------------------------------------------------------
	// There's probably a better way to implement colored fire with comps, since this mod uses only 2 types of flame, it's good enough.
	// Since [OccultFire] has the same def and the same base class it doesn't create a weird behavior in other parts of vanilla code.

	public class OccultFire : RimWorld.Fire
	{
		public bool occult = false;

		//---------------------------
		// field from base class
		private static List<Thing> flammableList = new List<Thing>();
		private float flammabilityMax = 0.5f;
		private int ticksSinceSpawn = 0;
		private static int lastFireCountUpdateTick;
		private static int fireCount;
		private Sustainer sustainer;
		private int ticksSinceSpread;
		private int ticksUntilSmoke;
		private float SpreadInterval
		{
			get
			{
				float num = 150f - (fireSize - 1f) * 40f;
				if (num < 75f)
				{
					num = 75f;
				}
				return num;
			}
		}

		//---------------------------
		// Also change label
		public override string LabelNoCount => "occult " + GenLabel.ThingLabel(this, 1); // Doesn't work!

		// Make it harder to extinguish

		//---------------------------

		public override void Tick()
		{
			ticksSinceSpawn++;

			// Probably the best way is to use reflection and modify base class field [ticksSinceSpawn]
			// [ticksSinceSpawn] is used in [ExposeData] and [Rimworld.Verb_BeatFire]
			// Vanilla code passes fire as [Fire] class, therefore it's important to [ticksSinceSpawn] variable
            FieldInfo fieldInfo = typeof(Fire).GetField("ticksSinceSpawn", BindingFlags.Instance | BindingFlags.NonPublic);
			fieldInfo.SetValue(this as Fire, ticksSinceSpawn);
			
			if (lastFireCountUpdateTick != Find.TickManager.TicksGame)
			{
				fireCount = base.Map.listerThings.ThingsOfDef(def).Count;
				lastFireCountUpdateTick = Find.TickManager.TicksGame;
			}
			if (sustainer != null)
			{
				sustainer.Maintain();
			}
			else if (!base.Position.Fogged(base.Map))
			{
				SoundInfo info = SoundInfo.InMap(new TargetInfo(base.Position, base.Map), MaintenanceType.PerTick);
				sustainer = SustainerAggregatorUtility.AggregateOrSpawnSustainerFor(this, SoundDefOf.FireBurning, info);
			}
			ticksUntilSmoke--;
			if (ticksUntilSmoke <= 0)
			{
				//SpawnSmokeParticles();
			}
			if (fireCount < 15 && fireSize > 0.7f && Rand.Value < fireSize * 0.01f)
			{
				MoteMaker.ThrowMicroSparks(DrawPos, base.Map);
			}
			if (fireSize > 1f)
			{
				ticksSinceSpread++;
				if ((float)ticksSinceSpread >= SpreadInterval)
				{
					TrySpread();
					ticksSinceSpread = 0;
				}
			}
			if (this.IsHashIntervalTick(150))
			{
				DoComplexCalcs();
			}
			if (ticksSinceSpawn >= 7500)
			{
				TryBurnFloor();
			}
			
		}

		//---------------------------

		private void TryBurnFloor()
		{
			if (parent == null && base.Spawned && base.Position.TerrainFlammableNow(base.Map))
			{
				base.Map.terrainGrid.Notify_TerrainBurned(base.Position);
			}
		}

		//---------------------------

		private void DoComplexCalcs()
		{
			bool flag = false;
			flammableList.Clear();
			flammabilityMax = 0f;
			if (!base.Position.GetTerrain(base.Map).extinguishesFire)
			{
				if (parent == null)
				{
					if (base.Position.TerrainFlammableNow(base.Map))
					{
						flammabilityMax = base.Position.GetTerrain(base.Map).GetStatValueAbstract(StatDefOf.Flammability);
					}
					List<Thing> list = base.Map.thingGrid.ThingsListAt(base.Position);
					for (int i = 0; i < list.Count; i++)
					{
						Thing thing = list[i];
						if (thing is Building_Door)
						{
							flag = true;
						}
						float statValue = thing.GetStatValue(StatDefOf.Flammability);
						if (!(statValue < 0.01f))
						{
							flammableList.Add(list[i]);
							if (statValue > flammabilityMax)
							{
								flammabilityMax = statValue;
							}
							if (parent == null && fireSize > 0.4f && list[i].def.category == ThingCategory.Pawn)
							{
								if(occult)
								{
									list[i].TryAttachOccultFire(fireSize * 0.2f);
								}
								else
								{
									list[i].TryAttachFire(fireSize * 0.2f);
								}
								
							}
						}
					}
				}
				else
				{
					flammableList.Add(parent);
					flammabilityMax = parent.GetStatValue(StatDefOf.Flammability);
				}
			}
			if (flammabilityMax < 0.01f)
			{
				Destroy();
				return;
			}
			Thing thing2 = (parent != null) ? parent : ((flammableList.Count <= 0) ? null : flammableList.RandomElement());
			if (thing2 != null && (!(fireSize < 0.4f) || thing2 == parent || thing2.def.category != ThingCategory.Pawn))
			{
				DoFireDamage(thing2);
			}
			if (base.Spawned)
			{
				float num = fireSize * 160f;
				if (flag)
				{
					num *= 0.15f;
				}
				GenTemperature.PushHeat(base.Position, base.Map, num);
				if (Rand.Value < 0.4f)
				{
					float radius = fireSize * 3f;
					SnowUtility.AddSnowRadial(base.Position, base.Map, radius, 0f - fireSize * 0.1f);
				}
				fireSize += 0.00055f * flammabilityMax * 150f;
				if (fireSize > 1.75f)
				{
					fireSize = 1.75f;
				}
				if (base.Map.weatherManager.RainRate > 0.01f && VulnerableToRain() && Rand.Value < 6f)
				{
					TakeDamage(new DamageInfo(DamageDefOf.Extinguish, (occult? 7f : 10f))); // if occult reduce by double
				}
			}
		}

		//---------------------------

		private bool VulnerableToRain()
		{
			if (!base.Spawned)
			{
				return false;
			}
			RoofDef roofDef = base.Map.roofGrid.RoofAt(base.Position);
			if (roofDef == null)
			{
				return true;
			}
			if (roofDef.isThickRoof)
			{
				return false;
			}
			return base.Position.GetEdifice(base.Map)?.def.holdsRoof ?? false;
		}

		//---------------------------

		private void DoFireDamage(Thing targ)
		{
			int num = GenMath.RoundRandom(Mathf.Clamp(0.0125f + 0.0036f * fireSize, 0.0125f, 0.05f) * 150f);
			if (num < 1)
			{
				num = 1;
			}
			Pawn pawn = targ as Pawn;
			if (pawn != null)
			{
				BattleLogEntry_DamageTaken battleLogEntry_DamageTaken = new BattleLogEntry_DamageTaken(pawn, RulePackDefOf.DamageEvent_Fire);
				Find.BattleLog.Add(battleLogEntry_DamageTaken);
				DamageInfo dinfo = new DamageInfo(DamageDefOf.Flame, num, 0f, -1f, this);
				dinfo.SetBodyRegion(BodyPartHeight.Undefined, BodyPartDepth.Outside);
				targ.TakeDamage(dinfo).AssociateWithLog(battleLogEntry_DamageTaken);
				if (pawn.apparel != null && pawn.apparel.WornApparel.TryRandomElement(out Apparel result))
				{
					result.TakeDamage(new DamageInfo(DamageDefOf.Flame, num, 0f, -1f, this));
				}
			}
			else
			{
				targ.TakeDamage(new DamageInfo(DamageDefOf.Flame, num, 0f, -1f, this));
			}
		}

		//---------------------------

		private Graphic occultGraphicInt;
		public Graphic OccultGraphic
		{
			get
			{
				if (occultGraphicInt == null)
				{
					if (def.graphicData == null) return BaseContent.BadGraphic;
					GraphicData data = new GraphicData();
					data.CopyFrom(def.graphicData);
					data.texPath = "Fire/Thing";
					occultGraphicInt = data.GraphicColoredFor(this);
				}
				return occultGraphicInt;
			}
		}

		//---------------------------

		public override void DrawAt(Vector3 drawLoc, bool flip = false)
		{
			if(occult)
			{
				OccultGraphic.Draw(drawLoc, flip ? Rotation.Opposite : Rotation, this);
			}
			else
			{
				Graphic.Draw(drawLoc, flip ? Rotation.Opposite : Rotation, this);
			}
		}

		//---------------------------

		new protected void TrySpread()
        {
            IntVec3 position = base.Position;
            bool flag;
            if (Rand.Chance(0.8f))
            {
                position = base.Position + GenRadial.ManualRadialPattern[Rand.RangeInclusive(1, 8)];
                flag = true;
            }
            else
            {
                position = base.Position + GenRadial.ManualRadialPattern[Rand.RangeInclusive(10, 20)];
                flag = false;
            }
            if (!position.InBounds(base.Map) || !Rand.Chance(FireUtility.ChanceToStartFireIn(position, base.Map)))
            {
                return;
            }
            if (!flag)
            {
                CellRect startRect = CellRect.SingleCell(base.Position);
                CellRect endRect = CellRect.SingleCell(position);
                if (GenSight.LineOfSight(base.Position, position, base.Map, startRect, endRect))
                {
					if(occult)
					{
						((OccultSpark)GenSpawn.Spawn(CultsDefOf.Cults_OccultSpark, base.Position, base.Map)).Launch(this, position, position, ProjectileHitFlags.All);
					}
					else
					{
						((Spark)GenSpawn.Spawn(ThingDefOf.Spark, base.Position, base.Map)).Launch(this, position, position, ProjectileHitFlags.All);
					}
                    
                }
            }
            else
            {
				if(occult)
				{
					OccultFireUtility.TryStartFireIn(position, base.Map, 0.1f);
				}
				else
				{
					FireUtility.TryStartFireIn(position, base.Map, 0.1f);
				}
                
            }
        }

		//---------------------------

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref occult, "occult", false);
		}

		//---------------------------

	}

	//------------------------------------------------------------------------------------
    //------------------------------------------------------------------------------------
	// Projectile for spreading occult fire

	public class OccultSpark : Projectile
	{
		protected override void Impact(Thing hitThing)
		{
			Map map = base.Map;
			base.Impact(hitThing);
			OccultFireUtility.TryStartFireIn(base.Position, map, 0.1f);
		}
	}

}