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
    /// A menu for the MoveActions relating to a ActionMenu. For example, any Jump actions would be in the ActionSubMenu for Jump.
    /// ActionSubMenus can lead to more ActionSubMenus, as is the case with "Change Partner"
    /// This is only used by the player
    /// <para>All ActionSubMenus have the "SubMenu" suffix</para>
    /// </summary>
    public abstract class ActionSubMenu : BattleMenu
    {
        /// <summary>
        /// The list of move actions in the submenu
        /// </summary>
        public List<MoveAction> BattleActions { get; protected set; } = new List<MoveAction>();

        /// <summary>
        /// The category of the menu.
        /// </summary>
        public Enumerations.MoveCategories MoveCategory = Enumerations.MoveCategories.None;

        /// <summary>
        /// The position of the submenu
        /// </summary>
        protected Vector2 Position = Vector2.Zero;

        protected TextBox BoxMenu = null;

        protected override int LastSelection => BattleActions.Count - 1;

        protected ActionSubMenu() : base(MenuTypes.Vertical)
        {
            BoxMenu = new TextBox(new Vector2(SpriteRenderer.Instance.WindowCenter.X, SpriteRenderer.Instance.WindowCenter.Y + 220f), new Vector2(320f, 80f), null);
            BoxMenu.SetText(string.Empty);
        }

        public void Initialize()
        {
            BoxMenu.SetText(BattleActions[0].MoveProperties.Description);

            for (int i = 0; i < BattleActions.Count; i++)
            {
                BattleActions[i].SetMoveCategory(MoveCategory);
                BattleActions[i].Initialize();
            }
        }

        protected override void OnSelectionChanged(int newSelection)
        {
            BoxMenu.SetText(BattleActions[CurSelection].MoveProperties.Description);
        }

        protected override void OnBackOut()
        {
            base.OnBackOut();
            BattleUIManager.Instance.PopMenu();
        }

        protected override void OnConfirm()
        {
            base.OnConfirm();

            if (BattleActions[CurSelection].Disabled == false)
            {
                BattleActions[CurSelection].OnMenuSelected();
            }
            else
            {
                //NOTE: Show the dialog here that the move can't be selected and state why
                string disabledString = BattleActions[CurSelection].DisabledString;

                Debug.LogError($"{BattleActions[CurSelection].Name} is disabled: {disabledString}");
            }
        }

        public override void Draw()
        {
            //List out actions with their name, icon, description, and FP cost
            for (int i = 0; i < BattleActions.Count; i++)
            {
                Vector2 pos = Position + new Vector2(0, i * 20);
                Color color = BattleActions[i].Disabled == false ? Color.White : Color.LightSlateGray;
                if (CurSelection != i || BattleUIManager.Instance.TopMenu != this) color *= .7f;
                SpriteRenderer.Instance.DrawText(AssetManager.Instance.TTYDFont, BattleActions[i].Name, pos, color, 0f, Vector2.Zero, 1f, .4f);
            }

            //Show description window at the bottom
            BoxMenu.Draw();
        }
    }
}
