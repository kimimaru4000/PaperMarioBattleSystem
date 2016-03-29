using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// A menu for the BattleActions relating to a ActionMenu. For example, any Jump actions would be in the ActionSubMenu for Jump.
    /// ActionSubMenus can lead to more ActionSubMenus, as is the case with "Change Partner"
    /// This is only used by the player
    /// <para>All ActionSubMenus have the "SubMenu" suffix</para>
    /// </summary>
    public abstract class ActionSubMenu
    {
        /*IMPORTANT
        * Make sure the UI supports deeper submenus. For example, the "Change Partner" option in "Strategies"
        * brings up the partner menu. Selecting a partner on that menu then switches partners
        */

        /// <summary>
        /// The list of battle actions in the submenu
        /// </summary>
        public List<BattleAction> BattleActions { get; protected set; } = new List<BattleAction>();

        /// <summary>
        /// Current action selected
        /// </summary>
        protected int CurSelection = 0;

        protected ActionSubMenu()
        {
            
        }

        protected ActionSubMenu(List<BattleAction> battleActions)
        {
            Initialize(battleActions);
        }

        protected void ChangeSelection(int amount)
        {
            CurSelection = HelperGlobals.Clamp(CurSelection + amount, 0, BattleActions.Count - 1);
        }

        public void Initialize(List<BattleAction> battleActions)
        {
            BattleActions = battleActions;
        }
        
        /// <summary>
        /// Closes the submenu
        /// </summary>
        public void Close()
        {
            CurSelection = 0;
        }

        public void Update()
        {
            //Selection
            if (Input.GetKeyDown(Keys.Up))
            {
                ChangeSelection(-1);
            }
            else if (Input.GetKeyDown(Keys.Down))
            {
                ChangeSelection(1);
            }

            if (Input.GetKeyDown(Keys.X))
            {
                Close();
            }
        }

        public void Draw()
        {
            //List out actions with their name, icon, description, and FP cost
        }
    }
}
