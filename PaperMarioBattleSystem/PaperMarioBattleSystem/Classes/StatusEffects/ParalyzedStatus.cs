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
    /// The Paralyzed Status Effect.
    /// Entities afflicted with this cannot move until it ends.
    /// </summary>
    public sealed class ParalyzedStatus : StopStatus
    {
        private CroppedTexture2D SparkIcon = null;

        /// <summary>
        /// The time Paralyzed was afflicted.
        /// </summary>
        private double AfflictionTime = 0d;

        public ParalyzedStatus(int duration) : base(duration)
        {
            StatusType = Enumerations.StatusTypes.Paralyzed;

            StatusIcon = new CroppedTexture2D(AssetManager.Instance.LoadRawTexture2D($"{ContentGlobals.UIRoot}/Battle/BattleGFX.png"),
                new Rectangle(354, 389, 16, 16));

            AfflictedMessage = "Your enemy's paralyzed and can't move!";

            SparkIcon = new CroppedTexture2D(AssetManager.Instance.LoadRawTexture2D($"{ContentGlobals.UIRoot}/Battle/BattleGFX.png"),
                new Rectangle(377, 391, 8, 12));
        }

        protected override void OnAfflict()
        {
            base.OnAfflict();

            AfflictionTime = Time.ActiveMilliseconds;
        }

        public override StatusEffect Copy()
        {
            return new ParalyzedStatus(Duration);
        }

        public override void DrawStatusInfo(Vector2 iconPos, float depth, float turnStringDepth)
        {
            //Move the icon closer since it's smaller than the other icons
            Vector2 sparkOrigin = SparkIcon.SourceRect.Value.GetCenterOrigin();
            iconPos += (sparkOrigin * 2);

            base.DrawStatusInfo(iconPos, depth, turnStringDepth);

            //Figure out a good way to draw PM-only Status Effect icons, as they're much smaller than TTYD ones

            #region Spark Position Offset Logic

            /*The spark does the following:
              1. Wait roughly .6 seconds
              2. Move right one pixel for roughly 2 frames
              3. Move back to its original position for roughly 2 frames
              4. Repeat step 1
              5. Repeat step 2
              6. Wait roughly .75 seconds
              7. Repeat step 1
              8. Repeat step 2
              9. Repeat the entire sequence starting from step 1
            */

            //The start of the interval; when the spark is static
            const double staticInterval = 630d;
            
            //The amount of time the spark moves and multiples of it
            const double offsetInterval = 60d;
            const double offsetIntervalTwo = offsetInterval * 2d;
            const double offsetIntervalThree = offsetInterval * 3d;
            const double offsetIntervalFour = offsetIntervalTwo * 2d;

            //The initial interval where the spark moves two times then waits
            const double firstMoveInterval = offsetIntervalFour + 750d;

            //The second interval where the spark moves once
            const double secondMoveInterval = firstMoveInterval + offsetIntervalFour;
            const double totalInterval = staticInterval + firstMoveInterval + secondMoveInterval;

            //Offset the total time by the time Paralyzed was afflicted
            //This corrects Paralyzed's icon animation to be instance-based instead of global
            double totalTime = Time.ActiveMilliseconds - AfflictionTime;

            double totalRange = (totalTime % totalInterval);

            Vector2 offsetPos = Vector2.Zero;
            const int offsetAmount = 1;

            //After we're past the static interval, check when to offset the spark
            if (totalRange > staticInterval)
            {
                //Subtract the static interval to get values starting from 0
                double firstRange = (totalRange - staticInterval);

                //We're in the first movement interval
                if (firstRange < firstMoveInterval)
                {
                    //Check if we're within the movement ranges and offset the spark if so
                    //There are two movements in this interval
                    if ((firstRange > offsetInterval && firstRange < offsetIntervalTwo)
                        || (firstRange > offsetIntervalThree && firstRange < offsetIntervalFour))
                    {
                        offsetPos.X = offsetAmount;
                    }
                }
                //Otherwise we're in the second movement interval
                else
                {
                    //Subtract the static and first intervals to get values starting from 0
                    double secondRange = (totalRange - staticInterval - firstMoveInterval);

                    //Check if we're within the movement range and offset the spark if so
                    //There's only one movement in this interval
                    if ((secondRange > offsetInterval && secondRange < offsetIntervalTwo))
                    {
                        offsetPos.X = offsetAmount;
                    }
                }
            }

            #endregion

            Vector2 sparkPos = iconPos + new Vector2(sparkOrigin.X, 2) + offsetPos;
            float sparkDepth = depth + .00001f;

            SpriteRenderer.Instance.DrawUI(SparkIcon.Tex, sparkPos, SparkIcon.SourceRect, Color.White, 0f, Vector2.Zero, 1f, false, false, sparkDepth);
        }
    }
}
