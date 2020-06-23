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

// https://github.com/roxxploxx/RimWorldModGuide/wiki

namespace Cults
{
    public class MyMapComponent : MapComponent
    {
        public MyMapComponent(Map map) : base(map)
        {
        }

        public override void FinalizeInit()
        {
            Messages.Message("Success", null, MessageTypeDefOf.PositiveEvent);
            Find.LetterStack.ReceiveLetter("Success", CultsDefOf.Cults_Letter.description, CultsDefOf.Cults_Letter, null);
            //TimeAssignmentSelector.selectedAssignment = CultsDefOf.TimeAssignment_Worship;
        }
    }

    [StaticConstructorOnStartup]
    public static class MyMod
    {
        static MyMod() //our constructor
        {
            Log.Message(ModsConfig.IsActive(ModContentPack.RoyaltyModPackageId).ToString());
            Log.Message("Cults mod success"); //Outputs "Hello World!" to the dev console.
        }
    }


    public class Building_StandardAltar : Building
    {
        public override void SpawnSetup(Map map, bool ral)
        {
            base.SpawnSetup(map, ral);
            Log.Message("Built altar " + ral.ToString() + " !");
            Log.Message("   " + CultsDefOf.Cults_StandardAltar.label);
        }

    }
}
