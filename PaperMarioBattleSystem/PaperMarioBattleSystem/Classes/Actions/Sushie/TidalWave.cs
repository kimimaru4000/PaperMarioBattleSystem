using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// Sushie's Tidal Wave attack
    /// </summary>
    public class TidalWave : MoveAction
    {
        public TidalWave()
        {
            Name = "Tidal Wave";
            MoveInfo = new MoveActionData(null, "A surge of water hits all enemies.", Enumerations.MoveResourceTypes.FP, 6,
                Enumerations.CostDisplayTypes.Shown, Enumerations.MoveAffectionTypes.Enemy, TargetSelectionMenu.EntitySelectionType.All,
                true,
                new Enumerations.HeightStates[] { Enumerations.HeightStates.Grounded, Enumerations.HeightStates.Airborne, Enumerations.HeightStates.Ceiling });

            DamageInfo = new DamageData(0, Enumerations.Elements.Water, false, Enumerations.ContactTypes.None, null, 
                Enumerations.DamageEffects.None);

            SetMoveSequence(new TidalWaveSequence(this));
            actionCommand = new TidalWaveCommand(MoveSequence);
        }
    }
}
