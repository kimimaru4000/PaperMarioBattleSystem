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
    public class JumpAction : MoveAction
    {
        public JumpAction()
        {
            Name = "Jump";

            MoveInfo = new MoveActionData(new CroppedTexture2D(AssetManager.Instance.LoadRawTexture2D($"{ContentGlobals.BattleGFX}.png"), new Rectangle(216, 845, 22, 22)),
                "Jump and stomp on an enemy.", MoveResourceTypes.FP, 0, CostDisplayTypes.Shown,
                MoveAffectionTypes.Other, TargetSelectionMenu.EntitySelectionType.Single, true,
                new HeightStates[] { HeightStates.Grounded, HeightStates.Hovering, HeightStates.Airborne }, User.GetOpposingEntityType(), EntityTypes.Neutral);

            //The base damage for Jump is Mario's current Boot level
            //If Mario isn't the one using this move, it defaults to 1
            int baseDamage = 1;
            MarioStats marioStats = User.BattleStats as MarioStats;
            if (marioStats != null) baseDamage = (int)marioStats.BootLevel;

            DamageInfo = new DamageData(baseDamage, Elements.Normal, false, ContactTypes.TopDirect, ContactProperties.None, null,
                DamageEffects.FlipsShelled | DamageEffects.RemovesWings);

            JumpSequence jumpSequence = new JumpSequence(this);
            SetMoveSequence(jumpSequence);
            actionCommand = new JumpCommand(MoveSequence, jumpSequence.JumpDuration, (int)(jumpSequence.JumpDuration / 2f));
        }
    }
}
