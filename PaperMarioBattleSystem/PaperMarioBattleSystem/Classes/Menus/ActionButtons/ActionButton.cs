using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// A button representing the main actions during battle. This includes battle actions like Jump, Hammer, etc.
    /// This is only used by the player
    /// <para>All ActionButtons have the "Button" suffix</para>
    /// </summary>
    public abstract class ActionButton
    {
        /// <summary>
        /// The name of the action the button represents
        /// </summary>
        protected string Name = "Button";

        /// <summary>
        /// The image of the button
        /// </summary>
        protected Texture2D ButtonImage = null;

        protected ActionButton(string name)
        {
            Name = name;
        }

        protected ActionButton(Texture2D buttonImage)
        {
            ButtonImage = buttonImage;
        }

        /// <summary>
        /// What occurs when this ActionButton is selected.
        /// In most cases, this would bring up a SubMenu, but in other instances (Ex. having no additional Jump badges equipped)
        /// This can lead directly to an action
        /// </summary>
        public virtual void OnSelected()
        {
            
        }

        public virtual void Draw(bool selected)
        {

        }
    }
}
