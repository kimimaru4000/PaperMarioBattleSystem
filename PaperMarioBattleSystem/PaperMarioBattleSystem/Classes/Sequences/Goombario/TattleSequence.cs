using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// Goombario and Goombella's Tattle sequence.
    /// <para>Goombario's Tattle does not have an Action Command. If the Action Command is disabled this will always succeed.</para>
    /// </summary>
    public sealed class TattleSequence : Sequence
    {
        private const double WaitTime = 1000d;
        private const double EndWait = 250d;

        private ITattleableEntity TattledEntity = null;

        public TattleSequence(MoveAction moveAction) : base(moveAction)
        {
            
        }

        protected override void OnStart()
        {
            base.OnStart();

            TattledEntity = (ITattleableEntity)EntitiesAffected[0];
        }

        protected override void OnEnd()
        {
            base.OnEnd();

            TattledEntity = null;
        }

        protected override void SequenceStartBranch()
        {
            switch (SequenceStep)
            {
                case 0:
                    User.AnimManager.PlayAnimation(AnimationGlobals.GoombellaBattleAnimations.TattleStartName);
                    CurSequenceAction = new WaitForAnimationSeqAction(AnimationGlobals.GoombellaBattleAnimations.TattleStartName);
                    break;
                case 1:
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
                    CurSequenceAction = new WaitSeqAction(EndWait);
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
                    //If the Action Command is enabled, start its input. Otherwise, Tattle automatically succeeds
                    //Goombario's Tattle doesn't have an Action Command while Goombella's does
                    if (CommandEnabled == true)
                    {
                        //Send in their actual draw position
                        actionCommand.StartInput(EntitiesAffected[0].DrawnPosition);
                    }
                    else
                    {
                        OnCommandSuccess();
                    }
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
                    //NOTE: Show dialogue bubble coming from Goombario/Goombella along with the enemy's tattle log entry (with image and stats)
                    string[] tattleDescriptions = TattledEntity.GetTattleDescription();

                    string tattle = "Tattle Description:";

                    //For now log it in the console
                    for (int i = 0; i < tattleDescriptions.Length; i++)
                    {
                        tattle += $"\n{tattleDescriptions[i]}";
                    }

                    Console.WriteLine(tattle);

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
                    User.AnimManager.PlayAnimation(AnimationGlobals.GoombellaBattleAnimations.TattleFailName);
                    CurSequenceAction = new WaitForAnimationSeqAction(AnimationGlobals.GoombellaBattleAnimations.TattleFailName);
                    ChangeSequenceBranch(SequenceBranch.End);
                    break;
                default:
                    PrintInvalidSequence();
                    break;
            }
        }

        //Tattle cannot miss since it doesn't actually interact with any BattleEntity in any way
        protected override void SequenceMissBranch()
        {
            switch (SequenceStep)
            {
                default:
                    PrintInvalidSequence();
                    break;
            }
        }
    }
}
