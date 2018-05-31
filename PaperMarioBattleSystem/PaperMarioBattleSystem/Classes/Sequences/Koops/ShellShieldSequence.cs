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
    /// The Sequence for Koops' Shell Shield.
    /// </summary>
    public class ShellShieldSequence : Sequence
    {
        /// <summary>
        /// The amount of HP the Shell has.
        /// </summary>
        protected int ShellHP = 2;

        /// <summary>
        /// The amount of Max HP the Shell has.
        /// </summary>
        protected int ShellMaxHP = 8;

        protected double WaitDur = 800d;
        protected double MoveDur = 600d;
        protected float ShellYHeight = -25f;

        private Shell ShellCreated = null;

        private ShellShieldActionCommandUI ShellShieldUI = null;

        public ShellShieldSequence(MoveAction moveAction, int maxShellHP) : base(moveAction)
        {
            ShellMaxHP = maxShellHP;
        }

        protected override void OnStart()
        {
            base.OnStart();

            ShellCreated = null;

            if (Action.CommandEnabled == true && Action.DrawActionCommandInfo == true)
            {
                ShellShieldUI = new ShellShieldActionCommandUI(actionCommand as ShellShieldCommand);
                BattleUIManager.Instance.AddUIElement(ShellShieldUI);
            }
        }

        protected override void OnEnd()
        {
            base.OnEnd();

            ShellCreated = null;

            if (ShellShieldUI != null)
            {
                BattleUIManager.Instance.RemoveUIElement(ShellShieldUI);
                ShellShieldUI = null;
            }
        }

        //Do nothing for success or failure, as only the HP for the Shell, which is received from the response, changes
        protected override void CommandSuccess()
        {
            
        }

        protected override void CommandFailed()
        {
            
        }

        public override void OnCommandResponse(in object response)
        {
            ShellHP = (int)response;
        }

        protected override void SequenceStartBranch()
        {
            switch (SequenceStep)
            {
                case 0:
                    User.AnimManager.PlayAnimation(AnimationGlobals.KoopsBattleAnimations.ShellSummonName);

                    CurSequenceAction = new WaitSeqAction(WaitDur);

                    //Check if the entity doesn't have a shell
                    //If so, move on, otherwise remove it first
                    if (EntitiesAffected[0].EntityProperties.HasAdditionalProperty(Enumerations.AdditionalProperty.DefendedByEntity) == false)
                    {
                        ChangeSequenceBranch(SequenceBranch.Main);
                    }
                    break;
                case 1:
                    /* 1. Add property to entity to know who is defending it
                     * 2. When defender dies, remove it
                     * 3. When entity dies (for good), remove it
                     * 4. Ability to remove it any time as well
                     * 
                     * This seems too complex for a simple property, so what else can we do?
                     * 
                     * Properties should be able to be added and removed easily, so have the property simply work while the means of
                     * adding and removing it is handled by the source.
                     * 
                     * In other words, have the Shell itself handle adding and removing the property.
                     * 
                     * If some other means of defending (Ex. via Item or Badge) is implemented, then those sources will need to handle adding and removing the property.
                    */

                    //Remove the Shell and take it out of battle
                    //NOTE: This can be problematic if the BattleEntity is something other than the previous Shell!
                    BattleEntity prevShell = EntitiesAffected[0].EntityProperties.GetAdditionalProperty<BattleEntity>(Enumerations.AdditionalProperty.DefendedByEntity);
                    User.BManager.RemoveEntity(prevShell, true);

                    CurSequenceAction = new WaitSeqAction(0d);

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
                    StartActionCommandInput(User);

                    CurSequenceAction = new WaitForCommandSeqAction(500d, actionCommand, CommandEnabled);
                    break;
                case 1:
                    if (CommandResult == ActionCommand.CommandResults.Success)
                    {
                        ShowCommandRankVFX(HighestCommandRank, EntitiesAffected[0].Position);
                    }

                    //Create the Shell and set its position data
                    ShellCreated = new Shell(ShellHP, ShellMaxHP);
                    ShellCreated.SetBattlePosition(EntitiesAffected[0].BattlePosition);
                    ShellCreated.Position = EntitiesAffected[0].Position + new Vector2(0f, ShellYHeight);

                    //Tell the Shell to defend the target and add the Shell to battle
                    ShellCreated.SetEntityToDefend(EntitiesAffected[0]);
                    User.BManager.AddEntities(new BattleEntity[] { ShellCreated }, null, true);

                    CurSequenceAction = new WaitSeqAction(WaitDur / 2d);
                    break;
                case 2:
                    //Move the Shell down
                    CurSequenceAction = new MoveToSeqAction(ShellCreated, EntitiesAffected[0].Position, MoveDur, Interpolation.InterpolationTypes.QuadOut, Interpolation.InterpolationTypes.QuadOut);

                    ChangeSequenceBranch(SequenceBranch.End);
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
                    //Play the idle animation and wait briefly
                    User.AnimManager.PlayAnimation(User.GetIdleAnim());

                    CurSequenceAction = new WaitSeqAction(0d);
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
                default:
                    PrintInvalidSequence();
                    break;
            }
        }

        //The Action Command can fail by waiting too long, but you still get the shell
        protected override void SequenceFailedBranch()
        {
            switch (SequenceStep)
            {
                default:
                    PrintInvalidSequence();
                    break;
            }
        }

        //Shell Shield cannot miss
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
