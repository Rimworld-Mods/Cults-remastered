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
                "Give ability", delegate
                {
                    pawn.abilities.GainAbility(CultsDefOf.Cults_Ability_PsionicBlast);
                    pawn.abilities.GainAbility(CultsDefOf.Cults_Ability_PsionicBurn);
                    pawn.abilities.GainAbility(CultsDefOf.Cults_Ability_WrathOfCthulhu);
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
		}

    }
}
