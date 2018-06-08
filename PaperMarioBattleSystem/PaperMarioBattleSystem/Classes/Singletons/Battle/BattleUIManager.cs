using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PaperMarioBattleSystem.Extensions;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// Manages UI during battle
    /// <para>This is a Singleton</para>
    /// </summary>
    public class BattleUIManager : IUpdateable, IDrawable, ICleanup
    {
        #region Singleton Fields

        public static BattleUIManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new BattleUIManager();
                }

                return instance;
            }
        }

        private static BattleUIManager instance = null;

        #endregion

        /// <summary>
        /// Tells whether the Input Menus are suppressed or not.
        /// When suppressed, they won't update or draw.
        /// </summary>
        private bool SuppressInputMenus = false;

        private List<UIElement> BattleUIElements = new List<UIElement>();

        private Stack<InputMenu> InputMenus = null;

        private TargetSelectionMenu SelectionMenu = null;

        public BattleHUD battleHUD { get; private set; }= null;

        /// <summary>
        /// The InputMenu at the top of the stack.
        /// </summary>
        public InputMenu TopMenu
        {
            get
            {
                if (InputMenus.Count == 0) return null;
                return InputMenus.Peek();
            }            
        }

        private BattleUIManager()
        {
            InputMenus = new Stack<InputMenu>();
            SelectionMenu = new TargetSelectionMenu();
            battleHUD = new BattleHUD(null);
        }

        public void CleanUp()
        {
            SelectionMenu = null;

            instance = null;
        }

        public void StartTargetSelection(TargetSelectionMenu.OnSelection onSelection, Enumerations.EntitySelectionType selectionType, params BattleEntity[] targets)
        {
            PushMenu(SelectionMenu);
            SelectionMenu.StartSelection(onSelection, selectionType, targets);
        }

        public void StartTargetSelection(TargetSelectionMenu.OnSelection onSelection, Enumerations.EntitySelectionType selectionType, int startIndex, params BattleEntity[] targets)
        {
            PushMenu(SelectionMenu);
            SelectionMenu.StartSelection(onSelection, selectionType, startIndex, targets);
        }

        #region Input Menu Stack

        public void PushMenu(InputMenu menu)
        {
            InputMenus.Push(menu);
        }

        public InputMenu PopMenu()
        {
            return InputMenus.Pop();
        }

        public void ClearMenuStack()
        {
            InputMenus.Clear();
        }

        public void SuppressMenus()
        {
            SuppressInputMenus = true;
        }

        public void UnsuppressMenus()
        {
            SuppressInputMenus = false;
        }

        #endregion

        #region Battle UI Elements

        public void AddUIElement(UIElement uiElement)
        {
            if (uiElement == null)
            {
                Debug.LogError($"{nameof(uiElement)} is null and won't be added to the list");
                return;
            }

            BattleUIElements.Add(uiElement);
        }

        public bool RemoveUIElement(UIElement uiElement)
        {
            bool removed = BattleUIElements.Remove(uiElement);
            if (removed == true)
            {
                uiElement?.CleanUp();
            }

            return removed;
        }

        /// <summary>
        /// Returns all UIElements in a new list.
        /// </summary>
        /// <returns>A new list containing all the BattleUIElements.</returns>
        public List<UIElement> GetAllUIElements()
        {
            return new List<UIElement>(BattleUIElements);
        }

        /// <summary>
        /// Puts all UIElements in a supplied list.
        /// </summary>
        /// <param name="uiElements">The list to put the UIElements into.</param>
        public void GetAllUIElements(List<UIElement> uiElements)
        {
            if (uiElements == null) return;

            uiElements.CopyFromList(BattleUIElements);
        }

        #endregion

        public void Update()
        {
            if (SuppressInputMenus == false)
            {
                TopMenu?.Update();
            }

            for (int i = 0; i < BattleUIElements.Count; i++)
            {
                BattleUIElements[i].Update();
            }
        }

        public void Draw()
        {
            battleHUD.Draw();

            if (SuppressInputMenus == false)
            {
                TopMenu?.Draw();
            }

            for (int i = 0; i < BattleUIElements.Count; i++)
            {
                BattleUIElements[i].Draw();
            }
        }
    }
}
