using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using static PaperMarioBattleSystem.Enumerations;
using static PaperMarioBattleSystem.StatusGlobals;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The base class for Status Effects.
    /// <para>Status Effects increment a turn at the start of each phase cycle.
    /// Status Effects that take effect each turn also occur at the start of each phase cycle</para>
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

        // <summary>
        // The priority of the StatusEffect.
        // StatusEffects with higher priorities have lower Priority values and affect the BattleEntity sooner
        // </summary>
        //protected int Priority { get; private set; } = 0;

        /// <summary>
        /// The BattleEntity afflicted with the StatusEffect
        /// </summary>
        public BattleEntity EntityAfflicted { get; private set; } = null;

        /// <summary>
        /// Tells whether the StatusEffect is Suspended or not
        /// </summary>
        public bool Suspended
        {
            get { return IsSuspended; }
            set
            {
                bool prevSuspended = IsSuspended;
                IsSuspended = value;

                //Only do something if the suspended value is different
                if (prevSuspended != IsSuspended)
                {
                    //If it's no longer suspended, perform the StatusEffect's afflict logic
                    if (IsSuspended == false)
                    {
                        OnAfflict();
                    }
                    //If it's now suspended, perform the StatusEffect's end logic
                    else
                    {
                        OnEnd();
                    }
                }
            }
        }

        private bool IsSuspended = false;

        private bool IsInfinite => (Duration <= InfiniteDuration);
        public bool IsFinished => (IsInfinite == false && TurnsPassed >= Duration);

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

            //Increment the number of turns passed, unless this StatusEffect doesn't go away
            if (IsInfinite == false)
            {
                TurnsPassed++;
            }

            //When the StatusEffect is finished, remove it
            if (IsFinished == true)
            {
                EntityAfflicted.RemoveStatus(StatusType);
            }
        }

        // <summary>
        // Sets the Priority of the StatusEffect
        // </summary>
        // <param name="priority">The value to set the Priority to</param>
        //protected void SetPriority(int priority)
        //{
        //    Priority = priority;
        //}

        /// <summary>
        /// Applies the StatusEffect's initial affliction logic to the entity
        /// </summary>
        public void Afflict()
        {
            OnAfflict();
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
        /// Applies the StatusEffect's effects to the entity at the start of the phase cycle
        /// </summary>
        public void PhaseCycleStart()
        {
            //Don't do anything if the StatusEffect is suspended
            if (Suspended == true) return;

            OnPhaseCycleStart();
        }

        /// <summary>
        /// What the StatusEffect does to the entity when it's applied or resumed
        /// </summary>
        protected abstract void OnAfflict();

        /// <summary>
        /// What the StatusEffect does to the entity when it wears off or suspended
        /// </summary>
        protected abstract void OnEnd();

        //NOTE: USE THESE INSTEAD OF OnAfflict() OR OnEnd()
        //What the StatusEffect does to the entity when it is resumed
        //protected abstract void OnResume();
        //What the StatusEffect does to the entity when it is suspended
        //protected abstract void OnSuspend();

        /// <summary>
        /// What the StatusEffect does when the phase cycle starts
        /// </summary>
        protected abstract void OnPhaseCycleStart();

        /// <summary>
        /// Returns a new instance of this StatusEffect with the same properties
        /// </summary>
        /// <returns>A deep copy of this StatusEffect</returns>
        public abstract StatusEffect Copy();
    }
}
