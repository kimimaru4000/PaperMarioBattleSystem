using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// A SequenceAction that makes one BattleEntity follow another with an offset for a specified amount of time.
    /// </summary>
    public sealed class FollowSeqAction : WaitSeqAction
    {
        private BattleEntity EntityToFollow = null;
        private Vector2 FollowOffset = Vector2.Zero;

        public FollowSeqAction(BattleEntity entityFollowing, BattleEntity entityToFollow, double duration, Vector2 followOffset)
            : base(duration)
        {
            Entity = entityFollowing;
            EntityToFollow = entityToFollow;
            FollowOffset = followOffset;
        }

        protected override void OnUpdate()
        {
            Entity.Position = EntityToFollow.Position + FollowOffset;

            base.OnUpdate();
        }
    }
}
