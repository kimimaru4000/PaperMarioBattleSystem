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
    public sealed class TornadoJumpAction : MoveAction
    {
        //The second part of Tornado Jump
        private MoveAction TornadoJumpSecondPart = null;

        public TornadoJumpAction(BattleEntity user) : base(user)
        {
            Name = "Tornado Jump";

            MoveInfo = new MoveActionData(new CroppedTexture2D(AssetManager.Instance.LoadRawTexture2D($"{ContentGlobals.BattleGFX}.png"), new Rectangle(939, 390, 38, 34)),
                "Execute superbly to damage\nall midair enemies.", MoveResourceTypes.FP, 3,
                CostDisplayTypes.Shown, MoveAffectionTypes.Other,
                TargetSelectionMenu.EntitySelectionType.Single, false,
                new HeightStates[] { HeightStates.Grounded, HeightStates.Hovering, HeightStates.Airborne }, User.GetOpposingEntityType(), EntityTypes.Neutral);

            //The base damage is Mario's current Boot level
            //If Mario isn't the one using this move, it defaults to 1
            int baseDamage = 1;
            MarioStats marioStats = User.BattleStats as MarioStats;
            if (marioStats != null) baseDamage = (int)marioStats.BootLevel;

            DamageInfo = new DamageData(baseDamage, Elements.Normal, false, ContactTypes.TopDirect, ContactProperties.None, null,
                DamageEffects.FlipsShelled | DamageEffects.RemovesWings);

            //Second part
            //The second part's damage is Piercing, starts as 2, and cannot be increased with Power Plus, All Or Nothing, or P-Up, D-Down
            //Equipping a 2nd badge increases the FP cost from 3 to 6 and increases the damage of the Jump by 1 and the air attack by 2
            TornadoJumpSecondPart = new MoveAction(User, string.Empty, new MoveActionData(null, string.Empty, MoveResourceTypes.FP, 0f, CostDisplayTypes.Shown,
                MoveAffectionTypes.Other, TargetSelectionMenu.EntitySelectionType.All, false, new HeightStates[] { HeightStates.Hovering, HeightStates.Airborne },
                new EntityTypes[] { EntityTypes.Enemy }), new NoSequence(TornadoJumpSecondPart),
                new DamageData(2, Elements.Normal, true, ContactTypes.None, ContactProperties.Ranged, null, DamageEffects.None));

            TornadoJumpSequence tornadoJumpSequence = new TornadoJumpSequence(this, TornadoJumpSecondPart);
            SetMoveSequence(tornadoJumpSequence);

            actionCommand = new TornadoJumpCommand(MoveSequence, tornadoJumpSequence.JumpDuration,
                (int)(tornadoJumpSequence.JumpDuration / 2f), 10000d);
        }
    }
}
