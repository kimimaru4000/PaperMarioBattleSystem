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
    /// A textbox that shows text
    /// </summary>
    public class TextBox : UIElement
    {
        /// <summary>
        /// The position of the center of the textbox
        /// </summary>
        public Vector2 Position = Vector2.Zero;

        /// <summary>
        /// The size of the textbox
        /// </summary>
        public Vector2 Size = Vector2.One;

        /// <summary>
        /// The Color of the textbox
        /// </summary>
        public Color BoxColor = Color.White;

        /// <summary>
        /// The Color of the text displayed in the textbox
        /// </summary>
        public Color TextColor = Color.Black;

        /// <summary>
        /// The text displayed in the textbox
        /// </summary>
        public string Text { get; protected set; } = string.Empty;

        /// <summary>
        /// The layer of the textbox
        /// </summary>
        public float Layer = .3f;

        /// <summary>
        /// The image for the textbox
        /// </summary>
        protected Texture2D Image = null;

        protected TextBox()
        {
            Image = AssetManager.Instance.LoadAsset<Texture2D>("UI/Box");
        }

        public TextBox(Vector2 position, Vector2 size, string text) : this()
        {
            SetSize(size);
            SetPosition(position);
            SetText(text);
        }

        public void SetSize(Vector2 size)
        {
            Size = size;
        }

        public void SetPosition(Vector2 position)
        {
            Position = position;
        }

        public void SetText(string text)
        {
            Text = text;
        }

        public void ScaleToText(SpriteFont font)
        {
            ScaleToText(font, Text);
        }

        public void ScaleToText(SpriteFont font, string text)
        {
            //Measure the size of the string with the font and add some padding
            SetSize(font.MeasureString(text) + new Vector2(10, 10));
        }

        public override void Update()
        {

        }

        public override void Draw()
        {
            Vector2 drawPos = Position - Size.HalveInt();
            SpriteRenderer.Instance.Draw(Image, drawPos, null, BoxColor, 0f, Vector2.Zero, Size, false, Layer, true);
            SpriteRenderer.Instance.DrawText(AssetManager.Instance.TTYDFont, Text, Position, TextColor, 0f, new Vector2(.5f, .5f), 1f, Layer + .0001f);
        }
    }
}
