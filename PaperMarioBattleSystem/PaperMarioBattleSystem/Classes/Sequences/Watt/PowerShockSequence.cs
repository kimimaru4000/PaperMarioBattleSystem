using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Sequence for Watt's Power Shock move.
    /// </summary>
    public sealed class PowerShockSequence : Sequence
    {
        private double MoveDur = 600d;
        private double MissWaitDur = 500d;

        public PowerShockSequence(MoveAction moveAction) : base(moveAction)
        {

        }

        protected override void SequenceStartBranch()
        {
            switch (SequenceStep)
            {
                case 0:
                    User.AnimManager.PlayAnimation(AnimationGlobals.RunningName);

                    Vector2 pos = BattleManager.Instance.GetPositionInFront(EntitiesAffected[0], User.EntityType == Enumerations.EntityTypes.Player);
                    CurSequenceAction = new MoveToSeqAction(pos, MoveDur);

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
                    User.AnimManager.PlayAnimation(AnimationGlobals.RunningName);

                    CurSequenceAction = new MoveToSeqAction(User.BattlePosition, MoveDur);
                    break;
                case 1:
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
                    User.AnimManager.PlayAnimation(AnimationGlobals.WattBattleAnimations.WattElectricChargeName);

                    StartActionCommandInput();
                    CurSequenceAction = new WaitForCommandSeqAction(500d, actionCommand, CommandEnabled);
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
                    //Inflict Paralyzed; succeeding has a higher chance
                    InteractionResult[] result = AttemptDamage(0, EntitiesAffected[0], Action.DamageProperties, true);

                    if (result[0].WasVictimHit == true && result[0].WasAttackerHit == false)
                    {
                        ShowCommandRankVFX(ActionCommand.CommandRank.Nice, EntitiesAffected[0].Position);
                    }

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
                    //Inflict Paralyzed; failing has a lower chance
                    AttemptDamage(0, EntitiesAffected[0], Action.DamageProperties, true);

                    ChangeSequenceBranch(SequenceBranch.End);
                    break;
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
                    User.AnimManager.PlayAnimation(AnimationGlobals.IdleName);

                    CurSequenceAction = new WaitSeqAction(MissWaitDur);

                    ChangeSequenceBranch(SequenceBranch.End);
                    break;
                default:
                    PrintInvalidSequence();
                    break;
            }
        }
    }
}
