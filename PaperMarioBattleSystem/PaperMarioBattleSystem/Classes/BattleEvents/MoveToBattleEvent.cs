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
    /// A Battle Event for moving BattleEntities to desired positions in a certain amount of time.
    /// </summary>
    public class MoveToBattleEvent : BattleEvent
    {
        /// <summary>
        /// The positions to move the BattleEntities to, respectively.
        /// If the length is shorter than the Entities array, BattleEntities at the extra indices won't have their positions updated.
        /// </summary>
        protected Vector2[] FinalPositions = null;
        protected double Duration = 0d;

        protected Vector2[] StartingPositions = null;

        protected Interpolation.InterpolationTypes XInterpolation = Interpolation.InterpolationTypes.Linear;
        protected Interpolation.InterpolationTypes YInterpolation = Interpolation.InterpolationTypes.Linear;

        private double ElapsedTime = 0d;

        public MoveToBattleEvent(BattleEntity[] entities, Vector2[] finalPositions, double duration,
            Interpolation.InterpolationTypes xInterpolation = Interpolation.InterpolationTypes.Linear,
            Interpolation.InterpolationTypes yInterpolation = Interpolation.InterpolationTypes.Linear)
        {
            Entities = entities;
            FinalPositions = finalPositions;
            Duration = duration;

            XInterpolation = xInterpolation;
            YInterpolation = yInterpolation;
        }

        /// <summary>
        /// Convenience constructor for a single BattleEntity.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="finalPosition"></param>
        /// <param name="duration"></param>
        public MoveToBattleEvent(BattleEntity entity, Vector2 finalPosition, double duration,
            Interpolation.InterpolationTypes xInterpolation = Interpolation.InterpolationTypes.Linear,
            Interpolation.InterpolationTypes yInterpolation = Interpolation.InterpolationTypes.Linear)
            : this(new BattleEntity[] { entity }, new Vector2[] { finalPosition }, duration, xInterpolation, yInterpolation)
        {
        }

        protected override void OnStart()
        {
            base.OnStart();

            if (Entities != null)
            {
                //Set starting positions
                StartingPositions = new Vector2[Entities.Length];

                for (int i = 0; i < Entities.Length; i++)
                {
                    StartingPositions[i] = Entities[i].Position;
                }
            }
        }

        protected override void OnEnd()
        {
            base.OnEnd();

            //Set to final positions
            for (int i = 0; i < Entities.Length; i++)
            {
                if (i >= FinalPositions.Length)
                    break;

                Entities[i].Position = FinalPositions[i];
            }
        }

        protected override void OnUpdate()
        {
            //End and return if either array is null
            if (Entities == null || FinalPositions == null)
            {
                Debug.LogError($"{nameof(Entities)} and/or {nameof(FinalPositions)} arrays are null! Ending BattleEvent");
                End();
                return;
            }

            //Increment elapsed time
            ElapsedTime += Time.ElapsedMilliseconds;

            //Set positions
            for (int i = 0; i < Entities.Length; i++)
            {
                if (i >= FinalPositions.Length)
                    break;

                Entities[i].Position = Interpolation.Interpolate(StartingPositions[i], FinalPositions[i], (float)(ElapsedTime / Duration), XInterpolation, YInterpolation);
            }

            //We're finished, so end
            if (ElapsedTime >= Duration)
            {
                End();
            }
        }
    }
}
