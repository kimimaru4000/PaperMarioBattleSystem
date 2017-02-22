using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Sequence for PowerBounce.
    /// </summary>
    public sealed class PowerBounceSequence : JumpSequence
    {
        private int DamageValue = 0;
        private int Bounces = 0;
        
        /// <summary>
        /// Whether to decrease Power Bounce's damage or not.
        /// If the damage dealt was 1, this will be set to true to continue dealing 1 damage.
        /// </summary>
        private bool StopDecreasing = false;
        
        public override int DamageDealt => DamageValue;
        
        public PowerBounceSequence(MoveAction moveAction) : base(moveAction)
        {
            
        }

        protected override void OnStart()
        {
            base.OnStart();
        
            DamageValue = GetTotalDamage(BaseDamage);
            Bounces = 0;
        
            StopDecreasing = false;
        }
        
        protected override void OnEnd()
        {
            base.OnEnd();
        
            Bounces = 0;
        
            StopDecreasing = false;
        }
        
        protected override void CommandSuccess()
        {
            base.CommandSuccess();
        
            Bounces++;
        }

        protected override void SequenceSuccessBranch()
        {
            switch (SequenceStep)
            {
                case 0:
        
                    //Check the damage dealt
                    int[] damageValues = AttemptDamage(DamageDealt, EntitiesAffected, Action.DamageInfo.Value, true);
        
                    //If the total damage dealt was 1, stop decreasing the damage to keep it doing 1
                    if (StopDecreasing == false && damageValues[0] == 1)
                        StopDecreasing = true;
        
                    //Only decrease the value if we're not at 1 damage
                    if (StopDecreasing == false)
                        DamageValue = UtilityGlobals.Clamp(DamageValue - 1, BattleGlobals.MinDamage, BattleGlobals.MaxDamage);
        
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
                    AttemptDamage(DamageDealt, CurTarget, Action.DamageInfo.Value, true);
                    ChangeSequenceBranch(SequenceBranch.End);
                    break;
                default:
                    base.SequenceFailedBranch();
                    break;
            }
        }
    }
}
