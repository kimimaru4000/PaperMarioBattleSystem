using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// A Battle Event for moving BattleEntities to desired positions in a certain amount of time.
    /// This adds after-images to the BattleEntities during the movement.
    /// </summary>
    public sealed class MoveWithAfterImagesBattleEvent : MoveToBattleEvent
    {
        /// <summary>
        /// The data used for creating the after-images.
        /// </summary>
        private AfterImageVFX AfterImageData = null;

        public MoveWithAfterImagesBattleEvent(BattleEntity[] entities, Vector2[] finalPositions, double duration, AfterImageVFX afterImageData,
            Interpolation.InterpolationTypes xInterpolation = Interpolation.InterpolationTypes.Linear,
            Interpolation.InterpolationTypes yInterpolation = Interpolation.InterpolationTypes.Linear)
            : base(entities, finalPositions, duration, xInterpolation, yInterpolation)
        {
            AfterImageData = afterImageData;
        }

        public MoveWithAfterImagesBattleEvent(BattleEntity entity, Vector2 finalPosition, double duration, AfterImageVFX afterImageData,
            Interpolation.InterpolationTypes xInterpolation = Interpolation.InterpolationTypes.Linear,
            Interpolation.InterpolationTypes yInterpolation = Interpolation.InterpolationTypes.Linear)
            : base(entity, finalPosition, duration, xInterpolation, yInterpolation)
        {
            AfterImageData = afterImageData;
        }

        protected override void OnStart()
        {
            base.OnStart();

            if (AfterImageData == null)
            {
                Debug.LogWarning($"{nameof(AfterImageData)} is null, so no after-images will be added");
                return;
            }

            if (Entities != null)
            {
                double duration = Duration;

                //Set duration to 0 if it's less so the after-images go away
                if (duration < 0)
                {
                    duration = 0;
                }

                for (int i = 0; i < Entities.Length; i++)
                {
                    //Add after-images for a duration
                    BattleObjManager.Instance.AddBattleObject(new AfterImageVFX(Entities[i], AfterImageData.MaxAfterImages,
                        AfterImageData.FramesBehind, AfterImageData.AlphaValue, AfterImageData.AlphaSetting, AfterImageData.AnimSetting, duration));
                }
            }

            //Clear the data since we don't want to hold onto it
            AfterImageData.Entity = null;
            AfterImageData = null;
        }
    }
}
