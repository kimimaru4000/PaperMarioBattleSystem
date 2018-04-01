using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

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
        /// Tells whether the Battle Menus are suppressed or not.
        /// When suppressed, they won't update or draw.
        /// </summary>
        private bool SuppressBattleMenus = false;

        private List<UIElement> BattleUIElements = new List<UIElement>();

        private Stack<BattleMenu> BattleMenus = null;

        private TargetSelectionMenu SelectionMenu = null;

        private BattleHUD battleHUD = null;

        /// <summary>
        /// The BattleMenu at the top of the stack
        /// </summary>
        public BattleMenu TopMenu
        {
            get
            {
                if (BattleMenus.Count == 0) return null;
                return BattleMenus.Peek();
            }            
        }

        private BattleUIManager()
        {
            BattleMenus = new Stack<BattleMenu>();
            SelectionMenu = new TargetSelectionMenu();
            battleHUD = new BattleHUD();
        }

        public void CleanUp()
        {
            SelectionMenu = null;

            instance = null;
        }

        public void StartTargetSelection(TargetSelectionMenu.OnSelection onSelection, TargetSelectionMenu.EntitySelectionType selectionType, params BattleEntity[] targets)
        {
            PushMenu(SelectionMenu);
            SelectionMenu.StartSelection(onSelection, selectionType, targets);
        }

        public void StartTargetSelection(TargetSelectionMenu.OnSelection onSelection, TargetSelectionMenu.EntitySelectionType selectionType, int startIndex, params BattleEntity[] targets)
        {
            PushMenu(SelectionMenu);
            SelectionMenu.StartSelection(onSelection, selectionType, startIndex, targets);
        }

        #region Battle Menu Stack

        public void PushMenu(BattleMenu menu)
        {
            BattleMenus.Push(menu);
        }

        public BattleMenu PopMenu()
        {
            return BattleMenus.Pop();
        }

        public void ClearMenuStack()
        {
            BattleMenus.Clear();
        }

        public void SuppressMenus()
        {
            SuppressBattleMenus = true;
        }

        public void UnsuppressMenus()
        {
            SuppressBattleMenus = false;
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
            return BattleUIElements.Remove(uiElement);
        }

        /// <summary>
        /// Returns all UIElements in a new list.
        /// </summary>
        /// <returns>A new list containing all the BattleUIElements.</returns>
        public List<UIElement> GetAllUIElements()
        {
            return new List<UIElement>(BattleUIElements);
        }

        #endregion

        public void Update()
        {
            if (SuppressBattleMenus == false)
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

            if (SuppressBattleMenus == false)
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
