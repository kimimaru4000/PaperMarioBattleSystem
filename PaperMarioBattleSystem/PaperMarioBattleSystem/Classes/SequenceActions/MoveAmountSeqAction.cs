using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// A SequenceAction that moves a BattleEntity a relative amount over the Duration.
    /// An example would be relative to the entity's current position, such as (50, -12)
    /// </summary>
    public sealed class MoveAmountSeqAction : MoveToSeqAction
    {
        private Vector2 AmountMoved = Vector2.Zero;
        private Vector2 OrigPos = Vector2.Zero;

        public MoveAmountSeqAction(Vector2 amount, double duration) : base(duration)
        {
            MoveDest = amount;
        }

        protected override void OnStart()
        {
            OrigPos = Entity.Position;

            //Set direction directly
            MoveDir.X = MoveDest.X < 0f ? -1f : MoveDest.X > 0f ? 1f : 0f;
            MoveDir.Y = MoveDest.Y < 0f ? -1f : MoveDest.Y > 0f ? 1f : 0f;

            MoveTotal.X = Math.Abs(MoveDest.X) / (float)Duration;
            MoveTotal.Y = Math.Abs(MoveDest.Y) / (float)Duration;
        }

        protected override void OnEnd()
        {
            AmountMoved = MoveDest;
            Entity.Position = OrigPos + AmountMoved;
        }

        protected override void OnUpdate()
        {
            AmountMoved += MoveDir * MoveAmt;
            Entity.Position = OrigPos + AmountMoved;

            //Check if the amount moved in the X and Y are at or beyond the amount that should've been moved
            if (((MoveDir.X < 0f && AmountMoved.X <= MoveDest.X) || (MoveDir.X >= 0f && AmountMoved.X >= MoveDest.X))
                && ((MoveDir.Y < 0f && AmountMoved.Y <= MoveDest.Y) || (MoveDir.Y >= 0f && AmountMoved.Y >= MoveDest.Y)))
            {
                End();
            }
        }
    }
}
