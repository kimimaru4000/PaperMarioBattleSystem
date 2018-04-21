using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using HtmlAgilityPack;
using static PaperMarioBattleSystem.DialogueGlobals;

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
        public const double DefaultTimeBetweenChars = 34d;
        public const float TextScrollSpeed = -4f;
        public const float FastTextScrollSpeed = -12f;

        public float YMoveAmount { get; private set; } = 0f;

        /// <summary>
        /// The text in the dialogue bubble.
        /// </summary>
        private string Text = string.Empty;

        /// <summary>
        /// How long to wait in between each displayed character.
        /// </summary>
        public double TimeBetweenCharacters = 100d;

        public readonly StringBuilder stringBuilder = new StringBuilder();

        private int CurTextIndex = 0;
        public int CurParagraphIndex = 0;

        private CroppedTexture2D BubbleImage = null;

        /// <summary>
        /// The star to indicate the end of text.
        /// </summary>
        public readonly ProgressDialogueStar ProgressTextStar = null;

        public Vector2 Position { get; set; } = new Vector2(100, 100);
        public Vector2 Scale { get; set; } = new Vector2(400, 95);

        public float TextYOffset = 0f;
        private bool DonePrintingText => (CurTextIndex >= Text.Length);

        /// <summary>
        /// Tells whether the dialogue bubble completed all of its text or not.
        /// </summary>
        public bool IsDone { get; private set; } = false;

        /// <summary>
        /// The BattlEntity speaking the dialogue. This can be null.
        /// </summary>
        public BattleEntity Speaker { get; private set; } = null;

        public RasterizerState BubbleRasterizerState { get; private set; } = null;

        /// <summary>
        /// The font the Dialogue Bubble renders text with.
        /// </summary>
        private SpriteFont BubbleFont = null;

        /// <summary>
        /// The font's glyphs.
        /// </summary>
        private Dictionary<char, SpriteFont.Glyph> FontGlyphs = null;

        /// <summary>
        /// The Message Routines to invoke.
        /// </summary>
        private Queue<MessageRoutine> MessageRoutines = new Queue<MessageRoutine>();
        private bool AddedRoutines = false;

        private BubbleData DBubbleData = null;

        public double ElapsedTime = 0d;

        public DialogueBubble()
        {
            TimeBetweenCharacters = DefaultTimeBetweenChars;

            ProgressTextStar = new ProgressDialogueStar();
            ProgressTextStar.Disabled = true;

            LoadGraphics();

            //Initialize the RasterizerState and enable the ScissorTest
            //This lets us use the ScissorRectangle to clip any text outside the textbox
            BubbleRasterizerState = new RasterizerState();
            BubbleRasterizerState.ScissorTestEnable = true;

            YMoveAmount = Scale.Y;
        }

        public void CleanUp()
        {
            BubbleRasterizerState.Dispose();
            BubbleRasterizerState = null;

            BubbleImage = null;
            Speaker = null;

            DBubbleData = null;
            FontGlyphs = null;
        }

        private void LoadGraphics()
        {
            Texture2D tex = AssetManager.Instance.LoadRawTexture2D($"{ContentGlobals.UIRoot}/Box.png");

            BubbleImage = new CroppedTexture2D(tex,
                new Rectangle(0, 0, 1, 1));//new Rectangle(413, 159, 126, 74));//, 57, 57, 31, 41);
        }

        /// <summary>
        /// Resets the Dialogue Bubble and sets new text for it.
        /// </summary>
        /// <param name="text">A string containing the text for the Dialogue Bubble to parse and print.</param>
        public void SetText(string text)
        {
            Reset();

            Text = text;

            DBubbleData = DialogueGlobals.ParseText(Text, out Text);
        }

        /// <summary>
        /// Sets the font to use for the Dialogue Bubble.
        /// </summary>
        /// <param name="spriteFont">The SpriteFont to use.</param>
        public void SetFont(SpriteFont spriteFont)
        {
            BubbleFont = spriteFont;
            FontGlyphs = BubbleFont.GetGlyphs();

            //YMoveAmount = BubbleFont.LineSpacing * 4f;
        }

        /// <summary>
        /// Attaches a speaker to the Dialogue Bubble. When text scrolls, the speaker will appear to speak.
        /// </summary>
        /// <param name="speaker">The BattleEntity to speak.</param>
        public void AttachSpeaker(BattleEntity speaker)
        {
            Speaker = speaker;
        }

        /// <summary>
        /// Makes the speaker play its talking animation if it's not doing so already.
        /// </summary>
        public void SpeakerStartTalk()
        {
            if (Speaker != null && Speaker.AnimManager.CurrentAnim.Key != AnimationGlobals.TalkName)
                Speaker.AnimManager.PlayAnimation(AnimationGlobals.TalkName);
        }

        /// <summary>
        /// Makes the speaker end its talking animation by going into its idle animation.
        /// </summary>
        public void SpeakerEndTalk()
        {
            if (Speaker != null && Speaker.AnimManager.CurrentAnim.Key == AnimationGlobals.TalkName)
                Speaker.AnimManager.PlayAnimation(Speaker.GetIdleAnim());
        }

        /// <summary>
        /// Resets the Dialogue Bubble.
        /// </summary>
        public void Reset()
        {
            Text = string.Empty;
            CurTextIndex = 0;
            CurParagraphIndex = 0;
            TextYOffset = 0f;

            MessageRoutines.Clear();
            AddedRoutines = false;

            SpeakerEndTalk();
            Speaker = null;

            stringBuilder.Clear();

            IsDone = false;
        }

        /// <summary>
        /// Marks the Dialogue Bubble as finished so it can be closed.
        /// </summary>
        public void Close()
        {
            IsDone = true;
        }

        public void Update()
        {
            //Return if done
            if (IsDone == true) return;

            //if (IsScrolling == true)
            //{
            //    HandleScrollText();
            //}
            //else
            {
                if (AddedRoutines == false)
                {
                    CheckAndParseMessageRoutines(CurTextIndex);

                    AddedRoutines = true;

                    if (MessageRoutines.Count > 0)
                        MessageRoutines.Peek().OnStart();
                }
                else
                {
                    if (MessageRoutines.Count == 0)
                    {
                        //If we're done printing, close the dialogue bubble
                        if (DonePrintingText == true)
                        {
                            SpeakerEndTalk();
                            Close();
                        }
                        //Otherwise keep printing
                        else
                        {
                            HandlePrintText();
                        }
                    }
                    else
                    {
                        if (MessageRoutines.Peek().Complete == true)
                        {
                            MessageRoutines.Peek().OnEnd();
                            MessageRoutines.Peek().CleanUp();

                            MessageRoutines.Dequeue();
                            if (MessageRoutines.Count > 0)
                                MessageRoutines.Peek().OnStart();
                        }
                        else
                        {
                            MessageRoutines.Peek().Update();
                        }
                    }
                }
            }

            //HandleInput();

            //Handle disabling of the progress star
            //ProgressTextStar.Disabled = (CurTextIndex < Text.Length || OffsetToScroll != TextYOffset);

            if (ProgressTextStar.Disabled == false)
                ProgressTextStar.Update();
        }

        #region Functional Methods

        /// <summary>
        /// Prints the next character to the dialogue bubble.
        /// </summary>
        private void PrintNextCharacter()
        {
            char curChar = Text[CurTextIndex];

            stringBuilder.Append(curChar);
            CurTextIndex++;

            AddedRoutines = false;
        }

        /// <summary>
        /// Prints the remaining characters in the current set of text in the dialogue bubble.
        /// </summary>
        private void PrintRemaining()
        {
            //Append the remaining part of the string
            int remainingCount = Text.Length - CurTextIndex;

            for (int i = 0; i < remainingCount; i++)
            {
                PrintNextCharacter();
            }
        }

        #endregion

        #region Input Methods

        private void HandlePrintText()
        {
            SpeakerStartTalk();
            ElapsedTime += Time.ElapsedMilliseconds;

            //If we should print a new character in the text, do so
            if (ElapsedTime >= TimeBetweenCharacters)
            {
                PrintNextCharacter();

                ElapsedTime = 0d;

                //if (DonePrintingText == true)
                //{
                //    //Stop the speaker from talking
                //    SpeakerEndTalk();
                //}
            }
        }

        #endregion

        #region Message Routine Parsing

        /// <summary>
        /// Checks for message routines at the specified text index.
        /// All found routines are parsed into instances and added to the <see cref="MessageRoutines"/> queue.
        /// </summary>
        /// <param name="curTextIndex">The index in the dialogue string to check for message routines.</param>
        private void CheckAndParseMessageRoutines(int curTextIndex)
        {
            if (DBubbleData.MessageRoutines.ContainsKey(curTextIndex) == false) return;

            List<HtmlNode> routines = DBubbleData.MessageRoutines[curTextIndex];

            for (int i = 0; i < routines.Count; i++)
            {
                ParseMessageRoutine(routines[i]);
            }
        }

        private void ParseMessageRoutine(in HtmlNode routine)
        {
            string tag = routine.Name;

            if (DialogueGlobals.IsKeyTag(tag) == true)
            {
                InputRoutine inputRoutine = new InputRoutine(this, Keys.O);
                AddMessageRoutine(inputRoutine);
            }

            if (DialogueGlobals.IsParagraphTag(tag) == true)
            {
                ScrollRoutine scrollRoutine = new ScrollRoutine(this, TextYOffset - YMoveAmount);
                AddMessageRoutine(scrollRoutine);
            }
        }

        private void AddMessageRoutine(MessageRoutine routine)
        {
            MessageRoutines.Enqueue(routine);
        }

        #endregion

        public void Draw()
        {
            //Handle the <clear> tag, which renders only the text
            if (DBubbleData.Clear == true) return;

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
                Vector2 offset = Vector2.Zero;
                Vector2 basePos = Position + new Vector2(10, 5f + TextYOffset);

                //Go through all the data and render the text
                for (int i = 0; i < DBubbleData.TextData.Count; i++)
                {
                    BubbleTextData bdata = DBubbleData.TextData[i];
                
                    for (int j = bdata.StartIndex; j < bdata.EndIndex && j < stringBuilder.Length; j++)
                    {
                        Vector2 finalPos = basePos + new Vector2(0, (bdata.ParagraphIndex * YMoveAmount));
                        finalPos.Y -= (bdata.NewLineCount * BubbleFont.LineSpacing);

                        Vector2 scale = bdata.Scale;

                        //Handle shaky text
                        if (bdata.Shake == true)
                        {
                            finalPos += DialogueGlobals.GetShakyTextOffset(new Vector2(1));
                        }
                
                        //Handle wavy text
                        if (bdata.Wave == true)
                        {
                            finalPos += DialogueGlobals.GetWavyTextOffset(j * TimeBetweenCharacters, new Vector2(2));
                        }
                
                        //Render the character
                        offset = SpriteRenderer.Instance.uiBatch.DrawCharacter(AssetManager.Instance.TTYDFont, stringBuilder[j], FontGlyphs, offset, finalPos, bdata.TextColor, 0f, Vector2.Zero, scale, SpriteEffects.None, .95f);
                    }
                
                    //offset = SpriteRenderer.Instance.uiBatch.DrawStringChars(AssetManager.Instance.TTYDFont, stringBuilder, offset, bdata.StartIndex, bdata.EndIndex, finalPos, bdata.TextColor, 0f, Vector2.Zero, Vector2.One, SpriteEffects.None, .95f);
                }

                //SpriteRenderer.Instance.uiBatch.DrawStringChars(AssetManager.Instance.TTYDFont, stringBuilder, Vector2.Zero, 0, stringBuilder.Length, Position + new Vector2(10, 5f + TextYOffset), Color.Black, 0f, Vector2.Zero, Vector2.One, SpriteEffects.None, .95f);
                //SpriteRenderer.Instance.DrawUIText(AssetManager.Instance.TTYDFont, stringBuilder, Position + new Vector2(10, 5f + TextYOffset), Color.Black, 0f, Vector2.Zero, 1f, .95f);
            }
        }
    }
}
