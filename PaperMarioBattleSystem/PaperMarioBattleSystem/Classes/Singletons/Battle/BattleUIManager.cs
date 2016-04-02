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
    public class BattleUIManager
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

        private Stack<BattleMenu> BattleMenus = null;

        private TargetSelectionMenu SelectionMenu = null;

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
        }

        public void Dispose()
        {
            SelectionMenu = null;

            instance = null;
        }

        public void StartTargetSelection(TargetSelectionMenu.OnSelection onSelection, TargetSelectionMenu.EntitySelectionType selectionType, params BattleEntity[] targets)
        {
            PushMenu(SelectionMenu);
            SelectionMenu.StartSelection(onSelection, selectionType, targets);
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

        #endregion

        public void Update()
        {
            TopMenu?.Update();
        }

        public void Draw()
        {
            TopMenu?.Draw();
        }
    }
}
