using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// An interface for any object that can have a color tint.
    /// </summary>
    public interface ITintable
    {
        /// <summary>
        /// The color to tint the object.
        /// </summary>
        Color TintColor { get; }
    }
}
