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
    /// Mash a button to fill the bar.
    /// </summary>
    public class MashButtonCommand : FillBarCommand
    {
        /// <summary>
        /// The value for infinite time on the Action Command.
        /// If the time to fill the bar is less than or equal to this, there won't be a time limit for filling the bar.
        /// </summary>
        public const double InfiniteTime = -1f;

        /// <summary>
        /// The amount the bar fills up per press.
        /// </summary>
        protected double AmountPerPress = 1f;

        /// <summary>
        /// How much time the player has to fill the bar.
        /// </summary>
        protected double TimeToFill = 10f;

        /// <summary>
        /// The button to press to fill the bar.
        /// </summary>
        protected Keys ButtonToPress = Keys.A;

        protected double ElapsedTime = 0f;

        public MashButtonCommand(IActionCommandHandler commandAction, double maxBarValue, double amountPerPress, double timeToFill, Keys buttonToPress)
            : base(commandAction, maxBarValue)
        {
            AmountPerPress = amountPerPress;
            TimeToFill = timeToFill;
            ButtonToPress = buttonToPress;
        }

        protected override void ReadInput()
        {
            if (TimeToFill > InfiniteTime)
            {
                ElapsedTime += Time.ElapsedMilliseconds;

                //It's a failure as the player didn't fill the bar in time
                if (ElapsedTime >= TimeToFill)
                {
                    OnComplete(CommandResults.Failure);
                    return;
                }
            }

            if (Input.GetKeyDown(ButtonToPress) == true)
            {
                FillBar(AmountPerPress);

                //The player filled the bar, so complete with a success
                if (IsBarFull == true)
                {
                    SendCommandRank(CommandRank.Good);
                    OnComplete(CommandResults.Success);
                }
            }
        }

        protected override void OnDraw()
        {
            DrawBar(new Vector2(250, 150), new Vector2(100, 30f));
        }
    }
}
