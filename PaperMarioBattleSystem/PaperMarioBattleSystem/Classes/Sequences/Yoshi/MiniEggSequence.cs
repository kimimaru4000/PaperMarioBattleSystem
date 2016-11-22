using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Sequence for Yoshi's Mini-Egg.
    /// </summary>
    public sealed class MiniEggSequence : Sequence
    {
        private int EggsToThrow = 1;
        private float MoveDuration = 500f;
        private double EggWait = 500d;

        //Mini-Egg will only do 1 damage each egg.
        //However, Power Plus P will increase Yoshi's Attack, causing the eggs to do more damage
        private int EggDamage = 1;

        public MiniEggSequence(MoveAction moveAction) : base(moveAction)
        {

        }

        protected override void OnStart()
        {
            EggDamage = GetTotalDamage(1);
        }

        //Do nothing for success for failure, as only the number of eggs thrown, which is received from the response, changes
        protected override void CommandSuccess()
        {
            
        }

        protected override void CommandFailed()
        {
            
        }

        public override void OnCommandResponse(object response)
        {
            User.PlayAnimation(AnimationGlobals.YoshiBattleAnimations.EggLayName);
            EggsToThrow = 1 + (int)response;
        }

        protected override void SequenceStartBranch()
        {
            switch (SequenceStep)
            {
                case 0:
                    User.PlayAnimation(AnimationGlobals.RunningName);
                    CurSequenceAction = new MoveTo(BattleManager.Instance.GetPositionInFront(BattleManager.Instance.GetFrontPlayer()), MoveDuration);
                    break;
                case 1:
                    User.PlayAnimation(AnimationGlobals.IdleName);
                    CurSequenceAction = new Wait(0f);
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
                    StartActionCommandInput();
                    CurSequenceAction = new WaitForCommand(10000d, actionCommand, CommandEnabled);
                    break;
                case 1:
                    CurSequenceAction = new Wait(EggWait);
                    break;
                default:
                    //Throw an egg, then wait
                    ThrowEgg();
                    EggsToThrow--;

                    CurSequenceAction = new Wait(500d);

                    //If there are no more eggs to throw, switch to the end
                    if (EggsToThrow <= 0)
                    {
                        ChangeSequenceBranch(SequenceBranch.End);
                    }

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
                    User.PlayAnimation(AnimationGlobals.RunningName);
                    CurSequenceAction = new MoveTo(User.BattlePosition, MoveDuration);
                    break;
                case 1:
                    User.PlayAnimation(AnimationGlobals.IdleName);
                    EndSequence();
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
                default:
                    PrintInvalidSequence();
                    break;
            }
        }

        private void ThrowEgg()
        {
            int rand = GeneralGlobals.Randomizer.Next(0, EntitiesAffected.Length);
            BattleEntity target = EntitiesAffected[rand];
            AttemptDamage(EggDamage, target, true);
        }
    }
}
