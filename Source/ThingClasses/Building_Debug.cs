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
    class Building_Debug : Building
    {
        public override IEnumerable<FloatMenuOption> GetFloatMenuOptions(Pawn pawn)
		{
            yield return new FloatMenuOption(
                "Give abilities", delegate
                {
                    List<AbilityDef> abilities = DefDatabase<AbilityDef>.AllDefs.Where(def => def.abilityClass == typeof(OccultAbility)).ToList();
                    foreach(AbilityDef def in abilities){
                        pawn.abilities.GainAbility(def);
                    }
                }
            );

            yield return new FloatMenuOption(
                "Give Occultism XP", delegate
                {
                    pawn.skills.Learn(CultsDefOf.Cults_Skill_Occultism, 3500f);
                }
            );

            yield return new FloatMenuOption(
                "+0.1 residual energy", delegate
                {
                    ResidualEnergy energy = this.Map.GetComponent<ResidualEnergy>();
                    energy.Severity += 0.1f;
                }
            );

            yield return new FloatMenuOption(
               "give verb", delegate
                {
                    // Verb_Magic verb = new Verb_Magic();
                    // VerbProperties verbProps = CultsDefOf.Cults_Ability_PsionicBlast.verbProperties;
                    // verb.verbProps = verbProps;
                    // pawn.verbTracker.AllVerbs.Add(verb);

                    AbilityComp ability = new AbilityComp();
                    ability.parent = pawn;
                    pawn.AllComps.Add(ability);
                }
            );
    
		}
    }


    class Verb_Magic : Verb_CastBase 
    {
        protected override bool TryCastShot()
		{
			return true;
		}
    }


    class AbilityComp : ThingComp 
    {
        public override IEnumerable<Gizmo> CompGetGizmosExtra()
		{
			Command_Action gizmo = new Command_Action();

            gizmo.defaultLabel = "Upgrade";
            gizmo.defaultDesc = "This is desc";
            gizmo.disabled = false;
            gizmo.icon = ContentFinder<Texture2D>.Get("Commands/Altar/Upgrade2Disabled");
            gizmo.action = delegate
            {
                TargetingParameters parms = new TargetingParameters();
                parms.canTargetPawns = true;
                Find.Targeter.BeginTargeting(parms, delegate (LocalTargetInfo target){
                    Log.Message(target.Label);
                    Log.Message(this.parent?.Label ?? "no parent");
                    Log.Message(this.parent.Label);
                    
                    AbilityDef def = Cults.CultsDefOf.Cults_Ability_BreathOfTheSea;
                    
                    foreach(AbilityCompProperties comp in def.comps)
                    {
                        CompAbilityEffect instance = (CompAbilityEffect)Activator.CreateInstance(comp.compClass);
                        if(comp is CompProperties_AbilityGiveHediff)
                        {
                            CompProperties_AbilityGiveHediff Comp = (CompProperties_AbilityGiveHediff)comp;
                            Log.Message("parts: " + Comp.bodyPartCount.ToString());
                            instance.props = comp;
                            // instance.parent = ability_instance; // parent doesn't work
                            instance.Apply(target, this.parent);
                        }
                        // instance.props = comp.;
                        // comp.compClass
                    }

                    // CompAbilityEffect_BreathOfTheSea effect = new CompAbilityEffect_BreathOfTheSea();
                    // // effect.props = CultsDefOf.Cults_Ability_BreathOfTheSea
                    // effect.castMagic(target);
                });
            };
            yield return  gizmo;
		}
    }


}