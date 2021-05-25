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

    public static class FloatingOptionsUtility
    { 
        //-------------------------------------------------
        // Deity

        public static void SelectDeity(Building_BaseAltar altar, Building_BaseAltar.CongregationParms parms)
        {
            // For congregation
            List<FloatMenuOption> options = new List<FloatMenuOption>();
            List<CosmicEntityDef> list = new List<CosmicEntityDef>();

            foreach (CosmicEntity candidate in CultKnowledge.deities)
            {
                if(candidate.isDiscovered) list.Add(candidate.def);
            }

            foreach(CosmicEntityDef thing in list)
            {
                options.Add(new FloatMenuOption(
                    thing.label, 
                    delegate 
                    { 
                        if(parms.deity != thing)
                        {
                            altar.setRewardForAllParms(null);
                        }
                        altar.setDeityForAllParms(thing);
                    }, 
                    thing.symbolTex, 
                    Color.white,
                    MenuOptionPriority.Default
                ));
            }

            if(options.NullOrEmpty()) return;
            Find.WindowStack.Add(new FloatMenu(options));
        }

        //-------------------------------------------------
        // Preacher

        public static void SelectPreacher(Building_BaseAltar altar, Building_BaseAltar.CongregationParms parms)
        {
            List<FloatMenuOption> options = new List<FloatMenuOption>();
            List<Pawn> list = new List<Pawn>();

            foreach (Pawn candidate in altar.Map.mapPawns.AllPawnsSpawned)
            {
                if (!candidate.IsColonist) continue;
                list.Add(candidate);
            }

            foreach(Pawn thing in list)
            {
                options.Add(new FloatMenuOption(
                    new String(' ', 8) + thing.Name.ToString(), 
                    delegate { altar.setPreacherForAllParms(thing); }, 
                    MenuOptionPriority.Default,
                    null,
                    null,
                    32,
                    (rect => {
                        rect.x = 0;
                        Widgets.ThingIcon(rect, thing);
                        return false;
                    })
                ));

                /*
                options.Add(new FloatMenuOption(
                    thing.Name.ToString(), 
                    delegate { parms.sacrifice = thing; }, 
                    MenuOptionPriority.Default
                ));
                */
            }

            if(options.NullOrEmpty()) return;
            Find.WindowStack.Add(new FloatMenu(options));
        }

        //-------------------------------------------------
        // Animal

        public static void SelectAnimalSacrifice(Building_BaseAltar altar, Building_BaseAltar.CongregationParms parms)
        {
            List<FloatMenuOption> options = new List<FloatMenuOption>();
            List<Pawn> list = new List<Pawn>();

            foreach (Pawn candidate in altar.Map.mapPawns.AllPawnsSpawned)
            {
                if (candidate.Faction != Faction.OfPlayer) continue;
                if (candidate.RaceProps == null) continue;
                if (candidate.RaceProps.Humanlike) continue;
                list.Add(candidate);
            }

            foreach(Pawn thing in list)
            {
                options.Add(new FloatMenuOption(
                    thing.Name.ToString(), 
                    delegate { parms.sacrifice = thing; }, 
                    thing.def,
                    MenuOptionPriority.Default
                ));
            }

            if(options.NullOrEmpty()) return;
            Find.WindowStack.Add(new FloatMenu(options));
        }

        //-------------------------------------------------
        // Human

        public static void SelectHumanSacrifice(Building_BaseAltar altar, Building_BaseAltar.CongregationParms parms)
        {
            List<FloatMenuOption> options = new List<FloatMenuOption>();
            List<Pawn> list = new List<Pawn>();

            foreach (Pawn candidate in altar.Map.mapPawns.AllPawnsSpawned)
            {
                if (!candidate.IsPrisonerOfColony) continue;
                list.Add(candidate);
            }

            foreach(Pawn thing in list)
            {
                
                options.Add(new FloatMenuOption(
                    new String(' ', 8) + thing.Name.ToString(), 
                    delegate { parms.sacrifice = thing; }, 
                    MenuOptionPriority.Default,
                    null,
                    null,
                    32,
                    (rect => {
                        rect.x = 0;
                        Widgets.ThingIcon(rect, thing);
                        return false;
                    })
                ));
            }

            if(options.NullOrEmpty()) return;
            Find.WindowStack.Add(new FloatMenu(options));
        }
        
        //-------------------------------------------------
        // Food

        public static void SelectFoodSacrifice(Building_BaseAltar altar, Building_BaseAltar.CongregationParms parms)
        {
            List<FloatMenuOption> options = new List<FloatMenuOption>();
            List<CongregationRecipeDef> recipes = DefDatabase<CongregationRecipeDef>.AllDefs.Where(r => r.requiredChoice == "Food").ToList();

            if(parms.preacher == null) return; // TODO: warning message?

            foreach (CongregationRecipeDef r in recipes)
            {
                List<ThingCount> chosen = new List<ThingCount>();
                if(IngredientFinder.TryFindBestBillIngredients(new Bill_Production(r), parms.preacher, altar, chosen))
                {
                    if(!chosen.NullOrEmpty()){
                        ThingDef def = chosen.First().Thing.def;
                        Texture2D tex;

                        if(def.graphic is Graphic_StackCount)
                        {
                            Graphic graphic = (def.graphic as Graphic_StackCount).SubGraphicForStackCount(r.isWorthy? def.stackLimit : 3, def);
                            tex = graphic.MatSingle.GetTexture("_MainTex") as Texture2D;
                        }
                        else
                        {
                            tex = def.graphic.MatSingle.GetTexture("_MainTex") as Texture2D;
                        }
                        
                        options.Add(new FloatMenuOption(
                            r.label, 
                            delegate {  }, // parms.sacrifice = thing;
                            tex,
                            new Color(1f, 1f, 1f, 1f),
                            MenuOptionPriority.Default
                        ));
                        
                    }
                };
            }

            if(options.NullOrEmpty())
            {
                Messages.Message("No suitable amount of food is available", null, MessageTypeDefOf.RejectInput);
                return;
            } 
            Find.WindowStack.Add(new FloatMenu(options));
        }

        //-------------------------------------------------
        // Artefact

        public static void SelectItemSacrifice(Building_BaseAltar altar, Building_BaseAltar.CongregationParms parms)
        {
            List<FloatMenuOption> options = new List<FloatMenuOption>();

            if(options.NullOrEmpty()) return;
            Find.WindowStack.Add(new FloatMenu(options));
        }


        //-------------------------------------------------
        // Reward

        public static void SelectRewardSpell(Building_BaseAltar altar, Building_BaseAltar.CongregationParms parms)
        {
            //Building_BaseAltar.Choice choice = ;
            List<FloatMenuOption> options = new List<FloatMenuOption>();
            List<SpellDef> list = new List<SpellDef>();

            if(parms.deity == null) return; // TODO: warning message?

            foreach (SpellDef def in parms.deity.spells)
            {
                if(def.hasChoice(altar.congregationChoice)) list.Add(def);
            }

            

            foreach(SpellDef thing in list)
            {
                options.Add(new FloatMenuOption(
                    thing.label,
                    delegate { parms.reward = thing; }, 
                    MenuOptionPriority.Default
                ));
            }

            if(options.NullOrEmpty()) return;
            Find.WindowStack.Add(new FloatMenu(options));
        }

    }
}