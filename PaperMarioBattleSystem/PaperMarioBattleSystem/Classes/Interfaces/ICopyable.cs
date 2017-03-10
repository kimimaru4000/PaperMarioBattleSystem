using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// An interface for objects that can be copied.
    /// </summary>
    /// <typeparam name="T">The type of the object to return when copied.</typeparam>
    public interface ICopyable<T>
    {
        /// <summary>
        /// Returns a copy of the object of type T.
        /// </summary>
        /// <returns>A copy of the object of type T.</returns>
        T Copy();
    }
}
