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
    [StaticConstructorOnStartup]
	internal class TexButton
	{
		public static readonly Texture2D CloseXBig = ContentFinder<Texture2D>.Get("UI/Widgets/CloseX");
        public static readonly Texture2D Rename = ContentFinder<Texture2D>.Get("UI/Buttons/Rename");
        public static readonly Texture2D Info = ContentFinder<Texture2D>.Get("UI/Buttons/InfoButton");
    }

    [StaticConstructorOnStartup]
    internal class Textures{
        public static readonly Texture2D BarInstantMarkerTex = ContentFinder<Texture2D>.Get("UI/Misc/BarInstantMarker");
		public static readonly Texture2D NeedUnitDividerTex = ContentFinder<Texture2D>.Get("UI/Misc/NeedUnitDivider");
        public static readonly Texture2D RedColorTex = SolidColorMaterials.NewSolidColorTexture(Color.red);

        public static readonly Texture2D AnimalColorTex = SolidColorMaterials.NewSolidColorTexture(new Color(.4f, .2f, .4f, 1f));
        public static readonly Texture2D HumanColorTex = SolidColorMaterials.NewSolidColorTexture(new Color(.6f, .2f, .2f, 1f));
        public static readonly Texture2D blackColorTex = SolidColorMaterials.NewSolidColorTexture(new Color(.1f, .1f, .1f, 1f));

        //public static readonly Texture2D Gizmo_ = ContentFinder<Texture2D>.Get("UI/Misc/NeedUnitDivider");


    }
}