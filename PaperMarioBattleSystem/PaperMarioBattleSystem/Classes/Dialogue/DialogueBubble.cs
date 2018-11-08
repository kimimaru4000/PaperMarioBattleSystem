using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using HtmlAgilityPack;
using PaperMarioBattleSystem.Extensions;
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
        /// <summary>
        /// The default time between printing characters.
        /// </summary>
        public const double DefaultTimeBetweenChars = 25d;

        /// <summary>
        /// The base text scrolling speed when going up or down a new paragraph.
        /// </summary>
        public const float TextScrollSpeed = -4f;

        /// <summary>
        /// The faster text scrolling speed when going up or down a new paragraph.
        /// </summary>
        public const float FastTextScrollSpeed = -12f;

        /// <summary>
        /// The button to press to go to the previous paragraph.
        /// </summary>
        public const Keys PreviousParagraphButton = Keys.X;

        /// <summary>
        /// The button to press to progress past an input prompt.
        /// </summary>
        public const Keys ProgressionButton = Keys.Z;

        /// <summary>
        /// The amount to move the text when going up or down a paragraph.
        /// </summary>
        public float YMoveAmount { get; private set; } = 0f;

        /// <summary>
        /// The full text in the dialogue bubble.
        /// </summary>
        private string Text = string.Empty;

        /// <summary>
        /// How long to wait in between each displayed character.
        /// </summary>
        public double TimeBetweenCharacters = DefaultTimeBetweenChars;

        /// <summary>
        /// The StringBuilder adding the characters in the dialogue that will be rendered.
        /// </summary>
        public readonly StringBuilder stringBuilder = new StringBuilder();

        /// <summary>
        /// The current character index that will be printed in the text.
        /// </summary>
        private int CurTextIndex = 0;
        
        /// <summary>
        /// The current paragraph index.
        /// </summary>
        public int CurParagraphIndex = 0;

        /// <summary>
        /// The bubble graphic for the Dialogue Bubble.
        /// </summary>
        private CroppedTexture2D BubbleImage = null;

        /// <summary>
        /// The star to indicate the end of text.
        /// </summary>
        public readonly ProgressDialogueStar ProgressTextStar = null;

        public Vector2 Position { get; set; } = new Vector2(200, 140);
        public Vector2 Scale { get; set; } = new Vector2(400, 95);

        /// <summary>
        /// The Y offset for the text. This is set when scrolling up or down a new paragraph.
        /// </summary>
        public float TextYOffset = 0f;

        /// <summary>
        /// Whether all text is done printing or not.
        /// </summary>
        private bool DonePrintingText => (CurTextIndex >= Text.Length);

        /// <summary>
        /// Tells whether the dialogue bubble completed all of its text or not.
        /// </summary>
        public bool IsDone { get; private set; } = false;

        /// <summary>
        /// The BattlEntity speaking the dialogue. This can be null.
        /// </summary>
        public BattleEntity Speaker { get; private set; } = null;

        /// <summary>
        /// The RasterizerState for the Dialogue Bubble.
        /// This enables the ScissorRectangle to clip text outside of the bubble.
        /// </summary>
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

        /// <summary>
        /// Tells whether Message Routines were already checked and added for this character index.
        /// </summary>
        private bool AddedRoutines = false;

        /// <summary>
        /// The data that the Dialogue Bubble uses to render text.
        /// </summary>
        public BubbleData DBubbleData { get; private set; } = null;

        /// <summary>
        /// The last time a character was printed.
        /// </summary>
        //private double LastCharPrintTime = 0d;

        /// <summary>
        /// The elapsed time for the dialogue bubble.
        /// </summary>
        private double ElapsedTextTime = 0d;

        /// <summary>
        /// The elapsed time that tracks when the next character should be printed.
        /// </summary>
        private double ElapsedCharPrintTime = 0d;

        /// <summary>
        /// Tracks the time each character was printed.
        /// Times are set to <see cref="ElapsedTextTime"/> when the character at <see cref="CurTextIndex"/> is printed.
        /// <para>NOTE: See if this can be better on memory.</para>
        /// </summary>
        private double[] CharPrintTimes = null;

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

            CharPrintTimes = null;
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

            if (CharPrintTimes == null || (CharPrintTimes != null && CharPrintTimes.Length != Text.Length))
                CharPrintTimes = new double[Text.Length];
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

            TimeBetweenCharacters = DefaultTimeBetweenChars;
            ElapsedTextTime = 0d;
            ElapsedCharPrintTime = 0d;
            //LastCharPrintTime = 0d;

            MessageRoutines.Clear();
            AddedRoutines = false;

            SpeakerEndTalk();
            Speaker = null;

            stringBuilder.Clear();
            ProgressTextStar.Disabled = true;

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
                    //If the user inputs the progress button or the time between characters is 0 or less, skip ahead
                    if (TimeBetweenCharacters <= 0f || Input.GetKeyDown(ProgressionButton) == true)
                    {
                        PrintRemaining();
                        ElapsedCharPrintTime = 0d;
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
                MessageRoutine curMsgRoutine = MessageRoutines.Peek();

                //If the current Message Routine has completed,
                if (curMsgRoutine.Complete == true)
                {
                    //And and cleanup the current one
                    curMsgRoutine.OnEnd();
                    curMsgRoutine.CleanUp();

                    //Remove it from the stack and set elapsed time to 0
                    MessageRoutines.Pop();

                    ElapsedCharPrintTime = 0d;
                }
                else
                {
                    //Start if not started
                    if (curMsgRoutine.HasStarted == false)
                    {
                        curMsgRoutine.OnStart();
                        curMsgRoutine.HasStarted = true;
                    }

                    //Update the current routine
                    curMsgRoutine.Update();
                }
            }

            //Update the progress star if disabled
            if (ProgressTextStar.Disabled == false)
                ProgressTextStar.Update();

            //Update text time
            ElapsedTextTime += Time.ElapsedMilliseconds;
        }

        #region Functional Methods

        /// <summary>
        /// Handles progressing text by printing it after the designated time.
        /// </summary>
        private void HandlePrintText()
        {
            SpeakerStartTalk();
            ElapsedCharPrintTime += Time.ElapsedMilliseconds;

            //If we should print a new character in the text, do so
            if (ElapsedCharPrintTime >= TimeBetweenCharacters)
            {
                PrintNextCharacter();

                ElapsedCharPrintTime = 0d;
            }
        }

        /// <summary>
        /// Prints the next character to the dialogue bubble.
        /// </summary>
        private void PrintNextCharacter()
        {
            char curChar = Text[CurTextIndex];

            stringBuilder.Append(curChar);
            CharPrintTimes[CurTextIndex] = ElapsedTextTime;
            CurTextIndex++;

            AddedRoutines = false;

            //LastCharPrintTime = ElapsedTextTime;
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

        /// <summary>
        /// Parses a Message Routine from data.
        /// </summary>
        /// <param name="routine">The HtmlNode with the Message Routine data.</param>
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

            if (DialogueGlobals.IsSpeedTag(tag) == true)
            {
                double timeBetween = DefaultTimeBetweenChars;

                if (double.TryParse(routine.Attributes[0].Value, out double result) == true)
                {
                    timeBetween = result;
                }

                SpeedRoutine speedRoutine = new SpeedRoutine(this, timeBetween);
                AddMessageRoutine(speedRoutine);
            }

            if (DialogueGlobals.IsSoundTag(tag) == true)
            {
                string soundPath = routine.Attributes[0].Value;
                SoundLoadTypes soundType = SoundLoadTypes.Raw;
                float soundVolume = 1f;

                if (routine.Attributes.Contains(DialogueGlobals.ValueAttributeTag) == true)
                {
                    soundPath = routine.Attributes[DialogueGlobals.ValueAttributeTag].Value;
                }

                if (routine.Attributes.Contains(DialogueGlobals.SoundTypeAttributeTag) == true)
                {
                    if (Enum.TryParse(routine.Attributes[DialogueGlobals.SoundTypeAttributeTag].Value, true, out soundType) == false)
                    {
                        //Default to Raw if this cannot be parsed
                        soundType = SoundLoadTypes.Raw;
                    }
                }

                if (routine.Attributes.Contains(DialogueGlobals.SoundVolumeAttributeTag) == true)
                {
                    if (float.TryParse(routine.Attributes[DialogueGlobals.SoundVolumeAttributeTag].Value, NumberStyles.Any,
                        CultureInfo.InvariantCulture, out soundVolume) == false)
                    {
                        //Default to max volume if this cannot be parsed
                        soundVolume = 1f;
                    }
                }

                SoundRoutine soundRoutine = new SoundRoutine(this, soundPath, soundType, soundVolume);
                AddMessageRoutine(soundRoutine);
            }
        }
        
        /// <summary>
        /// Adds a Message Routine to the Message Routine stack.
        /// </summary>
        /// <param name="routine">The Message Routine to add.</param>
        public void AddMessageRoutine(MessageRoutine routine)
        {
            MessageRoutines.Push(routine);
        }

        #endregion

        /// <summary>
        /// Draws the Dialogue Bubble's image and end-of-text star.
        /// </summary>
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

        /// <summary>
        /// Draws the Dialogue Bubble's text.
        /// </summary>
        public void DrawText()
        {
            if (stringBuilder.Length > 0)
            {
                //This origin value looks pretty good for dynamic and scaled text
                //Important to note is it needs to be the same for all characters, since each one is a different size in the font texture
                Vector2 origin = new Vector2(8, 10);

                Vector2 offset = Vector2.Zero;
                Vector2 basePos = Position + new Vector2(5f + origin.X, 5f + origin.Y + TextYOffset);

                //Go through all the data and render the text
                for (int i = 0; i < DBubbleData.TextData.Count; i++)
                {
                    BubbleTextData bdata = DBubbleData.TextData[i];
                
                    for (int j = bdata.StartIndex; j < bdata.EndIndex && j < stringBuilder.Length; j++)
                    {
                        Vector2 finalPos = basePos + new Vector2(0, (bdata.ParagraphIndex * YMoveAmount));
                        finalPos.Y -= (bdata.NewLineCount * BubbleFont.LineSpacing);

                        Vector2 scale = bdata.Scale;

                        //Handle dynamic text
                        if (bdata.DynamicSize.HasValue == true)
                        {
                            //Get the time difference between now and when this character was printed
                            double dynamicTimeOffset = ElapsedTextTime - CharPrintTimes[j];

                            if (dynamicTimeOffset < DialogueGlobals.DynamicScaleTime)
                            {
                                scale = DialogueGlobals.GetDynamicTextSize(dynamicTimeOffset, bdata.DynamicSize.Value, bdata.Scale);
                            }
                        }

                        //Handle shaky text
                        if (bdata.Shake == true)
                        {
                            finalPos += DialogueGlobals.GetShakyTextOffset(new Vector2(1));
                        }
                        
                        //Handle wavy text
                        if (bdata.Wave == true)
                        {
                            finalPos += DialogueGlobals.GetWavyTextOffset(ElapsedTextTime, j * Time.ElapsedMilliseconds, new Vector2(2));
                        }

                        //Render the character
                        offset = SpriteRenderer.Instance.uiBatch.DrawCharacter(BubbleFont, stringBuilder[j], FontGlyphs, offset, finalPos, bdata.TextColor, 0f, origin, scale, bdata.Scale, SpriteEffects.None, .95f);
                    }
                }
            }
        }
    }
}
