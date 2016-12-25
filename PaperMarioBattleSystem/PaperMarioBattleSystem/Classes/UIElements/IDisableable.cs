using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// An interface for anything that can be disabled.
    /// </summary>
    public interface IDisableable
    {
        /// <summary>
        /// Whether the object is disabled and cannot be selected.
        /// </summary>
        bool Disabled { get; set; }
    }
}
