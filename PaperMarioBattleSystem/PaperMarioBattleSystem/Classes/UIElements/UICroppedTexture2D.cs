using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// A UIElement that holds a <see cref="CroppedTexture2D"/>.
    /// </summary>
    public class UICroppedTexture2D : PosUIElement, IRotatable, IScalable, ITintable
    {
        public CroppedTexture2D CroppedTex2D = null;
        public bool FlipX = false;
        public bool FlipY = false;
        public float Rotation { get; set; } = 0f;
        public float Depth = 0f;
        public Vector2 Origin = Vector2.Zero;
        public Vector2 Scale { get; set; } = Vector2.One;
        public Color TintColor { get; set; } = Color.White;

        protected UICroppedTexture2D()
        {

        }

        public UICroppedTexture2D(CroppedTexture2D croppedtex2D)
        {
            CroppedTex2D = croppedtex2D;
        }

        public UICroppedTexture2D(CroppedTexture2D croppedtex2D, float depth, Color tintColor) : this(croppedtex2D)
        {
            Depth = depth;
            TintColor = tintColor;
        }

        public override void Update()
        {
            
        }

        public override void Draw()
        {
            //This is a UI element, so always render it on the UI layer
            SpriteRenderer.Instance.DrawUI(CroppedTex2D.Tex, Position, CroppedTex2D.SourceRect, TintColor, Rotation, Origin, Scale, FlipX, FlipY, Depth);
        }
    }
}
