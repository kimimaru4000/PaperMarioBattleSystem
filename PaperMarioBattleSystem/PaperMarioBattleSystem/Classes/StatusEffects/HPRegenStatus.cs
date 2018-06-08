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
    /// The HPRegen Status Effect.
    /// The entity heals an amount of HP each turn until it ends.
    /// </summary>
    public sealed class HPRegenStatus : StatusEffect
    {
        /// <summary>
        /// The amount of HP to heal each turn.
        /// </summary>
        private int AmountHealed = 0;

        private CroppedTexture2D HPIcon = null;

        //The HP icon tweens between yellow (about the same yellow as Electrified's Spark icon) and dark pink, starting with dark pink
        private readonly Color StartColor = new Color(237, 22, 90);
        private readonly Color EndColor = new Color(254, 231, 5);

        public HPRegenStatus(int amountHealed, int duration)
        {
            StatusType = Enumerations.StatusTypes.HPRegen;
            Alignment = StatusAlignments.Positive;

            StatusIcon = new CroppedTexture2D(AssetManager.Instance.LoadRawTexture2D($"{ContentGlobals.UIRoot}/Battle/BattleGFX.png"),
                new Rectangle(658, 106, 38, 46));

            AmountHealed = amountHealed;
            Duration = duration;

            AfflictedMessage = "HP will briefly recover!";

            HPIcon = new CroppedTexture2D(AssetManager.Instance.LoadRawTexture2D($"{ContentGlobals.UIRoot}/Battle/BattleGFX.png"),
                new Rectangle(503, 60, 35, 24));
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
                EntityAfflicted.HealHP(AmountHealed);
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
            return new HPRegenStatus(AmountHealed, Duration);
        }

        public override void DrawStatusInfo(Vector2 iconPos, float depth, float turnStringDepth)
        {
            base.DrawStatusInfo(iconPos, depth, turnStringDepth);

            float factor = Math.Abs((float)Math.Sin(Time.ActiveMilliseconds / StatusGlobals.RegenColorLerpTime));

            Color lerpedColor = Color.Lerp(StartColor, EndColor, factor);

            Vector2 hpOrigin = HPIcon.SourceRect.Value.GetCenterOrigin();
            Vector2 hpPos = iconPos + new Vector2((int)(hpOrigin.X / 2) - 6, (int)(hpOrigin.Y / 2) + 1);
            float hpDepth = depth + .00001f;

            SpriteRenderer.Instance.DrawUI(HPIcon.Tex, hpPos, HPIcon.SourceRect, lerpedColor, false, false, hpDepth);
        }
    }
}
