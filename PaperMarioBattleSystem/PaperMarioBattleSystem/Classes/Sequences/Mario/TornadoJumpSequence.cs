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
    /// The Sequence for Mario's Tornado Jump move.
    /// </summary>
    public sealed class TornadoJumpSequence : JumpSequence
    {
        public override int DamageDealt => (BaseDamage * DamageMod);

        private int DamageMod = 1;
        private double WaitTime = 1000d;

        private bool SecondPart = false;

        private int SuccessfulButtons = 0;

        public TornadoJumpSequence(MoveAction moveAction) : base(moveAction)
        {

        }

        protected override void OnStart()
        {
            base.OnStart();

            DamageMod = 1;
            SecondPart = false;
        }

        protected override void OnEnd()
        {
            base.OnEnd();

            DamageMod = 1;
            SecondPart = false;
        }

        protected override void CommandSuccess()
        {
            base.CommandSuccess();
            DamageMod = 2;
        }

        public override void OnCommandResponse(object response)
        {
            //Pressing each successful button during the tornado part of the Action Command causes Mario to spin faster
            SuccessfulButtons = (int)response;
        }

        protected override void SequenceMainBranch()
        {
            switch (SequenceStep)
            {
                case 0:
                    if (SecondPart == false)
                    {
                        base.SequenceMainBranch();
                    }
                    else
                    {
                        StartActionCommandInput();
                        CurSequenceAction = new WaitForCommandSeqAction(2000d, actionCommand, CommandEnabled);
                    }
                    break;
                case 1:
                    if (SecondPart == false)
                    {
                        base.SequenceMainBranch();
                    }
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
                case 0:
                    if (SecondPart == false)
                    {
                        base.SequenceSuccessBranch();

                        SecondPart = true;
                        ChangeSequenceBranch(SequenceBranch.Main);
                    }
                    else
                    {
                        CurSequenceAction = new WaitSeqAction(WaitTime);
                    }
                    break;
                case 1:
                    int aerialDamage = BaseDamage;
                    BattleEntity[] aerialTargets = EntitiesAffected;
                    InteractionParamHolder aerialDamageInfo = Action.DamageInfo.Value;

                    //Make sure the action used for this sequence is Tornado Jump
                    //If not, default to base damage and base targets
                    TornadoJump tornadoJump = Action as TornadoJump;
                    if (tornadoJump != null)
                    {
                        aerialDamage = tornadoJump.AerialDamage.Damage;
                        aerialTargets = tornadoJump.GetAerialTargets;
                        aerialDamageInfo = tornadoJump.AerialDamage;
                    }

                    AttemptDamage(aerialDamage, aerialTargets, aerialDamageInfo, true);

                    CurSequenceAction = new MoveAmountSeqAction(new Vector2(0f, JumpHeight), JumpDuration);
                    ChangeSequenceBranch(SequenceBranch.End);

                    break;
                default:
                    PrintInvalidSequence();
                    break;
            }
        }

        protected override void SequenceFailedBranch()
        {
            switch (SequenceStep)
            {
                case 0:
                    if (SecondPart == false)
                    {
                        base.SequenceFailedBranch();
                    }
                    else
                    {
                        User.AnimManager.PlayAnimation(AnimationGlobals.MarioBattleAnimations.TornadoJumpFailName);
                        CurSequenceAction = new WaitForAnimationSeqAction(AnimationGlobals.MarioBattleAnimations.TornadoJumpFailName);
                    }
                    break;
                case 1:
                    CurSequenceAction = new MoveAmountSeqAction(new Vector2(0f, JumpHeight), JumpDuration);
                    ChangeSequenceBranch(SequenceBranch.End);
                    break;
                default:
                    PrintInvalidSequence();
                    break;
            }
        }
    }
}
