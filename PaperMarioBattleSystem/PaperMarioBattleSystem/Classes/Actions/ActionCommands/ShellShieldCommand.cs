using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using static PaperMarioBattleSystem.ActionCommandGlobals;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// Press A to stop the cursor moving back and forth across the bar.
    /// Aim for the middle for the best result.
    /// </summary>
    public class ShellShieldCommand : ActionCommand
    {
        protected float CurBarVal = 0f;

        protected float BarScale = 100f;
        protected float MinBarVal = 0f;
        protected float MaxBarVal = 100f;

        //How long it takes the cursor to move from one end of the bar to the other.
        protected double CursorTime = 200d;
        protected double CursorTimeVal = 0d;

        protected Keys ButtonToPress = Keys.Z;

        protected CroppedTexture2D MovingCursor = null;
        protected CroppedTexture2D BarEnd = null;
        protected CroppedTexture2D BarMiddle = null;

        protected BarRangeData[] BarRanges = null;

        /// <summary>
        /// The time to do the Action Command.
        /// If the player fails to press the button in this time, the Action Command will send a Failure result.
        /// </summary>
        private double CommandTime = 5000d;
        private double ElapsedTime = 0d;

        protected Texture2D Box = null;

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

            Texture2D battleGFX = AssetManager.Instance.LoadRawTexture2D($"{ContentGlobals.BattleGFX}.png");
            MovingCursor = new CroppedTexture2D(battleGFX, new Rectangle(498, 304, 46, 38));

            BarEnd = new CroppedTexture2D(battleGFX, new Rectangle(514, 245, 6, 28));
            BarMiddle = new CroppedTexture2D(battleGFX, new Rectangle(530, 245, 1, 28));

            Box = AssetManager.Instance.LoadRawTexture2D($"{ContentGlobals.UIRoot}/Box.png");
        }

        public override void EndInput()
        {
            base.EndInput();

            MovingCursor = null;
            BarEnd = null;
            BarMiddle = null;

            BarRanges = null;

            Box = null;
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

        protected override void OnDraw()
        {
            base.OnDraw();

            Vector2 drawPos = new Vector2(150, 200);

            //Draw the bars
            //Draw the middle
            SpriteRenderer.Instance.Draw(BarMiddle.Tex, drawPos, BarMiddle.SourceRect, Color.White, 0f, Vector2.Zero, new Vector2(BarScale, 1f), false, false, .39f, true);

            //Draw the ends
            SpriteRenderer.Instance.Draw(BarEnd.Tex, drawPos - new Vector2(BarEnd.SourceRect.Value.Width, 0f), BarEnd.SourceRect, Color.White, 0f, Vector2.Zero, Vector2.One, false, false, .39f, true);
            SpriteRenderer.Instance.Draw(BarEnd.Tex, drawPos + new Vector2(BarScale, 0f), BarEnd.SourceRect, Color.White, 0f, Vector2.Zero, Vector2.One, true, false, .39f, true);

            //Draw the cursor
            //Regardless of MaxBarVal, needs to be rendered within the range
            float barValScaleFactor = BarScale / MaxBarVal;
            SpriteRenderer.Instance.Draw(MovingCursor.Tex, drawPos + new Vector2(CurBarVal * barValScaleFactor, 0f), MovingCursor.SourceRect, Color.White, 0f, new Vector2(.5f, 1f), Vector2.One, false, false, .4f, true);

            //Draw the values of the bar sections
            if (BarRanges != null)
            {
                for (int i = 0; i < BarRanges.Length; i++)
                {
                    BarRangeData barRange = BarRanges[i];
                    Vector2 scale = new Vector2((barRange.EndBarVal - barRange.StartBarVal) * barValScaleFactor, 18f);

                    Vector2 pos = new Vector2(drawPos.X + (barRange.StartBarVal * barValScaleFactor), drawPos.Y + 5f);
                    SpriteRenderer.Instance.Draw(Box, pos, null, barRange.SegmentColor, 0f, Vector2.Zero, scale, false, false, .41f, true);
                }
            }
        }
    }
}
