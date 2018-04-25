using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// A routine that waits an amount of time.
    /// </summary>
    public sealed class WaitRoutine : MessageRoutine
    {
        public double WaitTime = 0d;
        private double ElapsedTime = 0d;

        public WaitRoutine(DialogueBubble bubble, double waitTime) : base(bubble)
        {
            WaitTime = waitTime;
        }

        public override void OnStart()
        {
            ElapsedTime = 0d;

            DBubble.SpeakerEndTalk();
        }

        public override void OnEnd()
        {
            
        }

        public override void Update()
        {
            ElapsedTime += Time.ElapsedMilliseconds;

            if (ElapsedTime >= WaitTime)
            {
                Complete = true;
            }
        }
    }
}
