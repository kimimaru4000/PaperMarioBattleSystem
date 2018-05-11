using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Sequence for Watt's Electro Dash.
    /// </summary>
    public class ElectroDashSequence : Sequence
    {
        public double CommandDur = 2000d;
        public double MoveDur = 1000d;

        private double StartWaitDur = 400d;
        private double EndMoveDur = 700d;

        private float MissHeight = 15f;
        private double MissMoveDur = 300d;
        private double MissWaitDur = 500d;

        public ElectroDashSequence(MoveAction moveAction) : base(moveAction)
        {

        }

        protected override bool OnMiss()
        {
            base.OnMiss();

            ChangeJumpBranch(SequenceBranch.Miss);

            return false;
        }

        protected override void SequenceStartBranch()
        {
            switch (SequenceStep)
            {
                case 0:
                    User.AnimManager.PlayAnimation(AnimationGlobals.RunningName);
                    CurSequenceAction = new WaitSeqAction(StartWaitDur);

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

                    CurSequenceAction = new MoveToSeqAction(User.BattlePosition, EndMoveDur);
                    break;
                case 1:
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
                    Vector2 pos = new Vector2(BattleManagerUtils.GetPositionInFront(EntitiesAffected[0], User.EntityType == Enumerations.EntityTypes.Player).X, User.Position.Y);

                    StartActionCommandInput();
                    CurSequenceAction = new MoveToSeqAction(pos, MoveDur);
                    break;
                case 1:
                    User.AnimManager.PlayAnimation(AnimationGlobals.WattBattleAnimations.WattElectricChargeName);

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
                    CurSequenceAction = new MoveToSeqAction(new Vector2(EntitiesAffected[0].BattlePosition.X, User.Position.Y), 200d);
                    break;
                case 1:
                    InteractionResult[] result = AttemptDamage(Action.DamageProperties.Damage, EntitiesAffected[0], Action.DamageProperties, false);

                    if (result[0] != null && result[0].WasVictimHit == true)
                    {
                        ShowCommandRankVFX(HighestCommandRank, EntitiesAffected[0].Position);
                    }

                    CurSequenceAction = new WaitSeqAction(0d);
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
                    CurSequenceAction = new MoveToSeqAction(new Vector2(EntitiesAffected[0].BattlePosition.X, User.Position.Y), 200d);
                    break;
                case 1:
                    //Failing deals only one damage
                    AttemptDamage(1, EntitiesAffected[0], Action.DamageProperties, false);

                    CurSequenceAction = new WaitSeqAction(0d);
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
                    User.AnimManager.PlayAnimation(AnimationGlobals.RunningName);

                    //When Watt misses Electro Dash, she goes in a downwards arc to around where the next enemy slot would be
                    Vector2 destPos = BattleManagerUtils.GetPositionInFront(EntitiesAffected[0], EntitiesAffected[0].EntityType == Enumerations.EntityTypes.Player);
                    Vector2 curDest = User.Position + new Vector2(UtilityGlobals.DifferenceDivided(destPos.X, User.Position.X, 2f), MissHeight);

                    CurSequenceAction = new MoveToSeqAction(curDest, MissMoveDur, Interpolation.InterpolationTypes.Linear, Interpolation.InterpolationTypes.QuadOut);
                    break;
                case 1:
                    destPos = BattleManagerUtils.GetPositionInFront(EntitiesAffected[0], EntitiesAffected[0].EntityType == Enumerations.EntityTypes.Player);
                    curDest = User.Position + new Vector2(UtilityGlobals.DifferenceDivided(destPos.X, User.Position.X, 2f), -MissHeight);

                    CurSequenceAction = new MoveToSeqAction(curDest, MissMoveDur, Interpolation.InterpolationTypes.Linear, Interpolation.InterpolationTypes.QuadIn);
                    break;
                case 2:
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
