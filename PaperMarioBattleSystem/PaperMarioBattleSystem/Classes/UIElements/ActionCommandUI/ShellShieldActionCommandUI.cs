using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static PaperMarioBattleSystem.ActionCommandGlobals;

namespace PaperMarioBattleSystem
{
    public class ShellShieldActionCommandUI : ActionCommandUI<ShellShieldCommand>
    {
        protected float BarScale = 200f;

        protected CroppedTexture2D MovingCursor = null;
        protected CroppedTexture2D BarEnd = null;
        protected CroppedTexture2D BarMiddle = null;
        protected Texture2D Box = null;

        public ShellShieldActionCommandUI(ShellShieldCommand shellShieldCommand) : base(shellShieldCommand)
        {
            Texture2D battleGFX = AssetManager.Instance.LoadRawTexture2D($"{ContentGlobals.BattleGFX}.png");
            MovingCursor = new CroppedTexture2D(battleGFX, new Rectangle(498, 304, 46, 38));

            BarEnd = new CroppedTexture2D(battleGFX, new Rectangle(514, 245, 6, 28));
            BarMiddle = new CroppedTexture2D(battleGFX, new Rectangle(530, 245, 1, 28));

            Box = AssetManager.Instance.LoadRawTexture2D($"{ContentGlobals.UIRoot}/Box.png");
        }

        public override void Draw()
        {
            if (ActionCmd?.AcceptingInput == false) return;

            Vector2 drawPos = new Vector2(150, 200);

            //Draw the bars
            //Draw the middle
            SpriteRenderer.Instance.DrawUI(BarMiddle.Tex, drawPos, BarMiddle.SourceRect, Color.White, 0f, Vector2.Zero, new Vector2(BarScale, 1f), false, false, .39f);

            //Draw the ends
            SpriteRenderer.Instance.DrawUI(BarEnd.Tex, drawPos - new Vector2(BarEnd.SourceRect.Value.Width, 0f), BarEnd.SourceRect, Color.White, 0f, Vector2.Zero, Vector2.One, false, false, .39f);
            SpriteRenderer.Instance.DrawUI(BarEnd.Tex, drawPos + new Vector2(BarScale, 0f), BarEnd.SourceRect, Color.White, 0f, Vector2.Zero, Vector2.One, true, false, .39f);

            //Draw the cursor
            //Regardless of MaxBarVal, needs to be rendered within the range
            float barValScaleFactor = BarScale / ActionCmd.MaxBarVal;
            SpriteRenderer.Instance.DrawUI(MovingCursor.Tex, drawPos + new Vector2(ActionCmd.CurBarVal * barValScaleFactor, 0f), MovingCursor.SourceRect, Color.White, 0f, new Vector2(.5f, 1f), Vector2.One, false, false, .4f);

            //Draw the values of the bar sections
            if (ActionCmd.BarRanges != null)
            {
                for (int i = 0; i < ActionCmd.BarRanges.Length; i++)
                {
                    BarRangeData barRange = ActionCmd.BarRanges[i];
                    Vector2 scale = new Vector2((barRange.EndBarVal - barRange.StartBarVal) * barValScaleFactor, 18f);

                    Vector2 pos = new Vector2(drawPos.X + (barRange.StartBarVal * barValScaleFactor), drawPos.Y + 5f);
                    SpriteRenderer.Instance.DrawUI(Box, pos, null, barRange.SegmentColor, 0f, Vector2.Zero, scale, false, false, .41f);
                }
            }
        }
    }
}
