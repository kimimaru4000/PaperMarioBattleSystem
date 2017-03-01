using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// An interface for objects with names.
    /// </summary>
    public interface INameable
    {
        /// <summary>
        /// The name of the object.
        /// </summary>
        string Name { get; }
    }
}
