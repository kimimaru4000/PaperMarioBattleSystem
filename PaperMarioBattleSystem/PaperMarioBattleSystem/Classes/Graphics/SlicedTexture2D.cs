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
    /// Represents a Texture2D sliced into several regions.
    /// </summary>
    public abstract class SlicedTexture2D
    {
        /// <summary>
        /// The Texture2D to slice.
        /// </summary>
        public Texture2D Tex { get; protected set; }

        /// <summary>
        /// The Rectangle representing the region of the Texture2D to slice from.
        /// If null, the entire texture will be used.
        /// </summary>
        public Rectangle? SourceRect { get; protected set; }

        /// <summary>
        /// The sliced regions of the Texture2D.
        /// </summary>
        public Rectangle[] Regions { get; protected set; }

        /// <summary>
        /// The number of regions the Texture2D is sliced into.
        /// </summary>
        public abstract int Slices { get; }

        protected SlicedTexture2D(Texture2D tex, Rectangle? sourceRect)
        {
            SetTexture(tex);
            SetSourceRect(sourceRect);
        }

        public void SetTexture(Texture2D texture)
        {
            Tex = texture;
        }

        public void SetSourceRect(Rectangle? sourceRect)
        {
            SourceRect = sourceRect;
        }

        /// <summary>
        /// Retrieves a Rectangle corresponding to the region of the sliced texture based on the index.
        /// </summary>
        /// <param name="rectangle">The Rectangle containing the position and scale of the texture.</param>
        /// <param name="index">The index of the rectangle to get the slices from. This goes from 0 to (Slices - 1).</param>
        /// <returns>A Rectangle corresponding to the region of the sliced texture.</returns>
        public abstract Rectangle GetRectForIndex(Rectangle rectangle, int index);

        /// <summary>
        /// Retrieves a set of Rectangles corresponding to the regions of the sliced texture.
        /// </summary>
        /// <param name="rectangle">The Rectangle containing the position and scale of the texture.</param>
        /// <returns>An array of Rectangles associated with the regions of the sliced texture.</returns>
        public Rectangle[] CreateRegions(Rectangle rectangle)
        {
            List<Rectangle> regions = new List<Rectangle>();

            for (int i = 0; i < Slices; i++)
            {
                regions.Add(GetRectForIndex(rectangle, i));
            }
            
            return regions.ToArray();
        }
    }
}
