using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PaperMarioBattleSystem.Utilities;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The star that denotes the end of text in a dialogue bubble.
    /// </summary>
    public sealed class ProgressDialogueStar : IRotatable, IScalable, IUpdateable, IDisableable
    {
        /// <summary>
        /// How long it takes the star to rotate.
        /// </summary>
        private const double RotateTime = 400d;

        /// <summary>
        /// The minimum rotation value, in degrees, of the star.
        /// </summary>
        private const float MinRotation = -25f;

        /// <summary>
        /// The maximum rotation value, in degrees, of the star.
        /// </summary>
        private const float MaxRotation = 25f;

        /// <summary>
        /// The minimum scale of the star.
        /// </summary>
        private readonly Vector2 MinScale = new Vector2(.7f, .7f);

        /// <summary>
        /// The maximum scale of the star.
        /// </summary>
        private readonly Vector2 MaxScale = new Vector2(.8f, .8f);

        public float Rotation { get; set; } = 0f;
        public Vector2 Scale { get; set; } = Vector2.One;

        public CroppedTexture2D Graphic = null;

        public bool Disabled { get; set; } = false;

        private double ElapsedTime = 0d;

        public ProgressDialogueStar()
        {
            Texture2D tex = AssetManager.Instance.LoadRawTexture2D($"{ContentGlobals.BattleGFX}.png");

            Graphic = new CroppedTexture2D(tex, new Rectangle(393, 403, 60, 58));

            Reset();
        }

        public void Reset()
        {
            //Set the time to this value so that the star starts off moving rotating counter-clockwise like in TTYD
            ElapsedTime = RotateTime * 1.5d;
            Rotation = 0f;
            Scale = MaxScale;
        }

        public void Update()
        {
            /* The ProgressTextStar does the following:
              1. Rotates back and forth about 25 degrees from 0
              2. Scales down and up very slightly when rotating away from and towards 0, respectively
             */

            ElapsedTime += Time.ElapsedMilliseconds;

            //Scale twice as fast, as it should be at max scale with a rotation of 0 and min scale when fully rotated in either direction
            Scale = Interpolation.Interpolate(MinScale, MaxScale, UtilityGlobals.PingPong((ElapsedTime * 2) / RotateTime, 1), Interpolation.InterpolationTypes.Linear);
            Rotation = UtilityGlobals.ToRadians(Interpolation.Interpolate(MinRotation, MaxRotation, UtilityGlobals.PingPong(ElapsedTime / RotateTime, 1), Interpolation.InterpolationTypes.Linear));
        }
    }
}
