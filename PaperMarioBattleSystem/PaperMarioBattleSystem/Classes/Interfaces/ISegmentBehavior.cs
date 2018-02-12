using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PaperMarioBattleSystem.Enumerations;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// An interface for handling objects with segments.
    /// <para>Examples include Pokeys and Shy Stacks.</para>
    /// </summary>
    public interface ISegmentBehavior : ICleanup
    {
        /// <summary>
        /// How many max segments the object has.
        /// </summary>
        uint MaxSegments { get; }

        /// <summary>
        /// How many segments the object currently has.
        /// </summary>
        uint CurSegmentCount { get; }

        /// <summary>
        /// The DamageEffects the object has a segment removed from.
        /// </summary>
        DamageEffects SegmentRemovedOnEffects { get; }

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
