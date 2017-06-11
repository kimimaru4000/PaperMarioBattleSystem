using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The sequence for the Power Lift move.
    /// </summary>
    public sealed class PowerLiftSequence : CrystalStarMoveSequence
    {
        public PowerLiftSequence(SpecialMoveAction specialAction) : base(specialAction)
        {
        }

        protected override void SequenceMainBranch()
        {
            switch(SequenceStep)
            {
                default:
                    PrintInvalidSequence();
                    break;
            }
        }

        protected override void SequenceSuccessBranch()
        {
            switch (SequenceStep)
            {
                default:
                    PrintInvalidSequence();
                    break;
            }
        }

        protected override void SequenceFailedBranch()
        {
            switch (SequenceStep)
            {
                default:
                    PrintInvalidSequence();
                    break;
            }
        }
    }
}
