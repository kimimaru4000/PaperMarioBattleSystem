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
    /// A button representing the main actions during battle. This includes battle actions like Jump, Hammer, etc.
    /// This is only used by the player
    /// </summary>
    public sealed class ActionButton : IDisableable
    {
        /// <summary>
        /// The name of the main action the button represents.
        /// </summary>
        private string Name = "Button";

        /// <summary>
        /// The image to use for the button.
        /// </summary>
        private Texture2D ButtonImage = null;

        /// <summary>
        /// The move category of the button. All moves accessed through the button have the same category.
        /// </summary>
        public MoveCategories Category { get; private set; } = MoveCategories.None;

        /// <summary>
        /// The position of the button.
        /// </summary>
        public Vector2 Position = Vector2.Zero;

        /// <summary>
        /// The ActionSubMenu that this button leads to.
        /// </summary>
        private ActionSubMenu SubMenu = null;

        /// <summary>
        /// Whether the button is disabled and cannot be selected.
        /// This is often set by the NoSkills Status Effect.
        /// </summary>
        public bool Disabled { get; set; } = false;

        public ActionButton(string name, Texture2D buttonImage, MoveCategories moveCategory, ActionSubMenu subMenu)
        {
            Name = name;
            ButtonImage = buttonImage;
            Category = moveCategory;
            SubMenu = subMenu;
        }

        public void Initialize()
        {
            if (SubMenu != null)
            {
                SubMenu.MoveCategory = Category;
                SubMenu.Initialize();
            }

            //Check if the button should be disabled
            Disabled = BattleManager.Instance.EntityTurn.EntityProperties.IsMoveCategoryDisabled(Category);
        }

        /// <summary>
        /// What occurs when this ActionButton is selected.
        /// In most cases, this would bring up a SubMenu, but in other instances (Ex. having no additional Jump badges equipped)
        /// This can lead directly to an action
        /// </summary>
        public void OnSelected()
        {
            if (SubMenu != null)
            {
                if (Disabled == false)
                {
                    if (SubMenu.BattleActions.Count == 1 && SubMenu.AutoSelectSingle == true)
                    {
                        SubMenu.BattleActions[0].OnMenuSelected();
                    }
                    else
                    {
                        BattleUIManager.Instance.PushMenu(SubMenu);
                    }
                }
                else
                {
                    //NOTE: Find a way to get the correct message when the action can't be selected
                    string disabledString = $"Your {Category} moves are disabled!";

                    BattleEventManager.Instance.QueueBattleEvent((int)BattleGlobals.StartEventPriorities.Message,
                        new BattleManager.BattleState[] { BattleManager.BattleState.Turn, BattleManager.BattleState.TurnEnd },
                        new MessageBattleEvent(disabledString, MessageBattleEvent.DefaultWaitDuration));

                    Debug.LogError($"All {Category} moves are currently disabled for {BattleManager.Instance.EntityTurn.Name}!");
                }
            }
            else
            {
                Debug.LogError($"{nameof(SubMenu)} is null for {Name} so no actions further can be taken in this menu option. Fix this");
            }
        }

        public void Draw(bool selected)
        {
            Color iconColor = Disabled == false ? MoveAction.EnabledColor : MoveAction.DisabledColor;
            if (selected == false) iconColor = iconColor * MoveAction.UnselectedAlpha;

            Vector2 uiPos = Camera.Instance.SpriteToUIPos(Position);

            SpriteRenderer.Instance.Draw(ButtonImage, uiPos, iconColor, false, false, .4f, true);
            SpriteRenderer.Instance.DrawText(AssetManager.Instance.TTYDFont, Name, uiPos - new Vector2(0, 30), iconColor, .45f);
        }
    }
}
