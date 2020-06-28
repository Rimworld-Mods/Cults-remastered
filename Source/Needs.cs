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

// TODO: rename to cult affinity
namespace Cults{
    public class DivineAffinity : Need{

        public const float BaseGainPerTickRate = 150f;
        public const float BaseFallPerTick = 1E-05f;
        public const float ThreshVeryLow = 0.1f;
        public const float ThreshLow = 0.3f;
        public const float ThreshSatisfied = 0.5f;
        public const float ThreshHigh = 0.7f;
        public const float ThreshVeryHigh = 0.9f;

        //private bool baseSet = false;
        //public int ticksUntilBaseSet = 500;
        //private int lastGainTick = 0;

        static DivineAffinity() {

        }

        public DivineAffinity(Pawn pawn) : base(pawn)
        {
            this.threshPercents = new List<float>();
            this.threshPercents.Add(ThreshLow);
            this.threshPercents.Add(ThreshHigh);
        }

        public override void SetInitialLevel()
        {
            this.CurLevel = ThreshSatisfied;
        }

        public override void NeedInterval(){
            //Log.Message("Need Interval");
        }
    }
}