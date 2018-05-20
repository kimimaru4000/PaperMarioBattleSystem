using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The sequence for the Power Lift move.
    /// </summary>
    public sealed class PowerLiftSequence : CrystalStarMoveSequence
    {
        /// <summary>
        /// The number of turns the Attack and Defense stats are boosted for.
        /// </summary>
        private const int TurnsBoosted = 3;

        private double EndWaitTime = 1000d;

        /// <summary>
        /// The PowerLiftResponse containing information from the Power Lift Action Command.
        /// </summary>
        private ActionCommandGlobals.PowerLiftResponse Response = default(ActionCommandGlobals.PowerLiftResponse);

        private PowerLiftActionCommandUI PowerLiftUI = null;

        public PowerLiftSequence(MoveAction moveAction) : base(moveAction)
        {
        }

        protected override void OnStart()
        {
            base.OnStart();

            if (Action.CommandEnabled == true && Action.DrawActionCommandInfo == true)
            {
                PowerLiftUI = new PowerLiftActionCommandUI(actionCommand as PowerLiftCommand);
                BattleUIManager.Instance.AddUIElement(PowerLiftUI);
            }
        }

        protected override void OnEnd()
        {
            base.OnEnd();

            if (PowerLiftUI != null)
            {
                BattleUIManager.Instance.RemoveUIElement(PowerLiftUI);
                PowerLiftUI = null;
            }
        }

        public override void OnCommandResponse(in object response)
        {
            base.OnCommandResponse(response);

            Response = (ActionCommandGlobals.PowerLiftResponse)response;
        }

        protected override void SequenceMainBranch()
        {
            switch(SequenceStep)
            {
                case 0:
                    StartActionCommandInput();
                    CurSequenceAction = new WaitForCommandSeqAction(0d, actionCommand, CommandEnabled);
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
                    CurSequenceAction = new WaitSeqAction(EndWaitTime);
                    break;
                case 1:
                    //It's a success, so we know there was at least one boost
                    for (int i = 0; i < EntitiesAffected.Length; i++)
                    {
                        BattleEntity entity = EntitiesAffected[i];

                        //Inflict POWUp if we have Attack boosted
                        if (Response.AttackBoosted > 0)
                        {
                            entity.AfflictStatus(new POWUpStatus(Response.AttackBoosted, TurnsBoosted), true);
                        }

                        //Inflict DEFUp if we have Defense boosted
                        if (Response.DefenseBoosted > 0)
                        {
                            entity.AfflictStatus(new DEFUpStatus(Response.DefenseBoosted, TurnsBoosted), true);
                        }
                    }
                    ChangeSequenceBranch(SequenceBranch.End);
                    break;
                default:
                    PrintInvalidSequence();
                    break;
            }
        }

        //The only way to completely fail Power Lift is never increase any stat by at least one
        //It still spends the time as if it was boosting your stats but doesn't zoom in or anything; the characters just wait
        protected override void SequenceFailedBranch()
        {
            switch (SequenceStep)
            {
                case 0:
                    CurSequenceAction = new WaitSeqAction(EndWaitTime);
                    ChangeSequenceBranch(SequenceBranch.End);
                    break;
                default:
                    PrintInvalidSequence();
                    break;
            }
        }
    }
}
