using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PaperMarioBattleSystem.Utilities;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// A base class for Action Commands that involve filling up a bar
    /// </summary>
    public abstract class FillBarCommand : ActionCommand
    {
        /// <summary>
        /// The amount the bar progressed
        /// </summary>
        public double CurBarValue = 0d;

        /// <summary>
        /// The max value of the bar
        /// </summary>
        public double MaxBarValue = 100d;

        protected CroppedTexture2D BarEnd = null;
        protected CroppedTexture2D BarMiddle = null;
        protected CroppedTexture2D BarFill = null;
        protected Color BarFillColor = Color.White;

        public bool IsBarFull => (CurBarValue >= MaxBarValue);

        protected FillBarCommand(IActionCommandHandler commandAction, double maxBarValue) : base(commandAction)
        {
            MaxBarValue = maxBarValue;
        }

        public override void StartInput(params object[] values)
        {
            base.StartInput(values);

            Texture2D battleGFX = AssetManager.Instance.LoadRawTexture2D($"{ContentGlobals.BattleGFX}.png");

            BarEnd = new CroppedTexture2D(battleGFX, new Rectangle(514, 245, 6, 28));
            BarMiddle = new CroppedTexture2D(battleGFX, new Rectangle(530, 245, 1, 28));
            BarFill = new CroppedTexture2D(battleGFX, new Rectangle(541, 255, 1, 1));

            CurBarValue = 0d;
        }

        public override void EndInput()
        {
            base.EndInput();

            BarEnd = null;
            BarMiddle = null;
            BarFill = null;

            BarFillColor = Color.White;
        }

        protected void FillBar(double amount)
        {
            FillBar(amount, false);
        }

        protected void FillBar(double amount, bool clamp)
        {
            CurBarValue += amount;
            
            //Clamp the bar
            if (clamp == true)
            {
                CurBarValue = UtilityGlobals.Clamp(CurBarValue, 0d, MaxBarValue);
            }
        }

        /// <summary>
        /// Draws the bar at a position with a specified size.
        /// </summary>
        /// <param name="startPos">The position to draw the bar.</param>
        /// <param name="barSize"></param>
        /// <param name="maxValueOverride"></param>
        protected void DrawBar(Vector2 startPos, Vector2 barSize, double? maxValueOverride = null)
        {
            //Use the maxValueOverride for drawing the bar if it's not null. Otherwise, use the MaxBarValue
            //double maxValue = maxValueOverride.HasValue ? maxValueOverride.Value : MaxBarValue;
            //
            //double progressScale = CurBarValue / maxValue;
            //float progressDisplay = (float)progressScale * barSize.X;
            //
            ////Draw the middle
            //SpriteRenderer.Instance.DrawUI(BarMiddle.Tex, startPos, BarMiddle.SourceRect, Color.White, 0f, Vector2.Zero, new Vector2(barSize.X, barSize.Y), false, false, .7f);
            //
            ////Draw the ends
            //SpriteRenderer.Instance.DrawUI(BarEnd.Tex, startPos - new Vector2(BarEnd.SourceRect.Value.Width, 0f), BarEnd.SourceRect, Color.White, 0f, Vector2.Zero, Vector2.One, false, false, .7f);
            //SpriteRenderer.Instance.DrawUI(BarEnd.Tex, startPos + new Vector2(barSize.X, 0f), BarEnd.SourceRect, Color.White, 0f, Vector2.Zero, Vector2.One, true, false, .7f);

            //SpriteRenderer.Instance.Draw(BarImage, startPos, null, Color.Black, 0f, BarImage.GetCenterOrigin(), barSize, false, false, .7f, true);
            //SpriteRenderer.Instance.Draw(BarImage, startPos, null, Color.White, 0f, BarImage.GetCenterOrigin(), new Vector2(Math.Min(progressDisplay, barSize.X), barSize.Y), false, false, .71f, true);
        }

        protected void DrawBarFill(Vector2 startPos, Vector2 barSize, double? maxValueOverride = null)
        {
            //Use the maxValueOverride for drawing the fill if it's not null. Otherwise, use the MaxBarValue
            //double maxValue = maxValueOverride.HasValue ? maxValueOverride.Value : MaxBarValue;
            //
            //double progressScale = CurBarValue / maxValue;
            //float progressDisplay = (float)progressScale * barSize.X;
            //
            ////Regardless of MaxBarVal, needs to be rendered within the range
            //float barValScaleFactor = (float)(barSize.X / maxValue);
            //
            ////Draw the fill
            //SpriteRenderer.Instance.DrawUI(BarFill.Tex, startPos, BarFill.SourceRect, BarFillColor, 0f, Vector2.Zero, new Vector2(progressDisplay, barSize.Y), false, false, .71f);
        }
    }
}
