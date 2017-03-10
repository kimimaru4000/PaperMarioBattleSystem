using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PaperMarioBattleSystem.Enumerations;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// Mario's Tornado Jump action.
    /// Tornado Jump is unique in that it has two parts to it with different Action Commands and Sequences.
    /// <para>The first part is a single normal jump, when if successful, does double the damage of a normal jump, then
    /// goes into the second part. The second part has the player press 3 random buttons in order to damage all midair enemies.</para>
    /// </summary>
    public sealed class TornadoJump : MoveAction
    {
        //NOTE: Keep it like this for now and cast to get this value for the second part
        //If more actions are like this, we'll need to create a system to handle actions with more parts that are completely different
        public InteractionParamHolder AerialDamage { get; private set; }

        public BattleEntity[] GetAerialTargets
        {
            get
            {
                return BattleManager.Instance.GetEntities(EntityTypes.Enemy, HeightStates.Airborne);
            }
        }

        public TornadoJump()
        {
            Name = "Tornado Jump";

            MoveInfo = new MoveActionData(null, 3, "Execute superbly to damage\nall midair enemies.",
                TargetSelectionMenu.EntitySelectionType.Single, EntityTypes.Enemy,
                new HeightStates[] { HeightStates.Grounded, HeightStates.Airborne });

            //The second part's damage is Piercing, starts as 2, and cannot be increased with Power Plus, All Or Nothing, or P-Up, D-Down
            //Equipping a 2nd badge increases the FP cost from 3 to 6 and increases the damage of the Jump by 1 and the air attack by 2

            //The base damage is Mario's current Boot level
            //If Mario isn't the one using this move, it defaults to 1
            int baseDamage = 1;
            MarioStats marioStats = User.BattleStats as MarioStats;
            if (marioStats != null) baseDamage = (int)marioStats.BootLevel;

            DamageInfo = new InteractionParamHolder(null, null, baseDamage, Elements.Normal, false, ContactTypes.Direct, null);

            //NOTE: The aerial damage hits anything flying even if it has a HeightState that's not Airborne; this includes Embers!
            AerialDamage = new InteractionParamHolder(null, null, 2, Elements.Normal, true, ContactTypes.None, null);

            TornadoJumpSequence tornadoJumpSequence = new TornadoJumpSequence(this);
            SetMoveSequence(tornadoJumpSequence);

            actionCommand = new TornadoJumpCommand(MoveSequence, tornadoJumpSequence.JumpDuration,
                (int)(tornadoJumpSequence.JumpDuration / 2f), 10000d);
        }
    }
}
