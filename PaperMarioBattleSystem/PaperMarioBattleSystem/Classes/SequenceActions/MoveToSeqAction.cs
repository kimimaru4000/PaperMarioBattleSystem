using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using PaperMarioBattleSystem.Utilities;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// A SequenceAction that moves an IPosition to a desired location over the Duration.
    /// An example would be a position on the screen, such as (350, 400).
    /// </summary>
    public class MoveToSeqAction : SequenceAction
    {
        /// <summary>
        /// An object that can be positioned.
        /// </summary>
        protected IPosition PositionableObj = null;

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

        protected MoveToSeqAction(IPosition positionableObj, double duration) : base(duration)
        {
            PositionableObj = positionableObj;
        }

        public MoveToSeqAction(IPosition positionableObj, Vector2 destination, double duration,
            Interpolation.InterpolationTypes xInterpolation = Interpolation.InterpolationTypes.Linear,
            Interpolation.InterpolationTypes yInterpolation = Interpolation.InterpolationTypes.Linear) : this(positionableObj, duration)
        {
            MoveEnd = destination;

            XInterpolation = xInterpolation;
            YInterpolation = yInterpolation;
        }

        protected override void OnStart()
        {
            MoveStart = PositionableObj.Position;

            ElapsedTime = 0f;
        }

        protected override void OnEnd()
        {
            PositionableObj.Position = MoveEnd;

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
            PositionableObj.Position = Interpolation.Interpolate(MoveStart, MoveEnd, ElapsedTime / (float)Duration, XInterpolation, YInterpolation);

            //End after the designated amount of time has passed
            if (ElapsedTime >= Duration)
            {
                End();
            }
        }
    }
}
