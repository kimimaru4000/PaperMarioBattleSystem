using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PaperMarioBattleSystem.Utilities;

namespace PaperMarioBattleSystem
{
    public abstract class InputMenu : BattleMenu
    {
        /// <summary>
        /// The current selection in the menu.
        /// </summary>
        protected int CurSelection = 0;

        /// <summary>
        /// Whether the cursor wraps around to the first item from the last and vice versa.
        /// </summary>
        protected bool WrapCursor = false;

        /// <summary>
        /// The last selection in the menu.
        /// </summary>
        protected abstract int LastSelection { get; }

        protected InputMenu(MenuTypes menuType) : base(menuType)
        {

        }

        #region Input-Related Methods

        protected void ChangeSelection(int amount)
        {
            if (WrapCursor == false)
            {
                CurSelection = UtilityGlobals.Clamp(CurSelection + amount, 0, LastSelection);
            }
            else
            {
                CurSelection = UtilityGlobals.Wrap(CurSelection + amount, 0, LastSelection);
            }

            OnSelectionChanged(CurSelection);
        }

        protected virtual void OnSelectionChanged(int newSelection)
        {

        }

        /// <summary>
        /// Handles input for the menu cursor.
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
        /// Handles input for selecting and backing out of the menu.
        /// </summary>
        protected virtual void HandleSelectionInput()
        {
            if (Input.GetKeyDown(Keys.X)) OnBackOut();
            else if (Input.GetKeyDown(Keys.Z)) OnConfirm();
        }

        /// <summary>
        /// What occurs when the menu is backed out of.
        /// </summary>
        protected virtual void OnBackOut()
        {

        }

        /// <summary>
        /// What occurs when an item is confirmed on the menu.
        /// </summary>
        protected virtual void OnConfirm()
        {

        }

        #endregion

        /// <summary>
        /// Reads input for the InputMenu.
        /// The default implementation is based off the type of menu it is.
        /// </summary>
        public override void Update()
        {
            HandleCursorInput();
            HandleSelectionInput();
        }

        public virtual void Draw()
        {

        }
    }
}
