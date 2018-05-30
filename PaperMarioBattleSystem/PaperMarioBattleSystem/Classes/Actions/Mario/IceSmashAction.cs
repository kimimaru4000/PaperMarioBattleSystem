using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// Mario's Ice Smash action.
    /// It's identical to Hammer except it deals Ice damage, can freeze enemies, and has a slightly longer Action Command.
    /// </summary>
    public sealed class IceSmashAction : HammerAction
    {
        public IceSmashAction(BattleEntity user) : base(user)
        {
            Name = "Ice Smash";
            MoveInfo.Icon.SetRect(new Rectangle(891, 430, 40, 40));
            MoveInfo.ResourceCost = 3;
            MoveInfo.Description = "Wear this to use Ice Smash. " +
                           "3 FP are required to use this attack, which can freeze and immobilize an enemy if executed superbly. " +
                           "Wearing two or more of these badges requires more FP for the move, but enemies stay frozen longer.";

            DamageInfo.DamagingElement = Enumerations.Elements.Ice;
            DamageInfo.Statuses = new StatusChanceHolder[] { new StatusChanceHolder(100d, new FrozenStatus(3)) };

            SetMoveSequence(new HammerSequence(this, 0));
            actionCommand = new HammerCommand(MoveSequence, 4, 1000d);
        }
    }
}
