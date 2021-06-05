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
    class CompProperties_Altar : CompProperties {
        public CompProperties_Altar()
		{
			this.compClass = typeof(CompAltar);
		}
    }

    public class CompAltar : ThingComp
    {
        //private CompProperties_Altar Props => (CompProperties_Altar)this.props;

        public override void CompTick()
        {
            base.CompTick();
            Log.Message("altar is ticking");
        }

        public override void PostSpawnSetup(bool raf) // respawn after laod
        {
            Log.Message("spawn");
            //FieldInfo fieldInfo = typeof(ResearchProjectDef).GetField("researchMods", RSUUtil.universal);
        }


        public override string CompInspectStringExtra()
        {
            return "inspect text";
        }

        /*
        public override IEnumerable<Gizmo> CompGetGizmosExtra()
		{
            yield return new Command_Action
            {
                defaultLabel = "DEBUG: Reproduce",
                icon = ContentFinder<Texture2D>.Get("Commands/Altar/Upgrade2"),
                action = delegate
                {
                    this.parent.def = CultsDefOf.Cults_Building_BloodAltar;
                    this.parent.Notify_ColorChanged();
                    Log.Message("CLICK");
                    
                    
                }
            };
			yield break;
		}
        */

    }

}