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
    /// <para>Text for these bubbles involve control codes that have a variety of modifiers that apply a wide range of effects.
    /// See <see cref="DialogueGlobals"/> and/or documentation for the modifiers and their tags and attributes.</para>
    /// </summary>
    public class DialogueBubble : IPosition, IScalable, IUpdateable, IDrawable, ICleanup
    {
        public const double DefaultTimeBetweenChars = 25d;
        public const float TextScrollSpeed = -4f;
        public const float FastTextScrollSpeed = -12f;

        /// <summary>
        /// The button to press to go to the previous paragraph.
        /// </summary>
        public const Keys PreviousParagraphButton = Keys.I;

        /// <summary>
        /// The button to press to progress past an input prompt.
        /// </summary>
        public const Keys ProgressionButton = Keys.O;

        /// <summary>
        /// The amount to move the text when going up or down a paragraph.
        /// </summary>
        public float YMoveAmount { get; private set; } = 0f;

        /// <summary>
        /// The text in the dialogue bubble.
        /// </summary>
        private string Text = string.Empty;

        /// <summary>
        /// How long to wait in between each displayed character.
        /// </summary>
        public double TimeBetweenCharacters = DefaultTimeBetweenChars;

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
        private Stack<MessageRoutine> MessageRoutines = new Stack<MessageRoutine>();
        private bool AddedRoutines = false;

        public BubbleData DBubbleData { get; private set; } = null;

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

            //Check for adding Message Routines for this text index
            if (AddedRoutines == false)
            {
                CheckAndParseMessageRoutines(CurTextIndex);

                AddedRoutines = true;
            }
           
            //If we have no message routines, continue as normal
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
                    //If the user inputs the progress button, skip ahead
                    if (Input.GetKeyDown(ProgressionButton) == true)
                    {
                        PrintRemaining();
                        ElapsedTime = 0d;
                    }
                    //Otherwise, continue like normal
                    else
                    {
                        HandlePrintText();
                    }
                }
            }
            else
            {
                //If the current Message Routine has completed,
                if (MessageRoutines.Peek().Complete == true)
                {
                    //And and cleanup the current one
                    MessageRoutines.Peek().OnEnd();
                    MessageRoutines.Peek().CleanUp();

                    //Remove it from the stack and set elapsed time to 0
                    MessageRoutines.Pop();

                    ElapsedTime = 0d;
                }
                else
                {
                    //Start if not started
                    if (MessageRoutines.Peek().HasStarted == false)
                    {
                        MessageRoutines.Peek().OnStart();
                        MessageRoutines.Peek().HasStarted = true;
                    }

                    //Update the current routine
                    MessageRoutines.Peek().Update();
                }
            }

            //Update the progress star if disabled
            if (ProgressTextStar.Disabled == false)
                ProgressTextStar.Update();
        }

        #region Functional Methods

        /// <summary>
        /// Handles progressing text by printing it after the designated time.
        /// </summary>
        private void HandlePrintText()
        {
            SpeakerStartTalk();
            ElapsedTime += Time.ElapsedMilliseconds;

            //If we should print a new character in the text, do so
            if (ElapsedTime >= TimeBetweenCharacters)
            {
                PrintNextCharacter();

                ElapsedTime = 0d;
            }
        }

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
        /// Prints the remaining characters in the dialogue bubble until a Message Routine is added.
        /// </summary>
        private void PrintRemaining()
        {
            //Append the remaining part of the string
            int remainingCount = Text.Length - CurTextIndex;

            for (int i = 0; i < remainingCount; i++)
            {
                //Check for message routines
                //Break if we add one
                if (AddedRoutines == false)
                {
                    CheckAndParseMessageRoutines(CurTextIndex);

                    //We added a routine, so mark that we added it and break
                    if (MessageRoutines.Count > 0)
                    {
                        AddedRoutines = true;
                        break;
                    }
                }

                PrintNextCharacter();
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

            //Since we're using a stack, put them in backwards
            for (int i = routines.Count - 1; i >= 0; i--)
            {
                ParseMessageRoutine(routines[i]);
            }
        }

        private void ParseMessageRoutine(in HtmlNode routine)
        {
            string tag = routine.Name;

            if (DialogueGlobals.IsKeyTag(tag) == true)
            {
                InputRoutine inputRoutine = new InputRoutine(this, PreviousParagraphButton, ProgressionButton);
                AddMessageRoutine(inputRoutine);
            }

            if (DialogueGlobals.IsParagraphTag(tag) == true)
            {
                ScrollRoutine scrollRoutine = new ScrollRoutine(this, TextYOffset - YMoveAmount);
                AddMessageRoutine(scrollRoutine);
            }

            if (DialogueGlobals.IsWaitTag(tag) == true)
            {
                double waitDur = 0d;

                if (double.TryParse(routine.Attributes[0].Value, out double result) == true)
                {
                    waitDur = result;
                }

                WaitRoutine waitRoutine = new WaitRoutine(this, waitDur);
                AddMessageRoutine(waitRoutine);
            }
        }

        public void AddMessageRoutine(MessageRoutine routine)
        {
            MessageRoutines.Push(routine);
        }

        #endregion

        public void Draw()
        {
            //Handle the <clear> tag, which renders only the text
            if (DBubbleData.Clear == true) return;

            SpriteRenderer.Instance.DrawUI(BubbleImage.Tex, Position, BubbleImage.SourceRect, Color.White, 0f, Vector2.Zero, Scale, false, false, .9f);

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
                }
            }
        }
    }
}
