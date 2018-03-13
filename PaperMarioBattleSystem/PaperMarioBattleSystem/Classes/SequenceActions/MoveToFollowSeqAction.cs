using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// A SequenceAction that moves a BattleEntity to a desired location over a duration, with another BattleEntity following it at an offset.
    /// </summary>
    public sealed class MoveToFollowSeqAction : MoveToSeqAction
    {
        private BattleEntity EntityFollowing = null;
        private Vector2 FollowOffset = Vector2.Zero;

        public MoveToFollowSeqAction(BattleEntity entityFollowing, Vector2 destination, double duration, Vector2 followOffset,
            Interpolation.InterpolationTypes xInterpolation = Interpolation.InterpolationTypes.Linear,
            Interpolation.InterpolationTypes yInterpolation = Interpolation.InterpolationTypes.Linear)
            : base(destination, duration, xInterpolation, yInterpolation)
        {
            EntityFollowing = entityFollowing;
            FollowOffset = followOffset;
        }

        protected override void OnEnd()
        {
            base.OnEnd();

            EntityFollowing.Position = MoveEnd + FollowOffset;
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();

            if (IsDone == false)
            {
                EntityFollowing.Position = Entity.Position + FollowOffset;
            }
        }
    }
}
