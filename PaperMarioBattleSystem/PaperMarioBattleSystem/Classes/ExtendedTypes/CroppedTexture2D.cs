using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// A Texture2D with a rectangle designating the portion of the Texture2D to render.
    /// </summary>
    public sealed class CroppedTexture2D : ICopyable<CroppedTexture2D>
    {
        /// <summary>
        /// The Texture2D to render.
        /// </summary>
        public Texture2D Tex { get; private set; }

        /// <summary>
        /// The part of the Texture2D to render.
        /// </summary>
        public Rectangle? SourceRect { get; private set; }

        public CroppedTexture2D(Texture2D tex, Rectangle? sourceRect)
        {
            SetTexture(tex);
            SetRect(sourceRect);
        }

        public void SetTexture(Texture2D texture)
        {
            Tex = texture;
        }

        public void SetRect(Rectangle? sourceRect)
        {
            SourceRect = sourceRect;
        }

        /// <summary>
        /// Returns the Width and Height of SourceRect as the X and Y, respectively, in a Vector2.
        /// </summary>
        /// <returns>The Width and Height of the SourceRect as the X and Y, respectively, in a Vector2. If SourceRect is null, Vector2.Zero.</returns>
        public Vector2 WidthHeightToVector2()
        {
            Vector2 widthheight = Vector2.Zero;

            if (SourceRect.HasValue == true)
            {
                widthheight.X = SourceRect.Value.Width;
                widthheight.Y = SourceRect.Value.Height;
            }

            return widthheight;
        }

        public CroppedTexture2D Copy()
        {
            return new CroppedTexture2D(Tex, SourceRect);
        }
    }
}
