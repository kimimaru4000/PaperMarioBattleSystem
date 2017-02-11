using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// Yoshi's Mini-Egg move.
    /// </summary>
    public sealed class MiniEgg : MoveAction
    {
        public MiniEgg()
        {
            Name = "Mini-Egg";
            MoveInfo = new MoveActionData(null, 3, false, "Throws Yoshi eggs at all enemies and shrinks them.",
                TargetSelectionMenu.EntitySelectionType.All, Enumerations.EntityTypes.Enemy, null);
            DamageInfo = new InteractionParamHolder(null, null, 1, Enumerations.Elements.Normal, false, Enumerations.ContactTypes.None,
                new StatusEffect[] { new TinyStatus(3) });

            SetMoveSequence(new MiniEggSequence(this));
            actionCommand = new TimedLightCommand(MoveSequence, 4000d, 3, 500d, 1d, TimedLightCommand.LightDistributions.LastLightAtEnd);
        }
    }
}
