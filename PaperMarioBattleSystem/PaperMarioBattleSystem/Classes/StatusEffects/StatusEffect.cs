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
    /// The base class for Status Effects.
    /// <para>Status Effects that take effect regularly occur at the start and/or end of a phase, not each entity's individual turn.</para>
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
        /// The current number of turns the StatusEffect has been in effect
        /// </summary>
        protected int TurnsPassed { get; private set; } = 0;

        /// <summary>
        /// The BattleEntity afflicted with the StatusEffect
        /// </summary>
        public BattleEntity EntityAfflicted { get; private set; } = null;

        public bool IsFinished => (TurnsPassed >= Duration);

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
        /// Increments the number of turns the StatusEffect has been active and removes it from the entity afflicted when finished
        /// </summary>
        protected void IncrementTurns()
        {
            //Print this message if we somehow reached this method when the StatusEffect was already done
            //Don't return because we still want to remove it
            if (IsFinished == true)
            {
                Debug.LogError($"Attempting to increment turns for {StatusType} on entity {EntityAfflicted.Name} when it's already finished!");
            }

            //Increment the number of turns passed
            TurnsPassed++;

            //When the StatusEffect is finished, remove it
            if (IsFinished == true)
            {
                EntityAfflicted.RemoveStatus(StatusType);
            }
        }

        /// <summary>
        /// Immediately ends the StatusEffect.
        /// This does not remove it from the entity
        /// </summary>
        public void End()
        {
            TurnsPassed = Duration;

            OnEnd();
        }

        /// <summary>
        /// What the StatusEffect does to the entity when it's applied
        /// </summary>
        public abstract void OnAfflict();

        /// <summary>
        /// What the StatusEffect does to the entity when it wears off
        /// </summary>
        protected abstract void OnEnd();

        /// <summary>
        /// What the StatusEffect does when the phase for the entity starts
        /// </summary>
        public abstract void OnPhaseStart();

        /// <summary>
        /// What the StatusEffect does when the phase for the entity ends
        /// </summary>
        public abstract void OnPhaseEnd();

        /// <summary>
        /// Returns a new instance of this StatusEffect with the same properties
        /// </summary>
        /// <returns>A deep copy of this StatusEffect</returns>
        public abstract StatusEffect Copy();
    }
}
