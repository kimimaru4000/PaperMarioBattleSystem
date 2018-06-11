using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using PaperMarioBattleSystem.Extensions;
using PaperMarioBattleSystem.Utilities;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The sequence for Pokey's Body Throw move.
    /// </summary>
    public class BodyThrowSequence : Sequence
    {
        private ISegmentBehavior SegmentBehavior = null;
        private CroppedTexture2D SegmentTex = null;

        private UICroppedTexture2D SegmentTexUI = null;

        private double UpMoveTime = 1000d;
        private double WaitTime = 500d;
        private double FlingTime = 700d;

        public BodyThrowSequence(MoveAction moveAction, ISegmentBehavior segmentBehavior, CroppedTexture2D segmentTex) : base(moveAction)
        {
            SegmentBehavior = segmentBehavior;
            SegmentTex = segmentTex;
        }

        protected override void OnEnd()
        {
            base.OnEnd();

            if (SegmentTexUI != null)
            {
                User.BManager.battleUIManager.RemoveUIElement(SegmentTexUI);
                SegmentTexUI = null;
            }

            SegmentTex = null;
            SegmentBehavior = null;
        }

        protected override void SequenceStartBranch()
        {
            switch (SequenceStep)
            {
                case 0:
                    SegmentTexUI = new UICroppedTexture2D(SegmentTex);
                    SegmentTexUI.Origin = new Vector2(.5f, .5f);

                    User.BManager.battleUIManager.AddUIElement(SegmentTexUI);

                    //Start it out being underneath, where the segment would be
                    SegmentTexUI.Position = Camera.Instance.SpriteToUIPos(User.Position + new Vector2(0f, SegmentTex.SourceRect.Value.Height * SegmentBehavior.CurSegmentCount));

                    //Remove a segment
                    SegmentBehavior.HandleSegmentRemoved(1);

                    //Move the segment above the user's head
                    CurSequenceAction = new MoveToSeqAction(SegmentTexUI, Camera.Instance.SpriteToUIPos(User.GetDrawnPosAbove(new Vector2(0f, -SegmentTex.SourceRect.Value.Height))),
                        UpMoveTime, Interpolation.InterpolationTypes.Linear, Interpolation.InterpolationTypes.CubicInOut);

                    break;
                case 1:
                    //Wait a little bit
                    CurSequenceAction = new WaitSeqAction(WaitTime);

                    ChangeSequenceBranch(SequenceBranch.Main);
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
                    User.AnimManager.PlayAnimation(User.GetIdleAnim());

                    EndSequence();
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
                    //Fling the segment at the target
                    CurSequenceAction = new MoveToSeqAction(SegmentTexUI, Camera.Instance.SpriteToUIPos(EntitiesAffected[0].Position), FlingTime);
                    break;
                case 1:
                    //Attempt damage
                    InteractionResult[] interactionResult = AttemptDamage(BaseDamage, EntitiesAffected[0], Action.DamageProperties, false);

                    //If the victim Superguarded, we need to reflect the segment back at the attacker and damage it
                    //If the victim didn't Superguard, move on to the End branch
                    if (UtilityGlobals.DefensiveActionTypesHasFlag(interactionResult[0].VictimResult.DefensiveActionsPerformed,
                        Enumerations.DefensiveActionTypes.Superguard) == false)
                    {
                        ChangeSequenceBranch(SequenceBranch.End);
                    }

                    CurSequenceAction = new WaitSeqAction(0d);
                    break;
                case 2:
                    //Fling the segment back to the attacker
                    CurSequenceAction = new MoveToSeqAction(SegmentTexUI, Camera.Instance.SpriteToUIPos(User.Position), FlingTime);

                    break;
                case 3:
                    //Damage the attacker then move onto the end of the sequence
                    AttemptDamage(BaseDamage, User, Action.DamageProperties, false);

                    CurSequenceAction = new WaitSeqAction(0d);

                    ChangeSequenceBranch(SequenceBranch.End);
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

        public override void StartInterruption(Enumerations.Elements element)
        {
            //Simply don't do anything else on an interruption but play a sound
            //It's expected that the attacker can get hit here due to a Superguard flinging back the segment

            //Play the damaged sound
            SoundManager.Instance.PlaySound(SoundManager.Sound.Damaged);
        }
    }
}
