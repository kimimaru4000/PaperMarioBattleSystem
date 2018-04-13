using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// A dialogue bubble. It shows what characters are saying over time.
    /// </summary>
    /// <remarks>NOTE: Document the control codes in the PM games for the dialogue bubbles.
    /// Implementation will come down the road, but knowing them will be incredibly useful.</remarks>
    public class DialogueBubble : IPosition, IScalable, IUpdateable, IDrawable
    {
        private const float TextScrollSpeed = -4f;

        private readonly float YMoveAmount = 0f;

        //NOTE: Start with string arrays to get it working, but eventually we may want a single chunk of text with the command controls
        //We'll see as it goes - arrays are nice since they separate the dialogue
        private string[] TextArray = null;

        /// <summary>
        /// The text in the dialogue bubble.
        /// </summary>
        private string Text = string.Empty;

        /// <summary>
        /// How long to wait in between each displayed character.
        /// </summary>
        public double TimeBetweenCharacters = 100d;

        public readonly StringBuilder stringBuilder = new StringBuilder();

        private double ElapsedTime = 0d;

        private int CurArrayIndex = 0;
        private int CurTextIndex = 0;
        private int MaxArrayIndex = 0;

        private CroppedTexture2D BubbleImage = null;

        /// <summary>
        /// The star to indicate the end of text.
        /// </summary>
        private ProgressDialogueStar ProgressTextStar = null;

        public Vector2 Position { get; set; } = new Vector2(100, 100);
        public Vector2 Scale { get; set; } = new Vector2(400, 95);

        private float TextYOffset = 0f;
        private float OffsetToScroll = 0f;

        /// <summary>
        /// Tells whether the dialogue bubble completed all of its text or not.
        /// </summary>
        public bool IsDone { get; private set; } = false;

        public DialogueBubble(string[] textArray, double timeBetweenCharacters)
        {
            TextArray = textArray;

            Text = TextArray[CurArrayIndex];
            TimeBetweenCharacters = timeBetweenCharacters;

            ProgressTextStar = new ProgressDialogueStar();
            ProgressTextStar.Disabled = true;

            YMoveAmount = AssetManager.Instance.TTYDFont.LineSpacing * 4f;

            LoadGraphics();
        }

        private void LoadGraphics()
        {
            Texture2D tex = AssetManager.Instance.LoadRawTexture2D($"{ContentGlobals.UIRoot}/Box.png");

            BubbleImage = new CroppedTexture2D(tex,
                new Rectangle(0, 0, 1, 1));//new Rectangle(413, 159, 126, 74));//, 57, 57, 31, 41);
        }

        public void Update()
        {
            //Return if done
            if (IsDone == true) return;

            if (OffsetToScroll != TextYOffset)
            {
                HandleScrollText();
            }
            else
            {
                HandleInput();
                HandlePrintText();
            }

            ProgressTextStar.Disabled = (CurTextIndex < Text.Length || OffsetToScroll != TextYOffset);

            if (ProgressTextStar.Disabled == false)
                ProgressTextStar.Update();
        }

        private void HandlePrintText()
        {
            if (CurTextIndex < Text.Length)
            {
                ElapsedTime += Time.ElapsedMilliseconds;

                if (ElapsedTime >= TimeBetweenCharacters)
                {
                    stringBuilder.Append(Text[CurTextIndex]);
                    CurTextIndex++;
                    ElapsedTime = 0d;
                }
            }
        }

        private void HandleScrollText()
        {
            //Scroll text upwards or downwards depending on the new scroll value to go to
            if (TextYOffset < OffsetToScroll)
            {
                TextYOffset -= TextScrollSpeed;
                if (TextYOffset > OffsetToScroll)
                    TextYOffset = OffsetToScroll;
            }
            else
            {
                TextYOffset += TextScrollSpeed;
                if (TextYOffset < OffsetToScroll)
                    TextYOffset = OffsetToScroll;
            }
        }

        private void HandleInput()
        {
            HandleProgressText();
            HandlePreviousText();
        }

        private void HandleProgressText()
        {
            //Use single button for now for skipping
            if (Input.GetKeyDown(Keys.O) == true)
            {
                if (CurTextIndex < Text.Length)
                {
                    stringBuilder.Append(Text.Substring(CurTextIndex, Text.Length - CurTextIndex));
                    CurTextIndex = Text.Length;
                }
                else
                {
                    //Clear and check
                    CurArrayIndex++;

                    if (CurArrayIndex > MaxArrayIndex)
                    {
                        MaxArrayIndex = CurArrayIndex;
                    }
                    else
                    {
                        OffsetToScroll -= YMoveAmount;
                        Text = TextArray[CurArrayIndex];
                        CurTextIndex = Text.Length;
                        return;
                    }

                    if (CurArrayIndex >= TextArray.Length)
                    {
                        IsDone = true;
                    }
                    else
                    {
                        OffsetToScroll -= YMoveAmount;

                        stringBuilder.Append("\n\n\n\n");

                        Text = TextArray[CurArrayIndex];

                        CurTextIndex = 0;
                        ElapsedTime = 0d;
                    }
                }
            }
        }

        private void HandlePreviousText()
        {
            //Go back to previous text
            if (Input.GetKeyDown(Keys.I) == true)
            {
                if (CurArrayIndex > 0 && CurTextIndex >= Text.Length)
                {
                    CurArrayIndex--;

                    OffsetToScroll += YMoveAmount;

                    Text = TextArray[CurArrayIndex];
                    CurTextIndex = Text.Length;
                }
            }
        }

        public void Draw()
        {
            SpriteRenderer.Instance.DrawUI(BubbleImage.Tex, Position, BubbleImage.SourceRect, Color.White, 0f, Vector2.Zero, Scale, false, false, .9f);
            //SpriteRenderer.Instance.DrawUISliced(BubbleImage, new Rectangle((int)Position.X, (int)Position.Y, (int)BubbleSize.X, (int)BubbleSize.Y), Color.White, .9f);
            SpriteRenderer.Instance.DrawUIText(AssetManager.Instance.TTYDFont, stringBuilder, Position + new Vector2(10, 5 + TextYOffset), Color.Black, 0f, Vector2.Zero, 1f, .95f);

            if (ProgressTextStar.Disabled == false)
            {
                Vector2 widthHeight = BubbleImage.WidthHeightToVector2() * Scale;

                //Draw the star
                SpriteRenderer.Instance.DrawUI(ProgressTextStar.Graphic.Tex, Position + widthHeight, ProgressTextStar.Graphic.SourceRect,
                    Color.White, ProgressTextStar.Rotation, new Vector2(.5f, .5f), ProgressTextStar.Scale, false, false, 1f);
            }
        }
    }
}
