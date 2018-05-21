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
    /// Press A when the cursor is near the middle of the big circle.
    /// </summary>
    public sealed class TattleCommand : ActionCommand
    {
        /*I'm making this based on position instead of time as I feel this is linear enough where neither implementation would make a difference
         If for some reason this doesn't work out, feel free to make it timing dependent*/

        public const int BigCursorSize = 46;
        public const int SmallCursorStartOffset = 150;

        public Vector2 BigCursorPos { get; private set; } = Vector2.Zero;
        public Vector2 SmallCursorPos { get; private set; } = Vector2.Zero;

        private float SmallCursorSpeed = 2f;
        private float ElapsedTime = 0f;

        private Keys InputButton = Keys.Z;

        public Rectangle SuccessRect { get; private set; } = new Rectangle(0, 0, BigCursorSize, BigCursorSize);

        public bool WithinRange => (SuccessRect.Contains(SmallCursorPos));
        private bool PastRange => (SmallCursorPos.X > (SuccessRect.Right + SuccessRect.Width));

        public TattleCommand(IActionCommandHandler commandHandler, float smallCursorSpeed) : base(commandHandler)
        {
            SmallCursorSpeed = smallCursorSpeed;

            //Description = "Line up the small cursor with\n the center of the big cursor!"
        }

        public override void StartInput(params object[] values)
        {
            base.StartInput(values);

            if (values == null || values.Length == 0)
            {
                Debug.LogError($"{nameof(TattleCommand)} requires the position of the BattleEntity targeted to place the cursor in the correct spot!");
                return;
            }

            BigCursorPos = Camera.Instance.SpriteToUIPos((Vector2)values[0]);
            SmallCursorPos = BigCursorPos - new Vector2(SmallCursorStartOffset, 0);

            SuccessRect = new Rectangle((int)BigCursorPos.X - (BigCursorSize / 2), (int)BigCursorPos.Y - (BigCursorSize / 2), BigCursorSize, BigCursorSize);
        }

        protected override void ReadInput()
        {
            //Failed if no input was pressed on time
            if (PastRange == true)
            {
                OnComplete(CommandResults.Failure);
                return;
            }

            //Check if the player pressed the input button at the right time
            if (AutoComplete == true || Input.GetKeyDown(InputButton) == true)
            {
                //In range, so success
                if (WithinRange == true)
                {
                    SendCommandRank(CommandRank.Nice);

                    OnComplete(CommandResults.Success);
                    return;
                }
                //Out of range, so a failure
                else if (AutoComplete == false)
                {
                    OnComplete(CommandResults.Failure);
                    return;
                }
            }

            SmallCursorPos = new Vector2(SmallCursorPos.X + SmallCursorSpeed, SmallCursorPos.Y);
            ElapsedTime += (float)Time.ElapsedMilliseconds;
        }
    }
}
