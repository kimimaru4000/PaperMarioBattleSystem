using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    public class Wait : SequenceAction
    {
        protected double EndTime = 0f;

        public Wait(double duration) : base(duration)
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
