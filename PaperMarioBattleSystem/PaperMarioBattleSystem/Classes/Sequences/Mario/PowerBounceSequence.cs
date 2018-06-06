using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using PaperMarioBattleSystem.Utilities;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Sequence for PowerBounce.
    /// </summary>
    public sealed class PowerBounceSequence : JumpSequence
    {
        private int DamageValue = 0;
        private int Bounces = 0;
        
        public override int DamageDealt => DamageValue;
        
        public PowerBounceSequence(MoveAction moveAction) : base(moveAction)
        {
            
        }

        protected override void OnStart()
        {
            base.OnStart();
        
            DamageValue = GetTotalDamage(BaseDamage);
            Bounces = 0;
        }
        
        protected override void OnEnd()
        {
            base.OnEnd();
        
            Bounces = 0;
        }
        
        protected override void CommandSuccess()
        {
            base.CommandSuccess();
        
            Bounces++;
            SentRank = (ActionCommand.CommandRank)UtilityGlobals.Clamp((int)HighestCommandRank + 1, (int)ActionCommand.CommandRank.NiceM2, (int)ActionCommand.CommandRank.Excellent);
        }

        protected override void SequenceSuccessBranch()
        {
            switch (SequenceStep)
            {
                case 0:
                    base.SequenceSuccessBranch();
                    break;
                case 1:

                    /* NOTE: There's a minor bug with this caused by an edge case.
                     * 1. Exactly 1 damage is dealt to the victim, and the DamageValue is 1
                     * 2. The victim has a Weakness to the move used or the attacker has a Strength against at least one of the
                     * victim's PhysicalAttributes
                     * 3. The victim is hit with the move and has its Defense lowered
                     * 4. The result is the next hit's damage was not lowered and thus does 1 more damage than it should
                     * 
                     * This is easy to replicate by having Mario use Power Bounce on a standing Koopa Troopa that has a +1 Weakness to Normal.
                     * He will deal 1 damage to the Koopa Troopa, then on the next jump when the Koopa is flipped, he'll deal 2 damage
                     * rather than 1.
                     * 
                     * No enemies in the PM games that lose Defense on hit are weak to anything, so there's no way to observe how
                     * they handle the behavior. The current code should handle every case except this, so I feel it's not a big issue.
                     * 
                     * One potential way to fix this would be to manually calculate the total damage with the full interaction
                     * while observing changes in the victim's Defense. Then, adjust the DamageValue accordingly to ensure the proper
                     * amount of damage is dealt. With so many potential factors (Ex. making Power Bounce inflict statuses), it may be even
                     * more complex than this.
                     */

                    //Check the damage dealt
                    InteractionResult[] interactions = AttemptDamage(DamageDealt, EntitiesAffected, Action.DamageProperties, true);

                    int damage = 0;
                    if (interactions[0] != null)
                        damage = interactions[0].VictimResult.TotalDamage;

                    //Show VFX for the highest command rank
                    if (interactions[0] != null && interactions[0].WasVictimHit == true && interactions[0].WasAttackerHit == false)
                    {
                        ShowCommandRankVFX(HighestCommandRank, CurTarget.Position);
                    }

                    //If the attack's damage or the total damage dealt is greater than one, decrease the attack's damage by 1
                    //This ensures that we keep decreasing until the damage dealt is 1
                    //If the damage dealt is 0, it'll remain that way
                    if (DamageValue > 1 || damage > 1)
                        DamageValue--;
        
                    //Repeat the sequence if the player is under the Power Bounce cap
                    if (Bounces < BattleGlobals.MaxPowerBounces)
                    {
                        ChangeSequenceBranch(SequenceBranch.Main);
                    }
                    else
                    {
                        Debug.Log($"Reached Power Bounce limit with {Bounces} and real max is {BattleGlobals.MaxPowerBounces}!");
                        ChangeSequenceBranch(SequenceBranch.End);
                    }
                    break;
                default:
                    PrintInvalidSequence();
                    break;
            }
        }

        //NOTE: Needed to override this to specify that the damage dealt is the total damage, which Jump doesn't do
        protected override void SequenceFailedBranch()
        {
            switch (SequenceStep)
            {
                case 0:
                    base.SequenceFailedBranch();
                    break;
                case 1:
                    AttemptDamage(DamageDealt, CurTarget, Action.DamageProperties, true);
                    ChangeSequenceBranch(SequenceBranch.End);
                    break;
                default:
                    base.SequenceFailedBranch();
                    break;
            }
        }
    }
}
