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

    public class Building_BaseAltar : Building
    {
        //------------------------------------------------------------------------------
        enum Level { Standard = 0, Sacrificial = 1, Blood = 2, Nightmare = 3, None = 4 }

        private Level lvl
        {
            get
            {
                if(this.def == CultsDefOf.Cults_Building_StandardAltar) return Level.Standard;
                if(this.def == CultsDefOf.Cults_Building_SacrificialAltar) return Level.Sacrificial;
                if(this.def == CultsDefOf.Cults_Building_BloodAltar) return Level.Blood;
                if(this.def == CultsDefOf.Cults_Building_NightmareAltar) return Level.Nightmare;
                return Level.None;
            }
        }

        //------------------------------------------------------------------------------

        private bool canUpgrade
        {
            get
            {
                if(lvl == Level.Standard && Cults.CultsDefOf.Cults_ForbiddenAltarII.IsFinished) return true;
                if(lvl == Level.Sacrificial && Cults.CultsDefOf.Cults_ForbiddenAltarIII.IsFinished) return true;
                return false;  
            }
        }

        //------------------------------------------------------------------------------
        // Gizmos

        public override IEnumerable<Gizmo> GetGizmos()
        {
            // get default gizmos:
            foreach (Gizmo g in base.GetGizmos()) yield return g;

            if(lvl != Level.Blood)
            {
                // additional gizmos:
                Command_Action gizmo = new Command_Action();

                gizmo.defaultLabel = "Upgrade";
                gizmo.defaultDesc = "This is desc";
                gizmo.disabled = !canUpgrade || lvl == Level.Nightmare;
                gizmo.action = delegate
                {
                    if(lvl == Level.Standard)
                    {
                        this.def = CultsDefOf.Cults_Building_SacrificialAltar;
                        this.Notify_ColorChanged(); // [Notify_ColorChanged] changes texture
                        return;
                    }

                    if(lvl == Level.Sacrificial)
                    {
                        this.def = CultsDefOf.Cults_Building_BloodAltar;
                        this.Notify_ColorChanged();
                        return;
                    }
                };

                if(lvl == Level.Standard)    gizmo.icon = ContentFinder<Texture2D>.Get(canUpgrade?"Commands/Altar/Upgrade2":"Commands/Altar/Upgrade2Disabled");
                if(lvl == Level.Sacrificial) gizmo.icon = ContentFinder<Texture2D>.Get(canUpgrade?"Commands/Altar/Upgrade3":"Commands/Altar/Upgrade3Disabled");
                
                yield return gizmo;
            }

            yield break;
  
        }

        //------------------------------------------------------------------------------
        // Other

        public override IEnumerable<FloatMenuOption> GetFloatMenuOptions(Pawn pawn)
		{
            FloatMenuOption option = new FloatMenuOption("Give something", null);
            option.action = delegate {
                Log.Message("Hohohoh");
            };
			yield return option;
		}




    }





    /*
    public class Building_StandardAltar : Building_BaseAltar
    {
        public override void SpawnSetup(Map map, bool ral)
        {
            Log.Message("spawnd: " + this.def.defName);
            base.SpawnSetup(map, ral);
        }
    }

    public class Building_AnimalAltar : Building_BaseAltar
    {
        public override void SpawnSetup(Map map, bool ral)
        {
            Log.Message("spawnd: " + this.def.defName);
            base.SpawnSetup(map, ral);
        }
    }

    public class Building_HumanAltar : Building_BaseAltar
    {
        public override void SpawnSetup(Map map, bool ral)
        {
            Log.Message("spawnd: " + this.def.defName);
            base.SpawnSetup(map, ral);
        }
    }
    */

}
