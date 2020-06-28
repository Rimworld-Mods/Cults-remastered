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
    class CompProperties_OccultResearch : CompProperties {

        public string item = null;
        public int amount = 0;
        public CompProperties_OccultResearch()
		{
			this.compClass = typeof(CompSpawnerWombs);
		}
    }

    public class CompSpawnerWombs : CompForbiddable //ThingComp
    {
        public string message = "";
        private CompProperties_OccultResearch Props => (CompProperties_OccultResearch)this.props;

        public override void PostSpawnSetup(bool raf) // respawn after laod
        {
            Log.Message("spawn");
            //FieldInfo fieldInfo = typeof(ResearchProjectDef).GetField("researchMods", RSUUtil.universal);
        }

        public override void PostDraw()
        {
        }
        public override void PostExposeData()
		{
		}

        public override void CompTick()
        {
        }

        public override string CompInspectStringExtra()
        {
            string text = "Harmony: " + message;
            return text;
        }

        private bool f = false;
        public override IEnumerable<Gizmo> CompGetGizmosExtra()
		{
            yield return new Command_Action
            {
                defaultLabel = "DEBUG: Reproduce",
                icon = TexCommand.Draft,
                action = delegate
                {
                    if(f){
                        f = false;
                        parent.SetForbidden(false);
                    }else{
                        f = true;
                        parent.SetForbidden(true);
                    }
                    Log.Message("CLICK");
                    
                    
                }
            };
			yield break;
		}

    }

}