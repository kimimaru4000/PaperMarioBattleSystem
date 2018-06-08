using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The base class for all battle menus.
    /// </summary>
    public abstract class BattleMenu : IUpdateable
    {
        #region Enumerations

        /// <summary>
        /// The type of menu this is. The Other value indicates that the menu has its own style.
        /// </summary>
        protected enum MenuTypes
        {
            Other, Vertical, Horizontal
        }

        #endregion

        protected BattleMenu()
        {

        }

        protected BattleMenu(MenuTypes menuType)
        {
            MenuType = menuType;
        }

        /// <summary>
        /// The type of menu this is.
        /// </summary>
        protected MenuTypes MenuType = MenuTypes.Other;
        
        public abstract void Update();

        /// <summary>
        /// A class for a generic menu option. It contains a text string and a delegate.
        /// </summary>
        public class BattleMenuOption
        {
            public delegate void OnSelect();

            public string Option = string.Empty;
            public OnSelect OnSelectOption = null;

            public BattleMenuOption(string option, OnSelect onSelectOption)
            {
                Option = option;
                OnSelectOption = onSelectOption;
            }
        }
    }
}
