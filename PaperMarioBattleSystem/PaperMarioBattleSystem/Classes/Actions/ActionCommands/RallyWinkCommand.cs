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
    /// Mash the buttons to fill the bar up to a certain value in a certain amount of time.
    /// The buttons you have to press alternate every now and then.
    /// </summary>
    public class RallyWinkCommand : FillBarCommand
    {
        /// <summary>
        /// The total time for the Action Command.
        /// </summary>
        protected double CommandTime = 0d;

        /// <summary>
        /// The amount the bar fills up per press.
        /// </summary>
        protected double AmountPerPress = 1d;

        /// <summary>
        /// The amount the bar goes down each frame. 
        /// </summary>
        protected double DecelerationRate = .1d;

        /// <summary>
        /// The buttons to press during the Action Command.
        /// </summary>
        protected Keys[] ButtonsToPress = null;

        /// <summary>
        /// The time it takes to switch buttons.
        /// </summary>
        protected double ButtonSwitchTime = 800d;

        private double PrevButtonSwitchTime = 0d;

        protected Vector2 BarScale = Vector2.One;

        /// <summary>
        /// The index of the current button to press.
        /// </summary>
        protected int CurButtonIndex = 0;

        private double ElapsedCommandTime = 0d;

        protected Keys CurButton => ButtonsToPress[CurButtonIndex];

        ActionCommandGlobals.BarRangeData SuccessRange = default(ActionCommandGlobals.BarRangeData);

        public RallyWinkCommand(IActionCommandHandler commandAction, Keys[] buttonsToPress, double maxBarValue, double commandTime,
            double buttonSwitchTime, double amountPerPress, double decelerationRate, Vector2 barScale, ActionCommandGlobals.BarRangeData successRange)
            : base(commandAction, maxBarValue)
        {
            CommandTime = commandTime;
            ButtonsToPress = buttonsToPress;
            ButtonSwitchTime = buttonSwitchTime;

            AmountPerPress = amountPerPress;
            DecelerationRate = decelerationRate;

            BarScale = barScale;

            SuccessRange = successRange;
        }

        public override void StartInput(params object[] values)
        {
            base.StartInput(values);

            PrevButtonSwitchTime = Time.ActiveMilliseconds + ButtonSwitchTime;
        }

        public override void EndInput()
        {
            base.EndInput();

            ButtonsToPress = null;
        }

        protected override void ReadInput()
        {
            //End the Action Command after the command time
            if (ElapsedCommandTime >= CommandTime)
            {
                //Check to see how well the player did and send the appropriate result
                if (CurBarValue >= SuccessRange.StartBarVal)
                {
                    SendCommandRank(CommandRank.Nice);
                    OnComplete(CommandResults.Success);
                }
                else
                {
                    OnComplete(CommandResults.Failure);
                }

                return;
            }

            //Make the bar go down by a certain amount
            FillBar(-DecelerationRate, true);

            //Check if you pressed the correct button
            if (IsBarFull == false)
            {
                if (AutoComplete == true || Input.GetKeyDown(CurButton) == true)
                {
                    //If so, fill up the bar by the correct amount
                    FillBar(AmountPerPress, true);

                    //If the bar is full, the bar flashes color and deceleration no longer applies
                    if (IsBarFull == true)
                        DecelerationRate = 0d;
                }
            }
            else
            {
                //Interpolate the color of the bar
                //Interpolate the color of the bar
                float colorVal = UtilityGlobals.PingPong(ElapsedCommandTime / 300f, .3f, 1f);
                BarFillColor = new Color(colorVal, colorVal, colorVal, 1f);
            }

            ElapsedCommandTime += Time.ElapsedMilliseconds;

            //Check for switching buttons
            if (Time.ActiveMilliseconds >= PrevButtonSwitchTime)
            {
                //Wrap the button index
                CurButtonIndex = UtilityGlobals.Wrap(CurButtonIndex + 1, 0, ButtonsToPress.Length - 1);

                PrevButtonSwitchTime = Time.ActiveMilliseconds + ButtonSwitchTime;
            }
        }

        protected override void OnDraw()
        {
            base.OnDraw();

            Vector2 startPos = new Vector2(250, 200);

            DrawBar(startPos, BarScale);
            DrawBarFill(startPos + new Vector2(0f, 5f), new Vector2(BarScale.X, 18f));

            //Show the buttons you're supposed to press, with the current button highlighted
            for (int i = 0; i < ButtonsToPress.Length; i++)
            {
                Keys button = ButtonsToPress[i];
                Color color = Color.Black;
                if (button == CurButton) color = Color.White;

                SpriteRenderer.Instance.DrawUIText(AssetManager.Instance.TTYDFont, button.ToString(), startPos + new Vector2(i * 15, -30f), color, .5f);
            }
        }
    }
}
