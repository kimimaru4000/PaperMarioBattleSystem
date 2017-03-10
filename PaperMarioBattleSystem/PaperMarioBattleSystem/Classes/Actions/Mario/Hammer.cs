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

            MoveInfo = new MoveActionData(null, 0, true, "Whack an enemy with your Hammer.", TargetSelectionMenu.EntitySelectionType.First,
                EntityTypes.Enemy, new HeightStates[] { HeightStates.Grounded });

            //The base damage for Hammer is Mario's current Hammer level
            //If Mario isn't the one using this move, it defaults to 1
            int baseDamage = 1;
            MarioStats marioStats = User.BattleStats as MarioStats;
            if (marioStats != null) baseDamage = (int)marioStats.HammerLevel;

            DamageInfo = new InteractionParamHolder(null, null, baseDamage, Elements.Normal, false, ContactTypes.None, null);

            SetMoveSequence(new HammerSequence(this));
            actionCommand = new HammerCommand(MoveSequence, 4, 500d);
        }
    }
}
