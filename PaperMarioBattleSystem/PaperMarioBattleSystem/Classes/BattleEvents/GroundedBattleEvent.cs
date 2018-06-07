using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// Handles moving a winged BattleEntity down.
    /// This moves it down and sets its BattlePosition to the new position, also setting its HeightState.
    /// </summary>
    public sealed class GroundedBattleEvent : BattleEvent
    {
        private BattleEntity Entity = null;

        private Vector2 StartPos = Vector2.Zero;
        private Vector2 GroundedPos = Vector2.Zero;

        private float ElapsedTime = 0f;
        private float MoveDuration = 500f;

        //NOTE: See if we can get this to work with the DamagedBattleEvent
        private IAnimation HurtAnim = null;

        public GroundedBattleEvent(BattleEntity entity, Vector2 groundedPos)
        {
            Entity = entity;
            GroundedPos = groundedPos;
        }

        protected override void OnStart()
        {
            base.OnStart();

            HurtAnim = Entity.AnimManager.GetAnimation<IAnimation>(AnimationGlobals.HurtName);
            Entity.AnimManager.PlayAnimation(AnimationGlobals.HurtName);
            
            StartPos = Entity.BattlePosition;

            ElapsedTime = 0f;
        }

        protected override void OnEnd()
        {
            base.OnEnd();

            Entity.SetBattlePosition(GroundedPos);
            Entity.Position = GroundedPos;

            //Change HeightState
            Entity.ChangeHeightState(Enumerations.HeightStates.Grounded);

            if (Entity.IsDead == false)
                Entity.AnimManager.PlayAnimation(Entity.GetIdleAnim());

            ElapsedTime = 0f;
            StartPos = GroundedPos = Vector2.Zero;
            HurtAnim = null;
            Entity = null;
        }

        protected override void OnUpdate()
        {
            if (MoveDuration <= 0d)
            {
                End();
                return;
            }

            if (HurtAnim != null && HurtAnim.Finished == true)
            {
                HurtAnim.Play();
            }

            //Return if the entity is targeted
            if (Entity.IsTargeted == true)
                return;

            //Get current time
            ElapsedTime += (float)Time.ElapsedMilliseconds;

            //Lerp to get the position and scale by the duration
            Entity.Position = Vector2.Lerp(StartPos, GroundedPos, ElapsedTime / MoveDuration);

            //End after the designated amount of time has passed
            if (ElapsedTime >= MoveDuration)
            {
                End();
            }
        }
    }
}
