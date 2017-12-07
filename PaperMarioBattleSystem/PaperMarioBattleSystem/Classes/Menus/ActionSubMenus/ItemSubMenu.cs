using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The menu that shows Mario's list of Items.
    /// You can select one to use.
    /// </summary>
    public class ItemSubMenu : ActionSubMenu
    {
        /// <summary>
        /// The turn count that represents how many items you can use in this menu.
        /// <para>Double and Triple Dip allow 2 and 3 items, respectively.</para>
        /// </summary>
        private int DipTurnCount = 0;

        /// <summary>
        /// The amount of FP it costs to use an item.
        /// <para>Double and Triple Dip cost 4 and 8 FP, respectively.</para>
        /// </summary>
        private int FPCost = 0;

        /// <summary>
        /// Tells if the menu is the root menu, replacing the entity's typical battle menu after using Double or Triple Dip.
        /// If true, will cause a prompt to show up after attempting to back out of the menu.
        /// </summary>
        private bool IsRootMenu = false;

        /// <summary>
        /// Creates an ItemSubMenu.
        /// </summary>
        /// <param name="dipTurnCount">The number of item turns. This is used for Double/Triple Dip.</param>
        /// <param name="fpCost">The amount of FP it costs to use an item. This is used for Double/Triple Dip.</param>
        /// <param name="isRootMenu">Tells if the ItemSubMenu is the root menu. This is only true if Double/Triple Dip is used.</param>
        public ItemSubMenu(int dipTurnCount, int fpCost, bool isRootMenu = false)
        {
            Position = new Vector2(230, 150);

            DipTurnCount = dipTurnCount;
            FPCost = fpCost;
            IsRootMenu = isRootMenu;

            Item[] usableItems = Inventory.Instance.FindItems(Item.ItemCategories.Standard, 
                Item.ItemTypes.Healing | Item.ItemTypes.Damage | Item.ItemTypes.Status | Item.ItemTypes.Revival);
            for (int i = 0; i < usableItems.Length; i++)
            {
                //This cast fails if the Item doesn't derive from BattleItem
                //This can also happen if an Item that can't be used in battle had its ItemType set to the wrong value
                BattleItem item = (BattleItem)usableItems[i];

                //Set item properties
                ItemAction newItemAction = item.ActionAssociated;
                newItemAction.SetDipFPCost(FPCost);
                //Set the item turn count
                if (dipTurnCount > 1)
                {
                    newItemAction.SetOnItemUsed(SetEntityDipTurnCount);
                }

                BattleActions.Add(newItemAction);
            }

            if (BattleActions.Count == 0)
            {
                //Add the No Items action, which, when selected, brings up a battle message saying "You can't select that!"
                //and brings you back to the menu. This happens even with Double Dip and Triple Dip, essentially forcing you
                //to stop using items.

                MessageAction noItems = new MessageAction("No Items", null, "You have no items.",
                    (int)BattleGlobals.StartEventPriorities.Message, "You can't select that!");
                BattleActions.Add(noItems);
            }

            //Initialize here if this is the root menu, as it won't be initialized like it normally is
            if (IsRootMenu == true)
            {
                MoveCategory = Enumerations.MoveCategories.Item;
                Initialize();
            }
        }

        protected override void OnBackOut()
        {
            if (IsRootMenu == false)
            {
                base.OnBackOut();
            }
            else
            {
                //Push the CancelDipMenu
                BattleUIManager.Instance.PushMenu(new CancelDipMenu());
            }
        }

        private void SetEntityDipTurnCount()
        {
            BattleManager.Instance.EntityTurn.EntityProperties.AddAdditionalProperty(Enumerations.AdditionalProperty.DipItemTurns, DipTurnCount);
        }
    }
}
