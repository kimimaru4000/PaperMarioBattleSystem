using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The menu shown after selecting Items when the BattleEntity has the Double and/or Triple Dip Badge equipped.
    /// </summary>
    public sealed class ItemDipSubMenu : ActionSubMenu
    {
        public ItemDipSubMenu()
        {
            Position = new Vector2(230, 150);

            int doubleDipCount = BattleManager.Instance.EntityTurn.GetEquippedBadgeCount(BadgeGlobals.BadgeTypes.DoubleDip);
            int tripleDipCount = BattleManager.Instance.EntityTurn.GetEquippedBadgeCount(BadgeGlobals.BadgeTypes.TripleDip);

            BattleActions.Add(new MenuAction("Items", null, "Select items to use in Battle.", new ItemSubMenu(1, 0)));

            //Add Double Dip
            if (doubleDipCount > 0)
            {
                BattleActions.Add(new MenuAction("Double Dip", null, "Lets you use 2 items in one turn.", 4, new ItemSubMenu(2, 4)));
            }

            //Add Triple Dip
            if (doubleDipCount > 1 || tripleDipCount > 0)
            {
                BattleActions.Add(new MenuAction("Triple Dip", null, "Lets you use 3 items in one turn.", 8, new ItemSubMenu(3, 8)));
            }
        }
    }
}
