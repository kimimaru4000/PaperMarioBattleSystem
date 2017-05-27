using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace PaperMarioBattleSystem
{
    public class TidalWaveCommand : ActionCommand
    {
        /// <summary>
        /// The total duration of the command. It ends when it reaches this time
        /// </summary>
        private double TotalDuration = 3500d;
        private double StartTime = 0d;
        private int InputLimit = 14;

        private List<Keys> ButtonsPressed = null;
        private Keys NextButtonToPress = Keys.None;

        private Keys[] PossibleButtons = new Keys[] { Keys.Z, Keys.X, Keys.C };

        public TidalWaveCommand(IActionCommandHandler commandAction, double totalDuration = 3500d, int inputLimit = 14) : base(commandAction)
        {
            TotalDuration = totalDuration;
            InputLimit = inputLimit;
        }

        public override void StartInput(params object[] values)
        {
            base.StartInput(values);

            ButtonsPressed = new List<Keys>();

            StartTime = Time.ActiveMilliseconds + TotalDuration;

            NextButtonToPress = GetNextButton();
        }

        public override void EndInput()
        {
            base.EndInput();

            ButtonsPressed.Clear();
            ButtonsPressed = null;
            NextButtonToPress = Keys.None;

            StartTime = 0d;
        }

        /// <summary>
        /// Gets the next button to press for the command
        /// </summary>
        /// <returns>The next button to press for the command</returns>
        private Keys GetNextButton()
        {
            int buttonIndex = GeneralGlobals.Randomizer.Next(0, PossibleButtons.Length);
            return PossibleButtons[buttonIndex];
        }

        protected override void ReadInput()
        {
            if (Time.ActiveMilliseconds >= StartTime)
            {
                OnComplete(CommandResults.Success);
                return;
            }

            //If the user reached the max input amount, stop
            if (ButtonsPressed.Count >= InputLimit)
            {
                return;
            }

            //Check if the player pressed the correct button.
            //If a button from the possible ones was pressed and the correct button wasn't,
            //the command is failed and ends immediately
            bool pressedCorrectButton = false;
            bool pressedIncorrectButton = false;

            for (int i = 0; i < PossibleButtons.Length; i++)
            {
                Keys currentButton = PossibleButtons[i];
                if (Input.GetKeyDown(currentButton) == true)
                {
                    if (currentButton == NextButtonToPress)
                    {
                        //Add the button to the list, send the response, and retrieve the new one
                        ButtonsPressed.Add(NextButtonToPress);
                        SendResponse(ButtonsPressed.Count);

                        NextButtonToPress = GetNextButton();
                        pressedCorrectButton = true;
                        break;
                    }
                    else
                    {
                        pressedIncorrectButton = true;
                    }
                }
            }

            //If the correct button wasn't pressed and another button out of the possible options was pressed, it's a failure
            if (pressedIncorrectButton == true && pressedCorrectButton == false)
            {
                OnComplete(CommandResults.Failure);
            }
        }

        protected override void OnDraw()
        {
            base.OnDraw();

            Vector2 startDrawLoc = new Vector2(250, 150);
            int xPosDiff = 20;
            Vector2 nextPos = startDrawLoc + new Vector2(xPosDiff * ButtonsPressed.Count, 0);

            //Show the buttons pressed
            for (int i = 0; i < ButtonsPressed.Count; i++)
            {
                Keys button = ButtonsPressed[i];
                SpriteRenderer.Instance.DrawText(AssetManager.Instance.TTYDFont, button.ToString(), startDrawLoc + new Vector2(xPosDiff * i, 0), Color.Black, .7f);
            }

            //Show the button that should be pressed next, unless the input limit was reached
            if (ButtonsPressed.Count < InputLimit)
            {
                SpriteRenderer.Instance.DrawText(AssetManager.Instance.TTYDFont, NextButtonToPress.ToString(), nextPos, Color.White, .7f);
            }

            SpriteRenderer.Instance.DrawText(AssetManager.Instance.TTYDFont, Math.Round(StartTime - Time.ActiveMilliseconds, 2).ToString(), startDrawLoc + new Vector2(0, -20), Color.White, .7f);
        }
    }
}
