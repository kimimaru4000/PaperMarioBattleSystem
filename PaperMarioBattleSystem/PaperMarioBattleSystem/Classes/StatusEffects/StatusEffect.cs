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
        /// Alignments for StatusEffects indicating how they affect the afflicted BattleEntity.
        /// </summary>
        public enum StatusAlignments
        {
            Neutral, Positive, Negative
        }

        /// <summary>
        /// The type of StatusEffect this is.
        /// </summary>
        public StatusTypes StatusType { get; protected set; } = StatusTypes.None;

        /// <summary>
        /// The alignment of the StatusEffect.
        /// </summary>
        public StatusAlignments Alignment { get; protected set; } = StatusAlignments.Neutral;

        /// <summary>
        /// The duration of the StatusEffect, in turns.
        /// </summary>
        public int Duration { get; protected set; } = 1;

        /// <summary>
        /// The additional duration of the StatusEffect based on a BattleEntity's StatusProperties, in turns.
        /// This is automatically set when the StatusEffect is assigned a BattleEntity reference.
        /// </summary>
        public int AdditionalDuration { get; protected set; } = 0;

        /// <summary>
        /// The current number of turns the StatusEffect has been in effect.
        /// </summary>
        public int TurnsPassed { get; private set; } = 0;

        /// <summary>
        /// The priority of the StatusEffect.
        /// StatusEffects with higher Priority values affect the BattleEntity sooner.
        /// </summary>
        public int Priority => StatusGlobals.GetStatusPriority(StatusType);

        /// <summary>
        /// The BattleEntity afflicted with the StatusEffect.
        /// </summary>
        public BattleEntity EntityAfflicted { get; private set; } = null;

        /// <summary>
        /// The ways the StatusEffect is suppressed.
        /// </summary>
        private readonly Dictionary<StatusSuppressionTypes, int> SuppressionStates = new Dictionary<StatusSuppressionTypes, int>();

        /// <summary>
        /// The total duration of the StatusEffect, which is the sum of both the base Duration and the AdditionalDuration.
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
        public bool Ended { get; private set; } = false;

        /// <summary>
        /// The icon of the StatusEffect.
        /// </summary>
        public CroppedTexture2D StatusIcon { get; protected set; } = null;

        /// <summary>
        /// Tells if the Status Effect lasts indefinitely and won't end by turn count.
        /// </summary>
        public bool IsInfinite => (Duration <= InfiniteDuration);

        /// <summary>
        /// Tells if the Status Effect is finished through turn count.
        /// </summary>
        public bool IsTurnFinished => (IsInfinite == false && TurnsPassed >= TotalDuration);

        /// <summary>
        /// Sets the BattleEntity that is afflicted with this StatusEffect and factors in the BattleEntity's StatusProperties for this StatusEffect.
        /// </summary>
        /// <param name="entity">The BattleEntity to afflict with this StatusEffect.</param>
        public void SetEntity(BattleEntity entity)
        {
            EntityAfflicted = entity;

            //Set the additional duration
            AdditionalDuration = EntityAfflicted.EntityProperties.GetStatusProperty(StatusType).AdditionalTurns;
        }

        /// <summary>
        /// Clears the BattleEntity afflicted with this StatusEffect.
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
        /// Increments the number of turns the StatusEffect has been active and removes it from the BattleEntity afflicted when finished
        /// </summary>
        private void IncrementTurns()
        {
            //Handle problems with this method occurring after the status has ended
            if (Ended == true)
            {
                string entityName = EntityAfflicted?.Name ?? "N/A";

                Debug.LogError($"Attempting to increment turns for {StatusType} on {entityName} after it has ended!");

                //If the entity has already been cleared, return since there's no way to remove the status without the reference
                if (EntityAfflicted == null)
                {
                    return;
                }
            }

            //Print this message if we somehow reached this method when the StatusEffect was already done by turn count
            //Don't return because we still want to remove it
            if (IsTurnFinished == true)
            {
                Debug.LogError($"Attempting to increment turns for {StatusType} on entity {EntityAfflicted.Name} when it's already finished!");
            }

            //Increment the number of turns passed, unless this StatusEffect doesn't go away
            if (IsInfinite == false)
            {
                TurnsPassed++;
            }

            //When the StatusEffect is finished, remove it
            if (IsTurnFinished == true)
            {
                EntityAfflicted.EntityProperties.RemoveStatus(StatusType);
            }
        }

        /// <summary>
        /// Applies the StatusEffect's initial affliction logic to the BattleEntity.
        /// </summary>
        public void Afflict()
        {
            OnAfflict();
        }

        /// <summary>
        /// Immediately ends the StatusEffect.
        /// This does not remove it from the BattleEntity.
        /// </summary>
        public void End()
        {
            //If the StatusEffect already ended, don't end it again
            if (Ended == true) return;

            //Fully unsuppress to end the StatusEffect properly
            FullyUnsuppress();
            Ended = true;

            OnEnd();
        }

        /// <summary>
        /// Applies the StatusEffect's effects to the BattleEntity at the start of the phase cycle
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
        /// What the StatusEffect does to the BattleEntity when it's applied
        /// </summary>
        protected abstract void OnAfflict();

        /// <summary>
        /// What the StatusEffect does to the BattleEntity when it wears off
        /// </summary>
        protected abstract void OnEnd();

        /// <summary>
        /// What the StatusEffect does when the phase cycle starts
        /// </summary>
        protected abstract void OnPhaseCycleStart();

        /// <summary>
        /// What the StatusEffect does to the BattleEntity when it's suppressed in a particular way.
        /// </summary>
        /// <param name="statusSuppressionType">The type of suppression.</param>
        protected abstract void OnSuppress(StatusSuppressionTypes statusSuppressionType);

        /// <summary>
        /// What the StatusEffect does to the BattleEntity when it's unsuppressed in a particular way.
        /// </summary>
        /// <param name="statusSuppressionType">The type of suppression.</param>
        protected abstract void OnUnsuppress(StatusSuppressionTypes statusSuppressionType);

        /// <summary>
        /// Returns a new instance of this StatusEffect with the same properties.
        /// </summary>
        /// <returns>A deep copy of this StatusEffect</returns>
        public abstract StatusEffect Copy();

        /// <summary>
        /// Suppresses the StatusEffect in a particular way.
        /// </summary>
        /// <param name="statusSuppressionType">The StatusSuppressionTypes of how the StatusEffect should be suppressed.</param>
        public void Suppress(StatusSuppressionTypes statusSuppressionType)
        {
            int value = 0;

            //If this Status Effect isn't suppressed this way, add a new entry
            if (SuppressionStates.TryGetValue(statusSuppressionType, out value) == false)
            {
                SuppressionStates.Add(statusSuppressionType, 0);

                //Tell this Status Effect to suppress itself in this way the first time
                OnSuppress(statusSuppressionType);
            }

            //Add to the number of times this Status Effect is suppressed in this way
            value++;
            SuppressionStates[statusSuppressionType] = value;

            Debug.Log($"Status {StatusType} was suppressed by {statusSuppressionType} on {EntityAfflicted.Name} {value} time(s)!");
        }

        /// <summary>
        /// Unsuppresses the StatusEffect in a particular way.
        /// </summary>
        /// <param name="statusSuppressionType">The StatusSuppressionTypes of how the StatusEffect should be unsuppressed.</param>
        public void Unsuppress(StatusSuppressionTypes statusSuppressionType)
        {
            int value = 0;

            //If this Status Effect isn't suppressed in this way, there's nothing to unsuppress, so return
            if (SuppressionStates.TryGetValue(statusSuppressionType, out value) == false)
            {
                Debug.LogWarning($"{StatusType} on {EntityAfflicted.Name} is not {statusSuppressionType} suppressed, so it cannot be unsuppressed!");
                return;
            }

            //Subtract from the number of times this Status Effect is suppressed in this way
            value--;

            //Check if this Status Effect should no longer be suppressed in this way
            if (value <= 0)
            {
                SuppressionStates.Remove(statusSuppressionType);

                //Tell this Status Effect to unsuppress itself in this way
                OnUnsuppress(statusSuppressionType);
            }
            else
            {
                SuppressionStates[statusSuppressionType] = value;
            }

            Debug.Log($"Status {StatusType} was unsuppressed by {statusSuppressionType} on {EntityAfflicted.Name}. {value} suppressions of this type remain!");
        }

        /// <summary>
        /// Tells whether the StatusEffect is suppressed in a certain way or not.
        /// </summary>
        /// <param name="statusSuppressionType">The StatusSuppressionTypes of how the StatusEffect is suppressed.</param>
        /// <returns>true if the StatusEffect is suppressed in this way, otherwise false.</returns>
        public bool IsSuppressed(StatusSuppressionTypes statusSuppressionType)
        {
            return SuppressionStates.ContainsKey(statusSuppressionType);
        }

        /// <summary>
        /// Fully unsuppresses the StatusEffect.
        /// <para>This is called when the StatusEffect is ended to ensure it ends properly.</para>
        /// </summary>
        private void FullyUnsuppress()
        {
            //Don't do anything if nothing is suppressed
            if (SuppressionStates.Keys.Count == 0) return;

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
        /// Draws information about the StatusEffect, including its icon and turn count.
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
            SpriteRenderer.Instance.DrawUI(StatusIcon.Tex, iconPos, StatusIcon.SourceRect, Color.White, false, false, depth);

            //Draw turn count if it's not infinite
            if (string.IsNullOrEmpty(turnCountString) == false)
            {
                SpriteRenderer.Instance.DrawUIText(AssetManager.Instance.TTYDFont, turnCountString, iconPos + new Vector2(52, 20),
                    Color.White, 0f, new Vector2(1f, 0f), 1f, turnStringDepth);
            }
        }
    }
}
