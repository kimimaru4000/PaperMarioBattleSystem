using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// An interface for any type of object that can be scaled.
    /// </summary>
    public interface IScalable
    {
        Vector2 Scale { get; set; }
    }
}
