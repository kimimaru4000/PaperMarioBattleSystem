using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// A SequenceAction that moves a BattleEntity to a desired location over the Duration
    /// </summary>
    public class Move : SequenceAction
    {
        protected Vector2 Destination = Vector2.Zero;
        protected Vector2 Direction = Vector2.Zero;
        protected Vector2 MoveTotal = Vector2.Zero;

        //TEMPORARY UNTIL WE GET THE BATTLE ENTITY REFERENCE
        protected Vector2 CurPosition = Vector2.Zero;

        protected Vector2 MoveAmount
        {
            get
            {
                float elapsedTime = (float)Time.ElapsedMilliseconds;
                return new Vector2(MoveTotal.X * elapsedTime, MoveTotal.Y * elapsedTime);
            }
        }

        public Move(Vector2 destination, double duration) : base(duration)
        {

        }

        protected override void OnStart()
        {
            base.OnStart();
            Direction = CurPosition - Destination;
            MoveTotal.X = Math.Abs(Direction.X) / (float)Duration;
            MoveTotal.Y = Math.Abs(Direction.Y) / (float)Duration;
        }

        protected override void OnEnd()
        {
            base.OnEnd();
            CurPosition = Destination;
        }

        protected override void OnUpdate()
        {
            CurPosition += Direction * MoveAmount;

            //Check if the X and Y are at or beyond the destination
            if (((Direction.X < 0f && CurPosition.X <= Destination.X) || CurPosition.X >= Destination.X)
                && ((Direction.Y < 0f && CurPosition.Y <= Destination.Y) || CurPosition.Y >= Destination.Y))
            {
                End();
            }
        }
    }
}
