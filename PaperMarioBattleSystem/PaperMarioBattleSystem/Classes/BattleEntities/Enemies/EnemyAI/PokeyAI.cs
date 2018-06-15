using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using PaperMarioBattleSystem.Extensions;

namespace PaperMarioBattleSystem
{
    public class PokeyAI : EnemyAIBehavior
    {
        private Pokey PokeyRef;

        public PokeyAI(Pokey pokey) : base(pokey)
        {
            PokeyRef = pokey;
        }

        public override void PerformAction()
        {
            //Try to use an item; if so, return
            if (TryUseItem() == true) return;

            int randNum = RandomGlobals.Randomizer.Next(0, 2);

            //Jump if there are no segments left or we chose not to
            if (PokeyRef.SegmentBehavior?.CurSegmentCount <= 0 || randNum == 0)
            {
                Enemy.StartAction(new JumpAction(Enemy), false, Enemy.BManager.FrontPlayer.GetTrueTarget());
            }
            else
            {
                //Throw a body part at the target
                Enemy.StartAction(new BodyThrowAction(Enemy, PokeyRef.SegmentBehavior, PokeyRef.SegmentTex), false, Enemy.BManager.FrontPlayer.GetTrueTarget());
            }
        }
    }
}
