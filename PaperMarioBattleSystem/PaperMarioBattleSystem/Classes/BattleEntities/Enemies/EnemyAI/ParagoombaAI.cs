using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// Paragoomba enemy AI.
    /// </summary>
    public sealed class ParagoombaAI : GoombaAI
    {
        private Paragoomba paragoomba = null;

        public ParagoombaAI(Paragoomba pGoomba) : base(pGoomba)
        {
            paragoomba = pGoomba;
        }

        public override void PerformAction()
        {
            if (paragoomba.WingedBehavior.Grounded == false)
            {
                Enemy.StartAction(new DiveKick(), false, BattleManager.Instance.GetFrontPlayer().GetTrueTarget());
            }
            else
            {
                base.PerformAction();
            }
        }
    }
}
