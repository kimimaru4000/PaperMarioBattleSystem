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
    /// The star that denotes the end of text in a dialogue bubble.
    /// </summary>
    public sealed class ProgressDialogueStar : IRotatable, IScalable, IUpdateable, IDisableable
    {
        /// <summary>
        /// How long it takes the star to rotate.
        /// </summary>
        private const double RotateTime = 600d;

        /// <summary>
        /// The minimum rotation value, in degrees, of the star.
        /// </summary>
        private const float MinRotation = -30f;

        /// <summary>
        /// The maximum rotation value, in degrees, of the star.
        /// </summary>
        private const float MaxRotation = 30f;

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

            Scale = MaxScale;
            Rotation = 0;

            ElapsedTime = RotateTime / 2d;
        }

        public void Update()
        {
            /* The ProgressTextStar does the following:
              1. Rotates back and forth about 30 degrees from 0
              2. Scales down and up very slightly when rotating away from and towards 0, respectively
             */

            ElapsedTime += Time.ElapsedMilliseconds;

            Scale = Interpolation.Interpolate(MinScale, MaxScale, UtilityGlobals.PingPong(ElapsedTime * 2, RotateTime) / RotateTime, Interpolation.InterpolationTypes.Linear);

            float minRot = UtilityGlobals.ToRadians(MinRotation);
            float maxRot = UtilityGlobals.ToRadians(MaxRotation);

            Rotation = UtilityGlobals.PingPong(ElapsedTime / RotateTime, minRot, maxRot);
        }
    }
}
