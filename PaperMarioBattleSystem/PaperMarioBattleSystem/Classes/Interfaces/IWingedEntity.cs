using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// An interface for a BattleEntity that has wings and has a winged behavior.
    /// </summary>
    public interface IWingedEntity
    {
        /// <summary>
        /// The winged behavior of the BattleEntity.
        /// </summary>
        IWingedBehavior WingedBehavior { get; }
    }
}
