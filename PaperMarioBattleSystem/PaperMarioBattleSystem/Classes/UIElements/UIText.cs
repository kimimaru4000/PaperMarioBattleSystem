using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PaperMarioBattleSystem
{
    public class UIText : PosUIElement, ITintable
    {
        /// <summary>
        /// The text to display.
        /// </summary>
        public string Text = string.Empty;

        public float Depth = 0f;
        public Vector2 Origin = Vector2.Zero;

        public Color TintColor { get; set; } = Color.White;

        protected UIText()
        {

        }

        public UIText(string text)
        {
            Text = text;
        }

        public UIText(string text, Color textColor) : this(text)
        {
            TintColor = textColor;
        }

        public override void Update()
        {
            
        }

        public override void Draw()
        {
            SpriteRenderer.Instance.DrawUIText(AssetManager.Instance.TTYDFont, Text, Position, TintColor, 0f, Origin, 1f, Depth);
        }
    }
}
