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
    class ITab_Sacrifice : ITab
    {
        
        public ITab_Sacrifice()
        {
            this.size = WorshipCardUtility.defaultSize;
            this.labelKey = "Sacrifice";
        }

        
        protected override void FillTab()
        {
            //this.UpdateSize();
            SacrificeCardUtility.DrawTab();
        }
    }

    public static class SacrificeCardUtility
    {
        public static Vector2 defaultSize = new Vector2(500f, 350f);
        private static float margin = 17f;
        private static float secondColumnX = 255;

        //-------------------------------------------------

        enum Tab { Offer, Sacrifice };
        enum Choice { Item, Food, Animal, Human };

        static Tab currentTab = Tab.Offer;
        static Choice currentChoice = Choice.Food;

        //-------------------------------------------------

        private static SacrificeUtility sacrificeHuman = new SacrificeUtility();
        private static SacrificeUtility sacrificeAnimal = new SacrificeUtility();
        private static OfferUtility offerItem = new OfferUtility();
        private static OfferUtility offerFood = new OfferUtility();

        //-------------------------------------------------
        // helper classes

        public static void DrawTab()
        {
            Rect rect = new Rect(0, 0, defaultSize.x, defaultSize.y);
            DrawCultLabel();
            DrawSelections();
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

        public static void DrawSelections()
		{
            // 70 + 30
            Rect rect = new Rect(0, 100f, 200f, defaultSize.y - 100f);
			GUI.color = Color.white;
            
            List<TabRecord> list = new List<TabRecord>();
            list.Add(new TabRecord("Offer", delegate{ currentTab = Tab.Offer; }, currentTab == Tab.Offer));
            list.Add(new TabRecord("Sacrifice", delegate{ currentTab = Tab.Sacrifice; }, currentTab == Tab.Sacrifice));

			Widgets.DrawMenuSection(rect);
			TabDrawer.DrawTabs(rect, list);
            rect = rect.ContractedBy(9f);
            GUI.BeginGroup(rect);

            if(currentTab == Tab.Offer)
            {
                if(currentChoice == Choice.Food)
                {
                    offerFood.Draw(rect);
                }
                else if(currentChoice == Choice.Item)
                {
                    offerItem.Draw(rect);
                }
                else // automatically select
                {

                }
            }
            else // currentTab = Tab.Sacrifice
            {
                if(currentChoice == Choice.Animal)
                {
                    sacrificeAnimal.Draw(rect);
                }
                else if(currentChoice == Choice.Human)
                {
                    sacrificeHuman.Draw(rect);
                }
                else // automatically select
                {

                }
            }

            Text.Anchor = TextAnchor.UpperLeft;
            GUI.EndGroup();
            
		}

        private static void DrawListSelector(string label, float pos, float width, string selection, Action action)
        {
            Rect rect1 = new Rect(0, pos, width/2, 30f).ContractedBy(4);
            Text.Anchor = TextAnchor.MiddleLeft;
            Widgets.Label(rect1, label);

            Rect rect2 = new Rect(width/2, pos, width/2, 30f);
            if (Widgets.ButtonText(rect2, selection , true, false, true))
            {
                action.Invoke();
            }
        }

        private static void DrawSelector(Rect rect, string label, Choice choice, Texture tex)
		{
			rect = rect.ContractedBy(2f);
			GUI.DrawTexture(rect, tex);
			if (Widgets.ButtonInvisible(rect))
			{   
                currentChoice = choice;    
				SoundDefOf.Tick_High.PlayOneShotOnCamera();
			}
			GUI.color = Color.white;
			if (Mouse.IsOver(rect))
			{
				Widgets.DrawHighlight(rect);
			}
			Text.Font = GameFont.Small;
			Text.Anchor = TextAnchor.MiddleCenter;
			GUI.color = Color.white;
			Widgets.Label(rect, label);
			Text.Anchor = TextAnchor.UpperLeft;
			if (currentChoice == choice)
			{
				Widgets.DrawBox(rect, 2);
			}
			else
			{
				UIHighlighter.HighlightOpportunity(rect, "?????");
			}
		}

        //-------------------------------------------------
        // Nested classes

        class SacrificeUtility
        {
            CosmicEntity selectedDeity;
            Pawn selectedExecutioner;
            Pawn selectedSacrifice;
            string selectedReward;

            public void Draw(Rect rect)
            {
                Action action = delegate{};
                
                Rect rect3 = new Rect(0, 40f, rect.width/2, 30f);
                DrawSelector(rect3, "Animal", Choice.Animal, Textures.AnimalColorTex);

                Rect rect4 = new Rect(rect.width/2, 40f, rect.width/2, 30f);
                DrawSelector(rect4, "Human", Choice.Human, Textures.HumanColorTex);
                
                string s;
                string error = currentChoice == Choice.Animal? "animal" : "human";
                s = selectedDeity == null? error: selectedDeity.def.label;
                DrawListSelector("Deity", 80, rect.width, s, action);
                s = selectedExecutioner == null? "-": selectedExecutioner.def.label;
                DrawListSelector("Executioner", 120, rect.width,s , action);
                s = selectedSacrifice == null? "-": selectedSacrifice.def.label;
                DrawListSelector("Sacrifice", 160, rect.width,s , action);
                s = selectedReward == null? "-": selectedReward;
                DrawListSelector("Reward", 200, rect.width,s , action);
            }

        }

        class OfferUtility
        {
            CosmicEntity selectedDeity;
            Pawn selectedOfferror;
            Thing selectedOffer;
            string selectedReward;

            public void Draw(Rect rect)
            {
                Action action = delegate{};
                
                Rect rect3 = new Rect(0, 40f, rect.width/2, 30f);
                DrawSelector(rect3, "Food", Choice.Food, Textures.AnimalColorTex);

                Rect rect4 = new Rect(rect.width/2, 40f, rect.width/2, 30f);
                DrawSelector(rect4, "Item", Choice.Item, Textures.HumanColorTex);
                
                string s;
                s = selectedDeity == null? "-": selectedDeity.def.label;
                DrawListSelector("Deity", 80, rect.width, s, action);
                s = selectedOfferror == null? "-": selectedOfferror.def.label;
                DrawListSelector("Offerror", 120, rect.width,s , action);
                s = selectedOffer == null? "-": selectedOffer.def.label;
                DrawListSelector("Offer", 160, rect.width,s , action);
                s = selectedReward == null? "-": selectedReward;
                DrawListSelector("Reward", 200, rect.width,s , action);
            }
        }

        //-------------------------------------------------

    }
}


/*

                Rect rect2 = new Rect(0, 0, rect.width, 30f);
                Text.Anchor = TextAnchor.MiddleCenter;
                Widgets.Label(rect2, "Hello there");
                Widgets.DrawBox(rect2, 1);

*/