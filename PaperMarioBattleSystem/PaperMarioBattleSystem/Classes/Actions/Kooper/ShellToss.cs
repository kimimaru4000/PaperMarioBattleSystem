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

            MoveInfo = new MoveActionData(null, "Shoot yourself at an enemy.", MoveResourceTypes.FP, 0, CostDisplayTypes.Shown,
                MoveAffectionTypes.Enemy, TargetSelectionMenu.EntitySelectionType.First, true,
                new HeightStates[] { HeightStates.Grounded });

            DamageInfo = new DamageData(1, Elements.Normal, false, ContactTypes.None, null, DamageEffects.RemovesPart);

            SetMoveSequence(new ShellTossSequence(this));
            actionCommand = new HammerCommand(MoveSequence, 4, 500d);
        }
    }
}
