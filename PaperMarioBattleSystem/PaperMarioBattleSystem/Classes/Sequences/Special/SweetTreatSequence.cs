using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Sequence for Sweet Treat.
    /// </summary>
    public class SweetTreatSequence : CrystalStarMoveSequence
    {
        private double WaitDur = 800d;

        protected ActionCommandGlobals.SweetTreatResponse Response = default(ActionCommandGlobals.SweetTreatResponse);

        public SweetTreatSequence(MoveAction moveAction) : base(moveAction)
        {
            
        }

        public override void OnCommandResponse(in object response)
        {
            base.OnCommandResponse(response);

            Response = (ActionCommandGlobals.SweetTreatResponse)response;
        }

        protected override void SequenceMainBranch()
        {
            switch (SequenceStep)
            {
                case 0:
                    StartActionCommandInput(User.Position);

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
                    //Heal HP and FP based on how well you performed, but heal the Status Effects no matter what
                    //The Status Effects cured should be defined in the move information
                    HealingData marioHealing = new HealingData(Response.MarioHPRestored, Response.FPRestored, Action.HealingInfo.Value.StatusEffectsHealed);
                    HealingData partnerHealing = new HealingData(Response.PartnerHPRestored, 0, Action.HealingInfo.Value.StatusEffectsHealed);

                    PerformHeal(marioHealing, EntitiesAffected[0]);
                    PerformHeal(partnerHealing, EntitiesAffected[1]);

                    ChangeSequenceBranch(SequenceBranch.End);
                    break;
                case 1:
                    //Wait a bit
                    CurSequenceAction = new WaitSeqAction(WaitDur);
                    break;
                default:
                    PrintInvalidSequence();
                    break;
            }
        }

        //Sweet Treat can't fail, but trying to perform it without an Action Command would most likely do nothing
        protected override void SequenceFailedBranch()
        {
            switch (SequenceStep)
            {
                case 0:
                    ChangeSequenceBranch(SequenceBranch.End);
                    break;
                default:
                    PrintInvalidSequence();
                    break;
            }
        }
    }
}
