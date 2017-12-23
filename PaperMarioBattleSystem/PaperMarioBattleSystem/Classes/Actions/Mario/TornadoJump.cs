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
    /// Mario's Tornado Jump action.
    /// Tornado Jump is unique in that it has two parts to it with different Action Commands and Sequences.
    /// <para>The first part is a single normal jump, when if successful, does double the damage of a normal jump, then
    /// goes into the second part. The second part has the player press 3 random buttons in order to damage all midair enemies.</para>
    /// </summary>
    public sealed class TornadoJump : MoveAction
    {
        //NOTE: Keep it like this for now and cast to get this value for the second part
        //If more actions are like this, we'll need to create a system to handle actions with more parts that are completely different
        public DamageData AerialDamage { get; private set; }

        public BattleEntity[] GetAerialTargets
        {
            get
            {
                return BattleManager.Instance.GetEntities(EntityTypes.Enemy, HeightStates.Hovering, HeightStates.Airborne);
            }
        }

        public TornadoJump()
        {
            Name = "Tornado Jump";

            MoveInfo = new MoveActionData(new CroppedTexture2D(AssetManager.Instance.LoadRawTexture2D($"{ContentGlobals.BattleGFX}.png"), new Rectangle(939, 390, 38, 34)),
                "Execute superbly to damage\nall midair enemies.", MoveResourceTypes.FP, 3,
                CostDisplayTypes.Shown, MoveAffectionTypes.Other,
                TargetSelectionMenu.EntitySelectionType.Single, false,
                new HeightStates[] { HeightStates.Grounded, HeightStates.Hovering, HeightStates.Airborne }, User.GetOpposingEntityType(), EntityTypes.Neutral);

            //The second part's damage is Piercing, starts as 2, and cannot be increased with Power Plus, All Or Nothing, or P-Up, D-Down
            //Equipping a 2nd badge increases the FP cost from 3 to 6 and increases the damage of the Jump by 1 and the air attack by 2

            //The base damage is Mario's current Boot level
            //If Mario isn't the one using this move, it defaults to 1
            int baseDamage = 1;
            MarioStats marioStats = User.BattleStats as MarioStats;
            if (marioStats != null) baseDamage = (int)marioStats.BootLevel;

            DamageInfo = new DamageData(baseDamage, Elements.Normal, false, ContactTypes.TopDirect, ContactProperties.None, null,
                DamageEffects.FlipsShelled | DamageEffects.RemovesWings);

            AerialDamage = new DamageData(2, Elements.Normal, true, ContactTypes.None, ContactProperties.Ranged, null, DamageEffects.None);

            TornadoJumpSequence tornadoJumpSequence = new TornadoJumpSequence(this);
            SetMoveSequence(tornadoJumpSequence);

            actionCommand = new TornadoJumpCommand(MoveSequence, tornadoJumpSequence.JumpDuration,
                (int)(tornadoJumpSequence.JumpDuration / 2f), 10000d);
        }
    }
}
