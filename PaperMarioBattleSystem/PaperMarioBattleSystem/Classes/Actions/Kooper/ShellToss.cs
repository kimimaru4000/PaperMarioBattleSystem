using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using static PaperMarioBattleSystem.Enumerations;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// Kooper's Shell Toss action.
    /// </summary>
    public sealed class ShellToss : MoveAction
    {
        public ShellToss()
        {
            Name = "Shell Toss";

            MoveInfo = new MoveActionData(null, 0, true, "Shoot yourself at an enemy.", TargetSelectionMenu.EntitySelectionType.First,
                EntityTypes.Enemy, new HeightStates[] { HeightStates.Grounded });

            DamageInfo = new DamageData(1, Elements.Normal, false, ContactTypes.None, null, DamageEffects.RemovesPart);

            SetMoveSequence(new ShellTossSequence(this));
            actionCommand = new HammerCommand(MoveSequence, 4, 500d);
        }
    }
}
