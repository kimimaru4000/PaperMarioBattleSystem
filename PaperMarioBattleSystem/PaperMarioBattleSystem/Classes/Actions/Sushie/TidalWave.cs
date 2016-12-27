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
            MoveInfo = new MoveActionData(null, 6, true, "A surge of water hits all enemies", TargetSelectionMenu.EntitySelectionType.All,
                Enumerations.EntityTypes.Enemy, new Enumerations.HeightStates[] { Enumerations.HeightStates.Grounded, Enumerations.HeightStates.Airborne, Enumerations.HeightStates.Ceiling });

            DamageInfo = new InteractionParamHolder(null, null, 0, Enumerations.Elements.Water, false, Enumerations.ContactTypes.None, null);

            SetMoveSequence(new TidalWaveSequence(this));
            actionCommand = new TidalWaveCommand(MoveSequence);
        }
    }
}
