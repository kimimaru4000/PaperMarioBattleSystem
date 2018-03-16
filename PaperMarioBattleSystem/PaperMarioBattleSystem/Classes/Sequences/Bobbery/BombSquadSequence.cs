using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Sequence for Bobbery's Bomb Squad.
    /// </summary>
    public sealed class BombSquadSequence : Sequence
    {
        /// <summary>
        /// The number of bombs to throw.
        /// </summary>
        private int BombCount = 3;

        private double WaitDur = 400d;
        
        public BombSquadSequence(MoveAction moveAction, int bombCount) : base(moveAction)
        {
            BombCount = bombCount;
        }

        public override void OnCommandResponse(in object response)
        {
            base.OnCommandResponse(response);

            ActionCommandGlobals.BombSquadResponse bombSquadResponse = (ActionCommandGlobals.BombSquadResponse)response;

            //Add the bomb to battle
            BobberyBomb bobberyBomb = new BobberyBomb(Action.DamageProperties.Damage);
            bobberyBomb.Position = User.Position;

            BattleManager.Instance.AddEntities(new BattleEntity[] { bobberyBomb }, null, true);

            //Add a battle event to shoot the bomb out
            //NOTE: Some values are temporary (Ex. ground position)
            BattleEventManager.Instance.QueueBattleEvent((int)BattleGlobals.BattleEventPriorities.BobberyBomb - 1,
                new BattleManager.BattleState[] { BattleManager.BattleState.Turn, BattleManager.BattleState.TurnEnd },
                new ShootBobberyBombBattleEvent(bobberyBomb, bombSquadResponse.ThrowVelocity,
                bombSquadResponse.Gravity, BattleManager.Instance.PartnerPos.Y + 1));
        }

        protected override void SequenceStartBranch()
        {
            switch (SequenceStep)
            {
                case 0:
                    CurSequenceAction = new WaitSeqAction(WaitDur);

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
                    CurSequenceAction = new WaitSeqAction(WaitDur);
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
                    StartActionCommandInput(User.Position);

                    //The Action Command should move onto the Success branch when it's done
                    CurSequenceAction = new WaitForCommandSeqAction(0d, actionCommand, CommandEnabled);
                    break;
                default:
                    PrintInvalidSequence();
                    break;
            }
        }

        //Success is called when all the bombs have been thrown
        protected override void SequenceSuccessBranch()
        {
            switch (SequenceStep)
            {
                case 0:
                    CurSequenceAction = new WaitSeqAction(0d);

                    ChangeSequenceBranch(SequenceBranch.End);
                    break;
                default:
                    PrintInvalidSequence();
                    break;
            }
        }

        //Bomb Squad can't fail, so do the same thing as a success
        protected override void SequenceFailedBranch()
        {
            switch (SequenceStep)
            {
                case 0:
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
                default:
                    PrintInvalidSequence();
                    break;
            }
        }
    }
}
