using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Sequence for Multibounce.
    /// </summary>
    public sealed class MultibounceSequence : JumpSequence
    {
        private int NextTargetIndex = 0;
        private int CurrentTargetIndex = 0;

        public override BattleEntity CurTarget => EntitiesAffected[CurrentTargetIndex];

        public MultibounceSequence(MoveAction moveAction) : base(moveAction)
        {
            
        }

        protected override void OnStart()
        {
            base.OnStart();

            CurrentTargetIndex = NextTargetIndex = 0;
        }

        protected override void OnEnd()
        {
            base.OnEnd();

            CurrentTargetIndex = NextTargetIndex = 0;
        }

        protected override void CommandSuccess()
        {
            base.CommandSuccess();
            NextTargetIndex++;
        }

        protected override void SequenceSuccessBranch()
        {
            switch (SequenceStep)
            {
                case 0:
                    AttemptDamage(DamageDealt, CurTarget, Action.DamageInfo.Value, false);

                    //Restart with the next target
                    if (NextTargetIndex < EntitiesAffected.Length)
                    {
                        CurrentTargetIndex = NextTargetIndex;
                        ChangeSequenceBranch(SequenceBranch.Start);
                    }
                    //Otherwise end it since we're on the last target
                    else
                    {
                        ChangeSequenceBranch(SequenceBranch.End);
                    }
                    break;
                default:
                    PrintInvalidSequence();
                    break;
            }
        }
    }
}
