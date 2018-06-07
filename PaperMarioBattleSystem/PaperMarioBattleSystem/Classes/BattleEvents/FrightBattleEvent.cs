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
    /// A BattleEvent for when a BattleEntity is frightened via Spook, a Fright Jar, or a Fright Mask.
    /// The BattleEntity is removed from battle.
    /// </summary>
    public sealed class FrightBattleEvent : BattleEvent
    {
        private enum FrightStates
        {
            HoppingUp, HoppingDown, Running
        }

        private const double HopTime = 200d;

        private BattleEntity Entity = null;
        private Vector2 StartPos = Vector2.Zero;
        private Vector2 RunSpeed = Vector2.One;
        private double Duration = 0d;

        private Vector2 HopEndPos = Vector2.Zero;

        private double ElapsedTime = 0d;

        private FrightStates FrightState = FrightStates.HoppingUp;

        public FrightBattleEvent(BattleEntity entity, Vector2 runSpeed, double duration)
        {
            Entity = entity;
            RunSpeed = runSpeed;
            Duration = duration;
        }

        protected override void OnStart()
        {
            base.OnStart();

            StartPos = Entity.Position;
            HopEndPos = StartPos + new Vector2(0f, -50f);

            Entity?.AnimManager.PlayAnimation(AnimationGlobals.IdleName);
        }

        protected override void OnEnd()
        {
            base.OnEnd();

            //Remove the BattleEntity first
            Entity.BManager.RemoveEntity(Entity, true);
            Entity.Die();
        }

        protected override void OnUpdate()
        {
            //End and return if the BattleEntity is null
            if (Entity == null)
            {
                Debug.LogError($"{nameof(Entity)} is null! Ending BattleEvent");
                End();
                return;
            }
            
            ElapsedTime += Time.ElapsedMilliseconds;

            //First, perform a very quick hop up to show that the BattleEntity is frightened
            if (FrightState == FrightStates.HoppingUp)
            {
                Entity.Position = Interpolation.Interpolate(StartPos, HopEndPos, ElapsedTime / HopTime, Interpolation.InterpolationTypes.CubicIn);

                if (ElapsedTime >= HopTime)
                {
                    ElapsedTime = 0d;
                    FrightState = FrightStates.HoppingDown;
                }
            }
            //Go down from the hop
            else if (FrightState == FrightStates.HoppingDown)
            {
                Entity.Position = Interpolation.Interpolate(HopEndPos, StartPos, ElapsedTime / HopTime, Interpolation.InterpolationTypes.CubicOut);

                if (ElapsedTime >= HopTime)
                {
                    ElapsedTime = 0d;
                    FrightState = FrightStates.Running;

                    //Play the running animation
                    Entity.AnimManager.PlayAnimation(AnimationGlobals.RunningName, true);

                    //Speed up the animation to make it look like the BattleEntity is running in panic
                    Animation anim = Entity.AnimManager.GetAnimation<Animation>(AnimationGlobals.RunningName);
                    anim?.SetSpeed(3f);

                    Entity.SpriteFlip = true;
                }
            }
            //Run away
            else
            {
                Entity.Position += RunSpeed;

                if (ElapsedTime >= Duration)
                {
                    End();
                }
            }
        }
    }
}
