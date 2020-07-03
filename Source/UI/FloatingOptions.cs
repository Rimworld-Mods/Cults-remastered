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
    class FloatingMenus
    {
        public static void DeityMenu()
        {
            List<FloatMenuOption> list = new List<FloatMenuOption>();
            list.Add(new FloatMenuOption("NONE", delegate
            {
                CultKnowledge.selectedDeity = null; // deselect deity
            }, MenuOptionPriority.Default, null, null, 0f, null));

            if(CultKnowledge.deities != null){
                foreach (CosmicEntity deity in CultKnowledge.deities)
                {
                    Action action = delegate // select deity
                    {
                        CultKnowledge.selectedDeity = deity.def;
                    };

                    FloatMenuOption option = new FloatMenuOption(
                        deity.def.label, 
                        action, 
                        deity.def.symbolTex, 
                        new Color(1f, 0f, 0f, 1f), 
                        MenuOptionPriority.Default
                    );

                    list.Add(option);
                }
            }
            Find.WindowStack.Add(new FloatMenu(list));
        }

        public static void ColonistMenu(ref Pawn pawn)
        {   
            Pawn p = pawn;
            List<FloatMenuOption> list = new List<FloatMenuOption>();
            list.Add(new FloatMenuOption("NONE", delegate
            {
                p = null;
                //SacrificeCardUtility.sacrificeHuman.selectedExecutioner = null;
            }, MenuOptionPriority.Default, null, null, 0f, null));

            if(CultKnowledge.deities != null){
                foreach (CosmicEntity deity in CultKnowledge.deities)
                {
                    Action action = delegate // select deity
                    {
                        CultKnowledge.selectedDeity = deity.def;
                    };

                    FloatMenuOption option = new FloatMenuOption(
                        deity.def.label, 
                        action, 
                        deity.def.symbolTex, 
                        new Color(1f, 0f, 0f, 1f), 
                        MenuOptionPriority.Default
                    );

                    list.Add(option);
                }
            }
            Find.WindowStack.Add(new FloatMenu(list));
        }





    }
}