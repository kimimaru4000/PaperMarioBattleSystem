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
    /// The Electrified Status Effect.
    /// This grants the Electrified PhysicalAttribute to the entity, causing direct contact from non-Electrified entities to hurt the attacker.
    /// </summary>
    public sealed class ElectrifiedStatus : StatusEffect
    {
        private CroppedTexture2D SparkIcon = null;

        public ElectrifiedStatus(int duration)
        {
            StatusType = Enumerations.StatusTypes.Electrified;
            //Despite having positive effects, Electrified is classified as a Negative StatusEffect.
            //Stone Caps suppress Electrified
            Alignment = StatusAlignments.Negative;

            StatusIcon = new CroppedTexture2D(AssetManager.Instance.LoadAsset<Texture2D>($"{ContentGlobals.UIRoot}/Battle/BattleGFX"),
                new Rectangle(658, 106, 38, 46));

            Duration = duration;

            AfflictedMessage = "Electrified! Enemies that\nmake contact will get hurt!";

            SparkIcon = new CroppedTexture2D(AssetManager.Instance.LoadAsset<Texture2D>($"{ContentGlobals.UIRoot}/Battle/BattleGFX"),
                new Rectangle(458, 103, 26, 30));
        }

        protected override void OnAfflict()
        {
            EntityAfflicted.EntityProperties.AddPhysAttribute(Enumerations.PhysicalAttributes.Electrified);
        }

        protected override void OnEnd()
        {
            EntityAfflicted.EntityProperties.RemovePhysAttribute(Enumerations.PhysicalAttributes.Electrified);
        }

        protected override void OnPhaseCycleStart()
        {
            ProgressTurnCount();
        }

        protected override void OnSuppress(Enumerations.StatusSuppressionTypes statusSuppressionType)
        {
            if (statusSuppressionType == Enumerations.StatusSuppressionTypes.Effects)
            {
                EntityAfflicted.EntityProperties.RemovePhysAttribute(Enumerations.PhysicalAttributes.Electrified);
            }
        }

        protected override void OnUnsuppress(Enumerations.StatusSuppressionTypes statusSuppressionType)
        {
            if (statusSuppressionType == Enumerations.StatusSuppressionTypes.Effects)
            {
                EntityAfflicted.EntityProperties.AddPhysAttribute(Enumerations.PhysicalAttributes.Electrified);
            }
        }

        public override StatusEffect Copy()
        {
            return new ElectrifiedStatus(Duration);
        }

        public override void DrawStatusInfo(Vector2 iconPos, float depth, float turnStringDepth)
        {
            base.DrawStatusInfo(iconPos, depth, turnStringDepth);

            //Draw the Spark symbol
            //It's stored as a separate graphic since it scales

            //The spark is initially at a smaller scale and scales to its normal size roughly every 2 seconds for roughly 10 frames (I eyeballed it from a video)
            float sparkScale = .8f;
            const double smallInterval = 2000d;
            const double largeInterval = 170d;

            //Get the remainder of the small interval. If it's less than the large interval value, then scale up the spark
            double scaleInterval = (Time.ActiveMilliseconds % smallInterval);
            if (scaleInterval < largeInterval) sparkScale = 1f;

            Vector2 sparkOrigin = SparkIcon.SourceRect.Value.GetCenterOrigin();
            sparkOrigin = new Vector2((int)(sparkOrigin.X * 1.5f), (int)(sparkOrigin.Y * 1.5f) - 3);

            Vector2 sparkPos = iconPos + sparkOrigin;
            float sparkDepth = depth + .00001f;

            SpriteRenderer.Instance.Draw(SparkIcon.Tex, sparkPos, SparkIcon.SourceRect, Color.White, 0f, new Vector2(.5f, .5f), sparkScale, false, false, sparkDepth, true);
        }
    }
}
