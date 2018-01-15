using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Reflection.Emit;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// Mash a button to fill the bar as much as you can in the time allotted.
    /// There is a range of values evenly distributed across the bar.
    /// The end result will be a value based on how much of the bar is filled when the Action Command ends.
    /// <para>For attacks such as Huff N. Puff's Wind Breath, the start value should be higher than the end value so filling the bar
    /// decreases the damage dealt.</para>
    /// </summary>
    public class MashButtonRangeCommand : MashButtonCommand
    {
        protected double DecelerationRate = 1d;

        protected int StartValue = 0;
        protected int EndValue = 1;

        /// <summary>
        /// The current value based on the StartRange, EndRange, and amount the bar is filled.
        /// </summary>
        /// <returns>An integer between StartValue (inclusive) and EndValue (inclusive).</returns>
        protected int CurrentValue
        {
            get
            {
                //If the start value and end value are the same, return either
                if (StartValue == EndValue)
                    return StartValue;

                //Get somewhere between the values
                return UtilityGlobals.Lerp(StartValue, EndValue, (float)(CurBarValue / MaxBarValue));
            }
        }

        public MashButtonRangeCommand(IActionCommandHandler commandAction, double maxBarValue, double amountPerPress, double timeToFill,
            Keys buttonToPress, double decelerationAmount, int startValue, int endValue)
            : base(commandAction, maxBarValue, amountPerPress,timeToFill, buttonToPress)
        {
            DecelerationRate = decelerationAmount;

            StartValue = startValue;
            EndValue = endValue;
        }

        protected override void ReadInput()
        {
            //This command lasts a certain amount of time and is based on performance, so there's no clear failure
            //As a result, always mark it as a success
            if (ElapsedTime >= TimeToFill)
            {
                //Send the current value as a response
                SendResponse(CurrentValue);

                OnComplete(CommandResults.Success);
                return;
            }

            ElapsedTime += Time.ElapsedMilliseconds;

            //Keep lowering the bar by this amount each frame
            FillBar(-DecelerationRate, true);

            if (Input.GetKeyDown(ButtonToPress) == true)
            {
                //Fill the bar for pressing the button
                FillBar(AmountPerPress, true);
            }
        }

        protected override void OnDraw()
        {
            base.OnDraw();
            SpriteRenderer.Instance.DrawUIText(AssetManager.Instance.TTYDFont, CurrentValue.ToString(), new Vector2(355, 150), Color.White, .71f);
        }
    }
}
