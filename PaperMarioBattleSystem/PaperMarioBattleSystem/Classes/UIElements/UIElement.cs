using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The base class for all types of UI Elements.
    /// UI Elements are handled in the BattleUIManager.
    /// </summary>
    public abstract class UIElement : IUpdateable, IDrawable
    {
        /// <summary>
        /// Updates the UIElement.
        /// </summary>
        public abstract void Update();

        /// <summary>
        /// Draws the UIElement.
        /// </summary>
        public abstract void Draw();
    }
}
