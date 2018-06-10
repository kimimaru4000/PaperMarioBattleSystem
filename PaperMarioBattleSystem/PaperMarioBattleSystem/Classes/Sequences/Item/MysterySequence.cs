using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using PaperMarioBattleSystem.Utilities;
using static PaperMarioBattleSystem.Enumerations;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Sequence for the Mystery item.
    /// Show a single-line slot which selects a random item out of a set, then perform that item's Sequence.
    /// </summary>
    public sealed class MysterySequence : ItemSequence
    {
        /// <summary>
        /// The item set from the Mystery item.
        /// </summary>
        private List<BattleItem> MysteryItemSet = null;

        /// <summary>
        /// The random BattleItem chosen from the Mystery item set.
        /// </summary>
        private BattleItem ItemChosen = null;

        private BattleEntity EntityUsing = null;
        
        public MysterySequence(ItemAction itemAction) : base(itemAction)
        {

        }
        
        protected override void OnStart()
        {
            base.OnStart();

            MysteryAction mysteryAction = (MysteryAction)itemAction;
            MysteryItemSet = mysteryAction.RevisedItemSet;

            EntityUsing = User;
        }

        protected override void OnEnd()
        {
            base.OnEnd();

            MysteryItemSet = null;
        }

        protected override void SequenceMainBranch()
        {
            switch (SequenceStep)
            {
                case 0:
                    //NOTE: For now simply choose a random item and exclude the roulette until we get this working
                    int randItemIndex = RandomGlobals.Randomizer.Next(0, MysteryItemSet.Count);
                    ItemChosen = MysteryItemSet[randItemIndex];

                    Debug.Log($"Chose {ItemChosen.Name} to use for the Mystery!");
                    CurSequenceAction = new WaitSeqAction(0d);
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
                    //Remove the Mystery from our inventory
                    Inventory.Instance.RemoveItem(itemAction.ItemUsed);

                    //Subtract a turn from this entity so they go again with the item action specified
                    EntityUsing.SetTurnsUsed(EntityUsing.TurnsUsed - 1);

                    //Add the event so the entity uses the item after ending this sequence
                    //This approach makes it possible to use another Mystery item if it was chosen from this one
                    EntityUsing.TurnStartEvent -= OnUserTurnStart;
                    EntityUsing.TurnStartEvent += OnUserTurnStart;

                    //End the sequence
                    EndSequence();
                    break;
                default:
                    break;
            }
        }

        //Event handler when the user's next turn is started
        private void OnUserTurnStart()
        {
            EntityUsing.TurnStartEvent -= OnUserTurnStart;

            Debug.Log($"Using {ItemChosen.Name} for {EntityUsing.Name}, which was received via Mystery!");

            //Clear the menu stack as the action will be selected automatically
            EntityUsing.BManager.battleUIManager.ClearMenuStack();

            //Immediately start using the item
            ItemAction itemChosenAction = ItemChosen.GetActionAssociated(User);

            //Special case: if the item chosen is a Mystery, initialize it
            //We can't do this earlier in Initialize, otherwise we run into an infinite loop
            if (itemChosenAction is MysteryAction)
                itemChosenAction.Initialize();

            //1. Find out if this item targets enemies or allies
            //2. If it targets allies, only use it on the entity using it
            //3. If it targets enemies, use it on the first enemy if it targets only one, otherwise use it on all enemies
            List<BattleEntity> entitiesAffected = new List<BattleEntity>(itemChosenAction.GetEntitiesMoveAffects());
            if (itemChosenAction.MoveProperties.SelectionType == Enumerations.EntitySelectionType.Single
                || itemChosenAction.MoveProperties.SelectionType == Enumerations.EntitySelectionType.First)
            {
                //If this selects the first or a single entity and it's an ally, make sure it always targets the entity using the Mystery
                //Examples include a Mushroom or Honey Syrup
                if (UtilityGlobals.MoveAffectionTypesHasFlag(itemChosenAction.MoveProperties.MoveAffectionType,
                    MoveAffectionTypes.Self | MoveAffectionTypes.Ally))
                {
                    entitiesAffected.Clear();
                    entitiesAffected.Add(EntityUsing);
                }

                //If it affects a first or single entity and we have more in the list, remove all but the first one
                //If this is a single target damaging item that targets enemies, it'll choose the first enemy (Ex. Egg Bomb)
                if (entitiesAffected.Count > 1)
                {
                    entitiesAffected.RemoveRange(1, entitiesAffected.Count - 1);
                }
            }

            //Start the second half of the sequence
            EntityUsing.StartAction(itemChosenAction, true, entitiesAffected.ToArray());

            EntityUsing = null;
            ItemChosen = null;
        }
    }
}
