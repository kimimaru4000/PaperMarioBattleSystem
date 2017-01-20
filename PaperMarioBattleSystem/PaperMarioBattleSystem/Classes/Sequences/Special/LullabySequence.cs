using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Sequence for the Lullaby move.
    /// </summary>
    public sealed class LullabySequence : StarSpiritMoveSequence
    {
        public LullabySequence(SpecialMoveAction specialAction) : base(specialAction)
        {
            
        }

        protected override void SequenceMainBranch()
        {
            switch (SequenceStep)
            {
                case 0:
                    //Lullaby deals 0 damage; we need to try to deal damage to inflict Sleep
                    AttemptDamage(0, EntitiesAffected, true);

                    ChangeSequenceBranch(SequenceBranch.End);
                    break;
                default:
                    PrintInvalidSequence();
                    break;
            }
        }
    }
}
