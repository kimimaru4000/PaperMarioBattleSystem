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
    /// A UIElement that holds a <see cref="CroppedTexture2D"/>, which is drawn in 4 pieces.
    /// <para>This is for graphics that constitute 1/4 of a whole.
    /// This class assumes the graphic is the upper-left piece and rotates the other pieces according to draw them combined (Ex. top-left of "+" sign).</para>
    /// <para>The result is the four pieces have their pivots in the center, allowing them to be rotated as one.</para>
    /// </summary>
    public sealed class UIFourPiecedTex : UICroppedTexture2D
    {
        /// <summary>
        /// The position offset. The pieces have their pivots in the center, thus affecting how they're drawn.
        /// This allows for adjustment that works well with container UIElements like a <see cref="UIGrid"/>.
        /// </summary>
        public Vector2 PositionOffset = Vector2.Zero;

        public UIFourPiecedTex(CroppedTexture2D croppedtex2D): base(croppedtex2D)
        {

        }

        public UIFourPiecedTex(CroppedTexture2D croppedtex2D, Vector2 positionOffset) : this(croppedtex2D)
        {
            PositionOffset = positionOffset;
        }

        public UIFourPiecedTex(CroppedTexture2D croppedtex2D, float depth, Color tintColor) : base(croppedtex2D, depth, tintColor)
        {
        }

        public UIFourPiecedTex(CroppedTexture2D croppedtex2D, Vector2 positionOffset, float depth, Color tintColor) : base(croppedtex2D, depth, tintColor)
        {
            PositionOffset = positionOffset;
        }

        public override void Draw()
        {
            DrawPieces();
        }

        private void DrawPieces()
        {
            Vector2 origin = CroppedTex2D.WidthHeightToVector2();
            Texture2D tex = CroppedTex2D.Tex;
            Rectangle? sourcerect = CroppedTex2D.SourceRect;

            Vector2 position = Position + PositionOffset;

            SpriteRenderer.Instance.Draw(tex, position, sourcerect, TintColor, Rotation, origin, 1f, false, false, Depth, true);
            SpriteRenderer.Instance.Draw(tex, position, sourcerect, TintColor, Rotation, new Vector2(-origin.X, origin.Y), 1f, true, false, Depth, true);
            SpriteRenderer.Instance.Draw(tex, position, sourcerect, TintColor, Rotation, new Vector2(origin.X, 0), 1f, false, true, Depth, true);
            SpriteRenderer.Instance.Draw(tex, position, sourcerect, TintColor, Rotation, -origin, 1f, true, true, Depth, true);
        }
    }
}
