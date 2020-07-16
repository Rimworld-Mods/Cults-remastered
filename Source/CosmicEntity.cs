
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
    public class CosmicEntityDef : Def
    {
        public string descriptionLong;
        public float baseFavorGainRate = 0.0001f;
        public float maxFavor;
        public float discoveryChance = 0.02f;
        public List<float> favorThresholds;
        public List<Apparel> favoredApparel; // TODO: should be defs
        public List<string> domains; 
        public List<string> titles; 
        public readonly string symbol;
        public Texture2D symbolTex;
        public List<SpellDef> spells;

        public override void PostLoad()
        {
            LongEventHandler.ExecuteWhenFinished(delegate
            {
                this.symbolTex = ContentFinder<Texture2D>.Get(this.symbol, true);
            });
        }
    }

    public class CosmicEntity : IExposable
    {        
        public CosmicEntity(){}
        public CosmicEntityDef def;
        public float maxFavor => this.def.maxFavor;
        public float baseFavorGainRate => this.def.baseFavorGainRate;
        private float _currentFavor;
        private bool _isDiscovered;
        public bool isDiscovered => _isDiscovered;
        public float currentFavor => _currentFavor;

        public void GiveFavor(float f)
        {
            _currentFavor += f;
            if(_currentFavor >= this.def.maxFavor * 2) _currentFavor = this.def.maxFavor * 2;
        }
        public void Discover() => _isDiscovered = true;
        public void ExposeData()
        {
            Scribe_Defs.Look(ref this.def, "def");
            Scribe_Values.Look(ref this._currentFavor, "currentFavor", 0.0f, true);
            Scribe_Values.Look(ref this._isDiscovered, "isDiscovered", false, true);
        }
    }
}