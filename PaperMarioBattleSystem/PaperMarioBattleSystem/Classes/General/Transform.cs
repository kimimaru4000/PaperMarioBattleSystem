using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// A Transform. It has a position, rotation, and scale.
    /// </summary>
    public class Transform
    {
        public Vector2 Position = Vector2.Zero;
        public float Rotation = 0f;
        public Vector2 Scale = Vector2.Zero;

        public Transform()
        {

        }

        public Transform(Vector2 position, float rotation, Vector2 scale)
        {
            SetPosition(position);
            SetRotation(rotation);
            SetScale(scale);
        }

        public void SetPosition(Vector2 position)
        {
            Position = position;
        }

        public void SetRotation(float rotation)
        {
            Rotation = rotation;
        }

        public void SetScale(Vector2 scale)
        {
            Scale = scale;
        }
    }
}
