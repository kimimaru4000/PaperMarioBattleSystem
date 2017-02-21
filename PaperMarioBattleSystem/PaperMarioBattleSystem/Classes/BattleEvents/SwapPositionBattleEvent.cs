using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// A BattleEvent that swaps the position of two BattleEntities over time.
    /// This is used when swapping Mario and his Partner's turn.
    /// </summary>
    public sealed class SwapPositionBattleEvent : BattleEvent
    {
        /// <summary>
        /// The first BattleEntity to swap positions with.
        /// </summary>
        private BattleEntity FirstEntity = null;

        /// <summary>
        /// The second BattleEntity to swap positions with.
        /// </summary>
        private BattleEntity SecondEntity = null;

        /// <summary>
        /// The start position of the first BattleEntity.
        /// </summary>
        private Vector2 FirstStartPos = Vector2.Zero;

        /// <summary>
        /// The end position of the first BattleEntity.
        /// </summary>
        private Vector2 FirstEndPos = Vector2.Zero;

        /// <summary>
        /// The start position of the second BattleEntity.
        /// </summary>
        private Vector2 SecondStartPos = Vector2.Zero;

        /// <summary>
        /// The end position of the second BattleEntity.
        /// </summary>
        private Vector2 SecondEndPos = Vector2.Zero;

        /// <summary>
        /// The duration of the swap, in milliseconds.
        /// </summary>
        private float Duration = 500f;

        /// <summary>
        /// The elapsed time
        /// </summary>
        private float ElapsedTime = 0f;

        public SwapPositionBattleEvent(BattleEntity firstEntity, BattleEntity secondEntity, Vector2 firstEndPos, Vector2 secondEndPos, float duration)
        {
            FirstEntity = firstEntity;
            SecondEntity = secondEntity;

            FirstEndPos = firstEndPos;
            SecondEndPos = secondEndPos;

            Duration = duration;
        }

        protected override void OnStart()
        {
            BattleUIManager.Instance.SuppressMenus();

            FirstStartPos = FirstEntity.Position;
            SecondStartPos = SecondEntity.Position;

            //Play running animations
            if (FirstEntity.IsDead == false)
                FirstEntity.PlayAnimation(AnimationGlobals.RunningName);
            if (SecondEntity.IsDead == false)
                SecondEntity.PlayAnimation(AnimationGlobals.RunningName);

            ElapsedTime = 0f;
        }

        protected override void OnEnd()
        {
            BattleUIManager.Instance.UnsuppressMenus();

            FirstEntity.Position = FirstEndPos;
            SecondEntity.Position = SecondEndPos;

            //Play idle animations again
            //NOTE: Get the correct idle (Ex. Danger/Peril and various Status Effects cause entities to have different idle animations)
            if (FirstEntity.IsDead == false)
                FirstEntity.PlayAnimation(FirstEntity.GetIdleAnim());
            if (SecondEntity.IsDead == false)
                SecondEntity.PlayAnimation(SecondEntity.GetIdleAnim());

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

            //Lerp to get the positions and scale by the total duration
            FirstEntity.Position = Vector2.Lerp(FirstStartPos, FirstEndPos, ElapsedTime / Duration);
            SecondEntity.Position = Vector2.Lerp(SecondStartPos, SecondEndPos, ElapsedTime / Duration);

            //End after the designated amount of time has passed
            if (ElapsedTime >= Duration)
            {
                End();
            }
        }
    }
}
