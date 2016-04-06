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
    /// The base class for all battle menus
    /// </summary>
    public abstract class BattleMenu
    {
        #region Enumerations

        /// <summary>
        /// The type of menu this is for input. The Other value indicates that the menu has its own unique type of input
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

        protected BattleMenu(bool wrapCursor)
        {
            WrapCursor = wrapCursor;
        }

        /// <summary>
        /// The current selection in the menu
        /// </summary>
        protected int CurSelection = 0;

        /// <summary>
        /// The type of menu this is
        /// </summary>
        protected MenuTypes MenuType = MenuTypes.Other;

        /// <summary>
        /// Whether the cursor wraps around to the first item from the last and vice versa
        /// </summary>
        protected bool WrapCursor = false;

        /// <summary>
        /// The depth of the menu; deeper menus should have increasingly more depth
        /// </summary>
        public float MenuDepth = .1f;

        /// <summary>
        /// The last selection
        /// </summary>
        protected abstract int LastSelection { get; }

        #region Input-Related Methods

        protected void ChangeSelection(int amount)
        {
            if (WrapCursor == false)
            {
                CurSelection = HelperGlobals.Clamp(CurSelection + amount, 0, LastSelection);
            }
            else
            {
                CurSelection = HelperGlobals.Wrap(CurSelection + amount, 0, LastSelection);
            }

            OnSelectionChanged(CurSelection);
        }

        protected virtual void OnSelectionChanged(int newSelection)
        {
            
        }

        /// <summary>
        /// Handles input for the menu cursor
        /// </summary>
        protected virtual void HandleCursorInput()
        {
            Keys forwards = Keys.Down;
            Keys backwards = Keys.Up;

            if (MenuType == MenuTypes.Horizontal)
            {
                forwards = Keys.Right;
                backwards = Keys.Left;
            }

            if (Input.GetKeyDown(forwards)) ChangeSelection(1);
            else if (Input.GetKeyDown(backwards)) ChangeSelection(-1);
        }

        /// <summary>
        /// Handles input for selecting and backing out of the menu
        /// </summary>
        protected virtual void HandleSelectionInput()
        {
            if (Input.GetKeyDown(Keys.X)) OnBackOut();
            else if (Input.GetKeyDown(Keys.Z)) OnConfirm();
        }

        /// <summary>
        /// What occurs when the menu is backed out of
        /// </summary>
        protected virtual void OnBackOut()
        {
            
        }

        /// <summary>
        /// What occurs when an item is confirmed on the menu
        /// </summary>
        protected virtual void OnConfirm()
        {
            
        }

        #endregion

        /// <summary>
        /// Reads input for the BattleMenu.
        /// The default implementation is based off the type of menu it is
        /// </summary>
        public virtual void Update()
        {
            HandleCursorInput();
            HandleSelectionInput();
        }

        public virtual void Draw()
        {
            
        }
    }
}
