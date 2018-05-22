using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// Goombario and Goombella's Tattle sequence.
    /// <para>Goombario's Tattle does not have an Action Command. If the Action Command is disabled this will always succeed.</para>
    /// </summary>
    public sealed class TattleSequence : Sequence
    {
        private const double WaitTime = 1000d;
        private const double ShowWait = 2000d;
        private const double WindowMoveTime = 600d;
        private const double WindowOpenCloseTime = 300d;
        private const double EndWait = 250d;

        private ITattleableEntity TattledEntity = null;

        /// <summary>
        /// The tattle box showing the BattleEntity tattled inside.
        /// </summary>
        private TattleRenderObj TattleBox = null;

        private TattleActionCommandUI TattleUI = null;

        public TattleSequence(MoveAction moveAction) : base(moveAction)
        {
            
        }

        protected override void OnStart()
        {
            base.OnStart();

            TattledEntity = (ITattleableEntity)EntitiesAffected[0];

            if (Action.CommandEnabled == true && Action.DrawActionCommandInfo == true)
            {
                TattleUI = new TattleActionCommandUI(actionCommand as TattleCommand);
                BattleUIManager.Instance.AddUIElement(TattleUI);
            }
        }

        protected override void OnEnd()
        {
            base.OnEnd();

            if (TattleBox != null)
            {
                BattleObjManager.Instance.RemoveBattleObject(TattleBox);

                TattleBox = null;
            }

            TattledEntity = null;

            if (TattleUI != null)
            {
                BattleUIManager.Instance.RemoveUIElement(TattleUI);
                TattleUI = null;
            }
        }

        protected override void SequenceStartBranch()
        {
            switch (SequenceStep)
            {
                case 0:
                    User.AnimManager.PlayAnimation(AnimationGlobals.GoombellaBattleAnimations.TattleStartName);
                    CurSequenceAction = new WaitForAnimationSeqAction(AnimationGlobals.GoombellaBattleAnimations.TattleStartName);
                    break;
                case 1:
                    CurSequenceAction = new WaitSeqAction(WaitTime);
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
                    User.AnimManager.PlayAnimation(User.GetIdleAnim());
                    CurSequenceAction = new WaitSeqAction(EndWait);
                    break;
                case 1:
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
                    //If the Action Command is enabled, start its input. Otherwise, Tattle automatically succeeds
                    //Goombario's Tattle doesn't have an Action Command while Goombella's does
                    if (CommandEnabled == true)
                    {
                        //Send in their actual draw position
                        actionCommand.StartInput(EntitiesAffected[0].DrawnPosition);
                    }
                    else
                    {
                        OnCommandSuccess();
                    }
                    CurSequenceAction = new WaitForCommandSeqAction(500d, actionCommand, CommandEnabled);
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
                    if (CommandEnabled == true)
                    {
                        ShowCommandRankVFX(HighestCommandRank, EntitiesAffected[0].Position);
                    }

                    //Create the tattle box and add it so it updates
                    TattleBox = new TattleRenderObj(Camera.Instance.SpriteToUIPos(EntitiesAffected[0].DrawnPosition));
                    BattleObjManager.Instance.AddBattleObject(TattleBox);

                    string entityName = EntitiesAffected[0].Name;

                    //Check if the enemy is in the Tattle database
                    bool inDatabase = TattleDatabase.HasTattleDescription(entityName);

                    //Add the Tattle information to the player's Tattles if the enemy isn't in the database
                    if (inDatabase == false)
                    {
                        TattleDatabase.AddTattleLogEntry(entityName, TattledEntity.GetTattleLogEntry());
                        TattleDatabase.AddTattleDescriptionEntry(entityName, TattledEntity.GetTattleDescription());

                        //Mark the enemies to show their HP
                        BattleEntity[] entities = BattleManager.Instance.GetEntities(Enumerations.EntityTypes.Enemy, null);
                        for (int i = 0; i < entities.Length; i++)
                        {
                            if (entities[i].Name == entityName)
                            {
                                entities[i].AddShowHPProperty();
                            }
                        }
                    }

                    //Start moving the tattle box down
                    TattleBox.Start(WindowMoveTime);

                    CurSequenceAction = new WaitSeqAction(WindowMoveTime);

                    break;
                case 1:
                    //Open the tattle box to show the BattleEntity tattled
                    TattleBox.Open(WindowOpenCloseTime);

                    CurSequenceAction = new WaitSeqAction(WindowOpenCloseTime);
                    break;
                case 2:
                    //NOTE: Show dialogue bubble coming from Goombario/Goombella along with the enemy's Tattle log entry (with image and stats)
                    string tattleDescription = TattledEntity.GetTattleDescription();

                    string tattle = "Tattle Description:\n" + tattleDescription;

                    //Log it
                    Debug.Log(tattle);

                    //Create the dialogue bubble
                    DialogueManager.Instance.CreateBubble(tattleDescription, User);
                    CurSequenceAction = new WaitForDialogueSeqAction(DialogueManager.Instance.CurDialogueBubble);
                    break;
                //case 3:
                //    //Wait
                //    CurSequenceAction = new WaitSeqAction(ShowWait);
                //    break;
                case 3:
                    //Close the tattle box
                    TattleBox.Close(WindowOpenCloseTime);

                    CurSequenceAction = new WaitSeqAction(WindowOpenCloseTime);
                    break;
                case 4:
                    //Move the tattle box offscreen
                    TattleBox.End(WindowMoveTime);

                    CurSequenceAction = new WaitSeqAction(WindowMoveTime);
                    break;
                case 5:
                    //Remove the tattle box
                    BattleObjManager.Instance.RemoveBattleObject(TattleBox);
                    TattleBox = null;

                    CurSequenceAction = new WaitSeqAction(0d);
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
                    User.AnimManager.PlayAnimation(AnimationGlobals.GoombellaBattleAnimations.TattleFailName);
                    CurSequenceAction = new WaitForAnimationSeqAction(AnimationGlobals.GoombellaBattleAnimations.TattleFailName);
                    ChangeSequenceBranch(SequenceBranch.End);
                    break;
                default:
                    PrintInvalidSequence();
                    break;
            }
        }

        //Tattle cannot miss since it doesn't actually interact with any BattleEntity in any way
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
