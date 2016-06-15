using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// Mario's Guard action when being attacked.
    /// It's unique in that it reduces the damage Piercing attacks do
    /// </summary>
    public class Guard : BattleAction
    {
        public Guard()
        {
            Name = "Guard";
        }

        public override void OnCommandSuccess()
        {
            
        }

        public override void OnCommandFailed()
        {
            
        }

        public override void OnCommandResponse(int response)
        {

        }

        protected override void SequenceStartBranch()
        {
            
        }

        protected override void SequenceCommandBranch()
        {
            
        }

        protected override void SequenceSuccessBranch()
        {
            
        }

        protected override void SequenceFailedBranch()
        {
            
        }

        protected override void SequenceEndBranch()
        {
            
        }
    }
}
