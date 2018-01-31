using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// An interface for a BattleEntity that can be flipped and has a flippable behavior.
    /// </summary>
    public interface IFlippableEntity
    {
        /// <summary>
        /// The flippable behavior of the BattleEntity.
        /// </summary>
        IFlippableBehavior FlippedBehavior { get; }
    }
}
