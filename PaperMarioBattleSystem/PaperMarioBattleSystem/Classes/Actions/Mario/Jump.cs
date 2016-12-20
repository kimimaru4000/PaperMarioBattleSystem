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
    /// Mario's Jump action
    /// </summary>
    public class Jump : MoveAction
    {
        public Jump()
        {
            Name = "Jump";

            MoveInfo = new MoveActionData(null, 0, true, "Jump and stomp on an enemy.", TargetSelectionMenu.EntitySelectionType.Single,
                EntityTypes.Enemy, new HeightStates[] { HeightStates.Grounded, HeightStates.Airborne });

            //The base damage for Jump is Mario's current Boot level
            //If Mario isn't the one using this move, it defaults to 1
            int baseDamage = 1;
            MarioStats marioStats = User.BattleStats as MarioStats;
            if (marioStats != null) baseDamage = (int)marioStats.BootLevel;

            DamageInfo = new InteractionParamHolder(null, null, baseDamage, Elements.Normal, false, ContactTypes.JumpContact, null);

            JumpSequence jumpSequence = new JumpSequence(this);
            SetMoveSequence(jumpSequence);
            actionCommand = new JumpCommand(MoveSequence, jumpSequence.JumpDuration, (int)(jumpSequence.JumpDuration / 2f));
        }
    }
}
