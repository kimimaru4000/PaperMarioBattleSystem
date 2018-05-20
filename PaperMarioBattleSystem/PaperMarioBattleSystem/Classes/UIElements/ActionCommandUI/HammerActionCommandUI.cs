using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PaperMarioBattleSystem
{
    public class HammerActionCommandUI : FillLightsActionCommandUI
    {
        protected CroppedTexture2D LightBarEnd = null;
        protected CroppedTexture2D LightBarMiddle = null;

        public HammerActionCommandUI(HammerCommand hammerCommand) : base(hammerCommand, new Vector2(250, 230), 0, true)
        {
            Texture2D battleGFX = AssetManager.Instance.LoadRawTexture2D($"{ContentGlobals.BattleGFX}.png");

            LightBarEnd = new CroppedTexture2D(battleGFX, new Rectangle(795, 14, 15, 22));
            LightBarMiddle = new CroppedTexture2D(battleGFX, new Rectangle(813, 14, 1, 22));
        }

        public override void Update()
        {
            
        }

        public override void Draw()
        {
            if (ActionCmd.AcceptingInput == false) return;

            string text = "NO!";
            Color color = Color.Red;
            if (ActionCmd.AllLightsFilled == true)
            {
                text = "OKAY!";
                color = Color.Green;
            }

            SpriteRenderer.Instance.DrawUIText(AssetManager.Instance.TTYDFont, text, new Vector2(300, 150), color, .7f);

            Vector2 startPos = new Vector2(250, 230);
            Vector2 barStartPos = startPos + new Vector2(-30, 0f);

            Vector2 barScale = new Vector2(130, 1f);

            //Draw the bar
            SpriteRenderer.Instance.DrawUI(LightBarEnd.Tex, barStartPos, LightBarEnd.SourceRect, Color.White, 0f, new Vector2(.5f, .5f), Vector2.One, false, false, .69f);
            SpriteRenderer.Instance.DrawUI(LightBarMiddle.Tex, barStartPos + new Vector2((int)(LightBarEnd.WidthHeightToVector2().X / 2), 0f), LightBarMiddle.SourceRect, Color.White, 0f, new Vector2(.5f, .5f), barScale, false, false, .69f);
            SpriteRenderer.Instance.DrawUI(LightBarEnd.Tex, barStartPos + new Vector2(barScale.X + (int)(LightBarEnd.WidthHeightToVector2().X - 1), 0f), LightBarEnd.SourceRect, Color.White, 0f, new Vector2(.5f, .5f), Vector2.One, true, false, .69f);

            base.Draw();
        }
    }
}
