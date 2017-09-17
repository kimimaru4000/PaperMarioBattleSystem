using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static PaperMarioBattleSystem.Enumerations;
using static PaperMarioBattleSystem.StatusGlobals;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Payback Status Effect.
    /// When direct contact is made with the entity afflicted, the attacker receives half the damage dealt in a specific Element
    /// and can be inflicted with one or more StatusEffects.
    /// </summary>
    public class PaybackStatus : StatusEffect
    {
        protected PaybackHolder Paybackholder = PaybackHolder.Default;

        public PaybackStatus(int duration) : this(duration, new PaybackHolder(PaybackTypes.Half, Elements.Normal, null))
        {

        }

        public PaybackStatus(int duration, PaybackHolder paybackHolder)
        {
            StatusType = StatusTypes.Payback;
            Alignment = StatusAlignments.Positive;

            StatusIcon = new CroppedTexture2D(AssetManager.Instance.LoadAsset<Texture2D>($"{ContentGlobals.UIRoot}/Battle/BattleGFX"),
                new Rectangle(555, 107, 38, 46));

            Duration = duration;

            AfflictedMessage = "Direct attacks will be\ncountered!";

            Paybackholder = paybackHolder;
        }

        protected sealed override void OnAfflict()
        {
            EntityAfflicted.EntityProperties.AddPayback(Paybackholder);
        }

        protected sealed override void OnEnd()
        {
            EntityAfflicted.EntityProperties.RemovePayback(Paybackholder);
        }

        protected sealed override void OnPhaseCycleStart()
        {
            ProgressTurnCount();
        }

        protected sealed override void OnSuppress(StatusSuppressionTypes statusSuppressionType)
        {
            if (statusSuppressionType == StatusSuppressionTypes.Effects)
            {
                EntityAfflicted.EntityProperties.RemovePayback(Paybackholder);
            }
        }

        protected sealed override void OnUnsuppress(StatusSuppressionTypes statusSuppressionType)
        {
            if (statusSuppressionType == StatusSuppressionTypes.Effects)
            {
                EntityAfflicted.EntityProperties.AddPayback(Paybackholder);
            }
        }

        public override StatusEffect Copy()
        {
            return new PaybackStatus(Duration, Paybackholder);
        }
    }
}
