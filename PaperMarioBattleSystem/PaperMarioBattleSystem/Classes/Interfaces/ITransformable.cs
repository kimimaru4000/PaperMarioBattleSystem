using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// An interface for an object that has a position, rotation, and scale that can be manipulated.
    /// </summary>
    public interface ITransformable
    {
        /// <summary>
        /// The Transform of the object.
        /// </summary>
        Transform Transform { get; }
    }
}
