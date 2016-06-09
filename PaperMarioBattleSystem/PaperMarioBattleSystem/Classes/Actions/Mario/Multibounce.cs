using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PaperMarioBattleSystem
{
    public sealed class Multibounce : Jump
    {
        private int NextTargetIndex = 0;
        private int CurrentTargetIndex = 0;

        protected override BattleEntity CurTarget => EntitiesAffected[CurrentTargetIndex];

        public Multibounce()
        {
            Name = "Multibounce";
            Description = "Lets you do a Multibounce. Uses 2 FP. Jumps on all enemies in a row if action command is timed right.";

            SelectionType = TargetSelectionMenu.EntitySelectionType.All;
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

        public override void OnCommandSuccess()
        {
            base.OnCommandSuccess();
            NextTargetIndex++;
        }

        protected override void SequenceSuccessBranch()
        {
            switch (SequenceStep)
            {
                case 0:
                    AttemptDamage(DamageDealt, CurTarget);

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
