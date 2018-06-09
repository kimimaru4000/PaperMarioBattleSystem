using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using HtmlAgilityPack;
using PaperMarioBattleSystem.Utilities;
using PaperMarioBattleSystem.Extensions;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// Global values relating to dialogue.
    /// </summary>
    public static class DialogueGlobals
    {
        /// <summary>
        /// The HtmlDocument object used to parse Dialogue Bubble text.
        /// </summary>
        private static readonly HtmlDocument HTMLDoc = new HtmlDocument();

        /// <summary>
        /// Tells if a string is a text node.
        /// </summary>
        /// <param name="text">The string to test.</param>
        /// <returns>true if so, otherwise false.</returns>
        public static bool IsTextNode(in string text)
        {
            return (text == HtmlTextNode.HtmlNodeTypeNameText);
        }

        /// <summary>
        /// Tells if the string is a node that should be ignored.
        /// <para>Ignored nodes are currently comments and documents.</para>
        /// </summary>
        /// <param name="text">The string to test.</param>
        /// <returns>true if it should be ignored, otherwise false.</returns>
        public static bool IsIgnorableNode(in string text)
        {
            return (text == HtmlTextNode.HtmlNodeTypeNameComment || text == HtmlTextNode.HtmlNodeTypeNameDocument);
        }

        #region Bubble Functionality

        /// <summary>
        /// The string representing the paragraph tag.
        /// </summary>
        public const string NewParagraphTag = "p";

        /// <summary>
        /// Tells if the string is a paragraph tag.
        /// </summary>
        /// <param name="text">The string to test.</param>
        /// <returns>true if so, otherwise false.</returns>
        public static bool IsParagraphTag(in string text)
        {
            return (text == NewParagraphTag);
        }

        #endregion

        #region Text Modifiers

        /// <summary>
        /// The string representing the color text modifier.
        /// </summary>
        public const string ColorMod = "color";

        /// <summary>
        /// The string representing the dynamic text modifier.
        /// </summary>
        public const string DynamicMod = "dynamic";

        /// <summary>
        /// The time it takes for dynamic text to scale to its final value.
        /// </summary>
        public const double DynamicScaleTime = (8d / 60d) * Time.MsPerS;

        /// <summary>
        /// The string representing the shake text modifier.
        /// </summary>
        public const string ShakeMod = "shake";

        /// <summary>
        /// The string representing the wave text modifier.
        /// </summary>
        public const string WaveMod = "wave";

        /// <summary>
        /// The string representing the scale text modifier.
        /// </summary>
        public const string ScaleMod = "scale";

        /// <summary>
        /// Tells if the string is the color text modifier.
        /// </summary>
        /// <param name="text">The string to test.</param>
        /// <returns>true if so, otherwise false.</returns>
        public static bool IsColorMod(in string text)
        {
            return (text == ColorMod);
        }

        /// <summary>
        /// Tells if the string is the dynamic text modifier.
        /// </summary>
        /// <param name="text">The string to test.</param>
        /// <returns>true if so, otherwise false.</returns>
        public static bool IsDynamicMod(in string text)
        {
            return (text == DynamicMod);
        }

        /// <summary>
        /// Tells if the string is the shake text modifier.
        /// </summary>
        /// <param name="text">The string to test.</param>
        /// <returns>true if so, otherwise false.</returns>
        public static bool IsShakeMod(in string text)
        {
            return (text == ShakeMod);
        }

        /// <summary>
        /// Tells if the string is the wave text modifier.
        /// </summary>
        /// <param name="text">The string to test.</param>
        /// <returns>true if so, otherwise false.</returns>
        public static bool IsWaveMod(in string text)
        {
            return (text == WaveMod);
        }

        /// <summary>
        /// Tells if the string is the scale text modifier.
        /// </summary>
        /// <param name="text">The string to test.</param>
        /// <returns>true if so, otherwise false.</returns>
        public static bool IsScaleMod(in string text)
        {
            return (text == ScaleMod);
        }

        /// <summary>
        /// Tells if the string is a valid text modifier.
        /// </summary>
        /// <param name="text">The string to test.</param>
        /// <returns>true if so, otherwise false.</returns>
        public static bool IsTextModifier(in string text)
        {
            return (IsColorMod(text) == true || IsDynamicMod(text) == true || IsShakeMod(text) == true || IsWaveMod(text) == true)
                || IsScaleMod(text) == true;
        }

        /// <summary>
        /// Gets the positional offset for wavy text.
        /// </summary>
        /// <param name="timeVal">The time value to use. Can be <see cref="Time.ActiveMilliseconds"/> or another timer.</param>
        /// <param name="timeOffset">The time offset. This is usually the time between rendering each character in a dialogue bubble.</param>
        /// <param name="amount">The max amount to offset.</param>
        /// <returns>A Vector2 containing the offset for wavy text.</returns>
        public static Vector2 GetWavyTextOffset(double timeVal, double timeOffset, in Vector2 amount)
        {
            //Wavy text goes clockwise
            double time = (timeVal + timeOffset) / 75d;
            return new Vector2((float)Math.Cos(time) * amount.X, (float)Math.Sin(time) * amount.Y);
        }

        /// <summary>
        /// Gets the positional offset for shaky text.
        /// </summary>
        /// <param name="amount">The max amount to offset.</param>
        /// <returns>A Vector2 containing the offset for shaky text.</returns>
        public static Vector2 GetShakyTextOffset(in Vector2 amount)
        {
            float x = (float)RandomGlobals.Randomizer.RandomDouble(-amount.X, amount.X);
            float y = (float)RandomGlobals.Randomizer.RandomDouble(-amount.Y, amount.Y);
            return new Vector2(x, y);
        }

        /// <summary>
        /// Gets the size for dynamic text.
        /// </summary>
        /// <param name="timeOffset">The time offset. This is often the difference between when the character was printed and the current time.</param>
        /// <param name="dynamicScale">The scale of the dynamic modifier.</param>
        /// <param name="endingScale">The ending scale of the text.</param>
        /// <returns>A Vector2 containing hte size for dynamic text.</returns>
        public static Vector2 GetDynamicTextSize(in double timeOffset, in Vector2 dynamicScale, in Vector2 endingScale)
        {
            Vector2 scale = Interpolation.Interpolate(dynamicScale, endingScale, timeOffset / DynamicScaleTime, Interpolation.InterpolationTypes.Linear);

            return scale;
        }

        #endregion

        #region Message Modifiers

        /// <summary>
        /// The string representing the clear tag.
        /// </summary>
        public const string ClearTag = "clear";

        /// <summary>
        /// Tells if the string is a clear tag.
        /// </summary>
        /// <param name="text">The string to test.</param>
        /// <returns>true if so, otherwise false.</returns>
        public static bool IsClearTag(in string text)
        {
            return (text == ClearTag);
        }

        /// <summary>
        /// Tells if the string is a valid message modifier.
        /// <para>Note that Paragraph tags are message modifiers in addition to message routines.</para>
        /// </summary>
        /// <param name="text">The string to test.</param>
        /// <returns>true if so, otherwise false.</returns>
        public static bool IsMessageModifier(in string text)
        {
            return (IsParagraphTag(text) == true || IsClearTag(text) == true);
        }

        #endregion

        #region Message Routines

        /// <summary>
        /// The string representing the key tag.
        /// </summary>
        public const string KeyTag = "key";

        /// <summary>
        /// A shortened version of the key tag that is functionally identical.
        /// </summary>
        public const string ShortKeyTag = "k";

        /// <summary>
        /// The string representing the wait tag.
        /// </summary>
        public const string WaitTag = "wait";

        /// <summary>
        /// The string representing the speed tag.
        /// </summary>
        public const string SpeedTag = "speed";

        /// <summary>
        /// Tells if the string is a key tag.
        /// </summary>
        /// <param name="text">The string to test.</param>
        /// <returns>true if so, otherwise false.</returns>
        public static bool IsKeyTag(in string text)
        {
            return (text == ShortKeyTag || text == KeyTag);
        }

        /// <summary>
        /// Tells if the string is a wait tag.
        /// </summary>
        /// <param name="text">The string to test.</param>
        /// <returns>true if so, otherwise false.</returns>
        public static bool IsWaitTag(in string text)
        {
            return (text == WaitTag);
        }

        /// <summary>
        /// Tells if the string is a speed tag.
        /// </summary>
        /// <param name="text">The string to test.</param>
        /// <returns>true if so, otherwise false.</returns>
        public static bool IsSpeedTag(in string text)
        {
            return (text == SpeedTag);
        }

        /// <summary>
        /// Tells if the string is a valid message routine.
        /// <para>Note that Paragraph tags are message modifiers in addition to message routines.</para>
        /// </summary>
        /// <param name="text">The string to test.</param>
        /// <returns>true if so, otherwise false.</returns>
        public static bool IsMessageRoutine(in string text)
        {
            return (IsParagraphTag(text) == true || IsKeyTag(text) == true || IsWaitTag(text) == true || IsSpeedTag(text) == true);
        }

        #endregion

        #region Classes

        /// <summary>
        /// Represents data for a Dialogue Bubble.
        /// </summary>
        public class BubbleData
        {
            public int MaxParagraphIndex = 0;
            public List<BubbleTextData> TextData = new List<BubbleTextData>();
            public Dictionary<int, List<HtmlNode>> MessageRoutines = new Dictionary<int, List<HtmlNode>>();

            public bool Clear = false;
        }

        /// <summary>
        /// Represents Text Modifier data for a Dialogue Bubble.
        /// </summary>
        public class BubbleTextData
        {
            public int ParagraphIndex = 0;
            public int NewLineCount = 0;
            public int StartIndex = 0;
            public int EndIndex = 0;
            public Color TextColor = Color.Black;
            public Vector2? DynamicSize = null;
            public bool Shake = false;
            public bool Wave = false;
            public Vector2 Scale = Vector2.One;
        }

        #endregion

        /// <summary>
        /// Removes Dialogue Bubble control codes from a string.
        /// </summary>
        /// <param name="text">The string to remove control codes from.</param>
        /// <returns>A string without Dialogue Bubble control codes.</returns>
        public static string RemoveControlCodesFromText(in string text)
        {
            //If the parsed text is not the same as the text passed in, load in the text as HTML
            if (HTMLDoc.ParsedText != text)
                HTMLDoc.LoadHtml(text);

            //Return the inner text
            return HTMLDoc.DocumentNode.InnerText;
        }

        #region Parsing Methods

        /// <summary>
        /// Parses text for a Dialogue Bubble, reading in control codes and converting them to text modifiers, message modifiers, and message routines.
        /// </summary>
        /// <param name="bubbleText">The string containing the text for the Dialogue Bubble.</param>
        /// <param name="nonHtmlOutput">A string that will be the <paramref name="bubbleText"/> returned without control codes.</param>
        /// <returns>A new BubbleData with all the necessary data for a Dialogue Bubble to function.</returns>
        public static BubbleData ParseText(in string bubbleText, out string nonHtmlOutput)
        {
            //Load in the text as HTML
            HTMLDoc.LoadHtml(bubbleText);
            
            //Initialize data that we need for parsing
            BubbleData bubbleData = new BubbleData();
            BubbleTextData curBubbleTextData = new BubbleTextData();
            List<HtmlNode> activeModifiers = new List<HtmlNode>();
            int prevNewLineCount = 0;
            int curNewLineCount = 0;

            //Visit the first node, which is the root of the HTML document
            VisitNode(HTMLDoc.DocumentNode, HTMLDoc.DocumentNode, ref curBubbleTextData, activeModifiers, bubbleData, ref prevNewLineCount,
                ref curNewLineCount);

            //Set the output without control codes
            nonHtmlOutput = HTMLDoc.DocumentNode.InnerText;

            return bubbleData;
        }

        /// <summary>
        /// Visits an HtmlNode and handles it. This is part of Dialogue Bubble parsing.
        /// </summary>
        /// <param name="root">The root node; this should be the HtmlDocument's DocumentNode.</param>
        /// <param name="node">The current node being visited.</param>
        /// <param name="curBubbleTextData">The current BubbleTextData.</param>
        /// <param name="activeModifiers">The currently active text modifiers.</param>
        /// <param name="bubbleData">The BubbleData.</param>
        /// <param name="prevNewLineCount">The new line count up until this paragraph.</param>
        /// <param name="curNewLineCount">The current number of new lines in this paragraph.</param>
        private static void VisitNode(HtmlNode root, HtmlNode node, ref BubbleTextData curBubbleTextData, List<HtmlNode> activeModifiers,
            BubbleData bubbleData, ref int prevNewLineCount, ref int curNewLineCount)
        {
            if (node == null) return;

            //Check node; if not ignorable, do logic
            if (IsIgnorableNode(node.Name) == false)
            {
                //If it's not a text node, check what it is 
                if (IsTextNode(node.Name) == false)
                {
                    //If it's a valid text modifier, add this node to the list
                    if (IsTextModifier(node.Name) == true)
                    {
                        activeModifiers.Add(node);
                    }
                    else
                    {
                        //It may be a message modifier, so handle it
                        if (IsMessageModifier(node.Name) == true)
                        {
                            HandleMessageModifier(node, bubbleData, ref prevNewLineCount, ref curNewLineCount);
                        }
                        
                        //It may be a message routine, so handle it
                        if (IsMessageRoutine(node.Name) == true)
                        {
                            //The start index of the routine is at the end of the previous bubble
                            int routineStartIndex = 0;
                            if (bubbleData.TextData.Count > 0)
                                routineStartIndex = bubbleData.TextData[bubbleData.TextData.Count - 1].EndIndex;
                            
                            //Add the key if it doesn't exist
                            if (bubbleData.MessageRoutines.ContainsKey(routineStartIndex) == false)
                            {
                                bubbleData.MessageRoutines.Add(routineStartIndex, new List<HtmlNode>());
                            }

                            //Add this routine
                            bubbleData.MessageRoutines[routineStartIndex].Add(node);
                        }
                    }
                }
                //It's a text node; see where it lies
                else
                {
                    //Start searching from the last index from the last bubble text
                    //This prevents same text from being found earlier, causing the start and end indices to be incorrect
                    //An example that this problem fixes is: "Hello World! \n e \ntest2\n<p>test"
                    //The earlier "test" from "test2" would have been found in the later "test" but no longer does since we start later
                    int searchIndex = 0;
                    if (bubbleData.TextData.Count > 0)
                    {
                        searchIndex = bubbleData.TextData[bubbleData.TextData.Count - 1].EndIndex;
                    }

                    //Set start and end indices
                    curBubbleTextData.StartIndex = root.InnerText.IndexOf(node.InnerText, searchIndex);
                    curBubbleTextData.EndIndex = curBubbleTextData.StartIndex + node.InnerText.Length;

                    //Debug.Log($"\"{node.InnerText}\" with start: {curBubbleTextData.StartIndex} and end: {curBubbleTextData.EndIndex}");

                    //Apply all attributes
                    for (int i = 0; i < activeModifiers.Count; i++)
                    {
                        HtmlNode mod = activeModifiers[i];

                        int startPos = mod.StreamPosition;
                        int endPos = mod.EndNode.StreamPosition;

                        //Check if the node is inside this modifier
                        if (node.StreamPosition > startPos && node.StreamPosition < endPos)
                        {
                            //It is, so apply effects
                            HandleModifierType(mod, curBubbleTextData);
                        }
                        //It's out of range; remove this mod
                        else
                        {
                            activeModifiers.RemoveAt(i);
                            i--;
                        }
                    }

                    //Track the current new line count
                    curNewLineCount += node.InnerText.Count(IsNewLineChar);

                    //Set the paragraph index and add the new line count from all previous paragraphs
                    curBubbleTextData.ParagraphIndex = bubbleData.MaxParagraphIndex;
                    curBubbleTextData.NewLineCount = prevNewLineCount;

                    //Add the bubble data and start a new one
                    bubbleData.TextData.Add(curBubbleTextData);
                    curBubbleTextData = new BubbleTextData();
                }
            }

            //Visit all children of this node
            if (node.HasChildNodes == true)
            {
                for (int i = 0; i < node.ChildNodes.Count; i++)
                {
                    VisitNode(root, node.ChildNodes[i], ref curBubbleTextData, activeModifiers, bubbleData, ref prevNewLineCount, ref curNewLineCount);
                }
            }
        }

        /// <summary>
        /// Handles text modifiers by parsing them into data.
        /// </summary>
        /// <param name="mod">An HtmlNode representing the text modifier.</param>
        /// <param name="curBubbleTextData">The current BubbleTextData.</param>
        private static void HandleModifierType(HtmlNode mod, BubbleTextData curBubbleTextData)
        {
            string modName = mod.Name;

            if (IsColorMod(modName) == true)
            {
                //Get color as hex
                string color = mod.Attributes[0].Value;

                uint result = uint.Parse(color, System.Globalization.NumberStyles.HexNumber);

                curBubbleTextData.TextColor = new Color(result);
            }
            else if (IsDynamicMod(modName) == true)
            {
                //Parse the result as a float
                if (float.TryParse(mod.Attributes[0].Value, out float result) == true)
                {
                    curBubbleTextData.DynamicSize = new Vector2(result);
                }
            }
            else if (IsShakeMod(modName) == true) curBubbleTextData.Shake = true;
            else if (IsWaveMod(modName) == true) curBubbleTextData.Wave = true;
            else if (IsScaleMod(modName) == true)
            {
                //Parse the scale result as a float
                if (float.TryParse(mod.Attributes[0].Value, out float result) == true)
                {
                    curBubbleTextData.Scale = new Vector2(result);
                }
            }
        }

        /// <summary>
        /// Handles message modifiers by parsing them into data.
        /// </summary>
        /// <param name="msgMod">An HtmlNode representing the message modifier.</param>
        /// <param name="bubbleData">The BubbleData.</param>
        /// <param name="prevNewLineCount">The new line count up until this paragraph.</param>
        /// <param name="curNewLineCount">The current number of new lines in this paragraph.</param>
        private static void HandleMessageModifier(HtmlNode msgMod, BubbleData bubbleData, ref int prevNewLineCount, ref int curNewLineCount)
        {
            //If it's paragraph tag, increment the current paragraph index
            if (IsParagraphTag(msgMod.Name) == true)
            {
                bubbleData.MaxParagraphIndex++;

                //Add the current newline count and reset it
                prevNewLineCount += curNewLineCount;
                curNewLineCount = 0;
            }

            //If it's a clear tag, mark to not render the bubble itself
            if (IsClearTag(msgMod.Name) == true)
            {
                bubbleData.Clear = true;
            }
        }

        /// <summary>
        /// Tells if a character is a newline.
        /// </summary>
        /// <param name="c">The character to test.</param>
        /// <returns>true if the character is the newline character, '\n', otherwise false.</returns>
        private static bool IsNewLineChar(char c)
        {
            return (c == '\n');
        }

        #endregion
    }
}
