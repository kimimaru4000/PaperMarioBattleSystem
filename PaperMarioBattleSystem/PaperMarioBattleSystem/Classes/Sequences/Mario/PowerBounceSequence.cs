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

                    //If the total damage dealt was 1, stop decreasing the damage to keep it doing 1
                    if (StopDecreasing == false && damage == 1)
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
