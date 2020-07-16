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


// dialog // Find.WindowStack.Add(new Dialog_NamePawn(pawn));
/*

Select one of the discovered deities to star worshipping
You'll be rewarded after passing favor threshold, but be aware with rewards danger might come.
If 3 or more colonist worshipping at the same time, one of them might perform sermon, witch gives favor bonus.
New cosmic being can be discovered while worshipping.


//with rewards danger might come.
too much worhipping might hurt you colony's sanity

perform sermons
worship if idle

*/


namespace Cults
{

    class ITab_Worship : ITab
    {

        public ITab_Worship()
        {
            this.size = WorshipCardUtility.defaultSize;
            this.labelKey = "Worship";  // used to get the Tab's label
        }

        protected override void FillTab()
        {
            this.UpdateSize();
            WorshipCardUtility.DrawTab();
        }

        protected override void UpdateSize()
        {   
            if(CultKnowledge.deities == null) return;
            base.UpdateSize();
            
            float required_space = 70f + (CultKnowledge.deities.Count * 50f) + 30f;
            float max_height = 600f;

            this.size = new Vector2(
                WorshipCardUtility.defaultSize.x, 
                Mathf.Clamp(required_space, WorshipCardUtility.defaultSize.y, max_height)
            );
        }
    }

    //-----------------------------------------------------------------------------------------
    //-----------------------------------------------------------------------------------------

    public static class WorshipCardUtility
    {
        public static Vector2 defaultSize = new Vector2(500f, 350f);
        private static float margin = 17f;
        private static float secondColumnX = 255;

        public static void DrawTab()
        {
            Rect rect = new Rect(0, 0, defaultSize.x, defaultSize.y);
            DrawCultLabel();
            DrawProgressBars();
            DrawOptions();
            DrawDeityInfo();
        }

        public static void DrawCultLabel()
        {
            // Cult name label
            Rect rect = new Rect(margin, margin, 300f, 30f);
            string cult_title = Cults.CultKnowledge.GetCultName();
            Text.Font = GameFont.Medium;
            Widgets.Label(rect, cult_title);

            // Cult rename button
            rect = new Rect(defaultSize.x - 85f - margin, margin, 30f, 30f);
            if (Widgets.ButtonImage(rect, TexButton.Rename))
            {
                Find.WindowStack.Add(new Dialog_NamePlayerCult());
            }
        }

        public static void DrawProgressBars()
        {
            if(CultKnowledge.deities == null || CultKnowledge.deities.Count == 0) return;
            
            int i = 0;
            foreach(CosmicEntity deity in CultKnowledge.deities){
                if(deity.isDiscovered){
                    Rect rect = new Rect(0, 70f + (i * 50f), 225f, 50f);
                    ProgressBar bar = new ProgressBar(deity);
                    bar.Draw(rect);
                    i++;
                }
            }

            if(i == 0)
            {
                Rect rect = new Rect(margin, 70f, 225f, 50f);
                Text.Font = GameFont.Small;
                Widgets.Label(rect, "No deities are discovered!");
            }
        }

        public static void DrawOptions()
        {
            Building_BaseAltar selected = Find.Selector.SingleSelectedThing as Building_BaseAltar;
            Rect rect;
            
            rect = new Rect(secondColumnX, 86f, 75f, 25f);
            Text.Font = GameFont.Small;
            Widgets.Label(rect, "Worship");

            rect = new Rect(secondColumnX + 60f, 85f, 120f, 25f);

            string text = CultKnowledge.selectedDeity == null? "-" : CultKnowledge.selectedDeity.label;
            if (Widgets.ButtonText(rect, text, true, false, true))
            {
                OpenDeitySelectMenu(selected);
            }


            rect = new Rect(255f, 100f, 180f, 40f);
            Widgets.CheckboxLabeled(rect.BottomHalf(), "Perform sermons", ref CultKnowledge.performSermons, false);

        }

        public static void DrawDeityInfo()
        {

            if(CultKnowledge.selectedDeity != null)
            {
                string name = CultKnowledge.selectedDeity.label;
                string titles = "";
                string info = CultKnowledge.selectedDeity.description;

                Rect rect;
                float length = WorshipCardUtility.defaultSize.x - secondColumnX - margin;
                float height = WorshipCardUtility.defaultSize.y - 260f - margin;

                rect = new Rect(secondColumnX, 180f, length, 30f);
                Text.Font = GameFont.Medium;
                Widgets.Label(rect, name);

                rect = new Rect(secondColumnX, 210f, length, 120f);
                Text.Font = GameFont.Small;
                foreach(string t in CultKnowledge.selectedDeity.titles) titles += t + "\n";
                Widgets.Label(rect, titles);

                /*
                rect = new Rect(secondColumnX, 260f, length, height);
                Widgets.Label(rect, info);
                Widgets.DrawBox(rect,1);
                */
            }else{
                float length = WorshipCardUtility.defaultSize.x - secondColumnX - margin;
                string info = "Select the Deity and schedule time to start worship. \n\n"
                + "If 3 or more colonists worship at the same time, one of them might perform sermon, whitch gives favor bonus.";
            
                Rect rect = new Rect(secondColumnX, 180f, length, 120f);
                Text.Font = GameFont.Small;
                Widgets.Label(rect, info);
                //Widgets.DrawBox(rect,1);
            }


        }

