using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// A SequenceAction that moves a BattleEntity to a desired location over the Duration.
    /// An example would be a position on the screen, such as (350, 400)
    /// </summary>
    public class MoveTo : SequenceAction
    {
        protected Vector2 MoveDest = Vector2.Zero;
        protected Vector2 MoveDir = Vector2.Zero;
        protected Vector2 MoveTotal = Vector2.Zero;

        protected Vector2 MoveAmt
        {
            get
            {
                float elapsedTime = (float)Time.ElapsedMilliseconds;
                return new Vector2(MoveTotal.X * elapsedTime, MoveTotal.Y * elapsedTime);
            }
        }

        public MoveTo(Vector2 destination, double duration) : base(duration)
        {
            MoveDest = destination;
        }

        protected MoveTo(double duration) : base(duration)
        {

        }

        protected override void OnStart()
        {
            Vector2 entityPos = Entity.Position;

            MoveDir.X = MoveDest.X < entityPos.X ? -1f : 1f;
            MoveDir.Y = MoveDest.Y < entityPos.Y ? -1f : 1f;

            Vector2 movePerFrame = entityPos - MoveDest;

            MoveTotal.X = Math.Abs(movePerFrame.X) / (float)Duration;
            MoveTotal.Y = Math.Abs(movePerFrame.Y) / (float)Duration;
        }

        protected override void OnEnd()
        {
            Entity.Position = MoveDest;
        }

        protected override void OnUpdate()
        {
            Entity.Position += MoveDir * MoveAmt;

            //Check if the X and Y are at or beyond the destination
            if (((MoveDir.X < 0f && Entity.Position.X <= MoveDest.X) || (MoveDir.X >= 0 && Entity.Position.X >= MoveDest.X))
                && ((MoveDir.Y < 0f && Entity.Position.Y <= MoveDest.Y) || (MoveDir.Y >= 0 && Entity.Position.Y >= MoveDest.Y)))
            {
                End();
            }
        }
    }
}
