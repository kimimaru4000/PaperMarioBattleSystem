using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using PaperMarioBattleSystem.Utilities;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// Mash the button to fill the bar to get it further than the cursor.
    /// </summary>
    public sealed class RunAwayCommand : MashButtonCommand
    {
        private double DecelerationRate = .03f;

        public double CurCursorVal { get; private set; } = 0f;

        /// <summary>
        /// The time it takes the cursor to move across the bar one time.
        /// </summary>
        private double CursorMoveTime = 400d;

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
        }

        private void UpdateCursorVal()
        {
            //Move the cursor
            CurCursorVal = UtilityGlobals.PingPong((ElapsedTime + RandTimeOffset) / (CursorMoveTime / MaxBarValue), MaxBarValue);
        }
    }
}
