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
    /// The Action Command for Parakarry's Air Lift move.
    /// It's harder to perform depending on the enemy's percentage of being afflicted with the Lifted StatusEffect.
    /// </summary>
    public sealed class AirLiftCommand : MashButtonCommand
    {
        /// <summary>
        /// The rate the bar decelerates.
        /// </summary>
        private double DecelerationRate = .01d;

        /// <summary>
        /// The minimum bar value that deceleration affects.
        /// </summary>
        private double MinBarValue = 0d;

        public AirLiftCommand(IActionCommandHandler commandAction, double minBarValue, double maxBarValue, double amountPerPress,
            double decelerationRate, double timeToFill, Keys buttonToPress)
            : base(commandAction, maxBarValue, amountPerPress, timeToFill, buttonToPress)
        {
            MinBarValue = minBarValue;
            DecelerationRate = decelerationRate;
        }

        public override void StartInput(params object[] values)
        {
            base.StartInput(values);

            double amountPressScale = 1d;

            //The first value passed in should be the percentage to scale the amount of the bar filled per press by
            //BattleEntities with lower chances of being afflicted with Lifted make it more difficult to perform the Action Command
            if (values != null && values.Length == 1)
            {
                amountPressScale = (double)values[0];
            }

            AmountPerPress *= amountPressScale;
        }

        protected override void ReadInput()
        {
            ElapsedTime += Time.ElapsedMilliseconds;

            if (ElapsedTime >= TimeToFill)
            {
                //Send a response with the percentage of the bar filled
                //Automatically mark it as a success
                //Air Lift's Action Command doesn't send a CommandRank (or at least, it doesn't display it in PM)
                SendResponse(CurBarValue / MaxBarValue * 100d);
                OnComplete(CommandResults.Success);
                return;
            }

            //If the bar's not full, handle deceleration and check for input
            if (IsBarFull == false)
            {
                //Make the bar go down by a certain amount if it's past the minimum value
                if (CurBarValue > MinBarValue)
                    FillBar(-DecelerationRate, true);

                if (AutoComplete == true || Input.GetKeyDown(ButtonToPress) == true)
                {
                    //Fill the bar
                    FillBar(AmountPerPress, true);
                }
            }
            else
            {
                //Interpolate the color of the bar
                float colorVal = UtilityGlobals.PingPong(ElapsedTime / 300f, .3f, 1f);
                BarFillColor = new Color(colorVal, colorVal, colorVal, 1f);
            }
        }
    }
}
