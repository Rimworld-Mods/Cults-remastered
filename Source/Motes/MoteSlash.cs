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
    class MoteSlash : Mote
    {
        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            this.exactRotation = Rand.Value * 180;
            base.SpawnSetup(map, respawningAfterLoad);
        }

        public override void Tick()
        {
            this.exactScale += new Vector3(-0.2f, 0.0f, 0.4f);
            base.Tick();
        }
    }

}
