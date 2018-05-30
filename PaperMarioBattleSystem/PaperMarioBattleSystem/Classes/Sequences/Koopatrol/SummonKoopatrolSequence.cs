using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The sequence for summoning a Koopatrol.
    /// <para>NOTE: The PM version puts the new Koopatrol in the lowest available index.
    /// The TTYD version puts the new Koopatrol directly in front of the one that summoned it; if the front isn't possible, it's put behind.</para>
    /// </summary>
    public class SummonKoopatrolSequence : Sequence
    {
        public SummonKoopatrolSequence(MoveAction moveAction) : base(moveAction)
        {

        }

        protected override void SequenceStartBranch()
        {
            switch (SequenceStep)
            {
                case 0:
                    User.AnimManager.PlayAnimation(AnimationGlobals.KoopatrolBattleAnimations.SummonKoopatrolName);

                    CurSequenceAction = new WaitForAnimationSeqAction(User, AnimationGlobals.KoopatrolBattleAnimations.SummonKoopatrolName);
                    ChangeSequenceBranch(SequenceBranch.Main);
                    break;
                default:
                    PrintInvalidSequence();
                    break;
            }
        }

        protected override void SequenceEndBranch()
        {
            switch (SequenceStep)
            {
                case 0:
                    EndSequence();
                    break;
                default:
                    PrintInvalidSequence();
                    break;
            }
        }

        protected override void SequenceMainBranch()
        {
            switch (SequenceStep)
            {
                case 0:
                    //Summon another Koopatrol
                    Koopatrol koopatrol = new Koopatrol();
                    BattleManager.Instance.AddEntities(new BattleEntity[] { koopatrol }, null, true);

                    //Set the new Koopatrol's used turns to 0, as they cannot go right after being summoned
                    koopatrol.SetTurnsUsed(koopatrol.MaxTurns);

                    ChangeSequenceBranch(SequenceBranch.End);
                    break;
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

        protected override void SequenceMissBranch()
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
