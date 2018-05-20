using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PaperMarioBattleSystem
{
    public class GulpActionCommandUI : FillBarActionCommandUI<GulpCommand>
    {
        private CroppedTexture2D UnlitLight = null;
        private CroppedTexture2D LitLight = null;

        public GulpActionCommandUI(GulpCommand gulpCmd) : base(gulpCmd, new Vector2(250, 200), new Vector2(100f, 1f), gulpCmd.MaxBarValue - gulpCmd.SuccessRange)
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
            CroppedTexture2D light = UnlitLight;
            if (ActionCmd.WithinRange == true)
            {
                text = "OKAY!";
                color = Color.Green;
                light = LitLight;
            }

            SpriteRenderer.Instance.DrawUIText(AssetManager.Instance.TTYDFont, text, new Vector2(300, 150), color, .7f);

            base.Draw();

            //Get the start and end ranges
            float startScale = (float)(ActionCmd.SuccessStartValue / ActionCmd.MaxBarValue) * BarSize.X;
            float endScale = BarSize.X;

            Vector2 lightStartPos = StartPos + new Vector2((int)startScale, 0f);
            Vector2 lightEndPos = StartPos + new Vector2((int)endScale, 0f);

            int diff = (int)(lightEndPos.X - StartPos.X);

            SpriteRenderer.Instance.DrawUI(light.Tex, lightStartPos + new Vector2((diff / 2) - BarEnd.WidthHeightToVector2().X, 12f), light.SourceRect, Color.White, 0f, new Vector2(.5f, .5f), 1f, false, false, .8f);
        }
    }
}
