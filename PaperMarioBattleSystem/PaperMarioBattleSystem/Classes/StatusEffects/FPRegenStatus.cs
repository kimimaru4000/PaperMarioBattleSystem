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
    /// The FPRegen Status Effect.
    /// The entity heals an amount of FP each turn until it ends.
    /// </summary>
    public sealed class FPRegenStatus : StatusEffect
    {
        /// <summary>
        /// The amount of FP to heal each turn.
        /// </summary>
        private int AmountHealed = 0;

        private CroppedTexture2D FPIcon = null;

        public FPRegenStatus(int amountHealed, int duration)
        {
            StatusType = Enumerations.StatusTypes.FPRegen;
            Alignment = StatusAlignments.Positive;

            StatusIcon = new CroppedTexture2D(AssetManager.Instance.LoadAsset<Texture2D>($"{ContentGlobals.UIRoot}/Battle/BattleGFX"),
                new Rectangle(658, 106, 38, 46));

            AmountHealed = amountHealed;
            Duration = duration;

            AfflictedMessage = "FP will briefly recover!";

            FPIcon = new CroppedTexture2D(AssetManager.Instance.LoadAsset<Texture2D>($"{ContentGlobals.UIRoot}/Battle/BattleGFX"),
                new Rectangle(503, 10, 34, 25));
        }

        protected override void OnAfflict()
        {

        }

        protected override void OnEnd()
        {

        }

        protected override void OnPhaseCycleStart()
        {
            EntityAfflicted.HealFP(AmountHealed);
            IncrementTurns();
        }

        protected override void OnSuspend()
        {

        }

        protected override void OnResume()
        {

        }

        public override StatusEffect Copy()
        {
            return new FPRegenStatus(AmountHealed, Duration);
        }

        public override void DrawStatusInfo(Vector2 iconPos, float depth, float turnStringDepth)
        {
            base.DrawStatusInfo(iconPos, depth, turnStringDepth);

            //The FP icon tweens between green and light blue, starting with green
        }
    }
}
