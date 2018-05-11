using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static PaperMarioBattleSystem.Enumerations;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Sequence for Gulpit's Rock Spit move.
    /// </summary>
    public sealed class RockSpitSequence : Sequence
    {
        /// <summary>
        /// The BattleEntity spit out as a weapon. It dies and is removed from battle at the end of the Sequence.
        /// </summary>
        private BattleEntity EntityUsed = null;

        /// <summary>
        /// The tint color of the BattleEntity used.
        /// To hide it being eaten by the Gulpit, we make it transparent (for now).
        /// </summary>
        private Color PrevUsedTintColor = Color.White;

        private double WalkDur = 600d;

        public RockSpitSequence(MoveAction moveAction, BattleEntity entityUsed) : base(moveAction)
        {
            EntityUsed = entityUsed;
        }

        protected override void OnStart()
        {
            base.OnStart();

            PrevUsedTintColor = EntityUsed.TintColor;
        }

        protected override void OnEnd()
        {
            base.OnEnd();

            //Kill the entity used so it gets removed from battle
            EntityUsed.Die();

            PrevUsedTintColor = Color.White;
            EntityUsed = null;
        }

        protected override void SequenceStartBranch()
        {
            switch (SequenceStep)
            {
                case 0:
                    //Go in front of the entity
                    //bool approachFromLeft = User.BattlePosition.X < EntityUsed.BattlePosition.X;
                    Vector2 position = BattleManagerUtils.GetPositionInFront(EntityUsed, false);
                    User.AnimManager.PlayAnimation(AnimationGlobals.RunningName);

                    CurSequenceAction = new MoveToSeqAction(position, WalkDur);
                    break;
                case 1:
                    User.AnimManager.PlayAnimation(AnimationGlobals.GulpitBattleAnimations.SpitRockName);

                    //Show the used entity going inside its mouth; make invisible for now
                    EntityUsed.TintColor = Color.Transparent;

                    EntityUsed.Layer = User.Layer + .0001f;
                    AddSideSeqAction(new MoveToSeqAction(EntityUsed, User.Position + new Vector2(-18, -3), User.AnimManager.CurrentAnim.CurFrame.Duration));

                    CurSequenceAction = new WaitForAnimationSeqAction(AnimationGlobals.GulpitBattleAnimations.SpitRockName);
                    ChangeSequenceBranch(SequenceBranch.Main);
                    break;
                default:
                    PrintInvalidSequence();
                    break;
            }
        }

        protected override void SequenceMainBranch()
        {
            switch (SequenceStep)
            {
                case 0:
                    //Always play the standard Idle animation here, since the entity is waiting
                    User.AnimManager.PlayAnimation(AnimationGlobals.IdleName);

                    //Make the used entity fly towards the target
                    EntityUsed.TintColor = PrevUsedTintColor;
                    CurSequenceAction = new MoveToSeqAction(EntityUsed, BattleManagerUtils.GetPositionInFront(EntitiesAffected[0], false), WalkDur / 2d);
                    break;
                case 1:
                    //When hitting, it should eventually fly off after recoiling if a rock, or if it's like a bomb then explode
                    //There will need to be some type of collision behavior we can define
                    //For now just make it invisible
                    EntityUsed.TintColor = Color.Transparent;

                    //Attempt to deal damage
                    AttemptDamage(BaseDamage, EntitiesAffected[0], Action.DamageProperties, false);

                    ChangeSequenceBranch(SequenceBranch.End);
                    break;
                default:
                    PrintInvalidSequence();
                    break;
            }
        }

        protected override void SequenceEndBranch()
        {
            switch (SequenceStep)
            {
                case 0:
                    //Go back to your battle position
                    User.AnimManager.PlayAnimation(AnimationGlobals.RunningName);

                    CurSequenceAction = new MoveToSeqAction(User.BattlePosition, WalkDur);
                    break;
                case 1:
                    User.AnimManager.PlayAnimation(User.GetIdleAnim());

                    //End the sequence
                    EndSequence();
                    break;
                default:
                    PrintInvalidSequence();
                    break;
            }
        }

        protected override void SequenceSuccessBranch()
        {
            switch (SequenceStep)
            {
                default:
                    PrintInvalidSequence();
                    break;
            }
        }

        protected override void SequenceFailedBranch()
        {
            switch (SequenceStep)
            {
                default:
                    PrintInvalidSequence();
                    break;
            }
        }

        protected override void SequenceMissBranch()
        {
            switch (SequenceStep)
            {
                case 0:
                    CurSequenceAction = new WaitSeqAction(0d);

                    ChangeSequenceBranch(SequenceBranch.End);
                    break;
                default:
                    PrintInvalidSequence();
                    break;
            }
        }
    }
}
