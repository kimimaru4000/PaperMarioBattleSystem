using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using static PaperMarioBattleSystem.Enumerations;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The base class for Status Effects
    /// </summary>
    public abstract class StatusEffect
    {
        /// <summary>
        /// Alignments for StatusEffects indicating how they affect the afflicted BattleEntity
        /// </summary>
        public enum StatusAlignments
        {
            Neutral, Positive, Negative
        }

        /// <summary>
        /// The type of StatusEffect this is
        /// </summary>
        public StatusTypes StatusType { get; protected set; } = StatusTypes.None;

        /// <summary>
        /// The alignment of the StatusEffect
        /// </summary>
        public StatusAlignments Alignment { get; protected set; } = StatusAlignments.Neutral;

        /// <summary>
        /// The duration of the StatusEffect, in turns
        /// </summary>
        public int Duration { get; protected set; } = 1;

        /// <summary>
        /// The BattleEntity afflicted with the StatusEffect
        /// </summary>
        public BattleEntity EntityAfflicted { get; private set; } = null;

        /// <summary>
        /// Sets the BattleEntity that is afflicted with this StatusEffect
        /// </summary>
        /// <param name="entity">The BattleEntity to afflict with this StatusEffect</param>
        public void SetEntity(BattleEntity entity)
        {
            EntityAfflicted = entity;
        }

        /// <summary>
        /// Clears the BattleEntity afflicted with this StatusEffect
        /// </summary>
        public void ClearEntity()
        {
            EntityAfflicted = null;
        }

        /// <summary>
        /// What the StatusEffect does to the entity when it's applied
        /// </summary>
        public abstract void OnAfflict();

        /// <summary>
        /// What the StatusEffect does to the entity when it wears off
        /// </summary>
        public abstract void OnEnd();

        /// <summary>
        /// What the StatusEffect does when the entity's turn starts
        /// </summary>
        public abstract void OnTurnStart();

        /// <summary>
        /// What the StatusEffect does when the entity's turn ends
        /// </summary>
        public abstract void OnTurnEnd();

        /// <summary>
        /// Returns a new instance of this StatusEffect with the same properties
        /// </summary>
        /// <returns>A deep copy of this StatusEffect</returns>
        public abstract StatusEffect Copy();
    }
}
