using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Sequence for the Kissy-Kissy move.
    /// <para>Before latching on, the user will deal 0 damage to check for interruptions or a miss.</para>
    /// </summary>
    public class KissyKissySequence : Sequence
    {
        private double MoveTime = 600f;
        private double JumpTime = 300f;

        private int MaxAttacks = 1;
        private int NumAttacks = 1;

        private bool Missed = false;
        private bool Interrupted = false;

        private bool IsInfinite => (MaxAttacks <= BattleGlobals.InfiniteSuccessionAttacks);

        public KissyKissySequence(MoveAction moveAction, int numAttacks) : base(moveAction)
        {
            MaxAttacks = NumAttacks = numAttacks;
        }

        protected override void OnEnd()
        {
            base.OnEnd();

            Missed = false;
            Interrupted = false;
        }

        protected override bool OnMiss()
        {
            base.OnMiss();

            Missed = true;
            ChangeJumpBranch(SequenceBranch.Miss);

            return false;
        }

        protected override void OnInterruption(Enumerations.Elements element)
        {
            base.OnInterruption(element);

            Interrupted = true;
        }

        protected override void SequenceStartBranch()
        {
            switch (SequenceStep)
            {
                case 0:
                    //Move to the opponent
                    CurSequenceAction = new MoveToSeqAction(BattleManager.Instance.GetPositionInFront(EntitiesAffected[0], User.EntityType != Enumerations.EntityTypes.Enemy), MoveTime);
                    break;
                case 1:
                    //Jump up to their height
                    CurSequenceAction = new MoveToSeqAction(EntitiesAffected[0].BattlePosition + new Vector2(0f, 10f), JumpTime);
                    break;
                case 2:
                    //Fall down and latch
                    CurSequenceAction = new MoveAmountSeqAction(new Vector2(0f, -10f), JumpTime);
                    break;
                case 3:
                    //Override defensive actions as the latching shouldn't be guardable
                    DamageData damageData = Action.DamageProperties;
                    damageData.ContactType = Enumerations.ContactTypes.Latch;
                    damageData.Damage = 0;
                    damageData.DefensiveOverride = Enumerations.DefensiveActionTypes.Guard | Enumerations.DefensiveActionTypes.Superguard;

                    //Deal 0 damage
                    //If we go to the Main branch, then we can start the action command
                    //Otherwise an interruption or miss occurred, so handle that in those branches
                    AttemptDamage(0, EntitiesAffected[0], damageData, true);
                    ChangeSequenceBranch(SequenceBranch.Main);
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
                    //Start the action command if it's enabled
                    //Do not wait for it; continue the Sequence as normal until the Action Command is finished
                    //or the number of attacks is 0
                    if (CommandEnabled == true)
                    {
                        actionCommand.StartInput();
                    }

                    CurSequenceAction = new WaitSeqAction(0d);
                    break;
                case 1:

                    SequenceStep = 1;
                    //NOTE: Play an animation here and wait for it to finish
                    CurSequenceAction = new WaitSeqAction(1100d);
                    break;
                case 2:
                    //Deal damage and heal for the amount of damage dealt
                    InteractionResult[] interactions = AttemptDamage(BaseDamage, EntitiesAffected[0], Action.DamageProperties, false);

                    int damageDealt = 0;
                    if (interactions[0] != null)
                        damageDealt = interactions[0].VictimResult.TotalDamage;

                    //Don't heal if the damage was 0 or the attack missed or was interrupted somehow
                    if (damageDealt > 0 && Missed == false && Interrupted == false)
                    {
                        PerformHeal(new HealingData(damageDealt, 0, null), User);
                    }

                    CurSequenceAction = new WaitSeqAction(0d);
                    break;
                case 3:
                    //If not infinite, subtract from the number of attacks
                    if (IsInfinite == false)
                    {
                        NumAttacks--;
                    
                        //If the enemy didn't use up all of its attacks, go back to case 1 and start the attack again
                        if (NumAttacks > 0)
                        {
                            goto case 1;
                        }
                        else
                        {
                            //If all the attacks were used up, go to the end branch
                            ChangeSequenceBranch(SequenceBranch.End);
                        }
                    }
                    //If infinite, go back to case 1 to start the attack again
                    else
                    {
                        goto case 1;
                    }

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
                    //End the action command's input if it's currently going on
                    if (CommandEnabled == true && actionCommand.AcceptingInput == true)
                    {
                        actionCommand.EndInput();
                    }

                    //The entity goes back to its battle position
                    User.AnimManager.PlayAnimation(User.GetIdleAnim());
                    CurSequenceAction = new MoveToSeqAction(User.BattlePosition, MoveTime);
                    break;
                case 1:
                    EndSequence();
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
                    ChangeSequenceBranch(SequenceBranch.End);
                    break;
                default:
                    PrintInvalidSequence();
                    break;
            }
        }

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

        protected override void SequenceMissBranch()
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
