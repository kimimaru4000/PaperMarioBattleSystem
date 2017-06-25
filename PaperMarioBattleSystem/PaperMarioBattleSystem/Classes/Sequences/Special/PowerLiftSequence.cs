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
        /// The PowerLiftResponse containing information from the Power Lift Action Command.
        /// </summary>
        private ActionCommandGlobals.PowerLiftResponse Response = default(ActionCommandGlobals.PowerLiftResponse);

        public PowerLiftSequence(SpecialMoveAction specialAction) : base(specialAction)
        {
        }

        public override void OnCommandResponse(object response)
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
                default:
                    PrintInvalidSequence();
                    break;
            }
        }
    }
}
