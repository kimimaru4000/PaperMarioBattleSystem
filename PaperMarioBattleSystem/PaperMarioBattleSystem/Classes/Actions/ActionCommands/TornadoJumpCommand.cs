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
    /// Press the jump button once before Mario almost hits the enemy.
    /// If successful, press the 3 buttons shown in order to damage all aerial enemies.
    /// </summary>
    public sealed class TornadoJumpCommand : JumpCommand
    {
        private enum Steps
        {
            Jump, Tornado
        }

        private const int TornadoButtonCount = 3;

        private Steps Step = Steps.Jump;

        private Keys[] PossibleButtons = new Keys[] { Keys.Z, Keys.X, Keys.C };
        private Keys[] ButtonsToPress = null;

        private int ButtonIndex = 0;

        private double TornadoTime = 0d;
        private double TornadoEndTime = 0d;

        public TornadoJumpCommand(IActionCommandHandler commandAction, float totalRange, float leniency, double tornadoTime)
            : base(commandAction, totalRange, leniency)
        {
            TornadoTime = tornadoTime;
        }

        public override void StartInput()
        {
            base.StartInput();

            TornadoEndTime = Time.ActiveMilliseconds + TornadoTime;
            ButtonIndex = 0;

            //Set up the buttons to press
            if (Step == Steps.Tornado)
            {
                SetUpButtons();
            }
        }

        public override void EndInput()
        {
            base.EndInput();

            TornadoEndTime = 0d;
            ButtonsToPress = null;
            ButtonIndex = 0;
        }

        protected override void ReadInput()
        {
            if (Step == Steps.Jump)
            {
                JumpActionCommand();
            }
            else
            {
                TornadoActionCommand();
            }
        }

        protected override void OnDraw()
        {
            if (Step == Steps.Jump)
            {
                base.OnDraw();
            }
            else
            {
                Vector2 startDrawLoc = new Vector2(250, 150);
                int xPosDiff = 20;

                //Show the buttons to press; if the button was pressed, show it black
                for (int i = 0; i < ButtonsToPress.Length; i++)
                {
                    Keys button = ButtonsToPress[i];
                    Color buttonColor = Color.White;
                    if (ButtonIndex > i) buttonColor = Color.Black;

                    SpriteRenderer.Instance.DrawText(AssetManager.Instance.TTYDFont, button.ToString(), startDrawLoc + new Vector2(xPosDiff * i, 0), buttonColor, .7f);
                }

                SpriteRenderer.Instance.DrawText(AssetManager.Instance.TTYDFont, Math.Round(TornadoEndTime - Time.ActiveMilliseconds, 2).ToString(), startDrawLoc + new Vector2(0, -20), Color.White, .7f);
            }
        }

        private void JumpActionCommand()
        {
            //Failure if it wasn't pressed in time
            if ((float)Time.ActiveMilliseconds >= EndRange)
            {
                OnComplete(CommandResults.Failure);
                return;
            }

            if (Input.GetKeyDown(ButtonToPress))
            {
                //Send a success if within range and move onto the tornado part
                if (WithinRange == true)
                {
                    SendCommandRank(CommandRank.Nice);

                    Step = Steps.Tornado;
                    OnComplete(CommandResults.Success);
                }
                //Otherwise failure
                else
                {
                    OnComplete(CommandResults.Failure);
                }
            }
        }

        private void TornadoActionCommand()
        {
            //Fail if you took too long to press the buttons
            if (Time.ActiveMilliseconds >= TornadoEndTime)
            {
                OnComplete(CommandResults.Failure);
                return;
            }

            //Check if the player pressed the correct button.
            //If a button from the possible ones was pressed and the correct button wasn't,
            //the command is failed and ends immediately

            //NOTE: In TTYD this acts differently, as if you press any button on the controller, including ones
            //not used in this Action Command, it counts as a failure
            //On a computer it's harder to define what you can and can't press, as you can also be using a controller
            //This will remain the same unless we define a set of buttons that can be mapped
            bool pressedIncorrectButton = false;

            for (int i = 0; i < PossibleButtons.Length; i++)
            {
                Keys currentButton = PossibleButtons[i];
                if (Input.GetKeyDown(currentButton) == true)
                {
                    Keys nextButton = ButtonsToPress[ButtonIndex];

                    if (currentButton == nextButton)
                    {
                        ButtonIndex++;
                        SendResponse(ButtonIndex);

                        break;
                    }
                    //The wrong button was pressed, end in a Failure
                    else
                    {
                        pressedIncorrectButton = true;
                        break;
                    }
                }
            }

            //If all the buttons were pressed, end in a Success
            if (ButtonIndex >= TornadoButtonCount)
            {
                OnComplete(CommandResults.Success);
            }
            //The wrong button was pressed; end in a Failure
            else if (pressedIncorrectButton == true)
            {
                OnComplete(CommandResults.Failure);
            }
        }

        private void SetUpButtons()
        {
            ButtonsToPress = new Keys[TornadoButtonCount];

            for (int i = 0; i < ButtonsToPress.Length; i++)
            {
                int index = GeneralGlobals.Randomizer.Next(TornadoButtonCount);
                ButtonsToPress[i] = PossibleButtons[index];
            }
        }
    }
}
