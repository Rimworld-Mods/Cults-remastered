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

using HarmonyLib;

namespace Cults
{
    [HarmonyPatch(typeof(ResearchManager), "FinishProject")]
    public class ExtraFinishProjectFunction
    {
        // on research finish
        private static void Prefix(ref ResearchProjectDef proj) // on research finish "event"
        {
            CultKnowledge.DiscoverRandomDeity();
        }
    }


    [HarmonyPatch(typeof(Pawn_TimetableTracker), "CurrentAssignment", MethodType.Getter)]
    public class WorshipAssignment
    {
        private static TimeAssignmentDef Postfix(TimeAssignmentDef __result) 
        {
            // custom [TimeAssignment] def breaks vanilla pawn AI functions, do not return it
            return (__result == CultsDefOf.Cults_TimeAssignment_Worship)? TimeAssignmentDefOf.Anything : __result;
        }
    }


    [HarmonyPatch(typeof(InspectPaneFiller), "DrawTimetableSetting")]
    public class FixTimeAssignmentInspectPane
    {
        private static bool Prefix(ref WidgetRow row, ref Pawn pawn)
        {
            // override [InspectPanelFiller] UI current (schedule) assignment draw function
            TimeAssignmentDef realDef = pawn.timetable.times[GenLocalDate.HourOfDay(pawn)];

            if(realDef == CultsDefOf.Cults_TimeAssignment_Worship)
            {
                row.Gap(6f);
                row.FillableBar(93f, 16f, 1f, realDef.LabelCap, realDef.ColorTexture);
                return false; // skip original method
            }
            else
            {
                return true; // normal behavior
            }
        }
    }
    

    [HarmonyPatch(typeof(Verb_BeatFire), "TryCastShot")] 
    public class OccultFireBeating
    {
        // reduce occult fire taken damage value
		private static bool Prefix(LocalTargetInfo ___currentTarget, Verb_BeatFire __instance, ref bool __result)
		{
            OccultFire fire = (OccultFire)___currentTarget.Thing;
            Pawn casterPawn = __instance.CasterPawn;
            if (casterPawn.stances.FullBodyBusy || fire.TicksSinceSpawn == 0)
            {
                __result = false;
            }
            fire.TakeDamage(new DamageInfo(DamageDefOf.Extinguish, (fire.occult? 24f : 32f) , 0f, -1f, __instance.caster));
            casterPawn.Drawer.Notify_MeleeAttackOn(fire);

            __result = true;
            return false;
		}
    }


    [HarmonyPatch(typeof(SkillUI), "DrawSkill", new Type[]{ typeof(SkillRecord), typeof(Rect), typeof(SkillUI.SkillDrawMode), typeof(string) } )] 
    public class HideOccultSkill
    {
		private static bool Prefix(SkillRecord skill, Rect holdingRect)
		{
            if(skill.def == CultsDefOf.Cults_Skill_Occultism){
                if(skill.levelInt == 0){
                    return false; // not a cultist, skip
                }else{
                    GUI.BeginGroup(holdingRect);
                    float levelLabelWidth = -1f;
                    List<SkillDef> allDefsListForReading = DefDatabase<SkillDef>.AllDefsListForReading;
                    for (int i = 0; i < allDefsListForReading.Count; i++)
                    {
                        float x = Text.CalcSize(allDefsListForReading[i].skillLabel.CapitalizeFirst()).x;
                        if (x > levelLabelWidth)
                        {
                            levelLabelWidth = x;
                        }
			        }
                    Rect rect = new Rect(6f, 0f, levelLabelWidth + 6f, holdingRect.height);
                    Rect position = new Rect(rect.xMax, 0f, 24f, 24f);
                    Texture2D image = ContentFinder<Texture2D>.Get("UI/Icons/Medical/Bleeding");
                    GUI.DrawTexture(position, image);
                    GUI.EndGroup();
                    return true;
                }
            }
            return true; // not Occultism, skip
		}
    }


    [HarmonyPatch(typeof(PawnGenerator), "FinalLevelOfSkill")] // TODO: maybe add postfix to [PawnGenerator.GenerateSkills] and change pawn [Occultism] skill and passion
    public class DoNotGenerateOccultism 
    {
		private static bool Prefix(Pawn pawn, SkillDef sk, ref int __result)
		{
            if(sk == CultsDefOf.Cults_Skill_Occultism){
                __result = 0; // New pawns always have [Occultism] level 0
                return false;
            }else{
                return true;
            }
		}
    }


    [HarmonyPatch(typeof(Pawn), "DrawAt")]
    public class DrawPawnExtras
    {
		private static bool Prefix(Pawn __instance, Vector3 drawLoc, bool flip = false)
		{
            CompSpectralShield comp = __instance?.GetComp<CompSpectralShield>();
            comp?.DrawShield(drawLoc);
            return true;
		}
        
    }


    // PawnRenderer (if pawn has hediff, then render shield) 
    // Pawn. void DrawAt(Vector3 drawLoc, bool flip = false)

    // Thing class (if pawn has hediff, remove damage)
    // Pawn.public override void PreApplyDamage(ref DamageInfo dinfo, out bool absorbed)


        // public static void DrawAt_PostFix(Pawn_DrawTracker __instance, Vector3 loc)
        // {
        //     Pawn pawn = (Pawn) AccessTools.Field(typeof(Pawn_DrawTracker), "pawn").GetValue(__instance);
        //     if (pawn?.GetComp<CompTransmogrified>() is CompTransmogrified compTrans && compTrans.IsTransmogrified &&
        //         pawn.Spawned)
        //     {
        //         Material matSingle;
        //         matSingle = CultsDefOf.Cults_TransmogAura.graphicData.Graphic.MatSingle;

        //         float angle = 0f;
        //         angle = pawn.Rotation.AsAngle + (compTrans.Hediff.UndulationTicks * 100);

        //         float xCap = pawn.kindDef.lifeStages[0].bodyGraphicData.drawSize.x + 0.5f;
        //         float zCap = pawn.kindDef.lifeStages[0].bodyGraphicData.drawSize.y + 0.5f;

        //         float x = pawn.kindDef.lifeStages[0].bodyGraphicData.drawSize.x;
        //         float z = pawn.kindDef.lifeStages[0].bodyGraphicData.drawSize.y;
        //         float drawX = Mathf.Clamp((x + compTrans.Hediff.UndulationTicks) * compTrans.Hediff.graphicDiv, 0.01f,
        //             xCap);
        //         float drawY = Altitudes.AltitudeFor(AltitudeLayer.Terrain);
        //         float drawZ = Mathf.Clamp((z + compTrans.Hediff.UndulationTicks) * compTrans.Hediff.graphicDiv, 0.01f,
        //             zCap);
        //         Vector3 s = new Vector3(drawX, drawY, drawZ);
        //         Matrix4x4 matrix = default(Matrix4x4);
        //         matrix.SetTRS(loc, Quaternion.AngleAxis(angle, Vector3.up), s);
        //         Graphics.DrawMesh(MeshPool.plane10Back, matrix, matSingle, 0);
        //     }
        // }


        // thing comp
        //         public override void PostExposeData()
        // {
        //     base.PostExposeData();
        //     Scribe_Values.Look<bool>(ref this.isTransmogrified, "isTransmogrified", false);
        //     if (Scribe.mode == LoadSaveMode.PostLoadInit)
        //     {
        //         MakeHediff();
        //     }
        // }

        // hediff
        //         <addedPartProps>
        
        //   <solid>true</solid>
        //   <partEfficiency>1.1</partEfficiency>
        // </addedPartProps>
}