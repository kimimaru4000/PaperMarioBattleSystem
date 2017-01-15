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
        /// The additional duration of the StatusEffect based on an entity's StatusProperties, in turns.
        /// This is automatically set when the StatusEffect is assigned a BattleEntity reference.
        /// </summary>
        public int AdditionalDuration { get; protected set; } = 0;

        /// <summary>
        /// The current number of turns the StatusEffect has been in effect
        /// </summary>
        protected int TurnsPassed { get; private set; } = 0;

        /// <summary>
        /// The priority of the StatusEffect.
        /// StatusEffects with higher priorities have higher Priority values and affect the BattleEntity sooner
        /// </summary>
        protected int Priority => StatusGlobals.GetStatusPriority(StatusType);

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
                    //If it's no longer suspended, perform the StatusEffect's resume logic
                    if (IsSuspended == false)
                    {
                        OnResume();
                    }
                    //If it's now suspended, perform the StatusEffect's suspend logic
                    else
                    {
                        OnSuspend();
                    }
                }
            }
        }

        /// <summary>
        /// The total duration of the StatusEffect, which is the sum of both the base Duration and the AdditionalDuration
        /// </summary>
        public int TotalDuration
        {
            get
            {
                int totalDur = Duration + AdditionalDuration;
                return (totalDur < 0) ? 0 : totalDur;
            }
        }

        private bool IsSuspended = false;

        /// <summary>
        /// Tells whether the StatusEffect already ended or not.
        /// </summary>
        private bool Ended = false;

        /// <summary>
        /// The Battle Message shown when a BattleEntity is afflicted with the StatusEffect.
        /// </summary>
        public string AfflictedMessage { get; protected set; } = string.Empty;

        private bool IsInfinite => (Duration <= InfiniteDuration);
        public bool IsFinished => (IsInfinite == false && TurnsPassed >= TotalDuration);

        /// <summary>
        /// Sets the BattleEntity that is afflicted with this StatusEffect and factors in the entity's StatusProperties for this StatusEffect.
        /// </summary>
        /// <param name="entity">The BattleEntity to afflict with this StatusEffect.</param>
        public void SetEntity(BattleEntity entity)
        {
            EntityAfflicted = entity;

            //Set the additional duration
            AdditionalDuration = EntityAfflicted.EntityProperties.GetStatusProperty(StatusType).AdditionalTurns;
        }

        /// <summary>
        /// Clears the BattleEntity afflicted with this StatusEffect
        /// </summary>
        public void ClearEntity()
        {
            EntityAfflicted = null;
        }
        
        /// <summary>
        /// Refreshes the StatusEffect with properties from a new one with the same StatusType.
        /// This occurs if the StatusEffect is already inflicted on the BattleEntity.
        /// Default behavior is to refresh the duration.
        /// </summary>
        /// <param name="newStatus">The new StatusEffect, with the same StatusType as this one.</param>
        public virtual void Refresh(StatusEffect newStatus)
        {
            Duration = newStatus.Duration;
            TurnsPassed = 0;
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
                EntityAfflicted.EntityProperties.RemoveStatus(StatusType);
            }
        }

        /// <summary>
        /// Applies the StatusEffect's initial affliction logic to the entity
        /// </summary>
        public void Afflict()
        {
            //Show a battle message when the status is afflicted, provided the message isn't empty
            if (string.IsNullOrEmpty(AfflictedMessage) == false)
            {
                BattleManager.Instance.QueueBattleEvent((int)BattleGlobals.StartEventPriorities.Message + Priority,
                    new BattleManager.BattleState[] { BattleManager.BattleState.TurnEnd }, new MessageBattleEvent(AfflictedMessage, 2000d));
            }

            OnAfflict();
        }

        /// <summary>
        /// Immediately ends the StatusEffect.
        /// This does not remove it from the entity
        /// </summary>
        public void End()
        {
            //If the StatusEffect already ended, don't end it again
            if (Ended == true) return;

            //Resume to end the StatusEffect properly
            Suspended = false;
            Ended = true;

            TurnsPassed = TotalDuration;

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

        /// <summary>
        /// What the StatusEffect does when the phase cycle starts
        /// </summary>
        protected abstract void OnPhaseCycleStart();

        /// <summary>
        /// What the StatusEffect does to the entity when it is suspended
        /// </summary>
        protected abstract void OnSuspend();

        /// <summary>
        /// What the StatusEffect does to the entity when it is resumed
        /// </summary>
        protected abstract void OnResume();

        /// <summary>
        /// Returns a new instance of this StatusEffect with the same properties
        /// </summary>
        /// <returns>A deep copy of this StatusEffect</returns>
        public abstract StatusEffect Copy();

        #region Static Methods

        /// <summary>
        /// A Comparison method used to sort StatusEffects by their Priorities
        /// </summary>
        /// <param name="status1">The first StatusEffect to compare</param>
        /// <param name="status2">The second StatusEffect to compare</param>
        /// <returns>-1 if status1 has a higher priority, 1 if status2 has a higher priority, 0 if they have the same priorities</returns>
        public static int StatusPrioritySort(StatusEffect status1, StatusEffect status2)
        {
            if (status1 == null)
                return 1;
            if (status2 == null)
                return -1;

            if (status1.Priority < status2.Priority)
                return 1;
            else if (status1.Priority > status2.Priority)
                return -1;

            return 0;
        }

        #endregion
    }
}
