using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// A sequence of events performed during a BattleAction.
    /// </summary>
    public sealed class Sequence
    {
        public Queue<SequenceAction> Sequences = null;
        private bool StartNext = false;

        private SequenceAction CurSequence
        {
            get
            {
                if (Sequences.Count == 0) return null;
                return Sequences.Peek();
            }
        }

        public void AddSequence(SequenceAction sequenceAction)
        {
            Sequences.Enqueue(sequenceAction);
        }

        public void UpdateSequence()
        {
            if (CurSequence != null)
            {
                if (StartNext == true)
                {
                    CurSequence.Start();
                    StartNext = false;
                }

                CurSequence.Update();
                if (CurSequence.IsDone == true)
                {
                    Sequences.Dequeue();
                    StartNext = true;
                }
            }
        }
    }
}
