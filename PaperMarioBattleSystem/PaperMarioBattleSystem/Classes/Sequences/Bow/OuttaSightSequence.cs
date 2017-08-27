using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Sequence for Bow's Outta Sight move.
    /// </summary>
    public sealed class OuttaSightSequence : Sequence
    {
        private const double MoveTime = 500d;

        public OuttaSightSequence(MoveAction moveAction) : base(moveAction)
        {

        }

        protected override void SequenceStartBranch()
        {
            switch (SequenceStep)
            {
                case 0:
                    //Move behind Mario
                    Vector2 movePosition = BattleManager.Instance.GetMario().BattlePosition;

                    //NOTE: Approximately up and behind
                    movePosition.X -= 15;
                    movePosition.Y = User.Position.Y;

                    CurSequenceAction = new MoveToSeqAction(movePosition, MoveTime);
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
                    //Use up all of Mario and Bow's turns
                    BattleMario mario = BattleManager.Instance.GetMario();

                    User.SetTurnsUsed(User.MaxTurns - 1);
                    mario.SetTurnsUsed(mario.MaxTurns);

                    //Add evasion
                    User.AddEvasionMod(0d);
                    mario.AddEvasionMod(0d);

                    //Turn them both transparent
                    User.TintColor *= .3f;
                    mario.TintColor *= .3f;

                    //Mario assumes the Guard position
                    mario.AnimManager.PlayAnimation(AnimationGlobals.PlayerBattleAnimations.GuardName);

                    //NOTE: This sequence remains until the next phase cycle
                    //Figure out a way to have Sequences do this (would be very useful as Defend also works this way!)
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

        protected override void SequenceEndBranch()
        {
            switch (SequenceStep)
            {
                case 0:

                    //NOTE: Finish this - the end should occur next turn, not on the same turn
                    EndSequence();
                    break;
                default:
                    PrintInvalidSequence();
                    break;
            }
        }

        protected override void SequenceMissBranch()
        {
            switch(SequenceStep)
            {
                default:
                    PrintInvalidSequence();
                    break;
            }
        }
    }
}
