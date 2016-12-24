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
        }
    }
}
