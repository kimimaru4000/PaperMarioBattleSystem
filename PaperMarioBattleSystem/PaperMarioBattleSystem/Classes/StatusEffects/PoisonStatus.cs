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

        private float RedTint = 0.6277f;
        private float BlueTint = 0.3896f;
        
        public PoisonStatus(int duration)
        {
            StatusType = Enumerations.StatusTypes.Poison;
            Alignment = StatusAlignments.Negative;

            StatusIcon = new CroppedTexture2D(AssetManager.Instance.LoadRawTexture2D($"{ContentGlobals.UIRoot}/Battle/BattleGFX.png"),
                new Rectangle(555, 58, 38, 46));

            Duration = duration;

            AfflictedMessage = "Poisoned! The toxins will\nsteadily do damage!";
        }

        protected override void OnAfflict()
        {
            EntityAfflicted.TintColor = new Color((int)Math.Ceiling(EntityAfflicted.TintColor.R * RedTint), EntityAfflicted.TintColor.G, (int)Math.Ceiling(EntityAfflicted.TintColor.B * BlueTint), EntityAfflicted.TintColor.A);
        }

        protected override void OnEnd()
        {
            Color color = EntityAfflicted.TintColor;
            EntityAfflicted.TintColor = new Color((int)(color.R * (1f / RedTint)), color.G, (int)(color.B * (1f / BlueTint)), color.A);
        }

        protected override void OnPhaseCycleStart()
        {
            if (IsSuppressed(Enumerations.StatusSuppressionTypes.Effects) == false)
            {
                EntityAfflicted.TakeDamage(Enumerations.Elements.Poison, PoisonDamage, true);
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
            return new PoisonStatus(Duration);
        }
    }
}
