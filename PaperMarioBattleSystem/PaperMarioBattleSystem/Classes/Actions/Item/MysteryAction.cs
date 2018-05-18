using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PaperMarioBattleSystem.Utilities;
using static PaperMarioBattleSystem.Enumerations;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The MoveAction for the Mystery Item.
    /// </summary>
    public class MysteryAction : ItemAction
    {
        /// <summary>
        /// The revised item set from the Mystery item.
        /// If items that the Mystery can choose are unable to be used, such as damaging items in dark battles, then they will be omitted from the wheel.
        /// </summary>
        public List<BattleItem> RevisedItemSet = new List<BattleItem>();

        public MysteryAction(BattleItem item) : base(item)
        {
            
        }

        protected override void SetActionProperties()
        {
            Name = ItemUsed.Name;

            MoveInfo = new MoveActionData(ItemUsed.Icon, ItemUsed.Description, MoveResourceTypes.FP, 0, CostDisplayTypes.Hidden,
                MoveAffectionTypes.Self, ItemUsed.SelectionType, false, ItemUsed.HeightsAffected);
        }

        public override void Initialize()
        {
            //Check if any of the items can target anyone
            Mystery mystery = (Mystery)ItemUsed;

            BattleItem[] mysteryItems = mystery.GetItemSet();

            if (mysteryItems != null)
            {
                for (int i = 0; i < mysteryItems.Length; i++)
                {
                    ItemAction itemAction = mysteryItems[i].ActionAssociated;
                    
                    //If the item can target anyone, then add it to the revised set
                    BattleEntity[] entities = itemAction.GetEntitiesMoveAffects();
                    if (entities != null && entities.Length > 0)
                    {
                        RevisedItemSet.Add(mysteryItems[i]);
                    }
                    else
                    {
                        Debug.Log($"Mystery item: {mysteryItems[i].Name} is excluded from the revised item set because it can't target anyone!");
                    }
                }
            }

            //If no items in the Mystery can target anyone, disable the Mystery itself
            if (UtilityGlobals.IListIsNullOrEmpty(RevisedItemSet) == true)
            {
                Disabled = true;
                DisabledString = "There's no one this move can target!";
            }
        }

        public override void OnActionEnded()
        {
            base.OnActionEnded();

            RevisedItemSet.Clear();
        }
    }
}
