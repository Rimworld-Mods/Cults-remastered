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

// moved to GameCondition

namespace Cults
{
    // public class Alert_ResidualEnergy : Alert
	// {            
	// 	public override string GetLabel()
	// 	{
    //         return "High residual energy";
    //     }

	// 	public override TaggedString GetExplanation()
	// 	{
    //         return "Residual energy is very high";
	// 	}

	// 	public override AlertReport GetReport()
	// 	{        
    //         List<Map> maps = Find.Maps;

	// 		for (int i = 0; i < maps.Count; i++)
	// 		{   
    //             ResidualEnergy energy = maps[i].GetComponent<ResidualEnergy>();
    //             if(energy.Severity > 0.1){
    //                 return true;
    //             }

	// 		}
	// 		return false;
	// 	}
	// }
}