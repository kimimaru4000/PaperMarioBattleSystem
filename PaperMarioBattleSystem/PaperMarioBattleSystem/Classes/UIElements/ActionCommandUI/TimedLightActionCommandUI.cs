using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PaperMarioBattleSystem
{
    public class TimedLightActionCommandUI : FillBarActionCommandUI<TimedLightCommand>
    {
        protected CroppedTexture2D UnlitLight = null;
        protected CroppedTexture2D LitLight = null;

        public TimedLightActionCommandUI(TimedLightCommand timedLightCommand) : base(timedLightCommand, new Vector2(250, 200), new Vector2(100f, 1f), null)
        {
            Texture2D battleGFX = AssetManager.Instance.LoadRawTexture2D($"{ContentGlobals.BattleGFX}.png");

            UnlitLight = new CroppedTexture2D(battleGFX, new Rectangle(390, 298, 44, 44));
            LitLight = new CroppedTexture2D(battleGFX, new Rectangle(341, 297, 44, 46));
        }

        public override void Draw()
        {
            if (ActionCmd?.AcceptingInput == false) return;

            string text = "NO!";
            Color color = Color.Red;
            if (ActionCmd.WithinRange == true)
            {
                text = "OKAY!";
                color = Color.Green;
            }

            SpriteRenderer.Instance.DrawUIText(AssetManager.Instance.TTYDFont, text, new Vector2(300, 150), color, .7f);

            base.Draw();

            for (int i = 0; i < ActionCmd.NumLights; i++)
            {
                CroppedTexture2D light = UnlitLight;

                //Draw the light as lit if the bar is in or past the light
                if (ActionCmd.CurLight > i || (ActionCmd.CurLight == i && ActionCmd.WithinRange == true))
                    light = LitLight;

                //Get the start and end ranges
                float startScale = (float)(ActionCmd.LightRanges[i].StartRange / ActionCmd.MaxBarValue) * BarSize.X;
                float endScale = (float)(ActionCmd.LightRanges[i].EndRange / ActionCmd.MaxBarValue) * BarSize.X;

                Vector2 lightStartPos = StartPos + new Vector2((int)startScale, 0f);
                Vector2 lightEndPos = StartPos + new Vector2((int)endScale, 0f);

                //We know the start and end positions, so get the difference for the size
                int xDiff = (int)(lightEndPos.X - lightStartPos.X);

                //Get the midpoint
                Vector2 lightMidPos = new Vector2(lightStartPos.X + (xDiff / 2), lightStartPos.Y + 6f);

                //If the asset is 44x44, the range was 44, and the bar scale was 100, it would be 1
                //The asset should fit inside xDiff; that's the size it should be
                //diff / assetSize
                Vector2 lightScale = new Vector2(xDiff / (float)light.SourceRect.Value.Width);

                //Debug.DebugDrawLine(lightStartPos, lightStartPos + new Vector2(0, 24f), Color.White, .9f, 1, true);
                //Debug.DebugDrawLine(lightEndPos, lightEndPos + new Vector2(0, 24f), Color.White, .9f, 1, true);

                SpriteRenderer.Instance.DrawUI(light.Tex, lightMidPos, light.SourceRect, Color.White, 0f, new Vector2(.5f, 0), lightScale, false, false, .8f);
            }
        }
    }
}
