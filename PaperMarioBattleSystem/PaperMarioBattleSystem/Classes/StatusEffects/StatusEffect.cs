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
    public abstract class StatusEffect : ICopyable<StatusEffect>
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
        public int Priority => StatusGlobals.GetStatusPriority(StatusType);

        /// <summary>
        /// The BattleEntity afflicted with the StatusEffect
        /// </summary>
        public BattleEntity EntityAfflicted { get; private set; } = null;

        /// <summary>
        /// The ways the StatusEffect is suppressed.
        /// </summary>
        private readonly Dictionary<StatusSuppressionTypes, int> SuppressionStates = new Dictionary<StatusSuppressionTypes, int>();

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

        /// <summary>
        /// Tells whether the StatusEffect already ended or not.
        /// </summary>
        private bool Ended = false;

        /// <summary>
        /// The Battle Message shown when a BattleEntity is afflicted with the StatusEffect.
        /// </summary>
        public string AfflictedMessage { get; protected set; } = string.Empty;

        /// <summary>
        /// The Battle Message shown when the StatusEffect is removed from the BattleEntity.
        /// </summary>
        public string RemovedMessage { get; protected set; } = string.Empty;

        /// <summary>
        /// The icon of the StatusEffect.
        /// </summary>
        public CroppedTexture2D StatusIcon { get; protected set; } = null;

        public bool IsInfinite => (Duration <= InfiniteDuration);
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
        private void IncrementTurns()
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
                EntityAfflicted.EntityProperties.RemoveStatus(StatusType, true);
            }
        }

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
            //If the StatusEffect already ended, don't end it again
            if (Ended == true) return;

            //Fully unsuppress to end the StatusEffect properly
            FullyUnsuppress();
            Ended = true;

            TurnsPassed = TotalDuration;

            OnEnd();
        }

        /// <summary>
        /// Applies the StatusEffect's effects to the entity at the start of the phase cycle
        /// </summary>
        public void PhaseCycleStart()
        {
            OnPhaseCycleStart();
        }

        /// <summary>
        /// Progresses the StatusEffect's turn count.
        /// <para>If the StatusEffect is TurnCount suppressed, its turn count won't be incremented.</para>
        /// </summary>
        protected void ProgressTurnCount()
        {
            if (IsSuppressed(StatusSuppressionTypes.TurnCount) == true)
            {
                Debug.Log($"{StatusType} on {EntityAfflicted.Name} is suppressed by TurnCount, so it won't increment turns");
                return;
            }

            IncrementTurns();
        }

        /// <summary>
        /// What the StatusEffect does to the entity when it's applied
        /// </summary>
        protected abstract void OnAfflict();

        /// <summary>
        /// What the StatusEffect does to the entity when it wears off
        /// </summary>
        protected abstract void OnEnd();

        /// <summary>
        /// What the StatusEffect does when the phase cycle starts
        /// </summary>
        protected abstract void OnPhaseCycleStart();

        /// <summary>
        /// What the StatusEffect does to the entity when it's suppressed in a particular way.
        /// </summary>
        /// <param name="statusSuppressionType">The type of suppression.</param>
        protected abstract void OnSuppress(StatusSuppressionTypes statusSuppressionType);

        /// <summary>
        /// What the StatusEffect does to the entity when it's unsuppressed in a particular way.
        /// </summary>
        /// <param name="statusSuppressionType">The type of suppression.</param>
        protected abstract void OnUnsuppress(StatusSuppressionTypes statusSuppressionType);

        /// <summary>
        /// Returns a new instance of this StatusEffect with the same properties
        /// </summary>
        /// <returns>A deep copy of this StatusEffect</returns>
        public abstract StatusEffect Copy();

        /// <summary>
        /// Suppresses the Status Effect in a particular way.
        /// </summary>
        /// <param name="statusSuppressionType">The StatusSuppressionTypes of how the Status Effect should be suppressed.</param>
        public void Suppress(StatusSuppressionTypes statusSuppressionType)
        {
            //If this Status Effect isn't suppressed this way, add a new entry
            if (IsSuppressed(statusSuppressionType) == false)
            {
                SuppressionStates.Add(statusSuppressionType, 0);

                //Tell this Status Effect to suppress itself in this way the first time
                OnSuppress(statusSuppressionType);
            }

            //Add to the number of times this Status Effect is suppressed in this way
            SuppressionStates[statusSuppressionType]++;

            int value = SuppressionStates[statusSuppressionType];
            Debug.Log($"Status {StatusType} was suppressed by {statusSuppressionType} on {EntityAfflicted.Name} {value} time(s)!");
        }

        /// <summary>
        /// Unsuppresses the Status Effect in a particular way.
        /// </summary>
        /// <param name="statusSuppressionType">The StatusSuppressionTypes of how the Status Effect should be unsuppressed.</param>
        public void Unsuppress(StatusSuppressionTypes statusSuppressionType)
        {
            //If this Status Effect isn't suppressed in this way, there's nothing to unsuppress, so return
            if (IsSuppressed(statusSuppressionType) == false)
            {
                Debug.LogWarning($"{StatusType} on {EntityAfflicted.Name} is not {statusSuppressionType} suppressed, so it cannot be unsuppressed!");
                return;
            }

            //Subtract from the number of times this Status Effect is suppressed in this way
            SuppressionStates[statusSuppressionType]--;

            int value = SuppressionStates[statusSuppressionType];

            //Check if this Status Effect should no longer be suppressed in this way
            if (SuppressionStates[statusSuppressionType] <= 0)
            {
                SuppressionStates.Remove(statusSuppressionType);

                //Tell this Status Effect to unsuppress itself in this way
                OnUnsuppress(statusSuppressionType);
            }

            Debug.Log($"Status {StatusType} was unsuppressed by {statusSuppressionType} on {EntityAfflicted.Name}. {value} suppressions of this type remain!");
        }

        /// <summary>
        /// Tells whether the Status Effect is suppressed in a certain way or not.
        /// </summary>
        /// <param name="statusSuppressionType">The StatusSuppressionTypes of how the Status Effect is suppressed.</param>
        /// <returns>true if the Status Effect is suppressed in this way, otherwise false.</returns>
        public bool IsSuppressed(StatusSuppressionTypes statusSuppressionType)
        {
            return SuppressionStates.ContainsKey(statusSuppressionType);
        }

        /// <summary>
        /// Fully unsuppresses the Status Effect.
        /// <para>This is called when the Status Effect is ended to ensure it ends properly.</para>
        /// </summary>
        private void FullyUnsuppress()
        {
            StatusSuppressionTypes[] suppressionTypes = SuppressionStates.Keys.ToArray();
            for (int i = 0; i < suppressionTypes.Length; i++)
            {
                //Unsuppress it the number of times indicated so it goes through the same code as it would normally
                int suppressionTimes = SuppressionStates[suppressionTypes[i]];

                for (int j = 0; j < suppressionTimes; j++)
                {
                    Unsuppress(suppressionTypes[i]);
                }
            }
        }

        /// <summary>
        /// Draws information about the Status Effect, including its icon and turn count.
        /// </summary>
        /// <param name="iconPos">The position to draw the StatusEffect's information.</param>
        /// <param name="depth">The rendering depth to draw the StatusEffect's information at.</param>
        /// <param name="turnStringDepth">The rendering depth to draw the StatusEffect's remaining turn count at.</param>
        public virtual void DrawStatusInfo(Vector2 iconPos, float depth, float turnStringDepth)
        {
            //Don't draw the status if it doesn't have an icon
            if (StatusIcon == null || StatusIcon.Tex == null)
                return;

            string turnCountString = (TotalDuration - TurnsPassed).ToString();
            if (IsInfinite == true) turnCountString = string.Empty;

            //Draw icon
            SpriteRenderer.Instance.Draw(StatusIcon.Tex, iconPos, StatusIcon.SourceRect, Color.White, false, false, depth, true);

            //Draw turn count if it's not infinite
            if (string.IsNullOrEmpty(turnCountString) == false)
            {
                SpriteRenderer.Instance.DrawText(AssetManager.Instance.TTYDFont, turnCountString, iconPos + new Vector2(52, 20),
                    Color.White, 0f, new Vector2(1f, 0f), 1f, turnStringDepth);
            }
        }

        #region Static Methods

        /// <summary>
        /// A Comparison method used to sort StatusEffects by their Priorities
        /// </summary>
        /// <param name="status1">The first StatusEffect to compare</param>
        /// <param name="status2">The second StatusEffect to compare</param>
        /// <returns>-1 if status1 has a higher priority, 1 if status2 has a higher priority, and 0 if they have the same priorities.</returns>
        public static int StatusPrioritySort(StatusEffect status1, StatusEffect status2)
        {
            if (status1 == null)
                return 1;
            if (status2 == null)
                return -1;

            return StatusTypePrioritySort(status1.StatusType, status2.StatusType);
        }

        /// <summary>
        /// A Comparison method used to sort StatusTypes by their Priorities.
        /// </summary>
        /// <param name="statusType1">The first StatusType to compare.</param>
        /// <param name="statusType2">The second StatusType to compare.</param>
        /// <returns>-1 if statusType1 has a higher priority, 1 if statusType2 has a higher priority, and 0 if they have the same priorities.</returns>
        public static int StatusTypePrioritySort(StatusTypes statusType1, StatusTypes statusType2)
        {
            int priority1 = GetStatusPriority(statusType1);
            int priority2 = GetStatusPriority(statusType2);

            if (priority1 < priority2)
                return 1;
            else if (priority1 > priority2)
                return -1;

            return 0;
        }

        #endregion
    }
}
