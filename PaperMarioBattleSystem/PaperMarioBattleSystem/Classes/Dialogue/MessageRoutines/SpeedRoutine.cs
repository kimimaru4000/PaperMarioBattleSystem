using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// A routine that sets the time it takes to print characters.
    /// </summary>
    public sealed class SpeedRoutine : MessageRoutine
    {
        private double Speed = 0d;

        public SpeedRoutine(DialogueBubble bubble, in double speed) : base(bubble)
        {
            Speed = speed;
        }

        public override void OnStart()
        {
            DBubble.TimeBetweenCharacters = Speed;

            Complete = true;
        }

        public override void OnEnd()
        {
            
        }

        public override void Update()
        {
            
        }
    }
}
