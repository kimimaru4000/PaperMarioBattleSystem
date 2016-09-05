using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PaperMarioBattleSystem
{
    public sealed class PowerBounce : Jump
    {
        private int DamageValue = 0;
        private int Bounces = 0;
        
        /// <summary>
        /// Whether to decrease Power Bounce's damage or not.
        /// If the damage dealt was 1, this will be set to true to continue dealing 1 damage.
        /// </summary>
        private bool StopDecreasing = false;

        protected override int DamageDealt => DamageValue;

        public PowerBounce()
        {
            Name = "Power Bounce";
            Description = "Bounce multiple times on an enemy";
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

        public override void OnCommandSuccess()
        {
            base.OnCommandSuccess();

            Bounces++;
        }

        protected override void SequenceSuccessBranch()
        {
            switch (SequenceStep)
            {
                case 0:

                    //Check the damage dealt
                    int[] damageValues = AttemptDamage(DamageDealt, EntitiesAffected);

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
    }
}
