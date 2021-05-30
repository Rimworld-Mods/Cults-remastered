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
using Verse.Grammar;
using RimWorld;            // RimWorld specific functions are found here (like 'Building_Battery')
using RimWorld.Planet;     // RimWorld specific functions for world creation

namespace Cults
{
    public class ThingMover : Thing, IThingHolder
	{
		private Effecter effecter;
        private ThingOwner<Thing> innerContainer;
		protected Vector3 startVec;
		protected float distance;
		protected float ticksPassed = 0;
		protected bool wasDrafted = false;
		protected bool wasSelected = false;

		


		protected Thing MovingThing 
		{
			get
			{
				if (innerContainer.InnerListForReading.Count <= 0) return null;
				return innerContainer.InnerListForReading[0] as Thing;
			}
		}

		protected float TravelTickTime
		{
			get
			{
				if(this.def.pawnFlyer.flightSpeed <= 0) return distance * 60 / 20;
				return distance * 60 / this.def.pawnFlyer.flightSpeed;
			}
		}

		public ThingMover()
		{
			innerContainer = new ThingOwner<Thing>(this);
		}

		public ThingOwner GetDirectlyHeldThings()
		{
			return innerContainer;
		}
		
		public void GetChildHolders(List<IThingHolder> outChildren)
		{
			ThingOwnerUtility.AppendThingHoldersFromThings(outChildren, GetDirectlyHeldThings());
		}

		public static ThingMover MakeMover(ThingDef def, Thing thing, IntVec3 destCell)
		{
			ThingMover thingMover = (ThingMover)ThingMaker.MakeThing(def);
			thingMover.startVec = thing.TrueCenter();
			thingMover.distance = thing.Position.DistanceTo(destCell);
			thingMover.wasSelected = Find.Selector.IsSelected(thing);
			thingMover.wasDrafted = thing is Pawn ? ((Pawn)thing).Drafted : false;

			Find.Selector.ShelveSelected(thing);
			thing.DeSpawn();
			if (!thingMover.innerContainer.TryAdd(thing))
			{
				Log.Error("Thing add error: " + thing.Label);
				thing.Destroy();
			}
			
			return thingMover;
		}


		public override void Tick()
		{
			this.ticksPassed++;
			this.innerContainer.ThingOwnerTick();
			if(ticksPassed >= this.TravelTickTime)
			{
				RespawnThing();
				Destroy();
			}
		}


		public override void DrawAt(Vector3 endVec, bool flip = false)
		{
			Vector3 travelDirection = (endVec - startVec).normalized;
			Vector3 drawLoc = startVec + travelDirection * (distance * this.ticksPassed / this.TravelTickTime);
			this.MovingThing.DrawAt(drawLoc, flip);
		}

		protected virtual void RespawnThing()
		{
			Thing thing = this.MovingThing;
			innerContainer.TryDrop_NewTmp(thing, base.Position, thing.MapHeld, ThingPlaceMode.Direct, out Thing _, null, null, playDropSound: false);
            if(thing is Pawn && this.wasDrafted)
            {
                Pawn pawn = (Pawn)thing; 
                pawn.drafter.Drafted = true;
            }
			if(this.wasSelected)
			{
				Find.Selector.Unshelve(thing, playSound: false);
			}
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Deep.Look(ref innerContainer, "innerContainer", this);
			Scribe_Values.Look(ref startVec, "startVec");
			Scribe_Values.Look(ref ticksPassed, "ticksFlightTime", 0);
			Scribe_Values.Look(ref distance, "flightDistance", 0f);
			Scribe_Values.Look(ref wasDrafted, "wasDrafted", defaultValue: false);
			Scribe_Values.Look(ref wasSelected, "wasSelected", defaultValue: false);
		}

		public override void Destroy(DestroyMode mode = DestroyMode.Vanish)
		{
			effecter?.Cleanup();
			base.Destroy(mode);
		}

	}
}