using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// A dialogue bubble. It shows what characters are saying over time.
    /// </summary>
    /// <remarks>NOTE: Document the control codes in the PM games for the dialogue bubbles.
    /// Implementation will come down the road, but knowing them will be incredibly useful.</remarks>
    public class DialogueBubble : IUpdateable
    {
        /// <summary>
        /// The text in the dialogue bubble.
        /// </summary>
        public string Text = string.Empty;

        /// <summary>
        /// How long to wait in between each displayed character.
        /// </summary>
        public double TimeBetweenCharacters = 100d;

        public readonly StringBuilder stringBuilder = new StringBuilder();

        private double ElapsedTime = 0d;

        private int CurTextIndex = 0;

        /*Temp*/
        public CroppedTexture2D BubbleImage { get; private set; } = null;

        public DialogueBubble(string text, double timeBetweenCharacters)
        {
            Text = text;
            TimeBetweenCharacters = timeBetweenCharacters;

            BubbleImage = new CroppedTexture2D(AssetManager.Instance.LoadRawTexture2D($"{ContentGlobals.BattleGFX}.png"),
                new Rectangle(413, 159, 126, 74));
        }

        public void Update()
        {
            //Use single button for now for skipping
            if (Input.GetKeyDown(Keys.O) == true)
            {
                if (CurTextIndex < Text.Length)
                {
                    stringBuilder.Clear();
                    stringBuilder.Append(Text);
                    CurTextIndex = Text.Length;
                }
            }

            if (CurTextIndex < Text.Length)
            {
                ElapsedTime += Time.ElapsedMilliseconds;

                if (ElapsedTime >= TimeBetweenCharacters)
                {
                    stringBuilder.Append(Text[CurTextIndex]);
                    CurTextIndex++;
                    ElapsedTime = 0;
                }
            }
        }
    }
}
