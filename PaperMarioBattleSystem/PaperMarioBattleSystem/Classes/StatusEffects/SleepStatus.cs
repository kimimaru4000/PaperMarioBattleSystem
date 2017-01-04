using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Sleep Status Effect.
    /// Entities afflicted with this cannot move until it ends.
    /// There is a chance that the entity will wake up and end this status when it is attacked
    /// </summary>
    public sealed class SleepStatus : ImmobilizedStatus
    {
        public SleepStatus(int duration) : base(duration)
        {
            StatusType = Enumerations.StatusTypes.Sleep;

            AfflictedMessage = "Sleepy! It'll take time for the sleepiness to wear off!";
        }
        
        public sealed override StatusEffect Copy()
        {
            return new SleepStatus(Duration);
        }
    }
}
