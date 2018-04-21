using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// A MessageRoutine that waits for input.
    /// </summary>
    public sealed class InputRoutine : MessageRoutine
    {
        private Keys ButtonToPress;

        public InputRoutine(DialogueBubble bubble, Keys buttonToPress) : base(bubble)
        {
            ButtonToPress = buttonToPress;
        }

        public override void OnStart()
        {
            DBubble.ProgressTextStar.Disabled = false;
            DBubble.SpeakerEndTalk();
        }

        public override void OnEnd()
        {
            DBubble.ProgressTextStar.Disabled = true;
        }

        public override void Update()
        {
            //We should allow going back here
            //If so, we want to add the following to the front of the MessageRoutine Queue (which might need to be a list) in this order:
            //1. A ScrollRoutine that goes up
            //2. An InputRoutine
            //3. A ScrollRoutine that goes down

            //This would make it so you can stack them; if you wish to go back again, the same thing would apply
            //In addition, when you go back down to the paragraph this particular InputRoutine is on, everything would be the same

            if (Input.GetKeyDown(ButtonToPress) == true)
            {
                Complete = true;
            }
        }
    }
}
