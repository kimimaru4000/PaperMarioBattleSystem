using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using PaperMarioBattleSystem.Utilities;
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

        private BattleEntity[] CurTargets = null;
        private MoveAction TJAction = null;

        private TornadoJumpActionCommandUI TornadoJumpUI = null;

        public TornadoJumpSequence(MoveAction moveAction, MoveAction tjAction) : base(moveAction)
        {
            TJAction = tjAction;
        }

        protected override void SetupJumpUI()
        {
            
        }

        protected override void OnStart()
        {
            base.OnStart();

            CurTargets = EntitiesAffected;

            DamageMod = 1;
            SecondPart = false;

            if (Action.CommandEnabled == true && Action.DrawActionCommandInfo == true)
            {
                TornadoJumpUI = new TornadoJumpActionCommandUI(actionCommand as TornadoJumpCommand);
                BattleUIManager.Instance.AddUIElement(TornadoJumpUI);
            }
        }

        protected override void OnEnd()
        {
            base.OnEnd();

            if (CurTargets != null)
            {
                //Stop targeting
                for (int i = 0; i < CurTargets.Length; i++)
                {
                    CurTargets[i].StopTarget();
                }
            }

            //Clear the current targets
            CurTargets = null;
            TJAction = null;

            DamageMod = 1;
            SecondPart = false;

            if (TornadoJumpUI != null)
            {
                BattleUIManager.Instance.RemoveUIElement(TornadoJumpUI);
                TornadoJumpUI = null;
            }
        }

        protected override void CommandSuccess()
        {
            base.CommandSuccess();
            DamageMod = 2;
        }

        public override void OnCommandResponse(in object response)
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
                        //Update the current targets on the second part
                        if (TJAction != null)
                        {
                            CurTargets = TJAction.GetEntitiesMoveAffects();
                        }
                        else
                        {
                            Debug.LogWarning($"{Action.Name} is not of type {nameof(TornadoJumpAction)} in {nameof(TornadoJumpSequence)}!");
                        }

                        //Target the entities here
                        for (int i = 0; i < CurTargets.Length; i++)
                        {
                            CurTargets[i].TargetForMove(User);
                        }

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
                    }
                    else
                    {
                        CurSequenceAction = new WaitSeqAction(WaitTime);
                    }
                    break;
                case 1:
                    if (SecondPart == false)
                    {
                        base.SequenceSuccessBranch();

                        //Stop targeting the current targets. If it's a Winged entity, it won't be airborne anymore and thus
                        //won't take damage from the second part of the attack
                        for (int i = 0; i < CurTargets.Length; i++)
                        {
                            CurTargets[i].StopTarget();
                        }

                        SecondPart = true;
                        ChangeSequenceBranch(SequenceBranch.Main);
                    }
                    else
                    {
                        int aerialDamage = BaseDamage;
                        DamageData aerialDamageInfo = Action.DamageProperties;

                        //Make sure the action used for this sequence is Tornado Jump
                        //If not, default to base damage and base targets
                        if (TJAction != null && TJAction.DealsDamage == true)
                        {
                            aerialDamage = TJAction.DamageProperties.Damage;
                            aerialDamageInfo = TJAction.DamageProperties;
                        }
                        else
                        {
                            Debug.LogWarning($"{Action.Name}'s second part is not defined or does not deal damage!");
                        }

                        InteractionResult[] targetsHit = AttemptDamage(aerialDamage, CurTargets, aerialDamageInfo, true);

                        for (int i = 0; i < targetsHit.Length; i++)
                        {
                            InteractionResult targetHit = targetsHit[i];

                            if (targetHit == null || targetHit.WasAttackerHit == true || targetHit.WasVictimHit == false) continue;

                            ShowCommandRankVFX(HighestCommandRank, targetsHit[i].VictimResult.Entity.Position);
                        }

                        User.AnimManager.PlayAnimation(AnimationGlobals.JumpFallingName);
                        CurSequenceAction = new MoveAmountSeqAction(User, new Vector2(0f, JumpHeight), JumpDuration, Interpolation.InterpolationTypes.Linear, Interpolation.InterpolationTypes.Linear);
                        ChangeSequenceBranch(SequenceBranch.End);

                        SequenceStep = 1;
                    }
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
                        CurSequenceAction = new WaitForAnimationSeqAction(User, AnimationGlobals.MarioBattleAnimations.TornadoJumpFailName);
                    }
                    break;
                case 1:
                    if (SecondPart == false)
                    {
                        base.SequenceFailedBranch();
                    }
                    else
                    {
                        User.AnimManager.PlayAnimation(AnimationGlobals.JumpFallingName);
                        CurSequenceAction = new MoveAmountSeqAction(User, new Vector2(0f, JumpHeight), JumpDuration, Interpolation.InterpolationTypes.Linear, Interpolation.InterpolationTypes.QuadIn);
                        ChangeSequenceBranch(SequenceBranch.End);

                        SequenceStep = 1;
                    }
                    break;
                default:
                    PrintInvalidSequence();
                    break;
            }
        }
    }
}
