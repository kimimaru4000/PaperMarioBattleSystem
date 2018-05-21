using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using PaperMarioBattleSystem.Utilities;
using static PaperMarioBattleSystem.ActionCommandGlobals;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// Press A to stop the cursor moving back and forth across the bar.
    /// Aim for the middle for the best result.
    /// </summary>
    public class ShellShieldCommand : ActionCommand
    {
        public float CurBarVal { get; protected set; } = 0f;

        protected float BarScale = 100f;
        protected float MinBarVal = 0f;
        public float MaxBarVal { get; protected set; } = 100f;

        //How long it takes the cursor to move from one end of the bar to the other.
        protected double CursorTime = 200d;
        protected double CursorTimeVal = 0d;

        protected Keys ButtonToPress = Keys.Z;

        public BarRangeData[] BarRanges { get; protected set; } = null;

        /// <summary>
        /// The time to do the Action Command.
        /// If the player fails to press the button in this time, the Action Command will send a Failure result.
        /// </summary>
        private double CommandTime = 5000d;
        private double ElapsedTime = 0d;

        public ShellShieldCommand(IActionCommandHandler commandHandler, float barScale, float maxBarVal, double commandTime, double cursorTime,
            params BarRangeData[] barRanges) : base(commandHandler)
        {
            BarScale = barScale;
            MaxBarVal = maxBarVal;
            CommandTime = commandTime;
            CursorTime = cursorTime;

            //The further from the center, the lower the values are and the slower the cursor moves
            BarRanges = barRanges;
        }

        public override void StartInput(params object[] values)
        {
            base.StartInput(values);

            //The cursor starts in the middle and starts out moving to the left
            //Multiply by 1.5 to consider the offset
            CursorTimeVal = CursorTime * 1.5f;
            UpdateCursorVal();

            if (UtilityGlobals.IListIsNullOrEmpty(BarRanges) == true)
            {
                Debug.LogError($"{nameof(BarRanges)} is null or empty, so the command cannot be completed properly. Please input valid data!");
            }
        }

        public override void EndInput()
        {
            base.EndInput();

            BarRanges = null;
        }

        protected override void ReadInput()
        {
            //If the time passed or there's no valid data, end with a failure
            if (ElapsedTime >= CommandTime || UtilityGlobals.IListIsNullOrEmpty(BarRanges) == true)
            {
                OnComplete(CommandResults.Failure);
                return;
            }

            ElapsedTime += Time.ElapsedMilliseconds;
            CursorTimeVal += Time.ElapsedMilliseconds;

            //The cursor value updates before accepting input - this is evident if you press A when doing Shell Shield with frame advance
            UpdateCursorVal();

            if (AutoComplete == true)
            {
                //Assume the last one is the highest value
                BarRangeData rangeData = BarRanges[BarRanges.Length - 1];
                if (rangeData.IsValueInRange(CurBarVal) == true)
                {
                    //Send the response and rank, and complete with a Success
                    SendResponse(rangeData.Value);
                    SendCommandRank(rangeData.Rank);

                    OnComplete(CommandResults.Success);
                }
                return;
            }

            if (Input.GetKeyDown(ButtonToPress) == true)
            {
                //Check the range the cursor is in
                for (int i = 0; i < BarRanges.Length; i++)
                {
                    BarRangeData rangeData = BarRanges[i];

                    //Check for the range
                    if (rangeData.IsValueInRange(CurBarVal) == true)
                    {
                        //Send the response and rank, and complete with a Success
                        SendResponse(rangeData.Value);
                        SendCommandRank(rangeData.Rank);
                        
                        OnComplete(CommandResults.Success);
                        break;
                    }
                }
            }
        }

        private void UpdateCursorVal()
        {
            //The cursor's speed is greater towards the center, so do a Cosine wave
            float maxOverTwo = MaxBarVal / 2f;

            //Offset by the max bar value
            CurBarVal = ((float)Math.Cos(CursorTimeVal / CursorTime) * maxOverTwo) + maxOverTwo;
        }
    }
}
