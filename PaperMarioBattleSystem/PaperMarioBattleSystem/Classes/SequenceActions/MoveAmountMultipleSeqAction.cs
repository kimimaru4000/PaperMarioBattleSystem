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
    /// A SequenceAction that moves multiple BattleEntities a certain amount over time.
    /// </summary>
    public sealed class MoveAmountMultipleSeqAction : SequenceAction
    {
        /// <summary>
        /// The start positions of the BattleEntities.
        /// </summary>
        private Vector2[] MoveStarts = null;

        /// <summary>
        /// The end position of the BattleEntity.
        /// </summary>
        private Vector2[] MoveEnds = null;

        /// <summary>
        /// The elapsed time.
        /// </summary>
        private float ElapsedTime = 0f;

        private Interpolation.InterpolationTypes XInterpolation = Interpolation.InterpolationTypes.Linear;

        private Interpolation.InterpolationTypes YInterpolation = Interpolation.InterpolationTypes.Linear;

        private BattleEntity[] EntitiesToMove = null;
        private Vector2[] MoveAmounts = null;

        public MoveAmountMultipleSeqAction(BattleEntity[] entitiesToMove, Vector2[] amounts, double duration,
            Interpolation.InterpolationTypes xInterpolation = Interpolation.InterpolationTypes.Linear,
            Interpolation.InterpolationTypes yInterpolation = Interpolation.InterpolationTypes.Linear)
        {
            EntitiesToMove = entitiesToMove;
            MoveAmounts = amounts;

            Duration = duration;

            XInterpolation = xInterpolation;
            YInterpolation = yInterpolation;
        }

        protected override void OnStart()
        {
            base.OnStart();

            MoveStarts = new Vector2[EntitiesToMove.Length];
            MoveEnds = new Vector2[EntitiesToMove.Length];

            for (int i = 0; i < EntitiesToMove.Length; i++)
            {
                MoveStarts[i] = EntitiesToMove[i].Position;
                MoveEnds[i] = MoveStarts[i] + MoveAmounts[i];
            }

            ElapsedTime = 0f;
        }

        protected override void OnEnd()
        {
            for (int i = 0; i < EntitiesToMove.Length; i++)
            {
                EntitiesToMove[i].Position = MoveEnds[i];
            }

            ElapsedTime = 0f;

            EntitiesToMove = null;
            MoveAmounts = null;
            MoveStarts = null;
            MoveEnds = null;
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
            for (int i = 0; i < EntitiesToMove.Length; i++)
            {
                EntitiesToMove[i].Position = Interpolation.Interpolate(MoveStarts[i], MoveEnds[i], ElapsedTime / (float)Duration, XInterpolation, YInterpolation);
            }

            //End after the designated amount of time has passed
            if (ElapsedTime >= Duration)
            {
                End();
            }
        }
    }
}
