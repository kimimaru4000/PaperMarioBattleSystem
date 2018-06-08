using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PaperMarioBattleSystem.Extensions;

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

        //The FP icon tweens between green and light blue, starting with green
        private readonly Color StartColor = new Color(63, 210, 23);
        private readonly Color EndColor = new Color(96, 232, 225);

        public FPRegenStatus(int amountHealed, int duration)
        {
            StatusType = Enumerations.StatusTypes.FPRegen;
            Alignment = StatusAlignments.Positive;

            StatusIcon = new CroppedTexture2D(AssetManager.Instance.LoadRawTexture2D($"{ContentGlobals.UIRoot}/Battle/BattleGFX.png"),
                new Rectangle(658, 106, 38, 46));

            AmountHealed = amountHealed;
            Duration = duration;

            AfflictedMessage = "FP will briefly recover!";

            FPIcon = new CroppedTexture2D(AssetManager.Instance.LoadRawTexture2D($"{ContentGlobals.UIRoot}/Battle/BattleGFX.png"),
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
            if (IsSuppressed(Enumerations.StatusSuppressionTypes.Effects) == false)
            {
                EntityAfflicted.HealFP(AmountHealed);
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
            return new FPRegenStatus(AmountHealed, Duration);
        }

        public override void DrawStatusInfo(Vector2 iconPos, float depth, float turnStringDepth)
        {
            base.DrawStatusInfo(iconPos, depth, turnStringDepth);

            float factor = Math.Abs((float)Math.Sin(Time.ActiveMilliseconds / StatusGlobals.RegenColorLerpTime));

            Color lerpedColor = Color.Lerp(StartColor, EndColor, factor);

            Vector2 fpOrigin = FPIcon.SourceRect.Value.GetCenterOrigin();
            Vector2 fpPos = iconPos + new Vector2((int)(fpOrigin.X / 2) - 6, (int)(fpOrigin.Y / 2));
            float fpDepth = depth + .00001f;

            SpriteRenderer.Instance.DrawUI(FPIcon.Tex, fpPos, FPIcon.SourceRect, lerpedColor, false, false, fpDepth);
        }
    }
}
