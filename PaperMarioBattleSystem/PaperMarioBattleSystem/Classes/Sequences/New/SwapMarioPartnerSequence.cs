using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    public sealed class SwapMarioPartnerSequence : Sequence
    {
        public SwapMarioPartnerSequence(MoveAction moveAction) : base(moveAction)
        {

        }

        protected override void SequenceStartBranch()
        {
            switch (SequenceStep)
            {
                case 0:
                    CurSequenceAction = new WaitSeqAction(0d);
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
                    //Wait a bit
                    CurSequenceAction = new WaitSeqAction(300d);
                    break;
                case 1:
                    //Deal damage
                    InteractionResult[] interactions = AttemptDamage(BaseDamage, EntitiesAffected[0], Action.DamageProperties, false);

                    //Switch only if the victim was hit, damage was greater than 0, and the victim didn't perform any defensive actions
                    if (interactions[0] != null && interactions[0].WasVictimHit == true
                        && interactions[0].VictimResult.TotalDamage > 0
                        && interactions[0].VictimResult.DefensiveActionsPerformed == Enumerations.DefensiveActionTypes.None)
                    {
                        //Find a Partner to swap to
                        List<BattlePartner> allPartners = new List<BattlePartner>();
                        Inventory.Instance.partnerInventory.GetAllPartners(allPartners);

                        //Remove the current Partner from being selected
                        allPartners.Remove(BattleManager.Instance.Partner);

                        //If there's another Partner available, choose it
                        if (allPartners.Count != 0)
                        {
                            int randIndex = GeneralGlobals.Randomizer.Next(0, allPartners.Count);

                            SwapPartnerBattleEvent swapPartnerBattleEvent = new SwapPartnerBattleEvent(BattleManager.Instance.Partner, allPartners[randIndex],
                                300d, 300d);
                            BattleManager.Instance.battleEventManager.QueueBattleEvent((int)BattleGlobals.BattleEventPriorities.SwapPartner,
                                new BattleManager.BattleState[] { BattleManager.BattleState.Turn },
                                swapPartnerBattleEvent);
                        }
                    }

                    CurSequenceAction = new WaitSeqAction(0d);
                    EndSequence();
                    //ChangeSequenceBranch(SequenceBranch.End);
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
