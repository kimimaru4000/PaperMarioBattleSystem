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
        public int MaxLights = 4;

        /// <summary>
        /// The time between each light being lit
        /// </summary>
        protected double TimeBetweenLights = 500d;

        /// <summary>
        /// The number of lights filled
        /// </summary>
        public int LightsFilled = 0;
        protected double PrevLightTime = 0d;

        protected CroppedTexture2D UnlitLight = null;
        protected CroppedTexture2D LitLight = null;

        public bool AllLightsFilled => (LightsFilled == MaxLights);

        public FillLightsCommand(IActionCommandHandler commandAction, int maxLights, double timeBetweenLights) : base(commandAction)
        {
            MaxLights = maxLights;
            TimeBetweenLights = timeBetweenLights;
        }

        public override void StartInput(params object[] values)
        {
            base.StartInput(values);

            Texture2D battleGFX = AssetManager.Instance.LoadRawTexture2D($"{ContentGlobals.BattleGFX}.png");

            UnlitLight = new CroppedTexture2D(battleGFX, new Rectangle(390, 298, 44, 44));
            LitLight = new CroppedTexture2D(battleGFX, new Rectangle(341, 297, 44, 46));
        }

        public override void EndInput()
        {
            base.EndInput();

            UnlitLight = null;
            LitLight = null;
        }

        public void FillNextLight()
        {
            LightsFilled++;
            PrevLightTime = Time.ActiveMilliseconds + TimeBetweenLights;
        }

        /*public void DrawLights(Vector2 startPos, float distBetweenLights, bool lastLightBig)
        {
            //Make sure this distance is always positive
            distBetweenLights = Math.Abs(distBetweenLights);

            //Get the asset size so the lights can be spaced nicely
            Vector2 assetSize = UnlitLight.WidthHeightToVector2();

            //Go through all the lights
            for (int i = 0; i < MaxLights; i++)
            {
                Vector2 newpos = startPos;
                float baseScale = .5f;
                float newScale = baseScale;

                //Check if the last light should be big and enlarge it and offset it if so
                if (lastLightBig == true && i == (MaxLights - 1))
                {
                    newpos += new Vector2(assetSize.X * (baseScale / 2f), -assetSize.Y * (baseScale / 2f));
                    newScale = 1f;
                }

                newpos += new Vector2((i * (baseScale * assetSize.X)) + (i * (distBetweenLights * baseScale)), 0);

                CroppedTexture2D light = (i >= LightsFilled) ? UnlitLight : LitLight;

                SpriteRenderer.Instance.DrawUI(light.Tex, newpos, light.SourceRect, Color.White, 0f, new Vector2(.5f, .5f), newScale, false, false, .7f);
            }
        }*/
    }
}
