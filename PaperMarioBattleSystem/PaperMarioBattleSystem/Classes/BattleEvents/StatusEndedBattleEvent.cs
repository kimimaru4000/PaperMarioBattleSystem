using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// A BattleEvent for Status Effects ending.
    /// </summary>
    public sealed class StatusEndedBattleEvent : BattleEvent
    {
        private static readonly Vector2 RelativeMove = new Vector2(0, -60f);
        private const double MoveTime = 300d;

        private BattleEntity Entity = null;

        private Vector2 StartPos = Vector2.Zero;
        private Vector2 EndPos = Vector2.Zero;

        private double ElapsedTime = 0d;
        private bool MovedUp = false;

        public StatusEndedBattleEvent(BattleEntity entity)
        {
            Entity = entity;
        }

        protected override void OnStart()
        {
            if (Entity != null)
            {
                StartPos = Entity.Position;
                EndPos = StartPos + RelativeMove;

                Entity.AnimManager.PlayAnimation(Entity.GetIdleAnim());
            }

            ElapsedTime = 0d;
            MovedUp = false;
        }

        protected override void OnEnd()
        {
            if (Entity != null)
            {
                Entity.Position = StartPos;
            }

            Entity = null;

            ElapsedTime = 0d;
            MovedUp = false;
        }

        protected override void OnUpdate()
        {
            if (Entity == null || Entity.IsInBattle == false)
            {
                Debug.LogWarning("BattleEntity is null or not in battle. Ending this Battle Event immediately.");
                End();
                return;
            }

            ElapsedTime += Time.ElapsedMilliseconds;

            Vector2 start = StartPos;
            Vector2 end = EndPos;

            Interpolation.InterpolationTypes interpType = Interpolation.InterpolationTypes.QuadOut;

            if (MovedUp == true)
            {
                start = EndPos;
                end = StartPos;
                interpType = Interpolation.InterpolationTypes.QuadIn;
            }

            Entity.Position = Interpolation.Interpolate(start, end, ElapsedTime / MoveTime, interpType);

            if (ElapsedTime >= MoveTime)
            {
                if (MovedUp == false)
                {
                    MovedUp = true;
                    ElapsedTime = 0d;
                }
                else
                {
                    End();
                }
            }
        }
    }
}
