using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// A base class for Action Commands that involve lighting lights.
    /// </summary>
    public abstract class FillLightsCommand : ActionCommand
    {
        /// <summary>
        /// The max number of lights to fill
        /// </summary>
        protected int MaxLights = 4;

        /// <summary>
        /// The time between each light being lit
        /// </summary>
        protected double TimeBetweenLights = 500d;

        /// <summary>
        /// The number of lights filled
        /// </summary>
        protected int LightsFilled = 0;
        protected double PrevLightTime = 0d;

        protected Texture2D CircleImage = null;

        public bool AllLightsFilled => (LightsFilled == MaxLights);

        public FillLightsCommand(ICommandAction commandAction, int maxLights, double timeBetweenLights) : base(commandAction)
        {
            MaxLights = maxLights;
            TimeBetweenLights = timeBetweenLights;

            CircleImage = AssetManager.Instance.LoadAsset<Texture2D>($"UI/Circle");
        }

        public void FillNextLight()
        {
            LightsFilled++;
            PrevLightTime = Time.ActiveMilliseconds + TimeBetweenLights;
        }

        public void DrawLights(Vector2 startPos, float distBetweenLights, bool lastLightBig)
        {
            //Make sure this distance is always positive
            distBetweenLights = Math.Abs(distBetweenLights);

            for (int i = 0; i < MaxLights; i++)
            {
                Vector2 newpos = startPos + new Vector2((i * CircleImage.Width) + (i * distBetweenLights), 0);
                float scale = 1f;
                if (lastLightBig == true && i == (MaxLights - 1))
                {
                    newpos += new Vector2(CircleImage.Width / 2, -(CircleImage.Height / 2));
                    scale = 2f;
                }
                Color circleColor = i >= LightsFilled ? Color.Black : Color.White;

                SpriteRenderer.Instance.Draw(CircleImage, newpos, null, circleColor, 0f, CircleImage.GetCenterOrigin(), scale, false, .7f, true);
            }
        }
    }
}
