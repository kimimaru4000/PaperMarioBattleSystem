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

        protected override BattleEntity CurTarget => EntitiesAffected[CurrentTargetIndex];

        public MultibounceSequence()
        {
            //Name = "Multibounce";
            //Description = "Lets you do a Multibounce. Uses 2 FP. Jumps on all enemies in a row if action command is timed right.";
            //
            //FPCost = 2;
            //
            //SelectionType = TargetSelectionMenu.EntitySelectionType.All;
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
                    AttemptDamage(DamageDealt, CurTarget, false);

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
