using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    public class WaitSeqAction : SequenceAction
    {
        protected double EndTime = 0f;

        public WaitSeqAction(double duration) : base(duration)
        {
            
        }

        protected override void OnStart()
        {
            base.OnStart();

            EndTime = StartTime + Duration;
        }

        protected override void OnUpdate()
        {
            if (Time.ActiveMilliseconds >= EndTime)
            {
                End();
            }
        }
    }
}
