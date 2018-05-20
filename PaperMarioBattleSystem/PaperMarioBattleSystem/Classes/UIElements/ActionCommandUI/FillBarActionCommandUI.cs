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
    public class FillBarActionCommandUI<T> : ActionCommandUI<T> where T : FillBarCommand
    {
        protected Vector2 StartPos = Vector2.Zero;
        protected Vector2 BarSize = Vector2.Zero;
        protected double? MaxValueOverride = null;

        protected CroppedTexture2D BarEnd = null;
        protected CroppedTexture2D BarMiddle = null;
        protected CroppedTexture2D BarFill = null;
        protected Color BarFillColor = Color.White;

        protected double ElapsedTime = 0d;

        public FillBarActionCommandUI(T fillBarCmd, Vector2 startPos, Vector2 barSize, double? maxValueOverride) : base(fillBarCmd)
        {
            StartPos = startPos;
            BarSize = barSize;
            MaxValueOverride = maxValueOverride;

            Texture2D battleGFX = AssetManager.Instance.LoadRawTexture2D($"{ContentGlobals.BattleGFX}.png");

            BarEnd = new CroppedTexture2D(battleGFX, new Rectangle(514, 245, 6, 28));
            BarMiddle = new CroppedTexture2D(battleGFX, new Rectangle(530, 245, 1, 28));
            BarFill = new CroppedTexture2D(battleGFX, new Rectangle(541, 255, 1, 1));
        }

        public override void Update()
        {
            ElapsedTime += Time.ElapsedMilliseconds;

            //Interpolate the color of the bar if it's full
            if (ActionCmd.IsBarFull == true)
            {
                float colorVal = UtilityGlobals.PingPong(ElapsedTime / 300f, .3f, 1f);
                BarFillColor = new Color(colorVal, colorVal, colorVal, 1f);
            }
        }

        public override void Draw()
        {
            if (ActionCmd.AcceptingInput == false) return;

            DrawBar();
            DrawFill(StartPos + new Vector2(0f, 5f), new Vector2(BarSize.X, 18f));
        }

        protected void DrawBar()
        {
            //Use the maxValueOverride for drawing the bar if it's not null. Otherwise, use the MaxBarValue
            double maxValue = MaxValueOverride.HasValue ? MaxValueOverride.Value : ActionCmd.MaxBarValue;

            double progressScale = ActionCmd.CurBarValue / maxValue;
            float progressDisplay = (float)progressScale * BarSize.X;

            //Draw the middle
            SpriteRenderer.Instance.DrawUI(BarMiddle.Tex, StartPos, BarMiddle.SourceRect, Color.White, 0f, Vector2.Zero, new Vector2(BarSize.X, BarSize.Y), false, false, .7f);

            //Draw the ends
            SpriteRenderer.Instance.DrawUI(BarEnd.Tex, StartPos - new Vector2(BarEnd.SourceRect.Value.Width, 0f), BarEnd.SourceRect, Color.White, 0f, Vector2.Zero, Vector2.One, false, false, .7f);
            SpriteRenderer.Instance.DrawUI(BarEnd.Tex, StartPos + new Vector2(BarSize.X, 0f), BarEnd.SourceRect, Color.White, 0f, Vector2.Zero, Vector2.One, true, false, .7f);
        }

        protected void DrawFill(Vector2 startOffset, Vector2 newScale)
        {
            //Use the maxValueOverride for drawing the fill if it's not null. Otherwise, use the MaxBarValue
            double maxValue = MaxValueOverride.HasValue ? MaxValueOverride.Value : ActionCmd.MaxBarValue;

            double progressScale = ActionCmd.CurBarValue / maxValue;
            float progressDisplay = (float)progressScale * newScale.X;

            //Regardless of MaxBarVal, needs to be rendered within the range
            float barValScaleFactor = (float)(BarSize.X / maxValue);

            //Draw the fill
            SpriteRenderer.Instance.DrawUI(BarFill.Tex, startOffset, BarFill.SourceRect, BarFillColor, 0f, Vector2.Zero, new Vector2(progressDisplay, newScale.Y), false, false, .71f);
        }
    }
}
