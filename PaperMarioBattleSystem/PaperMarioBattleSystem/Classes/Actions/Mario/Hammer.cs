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
    /// Mario's Hammer action
    /// </summary>
    public class Hammer : MoveAction
    {
        public Hammer()
        {
            Name = "Hammer";

            MoveInfo = new MoveActionData(null, 0, "Whack an enemy with your Hammer.", TargetSelectionMenu.EntitySelectionType.First,
                EntityTypes.Enemy, new HeightStates[] { HeightStates.Grounded });

            DamageInfo = new InteractionParamHolder(null, null, (int)User.BattleStats.GetHammerLevel, Elements.Normal, false, ContactTypes.HammerContact, null);

            SetMoveSequence(new HammerSequence(this));
            actionCommand = new HammerCommand(MoveSequence, 4, 500d);
        }
    }
}
