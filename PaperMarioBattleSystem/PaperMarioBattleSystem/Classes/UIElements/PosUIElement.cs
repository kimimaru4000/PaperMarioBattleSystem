using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// A UIElement with a Position.
    /// </summary>
    public abstract class PosUIElement : UIElement
    {
        /// <summary>
        /// The position of the PosUIElement.
        /// </summary>
        public virtual Vector2 Position { get; set; } = Vector2.Zero;
    }
}
