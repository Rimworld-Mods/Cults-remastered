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
    using Choice = Building_BaseAltar.Choice;

    class ITab_Sacrifice : ITab
    {
        
        public ITab_Sacrifice()
        {
            this.size = SacrificeCardUtility.defaultSize;
            this.labelKey = "Sacrifice";
        }
        
        protected override void FillTab()
        {
            SacrificeCardUtility.DrawTab();
        }
    }

    public static class SacrificeCardUtility
    {
        public static Vector2 defaultSize = new Vector2(540f, 380f);
        private static Vector2 scrollPos = new Vector2(0,0);

        //-------------------------------------------------

        enum Tab { Offer, Sacrifice };

        static Tab currentTab = Tab.Offer;
        static Choice currentChoice{
            get => (Find.Selector.SingleSelectedThing as Building_BaseAltar).congregationChoice;
            set => (Find.Selector.SingleSelectedThing as Building_BaseAltar).congregationChoice = value;
        }
        
        //Choice.Food;
        static Choice previousChoice = Choice.Animal;

        //-------------------------------------------------
        // helper classes

        public static void DrawTab()
        {
            Rect rect = new Rect(0, 0, defaultSize.x, defaultSize.y);
            DrawCultLabel();
            DrawSelections();
        }

        private static void Swap<T>(ref T x, ref T y)
        {
            T t = y;
            y = x;
            x = t;
        }

        public static void DrawCultLabel()
        {
            // Cult name label
            float margin = 17;
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
            Rect rect = new Rect(0, 100f, 260f, defaultSize.y - 100f);
			GUI.color = Color.white;
            
            List<TabRecord> list = new List<TabRecord>();
            list.Add(new TabRecord("Offer", delegate{ currentTab = Tab.Offer; }, currentTab == Tab.Offer));
            list.Add(new TabRecord("Sacrifice", delegate{ currentTab = Tab.Sacrifice; }, currentTab == Tab.Sacrifice));

			Widgets.DrawMenuSection(rect);
			TabDrawer.DrawTabs(rect, list);
            rect = rect.ContractedBy(9f);

            GUI.BeginGroup(rect);
            DrawRitualMenu(rect);
            GUI.EndGroup();

            Text.Font = GameFont.Medium;
            Text.Anchor = TextAnchor.UpperLeft;
            Widgets.Label(new Rect(rect.xMax + 20, 70, defaultSize.x - rect.xMax, 70), "Success factors");
            DrawEvaluationList(new Rect(rect.xMax + 5, 100, defaultSize.x - rect.xMax - 5, defaultSize.y - 100).ContractedBy(4), ref scrollPos);
		}

        private static void DrawListSelector(string label, float pos, float width, string selection, Action action)
        {
            float ratio = 0.4f;
            Rect rect1 = new Rect(0, pos, width*ratio, 30f).ContractedBy(4);
            Text.Anchor = TextAnchor.MiddleLeft;
            Widgets.Label(rect1, label);

            Rect rect2 = new Rect(width*ratio, pos, width*(1-ratio), 30f);
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

        private static void DrawSpellDescription(Vector2 size, float pos)
        {
            Rect rect2 = new Rect(0, pos, size.x, size.y - pos);//.ContractedBy(4);
            GUI.DrawTexture(rect2, Textures.blackColorTex);

            Rect rect3 = new Rect(0, pos, size.x, size.y - pos).ContractedBy(4);
            Text.Anchor = TextAnchor.MiddleCenter;
            Widgets.Label(rect3, "Make a sacrifice without any worldly gains.");
        }

        //-------------------------------------------------
        // Ritual options UI

        public static void DrawRitualMenu(Rect rect)
        {
            Building_BaseAltar altar = Find.Selector.SingleSelectedThing as Building_BaseAltar;
            Building_BaseAltar.CongregationParms parms = altar.congregationParms;

            Action action = delegate{};
            float num = 0;
            
            if(currentTab == Tab.Offer)
            {
                Rect rect3 = new Rect(0, num, rect.width/2, 30f);
                DrawSelector(rect3, "Food", Choice.Food, Textures.AnimalColorTex);

                Rect rect4 = new Rect(rect.width/2, num, rect.width/2, 30f);
                DrawSelector(rect4, "Item", Choice.Item, Textures.HumanColorTex);

                if(currentChoice == Choice.Animal || currentChoice == Choice.Human) Swap(ref altar.congregationChoice, ref previousChoice);  // auto-select
            }
            else
            {
                Rect rect3 = new Rect(0, num, rect.width/2, 30f);
                DrawSelector(rect3, "Animal", Choice.Animal, Textures.AnimalColorTex);

                Rect rect4 = new Rect(rect.width/2, num, rect.width/2, 30f);
                DrawSelector(rect4, "Human", Choice.Human, Textures.HumanColorTex);

                if(currentChoice == Choice.Food || currentChoice == Choice.Item) Swap(ref altar.congregationChoice, ref previousChoice);  // auto-select
            }

            string s;
            string t;
            num += 40;
            s = parms.deity == null? "-": parms.deity.label;
            DrawListSelector("Deity", num, rect.width, s, delegate{ FloatingOptionsUtility.SelectDeity(altar, parms); });
            num += 40;
            s = parms.preacher == null? "-": parms.preacher.Name.ToStringShort;
            t = currentTab == Tab.Offer? "Preacher" : "Executioner";
            DrawListSelector(t, num, rect.width, s, delegate{ FloatingOptionsUtility.SelectPreacher(altar, parms); });
            num += 40;
            s = parms.sacrifice == null? "-": parms.sacrifice is Pawn? (parms.sacrifice as Pawn).Name.ToStringShort : parms.sacrifice.Label;
            t = currentTab == Tab.Offer? "Offer" : "Sacrifice";
            DrawListSelector(t, num, rect.width, s, delegate
            { 
                if(currentChoice == Choice.Animal) FloatingOptionsUtility.SelectAnimalSacrifice(altar, parms);
                if(currentChoice == Choice.Human) FloatingOptionsUtility.SelectHumanSacrifice(altar, parms);
                if(currentChoice == Choice.Food) FloatingOptionsUtility.SelectFoodSacrifice(altar, parms);
                if(currentChoice == Choice.Item) FloatingOptionsUtility.SelectItemSacrifice(altar, parms);
            });
            num += 40;
            s = parms.reward == null? "-": parms.reward.label;
            DrawListSelector("Reward", num, rect.width, s, delegate{ FloatingOptionsUtility.SelectRewardSpell(altar, parms); });
            
            DrawSpellDescription(rect.size, num + 40);
        }

        //-------------------------------------------------
        // Option selector menus

       

        //-------------------------------------------------
        // Evalutaion



        private static void DrawEvaluationList(Rect container, ref Vector2 thoughtScrollPosition) // 
		{
            Building_BaseAltar altar = Find.Selector.SingleSelectedThing as Building_BaseAltar;
            List<Evaluation> evaluations = EvaluationManager.EvaluateAltar(altar);

			Text.Font = GameFont.Small;
			float height = (float)evaluations.Count * 24f;
			Widgets.BeginScrollView(container, ref thoughtScrollPosition, new Rect(0f, 0f, container.width - 16f, height));
			//Text.Anchor = TextAnchor.MiddleLeft;
			for (int i = 0; i < evaluations.Count; i++)
			{
				DrawEvaluation(new Rect(0f, 24*i, container.width - 16f, 20f), evaluations[i]);
			}
			Widgets.EndScrollView();
			Text.Anchor = TextAnchor.UpperLeft;
		}
        private static void DrawEvaluation(Rect rect, Evaluation evaluation)
		{
            if (Mouse.IsOver(rect))
            {
                Widgets.DrawHighlight(rect);
                TooltipHandler.TipRegion(rect, new TipSignal(evaluation.description, 7291));
            }

            Text.Anchor = TextAnchor.UpperRight;
            GUI.color = evaluation.color;
            Widgets.Label(new Rect(rect.x + rect.xMax - 36, rect.y, 32f, rect.height), evaluation.score.ToString("##0"));

            Text.Anchor = TextAnchor.MiddleLeft;
            GUI.color = Color.white;
            Widgets.Label(new Rect(rect.x + 10, rect.y, rect.xMax - 10, rect.height), evaluation.label.CapitalizeFirst());
		}
    }
}


/*

// [Widgets.ThingIcon] to generate graphics

Apparel hood +5
Apparel robes +10
Stars are right +20
Statues +5, +10, +15
Eclipse +5
Aurora +5

Reward tier -0, -10, -20, -50
Stars are wrong -50

Executioneer spirituality
Executioneer talking
Sacrifice health
Temple quality (wealth, space, beauty, impressivness)

[ThingMaker]
Thing obj = (Thing)Activator.CreateInstance(def.thingClass);
obj.def = def;
obj.SetStuffDirect(stuff);
obj.PostMake();
return obj;

https://rimworldwiki.com/wiki/Psycasts
*/