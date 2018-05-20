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
    public sealed class TattleActionCommandUI : ActionCommandUI<TattleCommand>
    {
        private UIFourPiecedTex BigCursor = null;
        private UIFourPiecedTex SmallCursor = null;
        //private CroppedTexture2D BigCursor = null;
        //private CroppedTexture2D SmallCursor = null;

        private double ElapsedTime = 0d;

        public TattleActionCommandUI(TattleCommand tattleCommand) : base(tattleCommand)
        {
            Texture2D battleGFX = AssetManager.Instance.LoadRawTexture2D($"{ContentGlobals.UIRoot}/Battle/BattleGFX.png");

            BigCursor = new UIFourPiecedTex(new CroppedTexture2D(battleGFX, new Rectangle(14, 273, 46, 46)));
            SmallCursor = new UIFourPiecedTex(new CroppedTexture2D(battleGFX, new Rectangle(10, 330, 13, 12)));

            BigCursor.OriginOffset = new Vector2((float)BigCursor.CroppedTex2D.SourceRect.Value.Width, (float)BigCursor.CroppedTex2D.SourceRect.Value.Height);
            SmallCursor.OriginOffset = new Vector2(SmallCursor.CroppedTex2D.SourceRect.Value.Width, SmallCursor.CroppedTex2D.SourceRect.Value.Height);
        }

        public override void Update()
        {
            ElapsedTime += Time.ElapsedMilliseconds;

            BigCursor.Rotation = (float)(-ElapsedTime * UtilityGlobals.ToRadians(.1f));
            BigCursor.Position = ActionCmd.BigCursorPos;
            SmallCursor.Position = ActionCmd.SmallCursorPos;
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

            BigCursor.Draw();
            SmallCursor.Draw();

            if (ActionCmd.WithinRange == true)
            {
                Vector2 prevScale = BigCursor.Scale;

                //Cap the scale so it can be seen clearly at all times
                const float maxScale = .8f;

                //Get the absolute value of the distance from the cursor to the center
                //Divide by half the SuccessRect's width since we're scaling based on how close it is to the center
                float diff = Math.Abs(ActionCmd.SmallCursorPos.X - ActionCmd.SuccessRect.Center.X) / (ActionCmd.SuccessRect.Width / 2f);
                float scale = UtilityGlobals.Clamp(diff, 0f, maxScale);

                BigCursor.Scale = new Vector2(scale);
                BigCursor.Draw();

                BigCursor.Scale = prevScale;
            }
        }
    }
}