        public static void OpenDeitySelectMenu(Building_BaseAltar altar)
        {            
            List<FloatMenuOption> options = new List<FloatMenuOption>();
            List<CosmicEntityDef> list = new List<CosmicEntityDef>();

            foreach (CosmicEntity candidate in CultKnowledge.deities)
            {
                if(candidate.isDiscovered) list.Add(candidate.def);
            }

            // null option
            options.Add(new FloatMenuOption("NONE", delegate
            {
                CultKnowledge.selectedDeity = null; // deselect deity

            }, MenuOptionPriority.Default, null, null, 0f, null));

            // deity options
            foreach(CosmicEntityDef thing in list)
            {
                options.Add(new FloatMenuOption(
                    thing.label, 
                    delegate { CultKnowledge.selectedDeity = thing; }, 
                    thing.symbolTex, 
                    new Color(1f, 0f, 0f, 1f), 
                    MenuOptionPriority.Default
                ));
            }

            Find.WindowStack.Add(new FloatMenu(options));
        }
    }


    //-----------------------------------------------------------------------------------------
    //-----------------------------------------------------------------------------------------

    public class ProgressBar
    {
        private CosmicEntity deity;
        public string label => deity.def.label;
        public float max_level => deity.def.maxFavor;
        public float current_level => deity.currentFavor;
        public float current_level_percentage
        {
            get
            {
                return current_level / max_level;
            }
        }

        public ProgressBar(CosmicEntity deity)
        {
            this.deity = deity;
        }

        public void Draw(Rect rect)
        {
            if (rect.height > 70f)
            {
                float num = (rect.height - 70f) / 2f;
                rect.height = 70f;
                rect.y += num;
            }

            if (Mouse.IsOver(rect))
            {
                Widgets.DrawHighlight(rect);
                TooltipHandler.TipRegion(rect, new TipSignal(() => "Tooltip string", rect.GetHashCode()));
            }

            float num2 = 14f;
            float num3 = num2 + 15f; //(customMargin >= 0f) ? customMargin : (num2 + 15f);
            if (rect.height < 50f)
            {
                num2 *= Mathf.InverseLerp(0f, 50f, rect.height);
            }
            Text.Font = GameFont.Small; //((rect.height > 55f) ? GameFont.Small : GameFont.Tiny);
            Text.Anchor = TextAnchor.LowerLeft;
            Widgets.Label(new Rect(rect.x + num3 + rect.width * 0.1f, rect.y, rect.width - num3 - rect.width * 0.1f, rect.height / 2f), this.label);
            Text.Anchor = TextAnchor.UpperLeft;
            Rect rect2 = new Rect(rect.x, rect.y + rect.height / 2f, rect.width, rect.height / 2f);
            rect2 = new Rect(rect2.x + num3, rect2.y, rect2.width - num3 * 2f, rect2.height - num2);
            Rect rect3 = rect2;
            /*
            float num4 = 1f;
            if (max_level < 1f) num4 = max_level;
            rect3.width *= num4;
            */

            Rect barRect = Widgets.FillableBar(rect3, Mathf.Clamp(current_level_percentage, 0f, 1f), Textures.RedColorTex);

            if(current_level > max_level) 
            {
                Rect rect4 = new Rect(rect3.x, rect3.y, rect3.width  * (current_level_percentage-1), rect3.height);
                GUI.DrawTexture(rect4, Textures.PurpleColorTex);
            }

            //if(current_level_percentage >)




            // arrows
            Widgets.FillableBarChangeArrows(rect3, CultKnowledge.selectedDeity == this.deity.def? 1 : 0);

            for (int i = 0; i < deity.def.favorThresholds.Count; i++)
            {
                DrawBarThreshold(barRect, deity.def.favorThresholds[i] * 1f);
            }

            Text.Font = GameFont.Small;
        }

        private void DrawBarThreshold(Rect barRect, float threshPct)
        {
            if(threshPct == 0f || threshPct == 1f) return;
            float num = (float)((barRect.width <= 60f) ? 1 : 2);
            Rect position = new Rect(barRect.x + barRect.width * threshPct - (num - 1f), barRect.y + barRect.height / 2f, num, barRect.height / 2f);
            Texture2D image;
            if (threshPct < this.current_level_percentage)
            {
                image = BaseContent.BlackTex;
                GUI.color = new Color(1f, 1f, 1f, 0.9f);
            }
            else
            {
                image = BaseContent.GreyTex;
                GUI.color = new Color(1f, 1f, 1f, 0.5f);
            }
            GUI.DrawTexture(position, image);
            GUI.color = Color.white;
        }
    }

    //-----------------------------------------------------------------------------------------
    //-----------------------------------------------------------------------------------------



}