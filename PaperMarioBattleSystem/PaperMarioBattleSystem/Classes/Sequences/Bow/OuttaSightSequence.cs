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
                    //Move behind the ally
                    Vector2 movePosition = EntitiesAffected[0].BattlePosition;

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
                    //Use up all of the ally and Bow's turns
                    BattleEntity allyAffected = EntitiesAffected[0];

                    User.SetTurnsUsed(User.MaxTurns - 1);
                    allyAffected.SetTurnsUsed(allyAffected.MaxTurns);

                    //Add evasion
                    User.AddEvasionMod(0d);
                    allyAffected.AddEvasionMod(0d);

                    //Turn them both transparent
                    User.TintColor *= .3f;
                    allyAffected.TintColor *= .3f;

                    //The ally assumes the Guard position
                    allyAffected.AnimManager.PlayAnimation(AnimationGlobals.PlayerBattleAnimations.GuardName);

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
