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
    /// A 9-sliced Texture2D. It produces a set of source rectangles that slice the Texture2D into 9 slices, which can be used for rendering.
    /// </summary>
    public class NineSlicedTexture2D : ICopyable<NineSlicedTexture2D>
    {
        /// <summary>
        /// The Texture2D to slice.
        /// </summary>
        public Texture2D Tex { get; private set; }

        /// <summary>
        /// The Rectangle representing the region of the Texture2D to slice from.
        /// If null, the entire texture will be used.
        /// </summary>
        public Rectangle? SourceRect { get; private set; }

        public int LeftLine { get; private set; }
        public int RightLine { get; private set; }
        public int TopLine { get; private set; }
        public int BottomLine { get; private set; }

        public Rectangle[] Regions { get; private set; }

        public NineSlicedTexture2D(Texture2D tex, Rectangle? sourceRect, int leftCutoff, int rightCutoff, int topCutoff, int bottomCutoff)
        {
            SetTexture(tex);
            SetSourceRect(sourceRect);
            SetCutoffRegions(leftCutoff, rightCutoff, topCutoff, bottomCutoff);
        }

        public void SetTexture(Texture2D texture)
        {
            Tex = texture;
        }

        public void SetSourceRect(Rectangle? sourceRect)
        {
            SourceRect = sourceRect;
        }

        public void SetCutoffRegions(int leftCutoff, int rightCutoff, int topCutoff, int bottomCutoff)
        {
            LeftLine = leftCutoff;
            RightLine = rightCutoff;
            TopLine = topCutoff;
            BottomLine = bottomCutoff;

            Regions = CreateRegions(SourceRect.HasValue ? SourceRect.Value : Tex.Bounds);
        }

        /// <summary>
        /// Retrieves a Rectangle corresponding to the region of the 9-sliced texture based on the index.
        /// </summary>
        /// <param name="rectangle">The Rectangle containing the position and scale of the texture.</param>
        /// <param name="index">The index of the rectangle to get the slices from. This goes from 0 to 8 for 9 slices.</param>
        /// <returns>A Rectangle corresponding to the region of the 9-sliced texture.</returns>
        public Rectangle GetRectForIndex(Rectangle rectangle, int index)
        {
            int x = rectangle.X;
            int y = rectangle.Y;
            int width = rectangle.Width;
            int height = rectangle.Height;
            int middleWidth = width - LeftLine - RightLine;
            int middleHeight = height - TopLine - BottomLine;
            int bottomY = y + height - BottomLine;
            int rightX = x + width - RightLine;
            int leftX = x + LeftLine;
            int topY = y + TopLine;

            switch (index)
            {
                //Upper-region
                case 0: return new Rectangle(x, y, LeftLine, TopLine);
                case 1: return new Rectangle(leftX, y, middleWidth, TopLine);
                case 2: return new Rectangle(rightX, y, RightLine, TopLine);
                
                //Middle-region
                case 3: return new Rectangle(x, topY, LeftLine, middleHeight);
                case 4: return new Rectangle(leftX, topY, middleWidth, middleHeight);
                case 5: return new Rectangle(rightX, topY, RightLine, middleHeight);
                
                //Lower-region
                case 6: return new Rectangle(x, bottomY, LeftLine, BottomLine);
                case 7: return new Rectangle(leftX, bottomY, middleWidth, BottomLine);
                case 8: return new Rectangle(rightX, bottomY, RightLine, BottomLine);
               
                default: return rectangle;
            }
        }

        /// <summary>
        /// Retrieves a set of Rectangles corresponding to the regions of the 9-sliced texture.
        /// </summary>
        /// <param name="rectangle">The Rectangle containing the position and scale of the texture.</param>
        /// <returns>An array of Rectangles associated with the regions of the 9-sliced texture.</returns>
        public Rectangle[] CreateRegions(Rectangle rectangle)
        {
            return new Rectangle[]
            {
                GetRectForIndex(rectangle, 0),
                GetRectForIndex(rectangle, 1),
                GetRectForIndex(rectangle, 2),

                GetRectForIndex(rectangle, 3),
                GetRectForIndex(rectangle, 4),
                GetRectForIndex(rectangle, 5),

                GetRectForIndex(rectangle, 6),
                GetRectForIndex(rectangle, 7),
                GetRectForIndex(rectangle, 8)
            };
        }

        public NineSlicedTexture2D Copy()
        {
            return new NineSlicedTexture2D(Tex, SourceRect, LeftLine, RightLine, TopLine, BottomLine);
        }
    }
}
