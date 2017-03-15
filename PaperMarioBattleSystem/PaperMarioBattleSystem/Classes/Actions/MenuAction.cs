using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static PaperMarioBattleSystem.Enumerations;

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
            MoveInfo = new MoveActionData(icon, description, MoveResourceTypes.FP, 0, CostDisplayTypes.Shown, MoveAffectionTypes.None,
                TargetSelectionMenu.EntitySelectionType.Single, false, null);

            SubMenu = subMenu;
        }

        /// <summary>
        /// This constructor is for showing the FP cost for a menu item (Ex. Double Dip and Triple Dip).
        /// </summary>
        /// <param name="name"></param>
        /// <param name="icon"></param>
        /// <param name="description"></param>
        /// <param name="fpCost"></param>
        /// <param name="subMenu"></param>
        public MenuAction(string name, Texture2D icon, string description, int fpCost, ActionSubMenu subMenu) : this(name, icon, description, subMenu)
        {
            MoveInfo.ResourceCost = fpCost;
        }

        public override void SetMoveCategory(Enumerations.MoveCategories moveCategory)
        {
            base.SetMoveCategory(moveCategory);
            if (SubMenu != null)
            {
                SubMenu.MoveCategory = MoveCategory;
                SubMenu.Initialize();
            }
        }

        public override void OnMenuSelected()
        {
            if (SubMenu != null)
            {
                BattleUIManager.Instance.PushMenu(SubMenu);
            }
            else
            {
                Debug.LogError($"{nameof(SubMenu)} is null for {Name} so no further actions can be taken in this menu option. Fix this");
            }
        }
    }
}
