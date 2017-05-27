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
    /// A base class for Action Commands that involve filling up a bar
    /// </summary>
    public abstract class FillBarCommand : ActionCommand
    {
        /// <summary>
        /// The amount the bar progressed
        /// </summary>
        protected double CurBarValue = 0d;

        /// <summary>
        /// The max value of the bar
        /// </summary>
        protected double MaxBarValue = 100d;

        protected Texture2D BarImage = null;

        protected bool IsBarFull => (CurBarValue >= MaxBarValue);

        public FillBarCommand(IActionCommandHandler commandAction, double maxBarValue) : base(commandAction)
        {
            MaxBarValue = maxBarValue;

            BarImage = AssetManager.Instance.LoadAsset<Texture2D>("UI/Box");
        }

        public override void StartInput(params object[] values)
        {
            base.StartInput(values);

            CurBarValue = 0d;
        }

        protected void FillBar(double amount)
        {
            CurBarValue += amount;
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
            double maxValue = maxValueOverride.HasValue ? maxValueOverride.Value : MaxBarValue;

            double progressScale = CurBarValue / maxValue;
            float progressDisplay = (float)progressScale * barSize.X;

            SpriteRenderer.Instance.Draw(BarImage, startPos, null, Color.Black, 0f, BarImage.GetCenterOrigin(), barSize, false, false, .7f, true);
            SpriteRenderer.Instance.Draw(BarImage, startPos, null, Color.White, 0f, BarImage.GetCenterOrigin(), new Vector2(Math.Min(progressDisplay, barSize.X), barSize.Y), false, false, .71f, true);
        }
    }
}
