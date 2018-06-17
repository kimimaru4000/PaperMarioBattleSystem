using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// Hold R until the light lights up.
    /// <para>The amount the bar increases is based on elapsed time and the speed scale.
    /// The MaxBarValue represents the total duration of the Action Command.</para>
    /// </summary>
    public sealed class GulpCommand : FillBarCommand
    {
        /// <summary>
        /// The value the success range starts.
        /// </summary>
        public double SuccessStartValue { get; private set; } = 0d;

        /// <summary>
        /// The amount of time the light is lit up for the success to be valid.
        /// </summary>
        public double SuccessRange { get; private set; } = 0d;

        /// <summary>
        /// How much faster or slower to progress the bar.
        /// </summary>
        private double SpeedScale = 1f;

        private double EndTime = 0d;

        private Keys KeyToHold = Keys.Z;

        private bool StartedHolding = false;

        public bool WithinRange => (CurBarValue >= SuccessStartValue && CurBarValue < MaxBarValue);

        public GulpCommand(IActionCommandHandler commandAction, double totalDuration, double successRange, double speedScale, Keys buttonToHold) : base(commandAction, totalDuration)
        {
            SuccessRange = successRange;
            SpeedScale = speedScale;

            KeyToHold = buttonToHold;
        }

        public override void StartInput(params object[] values)
        {
            base.StartInput(values);

            SuccessStartValue = MaxBarValue - SuccessRange;

            EndTime = Time.ActiveMilliseconds + MaxBarValue;
        }

        protected override void ReadInput()
        {
            //If the command is going past the total duration, stop
            if (Time.ActiveMilliseconds > EndTime)
            {
                OnComplete(CommandResults.Failure);
                return;
            }

            //If you started holding then stopped, check when the player stopped holding
            if (StartedHolding == true && Input.GetKey(KeyToHold) == false && AutoComplete == false)
            {
                if (WithinRange == true)
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

            //Have the bar keep increasing, and make the light light up at a certain value range
            //The bar value can increase further than the light, but it won't show past the light
            if (AutoComplete == true || Input.GetKey(KeyToHold) == true)
            {
                StartedHolding = true;

                FillBar(Time.ElapsedMilliseconds * SpeedScale);

                if (WithinRange == true)
                    AutoComplete = false;

                //If the button was held too long, it's a failure
                if (CurBarValue > MaxBarValue)
                {
                    OnComplete(CommandResults.Failure);
                }
            }
        }
    }
}
