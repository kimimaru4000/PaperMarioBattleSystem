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
    /// A button representing the main actions during battle. This includes battle actions like Jump, Hammer, etc.
    /// This is only used by the player
    /// </summary>
    public sealed class ActionButton
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
        /// The position of the button.
        /// </summary>
        public Vector2 Position = Vector2.Zero;

        /// <summary>
        /// The ActionSubMenu that this button leads to.
        /// </summary>
        private ActionSubMenu SubMenu = null;

        public ActionButton(string name, Texture2D buttonImage, /*Vector2 position,*/ ActionSubMenu subMenu)
        {
            Name = name;
            ButtonImage = buttonImage;
            //Position = position;
            SubMenu = subMenu;
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
                BattleUIManager.Instance.PushMenu(SubMenu);
            }
            else
            {
                Debug.LogError($"{nameof(SubMenu)} is null for {Name} so no actions further can be taken in this menu option. Fix this");
            }
        }

        public void Draw(bool selected)
        {
            Color color = Color.White * .75f;
            if (selected == true) color = Color.White;

            Vector2 uiPos = Camera.Instance.SpriteToUIPos(Position);

            SpriteRenderer.Instance.Draw(ButtonImage, uiPos, color, false, .4f, true);
            SpriteRenderer.Instance.DrawText(AssetManager.Instance.Font, Name, uiPos - new Vector2(0, 30), color, .45f);
        }
    }
}
