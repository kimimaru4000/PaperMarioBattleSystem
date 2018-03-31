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
    /// The Burn Status Effect.
    /// The entity takes 1 HP in Fire damage at the start of each phase cycle
    /// </summary>
    public sealed class BurnStatus : StatusEffect
    {
        private int FireDamage = 1;

        public BurnStatus(int duration)
        {
            StatusType = Enumerations.StatusTypes.Burn;
            Alignment = StatusAlignments.Negative;

            StatusIcon = new CroppedTexture2D(AssetManager.Instance.LoadRawTexture2D($"{ContentGlobals.UIRoot}/Battle/BattleGFX.png"),
                new Rectangle(604, 58, 38, 46));

            Duration = duration;

            AfflictedMessage = "Burned! The fire will\nsteadily do damage!";
        }

        protected override void OnAfflict()
        {
            //Remove the Frozen status if the entity was afflicted with Burn
            if (EntityAfflicted.EntityProperties.HasStatus(Enumerations.StatusTypes.Frozen) == true)
            {
                Debug.Log($"{StatusType} was inflicted on an entity afflicted with {Enumerations.StatusTypes.Frozen}, negating both effects!");
                EntityAfflicted.RemoveStatus(Enumerations.StatusTypes.Frozen, true, false);

                //Also remove Burn, as these two statuses negate each other
                EntityAfflicted.RemoveStatus(Enumerations.StatusTypes.Burn, true, false);
            }
        }

        protected override void OnEnd()
        {

        }

        protected override void OnPhaseCycleStart()
        {
            if (IsSuppressed(Enumerations.StatusSuppressionTypes.Effects) == false)
            {
                EntityAfflicted.TakeDamage(Enumerations.Elements.Fire, FireDamage, true);
            }
            ProgressTurnCount();
        }

        protected override void OnSuppress(Enumerations.StatusSuppressionTypes statusSuppressionType)
        {
            
        }

        protected override void OnUnsuppress(Enumerations.StatusSuppressionTypes statusSuppressionType)
        {
            
        }

        public override StatusEffect Copy()
        {
            return new BurnStatus(Duration);
        }
    }
}
