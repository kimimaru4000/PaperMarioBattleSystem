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
    public class TextBox : PosUIElement
    {
        /// <summary>
        /// The default buffer room used when fitting the TextBox to text.
        /// </summary>
        public static readonly Vector2 DefaultBufferRoom = new Vector2(10, 10);

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
            Image = AssetManager.Instance.LoadRawTexture2D($"{ContentGlobals.UIRoot}/Box.png");
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

        /// <summary>
        /// Scales the TextBox to fit its current text, with some buffer room.
        /// <para>The default buffer room is used.</para>
        /// </summary>
        /// <param name="font">The font to scale by.</param>
        public void ScaleToText(SpriteFont font)
        {
            ScaleToText(font, Text);
        }

        /// <summary>
        /// Scales the TextBox to fit a particular string, with some buffer room.
        /// </summary>
        /// <param name="font">The font to scale by.</param>
        /// <param name="text">The text to use to scale.</param>
        /// <para>The default buffer room is used.</para>
        public void ScaleToText(SpriteFont font, string text)
        {
            ScaleToText(font, text, DefaultBufferRoom);
        }

        /// <summary>
        /// Scales the TextBox to fit a particular string, with a designated amount of buffer room.
        /// </summary>
        /// <param name="font">The font to scale by.</param>
        /// <param name="text">The text to use to scale.</param>
        /// <param name="bufferRoom">The amount of buffer room on each side.</param>
        public void ScaleToText(SpriteFont font, string text, Vector2 bufferRoom)
        {
            //Measure the size of the string with the font and add some padding
            SetSize(font.MeasureString(text) + bufferRoom);
        }

        public override void Update()
        {

        }

        public override void Draw()
        {
            Vector2 drawPos = Position - Size.HalveInt();
            SpriteRenderer.Instance.DrawUI(Image, drawPos, null, BoxColor, 0f, Vector2.Zero, Size, false, false, Layer);
            SpriteRenderer.Instance.DrawUIText(AssetManager.Instance.TTYDFont, Text, Position, TextColor, 0f, new Vector2(.5f, .5f), 1f, Layer + .0001f);
        }
    }
}
