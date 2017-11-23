using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// A Battle Event that shoots a Bobbery Bomb with a velocity.
    /// The bomb stops when it hits the ground.
    /// <para>This is used in Bobbery's Bomb Squad sequence.</para>
    /// </summary>
    public sealed class ShootBobberyBombBattleEvent : BattleEvent
    {
        private BobberyBomb bobberyBomb = null;

        //NOTE: Some of this is temporary until we have some sort of physics system
        private Vector2 InitVelocity = Vector2.Zero;
        private float Gravity = 0f;
        private float GroundY = 0f;

        private Vector2 CurVelocity = Vector2.Zero;

        public ShootBobberyBombBattleEvent(BobberyBomb bomb, Vector2 velocity, float gravity, float groundY)
        {
            bobberyBomb = bomb;
            InitVelocity = velocity;
            Gravity = gravity;
            GroundY = groundY;
        }

        protected override void OnStart()
        {
            base.OnStart();

            CurVelocity = InitVelocity;
        }

        protected override void OnEnd()
        {
            base.OnEnd();

            //Initialize the bomb
            bobberyBomb.InitializeBomb();

            //Reset values and clear reference
            bobberyBomb = null;
            CurVelocity = Vector2.Zero;
            InitVelocity = Vector2.Zero;
            Gravity = 0f;
            GroundY = 0f;
        }

        protected override void OnUpdate()
        {
            //If we reached the ground, put the bomb on it and end
            if (bobberyBomb.Position.Y >= GroundY)
            {
                bobberyBomb.Position = new Vector2(bobberyBomb.Position.X, GroundY);

                End();
                return;
            }

            bobberyBomb.Position += CurVelocity;

            //Add gravity to the current velocity
            CurVelocity.Y += Gravity;
        }
    }
}
