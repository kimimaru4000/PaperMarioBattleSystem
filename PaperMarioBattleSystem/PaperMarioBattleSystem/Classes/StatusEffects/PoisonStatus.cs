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
    /// The Poison Status Effect.
    /// The entity takes 1 HP in Poison damage at the start of each phase cycle
    /// </summary>
    public sealed class PoisonStatus : StatusEffect
    {
        private int PoisonDamage = 1;

        public PoisonStatus(int duration)
        {
            StatusType = Enumerations.StatusTypes.Poison;
            Alignment = StatusAlignments.Negative;

            StatusIcon = new CroppedTexture2D(AssetManager.Instance.LoadAsset<Texture2D>($"{ContentGlobals.UIRoot}/Battle/BattleGFX"),
                new Rectangle(555, 58, 38, 46));

            Duration = duration;

            AfflictedMessage = "Poisoned! The toxins will\nsteadily do damage!";
        }

        protected override void OnAfflict()
        {
            
        }

        protected override void OnEnd()
        {
            
        }

        protected override void OnPhaseCycleStart()
        {
            //Don't damage the BattleEntity if it's Invincible
            //NOTE: Find a way to route this damage through the damage calculation
            if (EntityAfflicted.IsInvincible() == false)
            {
                EntityAfflicted.TakeDamage(Enumerations.Elements.Poison, PoisonDamage, true);
            }
            IncrementTurns();
        }

        protected override void OnSuppress(Enumerations.StatusSuppressionTypes statusSuppressionType)
        {

        }

        protected override void OnUnsuppress(Enumerations.StatusSuppressionTypes statusSuppressionType)
        {

        }

        public override StatusEffect Copy()
        {
            return new PoisonStatus(Duration);
        }
    }
}
