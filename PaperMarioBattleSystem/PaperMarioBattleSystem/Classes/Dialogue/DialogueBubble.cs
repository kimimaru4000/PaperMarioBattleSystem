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
    /// <remarks>NOTE: Look at the control codes in the PM games for the dialogue bubbles.
    /// Implementation will come down the road, but knowing them will be incredibly useful.
    /// 
    /// Look into HTML or XML parsers that might help with this, as the control codes follow a similar syntax.</remarks>
    public class DialogueBubble : IPosition, IScalable, IUpdateable, IDrawable, ICleanup
    {
        private const float TextScrollSpeed = -4f;
        private const float FastTextScrollSpeed = -12f;

        public readonly float YMoveAmount = 0f;

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

        private float CurScrollSpeed = TextScrollSpeed;

        private bool IsScrolling => (OffsetToScroll != TextYOffset);

        /// <summary>
        /// Tells whether the dialogue bubble completed all of its text or not.
        /// </summary>
        public bool IsDone { get; private set; } = false;

        public RasterizerState BubbleRasterizerState { get; private set; } = null;

        public DialogueBubble(string[] textArray, double timeBetweenCharacters)
        {
            TextArray = textArray;

            Text = TextArray[CurArrayIndex];
            TimeBetweenCharacters = timeBetweenCharacters;

            ProgressTextStar = new ProgressDialogueStar();
            ProgressTextStar.Disabled = true;

            YMoveAmount = AssetManager.Instance.TTYDFont.LineSpacing * 4f;

            LoadGraphics();

            //Initialize the RasterizerState and enable the ScissorTest
            //This lets us use the ScissorRectangle to clip any text outside the textbox
            BubbleRasterizerState = new RasterizerState();
            BubbleRasterizerState.ScissorTestEnable = true;
        }

        public void CleanUp()
        {
            BubbleRasterizerState.Dispose();
            BubbleRasterizerState = null;

            BubbleImage = null;
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

            if (IsScrolling == true)
            {
                HandleScrollText();
            }
            else
            {
                HandlePrintText();
            }

            HandleInput();

            //Handle disabling of the progress star
            ProgressTextStar.Disabled = (CurTextIndex < Text.Length || OffsetToScroll != TextYOffset);

            if (ProgressTextStar.Disabled == false)
                ProgressTextStar.Update();
        }

        private void HandlePrintText()
        {
            //If we're not done printing the text, keep going
            if (CurTextIndex < Text.Length)
            {
                ElapsedTime += Time.ElapsedMilliseconds;

                //If we should print a new character in the text, do so
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
                //Scroll up
                TextYOffset -= CurScrollSpeed;
                if (TextYOffset > OffsetToScroll)
                {
                    TextYOffset = OffsetToScroll;
                }
            }
            else
            {
                //Scroll down
                TextYOffset += CurScrollSpeed;
                if (TextYOffset < OffsetToScroll)
                {
                    TextYOffset = OffsetToScroll;
                }
            }
        }

        private void HandleInput()
        {
            HandleProgressText();

            if (IsScrolling == false)
                HandlePreviousText();
        }

        private void HandleProgressText()
        {
            //Handle skipping through text with a button input
            if (Input.GetKeyDown(Keys.O) == true)
            {
                //If we're scrolling, pressing the button only increases the scroll speed
                if (IsScrolling == true)
                {
                    CurScrollSpeed = FastTextScrollSpeed;
                    return;
                }

                //We're not done printing
                if (CurTextIndex < Text.Length)
                {
                    //Append the remaining part of the string and set the index to the final value
                    stringBuilder.Append(Text.Substring(CurTextIndex, Text.Length - CurTextIndex));
                    CurTextIndex = Text.Length;
                }
                //We're done printing - progress
                else
                {
                    //Increment array index
                    CurArrayIndex++;

                    //Reset scroll speed
                    CurScrollSpeed = TextScrollSpeed;
                    
                    //If we haven't been to this text yet, set that we just visited it
                    if (CurArrayIndex > MaxArrayIndex)
                    {
                        MaxArrayIndex = CurArrayIndex;
                    }
                    //We have been to this text; it's all already there, so simply scroll to it
                    else
                    {
                        OffsetToScroll -= YMoveAmount;
                        Text = TextArray[CurArrayIndex];
                        CurTextIndex = Text.Length;
                        return;
                    }

                    //If this is the last text in the bubble, mark it as done
                    if (CurArrayIndex >= TextArray.Length)
                    {
                        IsDone = true;
                    }
                    //Move onto the next set of text
                    else
                    {
                        //Move the text up
                        OffsetToScroll -= YMoveAmount;

                        //Append new lines to offset the next set of text that will be printed
                        stringBuilder.Append("\n\n\n\n");

                        //Set text to the new value and reset the text index so it can print
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
                //Don't allow going back to the previous text if we're on the last one or the current text didn't finish printing
                if (CurArrayIndex > 0 && CurTextIndex >= Text.Length)
                {
                    //Subtract index
                    CurArrayIndex--;

                    //Scroll back up
                    OffsetToScroll += YMoveAmount;

                    //Set text and text index
                    Text = TextArray[CurArrayIndex];
                    CurTextIndex = Text.Length;

                    //Reset scroll speed
                    CurScrollSpeed = TextScrollSpeed;
                }
            }
        }

        public void Draw()
        {
            SpriteRenderer.Instance.DrawUI(BubbleImage.Tex, Position, BubbleImage.SourceRect, Color.White, 0f, Vector2.Zero, Scale, false, false, .9f);
            //SpriteRenderer.Instance.DrawUISliced(BubbleImage, new Rectangle((int)Position.X, (int)Position.Y, (int)BubbleSize.X, (int)BubbleSize.Y), Color.White, .9f);

            if (ProgressTextStar.Disabled == false)
            {
                Vector2 widthHeight = BubbleImage.WidthHeightToVector2() * Scale;

                //Draw the star
                SpriteRenderer.Instance.DrawUI(ProgressTextStar.Graphic.Tex, Position + widthHeight, ProgressTextStar.Graphic.SourceRect,
                    Color.White, ProgressTextStar.Rotation, new Vector2(.5f, .5f), ProgressTextStar.Scale, false, false, 1f);
            }
        }

        public void DrawText()
        {
            if (stringBuilder.Length > 0)
            {
                SpriteRenderer.Instance.DrawUIText(AssetManager.Instance.TTYDFont, stringBuilder, Position + new Vector2(10, 5 + TextYOffset), Color.Black, 0f, Vector2.Zero, 1f, .95f);
            }
        }
    }
}
