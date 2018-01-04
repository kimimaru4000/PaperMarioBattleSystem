using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// An interface for any objects with a position.
    /// </summary>
    public interface IPosition
    {
        Vector2 Position { get; }
    }
}
