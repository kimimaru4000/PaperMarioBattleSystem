using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PaperMarioBattleSystem.Enumerations;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// An interface for BattleEntities with segments.
    /// <para>Examples include Pokeys and Shy Stacks.</para>
    /// </summary>
    public interface ISegmentEntity
    {
        /// <summary>
        /// How many max segments the BattleEntity has.
        /// </summary>
        uint MaxSegments { get; }

        /// <summary>
        /// How many segments the BattleEntity currently has.
        /// </summary>
        uint CurSegmentCount { get; }

        /// <summary>
        /// What happens when segments are removed.
        /// </summary>
        /// <param name="segmentsRemoved">The number of segments to remove.</param>
        void HandleSegmentRemoved(uint segmentsRemoved);

        /// <summary>
        /// What happens when segments are added.
        /// </summary>
        /// <param name="segmentsAdded">The number of segments to add.</param>
        void HandleSegmentAdded(uint segmentsAdded);
    }
}
