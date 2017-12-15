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
    public class MoveToSeqAction : SequenceAction
    {
        /// <summary>
        /// The start position of the BattleEntity.
        /// </summary>
        protected Vector2 MoveStart = Vector2.Zero;

        /// <summary>
        /// The end position of the BattleEntity.
        /// </summary>
        protected Vector2 MoveEnd = Vector2.Zero;

        /// <summary>
        /// The elapsed time.
        /// </summary>
        protected float ElapsedTime = 0f;

        protected Interpolation.InterpolationTypes XInterpolation = Interpolation.InterpolationTypes.Linear;

        protected Interpolation.InterpolationTypes YInterpolation = Interpolation.InterpolationTypes.Linear;

        public MoveToSeqAction(Vector2 destination, double duration,
            Interpolation.InterpolationTypes xInterpolation = Interpolation.InterpolationTypes.Linear,
            Interpolation.InterpolationTypes yInterpolation = Interpolation.InterpolationTypes.Linear) : base(duration)
        {
            MoveEnd = destination;

            XInterpolation = xInterpolation;
            YInterpolation = yInterpolation;
        }

        public MoveToSeqAction(BattleEntity entity, Vector2 destination, double duration,
            Interpolation.InterpolationTypes xInterpolation = Interpolation.InterpolationTypes.Linear,
            Interpolation.InterpolationTypes yInterpolation = Interpolation.InterpolationTypes.Linear) : base(entity, duration)
        {
            MoveEnd = destination;

            XInterpolation = xInterpolation;
            YInterpolation = yInterpolation;
        }

        protected MoveToSeqAction(double duration) : base(duration)
        {
            
        }

        protected override void OnStart()
        {
            MoveStart = Entity.Position;

            ElapsedTime = 0f;
        }

        protected override void OnEnd()
        {
            Entity.Position = MoveEnd;

            ElapsedTime = 0f;
        }

        protected override void OnUpdate()
        {
            //End immediately if the duration is 0 or less
            if (Duration <= 0d)
            {
                End();
                return;
            }

            //Get current time
            ElapsedTime += (float)Time.ElapsedMilliseconds;

            //Interpolate to get the position and scale by the total duration
            Entity.Position = Interpolation.Interpolate(MoveStart, MoveEnd, ElapsedTime / (float)Duration, XInterpolation, YInterpolation);

            //End after the designated amount of time has passed
            if (ElapsedTime >= Duration)
            {
                End();
            }
        }
    }
}
