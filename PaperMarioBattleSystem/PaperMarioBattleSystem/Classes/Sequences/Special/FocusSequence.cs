using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Sequence for the Focus move.
    /// </summary>
    public sealed class FocusSequence : StarSpiritMoveSequence
    {
        public FocusSequence(MoveAction moveAction) : base(moveAction)
        {

        }

        protected override void SequenceMainBranch()
        {
            switch (SequenceStep)
            {
                case 0:
                    if (User.EntityType == Enumerations.EntityTypes.Player)
                    {
                        //Increase Star Spirit Star Power
                        BattlePlayer player = (BattlePlayer)User;
                        StarPowerBase starPower = player.GetStarPower(StarPowerGlobals.StarPowerTypes.StarSpirit);
                        if (starPower == null)
                        {
                            Debug.LogError($"Somehow, {nameof(starPower)} is null from {nameof(BattlePlayer.GetStarPower)}. Fix this.");
                        }
                        else
                        {
                            //Base gain from Focus
                            float spuGained = StarPowerGlobals.FocusSPUGain;

                            //Check how many Deep Focus badges are equipped
                            int deepFocusEquipped = User.GetEquippedBadgeCount(BadgeGlobals.BadgeTypes.DeepFocus);
                            
                            //Add the gain from each Deep Focus badge onto the total gain
                            spuGained += (deepFocusEquipped * StarPowerGlobals.DeepFocusSPUIncrease);

                            //Give Star Power
                            starPower.GainStarPower(spuGained);
                        }
                    }
                    else
                    {
                        Debug.LogWarning($"{User.Name} is not a {nameof(BattlePlayer)} and has no Star Power reference to increase.");
                    }

                    ChangeSequenceBranch(SequenceBranch.End);
                    break;
                default:
                    PrintInvalidSequence();
                    break;
            }
        }
    }
}
