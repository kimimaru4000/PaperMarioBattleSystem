using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The game camera. It pans, rotates, and zooms at specified moments.
    /// <para>This is a Singleton for now</para>
    /// </summary>
    public class Camera
    {
        #region Singleton Fields

        public static Camera Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Camera();
                }

                return instance;
            }
        }

        private static Camera instance = null;

        #endregion

        /// <summary>
        /// The position of the camera. (0,0) is the center of the screen
        /// </summary>
        public Vector2 Position { get; protected set; } = Vector2.Zero;

        /// <summary>
        /// The scale of the Camera. Negative values will flip everything on-screen
        /// </summary>
        public float Scale { get; protected set; } = 1f;

        /// <summary>
        /// The rotation of the camera
        /// </summary>
        public float Rotation { get; protected set; } = 0f;

        public Matrix Transform { get; protected set; } = default(Matrix);

        private Camera()
        {
            Initialize(new Vector2(0f, 0f), 0f, 1f);
        }

        public void Initialize(Vector2 position, float rotation, float scale)
        {
            SetTranslation(position);
            SetRotation(rotation);
            SetZoom(scale);
        }

        #region Transform Manipulations

        public void SetTranslation(Vector2 translation)
        {
            Position = translation;
        }

        public void Translate(Vector2 amount)
        {
            Position += amount;
        }

        public void SetRotation(float rotation)
        {
            Rotation = rotation;
        }

        public void Rotate(float amount)
        {
            Rotation += amount;
        }

        public void SetZoom(float scale)
        {
            Scale = scale;
        }

        public void Zoom(float amount)
        {
            Scale += amount;
        }

        #endregion

        /// <summary>
        /// Calculates the Camera's Transform using Matrix multiplication
        /// <para>Obtained from: http://www.david-amador.com/2009/10/xna-camera-2d-with-zoom-and-rotation/</para>
        /// </summary>
        /// <returns>The Matrix representing the Camera's Transform</returns>
        public Matrix CalculateTransformation()
        {
            Vector2 windowSize = SpriteRenderer.Instance.GameWindowSize;
            Transform = Matrix.CreateTranslation(new Vector3(-Position.X, -Position.Y, 0f)) *
                        Matrix.CreateRotationZ(Rotation) *
                        Matrix.CreateScale(new Vector3(Scale, Scale, 0)) *
                        Matrix.CreateTranslation(windowSize.X * .5f, windowSize.Y * .5f, 0f);

            return Transform;
        }
    }
}
