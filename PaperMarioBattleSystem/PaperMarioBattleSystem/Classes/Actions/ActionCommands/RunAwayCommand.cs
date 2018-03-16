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
    /// Mash the button to fill the bar to get it further than the cursor.
    /// </summary>
    public sealed class RunAwayCommand : MashButtonCommand
    {
        private double DecelerationRate = .03f;

        private double CurCursorVal = 0f;
        private float BarScale = 100f;

        /// <summary>
        /// The time it takes the cursor to move across the bar one time.
        /// </summary>
        private double CursorMoveTime = 400d;

        private CroppedTexture2D MovingCursor = null;

        private double RandTimeOffset = 0d;

        public RunAwayCommand(IActionCommandHandler commandAction, double maxBarValue, double amountPerPress,
            double decelerationRate, double cursorMoveTime, double timeToFill, Keys buttonToPress)
            : base(commandAction, maxBarValue, amountPerPress, timeToFill, buttonToPress)
        {
            DecelerationRate = decelerationRate;
            CursorMoveTime = cursorMoveTime;
        }

        public override void StartInput(params object[] values)
        {
            base.StartInput(values);

            Texture2D battleGFX = AssetManager.Instance.LoadRawTexture2D($"{ContentGlobals.BattleGFX}.png");

            MovingCursor = new CroppedTexture2D(battleGFX, new Rectangle(498, 304, 46, 38));

            //Start the cursor at a random point on the bar
            RandTimeOffset = GeneralGlobals.Randomizer.NextDouble() * (CursorMoveTime * 2);
            UpdateCursorVal();
        }

        protected override void ReadInput()
        {
            //If we're past the run time, check if the bar is past the cursor
            if (ElapsedTime >= TimeToFill)
            {
                //Default to a fail
                CommandResults results = CommandResults.Failure;

                //If the bar is at or past the cursor, we succeeded
                if (CurBarValue >= CurCursorVal)
                {
                    results = CommandResults.Success;
                }

                OnComplete(results);
                return;
            }

            ElapsedTime += Time.ElapsedMilliseconds;

            //Move the cursor
            UpdateCursorVal();

            //If the bar's not full, handle deceleration and check for input
            if (IsBarFull == false)
            {
                //Make the bar go down by a certain amount
                FillBar(-DecelerationRate, true);

                if (AutoComplete == true || Input.GetKeyDown(ButtonToPress) == true)
                {
                    FillBar(AmountPerPress, true);
                }
            }
            else
            {
                //Interpolate the color of the bar
                float colorVal = UtilityGlobals.PingPong(ElapsedTime / 300f, 1f - .3f) + .3f;
                BarFillColor = new Color(colorVal, colorVal, colorVal, 1f);
            }
        }

        private void UpdateCursorVal()
        {
            //Move the cursor
            CurCursorVal = UtilityGlobals.PingPong((ElapsedTime + RandTimeOffset) / (CursorMoveTime / MaxBarValue), MaxBarValue);
        }

        protected override void OnDraw()
        {
            Vector2 barPos = new Vector2(250, 150);
            Vector2 barScale = new Vector2(BarScale, 1f);

            DrawBar(barPos, barScale);
            DrawBarFill(barPos + new Vector2(0f, 5f), new Vector2(barScale.X, 18f));

            //Draw the cursor
            //Regardless of MaxBarValue, needs to be rendered within the range
            float barValScaleFactor = BarScale / (float)MaxBarValue;
            SpriteRenderer.Instance.DrawUI(MovingCursor.Tex, barPos + new Vector2((float)CurCursorVal * barValScaleFactor, 0f), MovingCursor.SourceRect, Color.White, 0f, new Vector2(.5f, 1f), Vector2.One, false, false, .4f);
        }
    }
}
