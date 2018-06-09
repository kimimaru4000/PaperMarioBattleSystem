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
    /// <summary>
    /// Press a random sequence of buttons in the correct order within the time limit.
    /// </summary>
    public class MultiButtonCommand : ActionCommand
    {
        /// <summary>
        /// The minimum number of buttons that can appear in the button sequence.
        /// This should be greater than 0.
        /// </summary>
        protected int MinButtons = 1;

        /// <summary>
        /// The maximum number of buttons that can appear in the button sequence.
        /// </summary>
        protected int MaxButtons = 5;

        /// <summary>
        /// How long the player has to input all the buttons.
        /// </summary>
        public double InputDuration { get; protected set; } = 0d;

        /// <summary>
        /// The valid buttons that can be featured in the sequence.
        /// </summary>
        protected Keys[] ValidSequenceButtons = null;

        /// <summary>
        /// The final generated button sequence.
        /// </summary>
        public Keys[] ButtonSequence { get; protected set; }= null;

        /// <summary>
        /// The current index of the button to press.
        /// </summary>
        public int CurButtonIndex {get; protected set;}= 0;

        /// <summary>
        /// The next button to press in the button sequence.
        /// </summary>
        protected Keys NextButtonToPress => ButtonSequence[CurButtonIndex];

        /// <summary>
        /// The amount of time elapsed since the Action Command started.
        /// </summary>
        private double ElapsedTime = 0d;

        public MultiButtonCommand(IActionCommandHandler commandHandler, int minButtons, int maxButtons, double inputDuration,
            params Keys[] validSequenceButtons) : base(commandHandler)
        {
            MinButtons = minButtons;
            MaxButtons = maxButtons;
            InputDuration = inputDuration;
            ValidSequenceButtons = validSequenceButtons;
        }

        public override void StartInput(params object[] values)
        {
            base.StartInput(values);

            //Generate the button sequence
            GenerateButtonSequence();

            ElapsedTime = 0d;
        }

        protected override void ReadInput()
        {
            //End the Action Command with a Failure when time is up
            if (ElapsedTime >= InputDuration)
            {
                OnComplete(CommandResults.Failure);
                return;
            }

            //Increment time
            ElapsedTime += Time.ElapsedMilliseconds;

            //Check if the player pressed the correct button
            //If a button from the possible ones was pressed and the correct button wasn't,
            //the command is failed and ends immediately
            bool pressedCorrectButton = false;
            bool pressedIncorrectButton = false;

            //Go through all valid buttons and see which one was pressed
            for (int i = 0; i < ValidSequenceButtons.Length; i++)
            {
                Keys buttonPressed = ValidSequenceButtons[i];
                if (AutoComplete == true || Input.GetKeyDown(buttonPressed) == true)
                {
                    //If the button pressed is the next one that should be pressed, we pressed the correct one
                    if (AutoComplete == true || buttonPressed == NextButtonToPress)
                    {
                        pressedCorrectButton = true;
                        break;
                    }
                    else
                    {
                        pressedIncorrectButton = true;
                    }
                }
            }

            //If we pressed the correct button, progress
            if (pressedCorrectButton == true)
            {
                //Increment the button index and check if we pressed all the buttons
                CurButtonIndex++;
                if (CurButtonIndex >= ButtonSequence.Length)
                {
                    //If we pressed all the buttons, end the Action Command on a success
                    SendCommandRank(CommandRank.Nice);
                    OnComplete(CommandResults.Success);
                }
            }
            //If we pressed the incorrect button and not the correct one, it's a failure
            else if (pressedIncorrectButton == true)
            {
                OnComplete(CommandResults.Failure);
            }
        }

        protected virtual void GenerateButtonSequence()
        {
            //Get the number of buttons to be in the sequence
            int buttonsInSequence = RandomGlobals.Randomizer.Next(MinButtons, MaxButtons + 1);
            ButtonSequence = new Keys[buttonsInSequence];

            //Go through the number of buttons and assign a random button out of the valid ones
            for (int i = 0; i < ButtonSequence.Length; i++)
            {
                int randButtonIndex = RandomGlobals.Randomizer.Next(0, ValidSequenceButtons.Length);
                ButtonSequence[i] = ValidSequenceButtons[randButtonIndex];
            }
        }
    }
}
