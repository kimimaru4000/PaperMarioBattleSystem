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
        public ItemSubMenu()
        {
            Position = new Vector2(230, 150);

            Item[] usableItems = Inventory.Instance.FindItems(Item.ItemTypes.Healing, Item.ItemTypes.Damage);
            for (int i = 0; i < usableItems.Length; i++)
            {
                //This cast fails if the Item doesn't derive from BattleItem
                //This can also happen if an Item that can't be used in battle had its ItemType set to the wrong value
                BattleItem item = (BattleItem)usableItems[i];

                ItemAction newItemAction = new ItemAction(item);
                BattleActions.Add(newItemAction);
            }

            if (BattleActions.Count == 0)
            {
                //Add the No Items action, which, when selected, brings up a battle message saying "You can't select that!"
                //(these messages aren't implemented yet) and brings you back to the menu.
                //This happens even with Double Dip and Triple Dip, essentially forcing you to stop using items.

                //Aside: For those Badges, we can have a condensed battle menu show up for Mario or his Partner
                //to prevent the normal one from showing up

                MoveAction noItems = new MoveAction("No Items", new MoveActionData(null, 0, "You have no items.", TargetSelectionMenu.EntitySelectionType.First, Enumerations.EntityTypes.Player, false, null), new NoSequence(null));
                BattleActions.Add(noItems);
            }
        }
    }
}
