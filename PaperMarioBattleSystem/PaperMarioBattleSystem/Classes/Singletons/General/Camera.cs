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
        /// The constant for translating the camera to the center of the screen
        /// </summary>
        private const float TranslationConstant = .5f;
        
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

        #region Conversions

        /// <summary>
        /// Converts a position on the Sprite layer to its respective position on the UI layer
        /// </summary>
        /// <param name="position">The position on the Sprite layer</param>
        /// <returns>The position on the UI layer corresponding to the position on the Sprite layer</returns>
        public Vector2 SpriteToUIPos(Vector2 position)
        {
            Vector2 convert = new Vector2(RenderingGlobals.BaseResolutionWidth, RenderingGlobals.BaseResolutionHeight) * TranslationConstant;
            convert.X = (int)convert.X;
            convert.Y = (int)convert.Y;
            return position + convert;
        }

        /// <summary>
        /// Converts a position on the UI layer to its respective position on the Sprite layer
        /// </summary>
        /// <param name="position">The position on the UI layer</param>
        /// <returns>The position on the Sprite layer corresponding to the position on the UI layer</returns>
        public Vector2 UIToSpritePos(Vector2 position)
        {
            Vector2 convert = new Vector2(RenderingGlobals.BaseResolutionWidth, RenderingGlobals.BaseResolutionHeight) * -TranslationConstant;
            convert.X = (int)convert.X;
            convert.Y = (int)convert.Y;
            return position + convert;
        }

        #endregion

        /// <summary>
        /// Calculates the Camera's Transform using Matrix multiplication
        /// <para>Obtained from: http://www.david-amador.com/2009/10/xna-camera-2d-with-zoom-and-rotation/</para>
        /// </summary>
        /// <returns>The Matrix representing the Camera's Transform</returns>
        public Matrix CalculateTransformation()
        {
            Transform = Matrix.CreateTranslation(new Vector3(-Position.X, -Position.Y, 0f)) *
                        Matrix.CreateRotationZ(Rotation) *
                        Matrix.CreateScale(new Vector3(Scale, Scale, 1)) *
                        Matrix.CreateTranslation(RenderingGlobals.BaseResolutionWidth * TranslationConstant, RenderingGlobals.BaseResolutionHeight * TranslationConstant, 0f);

            return Transform;
        }
    }
}
