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
    /// A type of MoveAction used only in ActionSubMenus to bring up other ActionSubMenus.
    /// <para>This doesn't have a Sequence, so attempting to start it as an actual MoveAction will throw an error.</para>
    /// </summary>
    public sealed class MenuAction : MoveAction
    {
        /// <summary>
        /// The ActionSubMenu to bring up.
        /// </summary>
        private ActionSubMenu SubMenu = null;

        public MenuAction(string name, Texture2D icon, string description, ActionSubMenu subMenu)
        {
            Name = name;
            MoveInfo = new MoveActionData(icon, 0, description, TargetSelectionMenu.EntitySelectionType.Single,
                Enumerations.EntityTypes.Player, false, null);

            SubMenu = subMenu;
        }

        public override void OnMenuSelected()
        {
            if (SubMenu != null)
            {
                BattleUIManager.Instance.PushMenu(SubMenu);
            }
            else
            {
                Debug.LogError($"{nameof(SubMenu)} is null for {Name} so no actions further can be taken in this menu option. Fix this");
            }
        }
    }
}
