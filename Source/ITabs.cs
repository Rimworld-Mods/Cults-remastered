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

namespace Cults{

    class ITab_Worship : ITab{

        public ITab_Worship()
        {
            this.size = new Vector2(480f, 440f);
            this.labelKey = "Worship";  // used to get the Tab's label
        }

        private void DrawCultLabel(){
            Rect rect = new Rect(0f, 0f, 300f, 30f);
            string cult_title = "Order of Cthulhu";
            Text.Font = GameFont.Medium;
            Widgets.Label(rect, cult_title);

            Widgets.DrawBox(rect, 0);
            //CharacterCardUtility.DoNameInputRect(rect, ref cult_title, 24);
        }

        private void DrawRenameButton(){
            Rect rect = new Rect( this.size.x - 125f, 0f, 30f, 30f);
            //TooltipHandler.TipRegionByKey(rect, "RenameColonist");


            Widgets.ButtonImage(rect, TexButton.Rename);
            Widgets.DrawBox(rect, 0);
               // Find.WindowStack.Add(new Dialog_NamePawn(pawn));
        }


        System.Random rand = new System.Random();


        protected override void FillTab()
        {
            Rect rect = new Rect(17f, 17f, this.size.x, this.size.y);
            Widgets.DrawBox(rect, 0);
            
            GUI.BeginGroup(rect);
            DrawCultLabel();
            DrawRenameButton();
            GUI.EndGroup();

            List<string> deities = new List<string>();
            deities.Add("Chulhu");
            deities.Add("Bast");
            deities.Add("Dagon");
            deities.Add("Nyarlathotep");
            deities.Add("Shub-Niggurath");
            deities.Add("Tsathoggua");
            deities.Add("Hastur");

            var rand = new System.Random();
            for (int i = 0; i < deities.Count; i++)
            {
                Rect deity_rect = new Rect(0, 70f + (i * 50f) , 225f, 50f);
                ProgressBar bar = new ProgressBar(deities[i]);
                bar.current_level = 0.87f;
                bar.Draw(deity_rect);
            }

            Rect temple_rect = new Rect(225, 70f, 250f, 75f);
            ProgressBar temple_bar = new ProgressBar("Temple");
            temple_bar.is_default_color = true;
            temple_bar.current_level = 0.35f;
            temple_bar.Draw(temple_rect);



            
            //Find.WindowStack.Add(new Dialog_DebugSettingsMenu());

            ThingWithComps selected = Find.Selector.SingleSelectedThing as ThingWithComps;
            if(selected != null){
                Log.Message("Selected " + selected.def.label);
            }else{
                Log.Message("selection is null");
            }
        }
    }


    public class ProgressBar{
        private string label;
        private List<float> threshold_percentages = new List<float>();

        public bool is_default_color = false;
        public float max_level = 1.0f;
        public float current_level = 0.0f;
        public float current_level_percentage {
            get{
                return current_level / max_level;
            }
        }

        public ProgressBar(string label = "null"){
            this.label = label;

            threshold_percentages.Add(0.05f);
            threshold_percentages.Add(0.20f);
            threshold_percentages.Add(0.40f);
            threshold_percentages.Add(0.65f);
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
			float num4 = 1f;
			if (max_level < 1f) num4 = max_level;
			rect3.width *= num4;

            Rect barRect = is_default_color? 
                Widgets.FillableBar(rect3, current_level_percentage) : 
                Widgets.FillableBar(rect3, current_level_percentage, TexButton.RedColorTex);

            for (int i = 0; i < threshold_percentages.Count; i++)
            {
                DrawBarThreshold(barRect, threshold_percentages[i] * num4);
            }
        
			Text.Font = GameFont.Small;
		}

        private void DrawBarThreshold(Rect barRect, float threshPct)
        {
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

    public class WornAtCardUtility
    {
        public static Vector2 CardSize = new Vector2(395f, 536f); // set the card's size
        
        public static void DrawCard(Rect rect, ThingWithComps wornAtThing )
        {
            /*
            GUI.BeginGroup();
            ... draw ...
            GUI.EndGroup();
            */
        }
    }


    public class Cults_MainTabWindow_Restrict : MainTabWindow_Restrict {
        public override void PostOpen()
		{
			base.PostOpen();
            Log.Message("Open");
		}

        public override void DoWindowContents(Rect fillRect)
		{
			base.DoWindowContents(fillRect);
            Log.Message("Drawing");
		}
    }


    
    [StaticConstructorOnStartup]
	internal class TexButton
	{
		public static readonly Texture2D CloseXBig = ContentFinder<Texture2D>.Get("UI/Widgets/CloseX");
        public static readonly Texture2D Rename = ContentFinder<Texture2D>.Get("UI/Buttons/Rename");
        public static readonly Texture2D BarInstantMarkerTex = ContentFinder<Texture2D>.Get("UI/Misc/BarInstantMarker");
		public static readonly Texture2D NeedUnitDividerTex = ContentFinder<Texture2D>.Get("UI/Misc/NeedUnitDivider");
        public static readonly Texture2D RedColorTex = SolidColorMaterials.NewSolidColorTexture(Color.red);
    }


}