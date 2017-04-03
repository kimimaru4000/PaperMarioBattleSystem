using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The base class for all types of VFXElements.
    /// VFXElements are handled in the <see cref="BattleVFXManager"/>.
    /// </summary>
    public abstract class VFXElement : IUpdateable, IDrawable
    {
        /// <summary>
        /// Whether the VFXElement should be removed or not.
        /// This is most useful for VFXElements that handle themselves.
        /// </summary>
        public bool ShouldRemove = false;

        /// <summary>
        /// Updates the VFXElement.
        /// </summary>
        public abstract void Update();

        /// <summary>
        /// Draws the VFXElement.
        /// </summary>
        public abstract void Draw();
    }
}
