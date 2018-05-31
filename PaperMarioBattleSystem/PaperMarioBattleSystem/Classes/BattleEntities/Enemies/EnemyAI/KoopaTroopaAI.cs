using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// Koopa Troopa enemy AI.
    /// </summary>
    public class KoopaTroopaAI : EnemyAIBehavior
    {
        private KoopaTroopa koopaTroopa = null;

        /// <summary>
        /// The MoveAction to perform.
        /// </summary>
        protected virtual MoveAction ActionPerformed => new ShellTossAction(Enemy);

        public KoopaTroopaAI(KoopaTroopa koopatroopa) : base(koopatroopa)
        {
            koopaTroopa = koopatroopa;
        }

        public override void PerformAction()
        {
            //If it's flipped, don't do anything
            if (koopaTroopa.FlippedBehavior.Flipped == false)
            {
                koopaTroopa.StartAction(ActionPerformed, false, Enemy.BManager.FrontPlayer.GetTrueTarget());
            }
            else
            {
                koopaTroopa.StartAction(new NoAction(Enemy), true, null);
            }
        }
    }
}
