using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// An interface for a BattleEntity with segments and has a segment behavior.
    /// </summary>
    public interface ISegmentEntity
    {
        /// <summary>
        /// The segment behavior of the BattleEntity.
        /// </summary>
        ISegmentBehavior SegmentBehavior { get; }
    }
}
