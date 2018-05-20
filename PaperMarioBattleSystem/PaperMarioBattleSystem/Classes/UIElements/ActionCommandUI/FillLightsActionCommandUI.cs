using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PaperMarioBattleSystem
{
    public class FillLightsActionCommandUI : ActionCommandUI<FillLightsCommand>
    {
        public Vector2 StartPos = Vector2.Zero;
        public float DistBetweenLights = 0f;
        public bool LastLightBig = false;

        protected CroppedTexture2D UnlitLight = null;
        protected CroppedTexture2D LitLight = null;

        public FillLightsActionCommandUI(FillLightsCommand fillLights, Vector2 startPos, float distBetweenLights, bool lastLightBig)
            : base(fillLights)
        {
            StartPos = startPos;

            //Make sure this distance is always positive
            DistBetweenLights = Math.Abs(distBetweenLights);

            LastLightBig = lastLightBig;

            Texture2D battleGFX = AssetManager.Instance.LoadRawTexture2D($"{ContentGlobals.BattleGFX}.png");

            UnlitLight = new CroppedTexture2D(battleGFX, new Rectangle(390, 298, 44, 44));
            LitLight = new CroppedTexture2D(battleGFX, new Rectangle(341, 297, 44, 46));
        }

        public override void Update()
        {
            
        }

        public override void Draw()
        {
            //Get the asset size so the lights can be spaced nicely
            Vector2 assetSize = UnlitLight.WidthHeightToVector2();

            //Go through all the lights
            for (int i = 0; i < ActionCmd.MaxLights; i++)
            {
                Vector2 newpos = StartPos;
                float baseScale = .5f;
                float newScale = baseScale;

                //Check if the last light should be big and enlarge it and offset it if so
                if (LastLightBig == true && i == (ActionCmd.MaxLights - 1))
                {
                    newpos += new Vector2(assetSize.X * (baseScale / 2f), -assetSize.Y * (baseScale / 2f));
                    newScale = 1f;
                }

                newpos += new Vector2((i * (baseScale * assetSize.X)) + (i * (DistBetweenLights * baseScale)), 0);

                CroppedTexture2D light = (i >= ActionCmd.LightsFilled) ? UnlitLight : LitLight;

                SpriteRenderer.Instance.DrawUI(light.Tex, newpos, light.SourceRect, Color.White, 0f, new Vector2(.5f, .5f), newScale, false, false, .7f);
            }
        }
    }
}
