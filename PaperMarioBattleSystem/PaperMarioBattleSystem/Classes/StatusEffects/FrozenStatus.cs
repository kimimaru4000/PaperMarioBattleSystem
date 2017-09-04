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
    public sealed class FrozenStatus : ImmobilizedStatus
    {
        /// <summary>
        /// The amount of Ice damage the entity takes when the status ends.
        /// </summary>
        private const int IceDamage = 1;

        public FrozenStatus(int duration) : base(duration)
        {
            StatusType = Enumerations.StatusTypes.Frozen;

            StatusIcon = new CroppedTexture2D(AssetManager.Instance.LoadAsset<Texture2D>($"{ContentGlobals.UIRoot}/Battle/BattleGFX"),
                new Rectangle(604, 107, 38, 46));

            AfflictedMessage = "Frozen! Movement will be\nimpossible for a while!";
        }

        protected override void OnAfflict()
        {
            base.OnAfflict();

            //Remove the Burn status if the entity was afflicted with Frozen
            if (EntityAfflicted.EntityProperties.HasStatus(Enumerations.StatusTypes.Burn) == true)
            {
                Debug.Log($"{StatusType} was inflicted on an entity afflicted with {Enumerations.StatusTypes.Burn}, negating both effects!");
                EntityAfflicted.EntityProperties.RemoveStatus(Enumerations.StatusTypes.Burn, true);

                //Also remove Frozen, as these two statuses negate each other
                EntityAfflicted.EntityProperties.RemoveStatus(Enumerations.StatusTypes.Frozen, true);
            }
        }

        protected sealed override void OnEnd()
        {
            base.OnEnd();

            //The entity takes 1 Ice damage when Frozen ends
            EntityAfflicted.TakeDamage(Enumerations.Elements.Ice, IceDamage, true);
        }

        public sealed override StatusEffect Copy()
        {
            return new FrozenStatus(Duration);
        }
    }
}
