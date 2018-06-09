using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Frozen Status Effect.
    /// Entities afflicted with this cannot move until it ends, in which the entity will take 1 Ice damage.
    /// If the entity is afflicted with Burn while it is Frozen, both effects will negate each other.
    /// </summary>
    public sealed class FrozenStatus : StopStatus
    {
        /// <summary>
        /// The amount of Ice damage the entity takes when the status ends.
        /// </summary>
        private const int IceDamage = 1;

        public FrozenStatus(int duration) : base(duration)
        {
            StatusType = Enumerations.StatusTypes.Frozen;

            StatusIcon = new CroppedTexture2D(AssetManager.Instance.LoadRawTexture2D($"{ContentGlobals.UIRoot}/Battle/BattleGFX.png"),
                new Rectangle(604, 107, 38, 46));

            AfflictedMessage = "Frozen! Movement will be\nimpossible for a while!";
            ShouldQueueEndEvent = false;
        }

        protected override void OnPhaseCycleStart()
        {
            //The entity takes 1 Ice damage when Frozen ends due to turn count
            //Check if it's Effects suppressed first
            if (IsSuppressed(Enumerations.StatusSuppressionTypes.Effects) == false)
            {
                //If it's not infinite and not suppressed by turn count, we must be incrementing the turns
                if (IsInfinite == false && IsSuppressed(Enumerations.StatusSuppressionTypes.TurnCount) == false)
                {
                    int lastTurn = TurnsPassed + 1;

                    //If we're about to end the status, damage the entity
                    if (lastTurn >= TotalDuration)
                    {
                        EntityAfflicted.TakeDamage(Enumerations.Elements.Ice, IceDamage, true);
                    }
                }
            }

            base.OnPhaseCycleStart();
        }

        protected override void OnAfflict()
        {
            base.OnAfflict();

            //Remove the Burn status if the entity was afflicted with Frozen
            if (EntityAfflicted.EntityProperties.HasStatus(Enumerations.StatusTypes.Burn) == true)
            {
                Debug.Log($"{StatusType} was inflicted on an entity afflicted with {Enumerations.StatusTypes.Burn}, negating both effects!");
                EntityAfflicted.EntityProperties.RemoveStatus(Enumerations.StatusTypes.Burn);

                //Also remove Frozen, as these two statuses negate each other
                EntityAfflicted.EntityProperties.RemoveStatus(Enumerations.StatusTypes.Frozen);
            }
        }

        protected sealed override void OnEnd()
        {
            base.OnEnd();
        }

        public sealed override StatusEffect Copy()
        {
            return new FrozenStatus(Duration);
        }
    }
}
