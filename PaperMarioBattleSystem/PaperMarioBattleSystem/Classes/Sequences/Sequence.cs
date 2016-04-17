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
        public Queue<SequenceAction> Sequences = new Queue<SequenceAction>();
        private bool StartNext = true;

        public BattleEntity Entity => BattleManager.Instance.EntityTurn;

        private SequenceAction CurSequence
        {
            get
            {
                if (Sequences.Count == 0) return null;
                return Sequences.Peek();
            }
        }

        public void AddSequence(params SequenceAction[] sequenceActions)
        {
            for (int i = 0; i < sequenceActions.Length; i++)
            {
                Sequences.Enqueue(sequenceActions[i]);
            }
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
