using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// An interface for objects that perform cleanup logic.
    /// This is where they can clear events and other data.
    /// </summary>
    public interface ICleanup
    {
        /// <summary>
        /// Performs clean up on the object, clearing events and other data.
        /// This should be called when the object in question will no longer be used.
        /// </summary>
        void CleanUp();
    }
}
