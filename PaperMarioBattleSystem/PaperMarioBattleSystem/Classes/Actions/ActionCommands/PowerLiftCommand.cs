using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// Move the cursor across the 3x3 grid and select the red and blue arrows to boost your stats.
    /// Hitting a Poison Mushroom halves the cursor's speed for a short time.
    /// </summary>
    public sealed class PowerLiftCommand : ActionCommand
    {
        private double CommandTime = 30000d;
        private int CursorSpeed = 3;

        private float PoisonFactor = .5f;
        private double PoisonDur = 2000d;

        public PowerLiftCommand(IActionCommandHandler commandHandler, double commandTime) : base(commandHandler)
        {
            CommandTime = commandTime;
        }

        protected override void ReadInput()
        {
            
        }
    }
}
