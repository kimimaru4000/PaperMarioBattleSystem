using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using HtmlAgilityPack;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// Global values relating to dialogue.
    /// </summary>
    public static class DialogueGlobals
    {
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
        /// <param name="timeOffset">The time offset. This is usually the time between rendering each character in a dialogue bubble.</param>
        /// <param name="amount">The max amount to offset.</param>
        /// <returns>A Vector2 containing the offset for wavy text.</returns>
        public static Vector2 GetWavyTextOffset(double timeOffset, Vector2 amount)
        {
            //Wavy text goes clockwise
            double time = (Time.ActiveMilliseconds + timeOffset) / 75d;
            return new Vector2((float)Math.Cos(time) * amount.X, (float)Math.Sin(time) * amount.Y);
        }

        /// <summary>
        /// Gets the positional offset for shaky text.
        /// </summary>
        /// <param name="amount">The max amount to offset.</param>
        /// <returns>A Vector2 containing the offset for wavy text.</returns>
        public static Vector2 GetShakyTextOffset(Vector2 amount)
        {
            float x = (float)GeneralGlobals.Randomizer.RandomDouble(-amount.X, amount.X);
            float y = (float)GeneralGlobals.Randomizer.RandomDouble(-amount.Y, amount.Y);
            return new Vector2(x, y);
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
        public const string WaitKeyTag = "wait";

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
            return (text == WaitKeyTag);
        }

        /// <summary>
        /// Tells if the string is a valid message routine.
        /// <para>Note that Paragraph tags are message modifiers in addition to message routines.</para>
        /// </summary>
        /// <param name="text">The string to test.</param>
        /// <returns>true if so, otherwise false.</returns>
        public static bool IsMessageRoutine(in string text)
        {
            return (IsParagraphTag(text) == true || IsKeyTag(text) == true || IsWaitTag(text) == true);
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
            public float DynamicSize = 1f;
            public bool Shake = false;
            public bool Wave = false;
            public Vector2 Scale = Vector2.One;
        }

        #endregion

        #region Parsing Methods

        public static BubbleData ParseText(in string bubbleText, out string nonHtmlOutput)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(bubbleText);

            BubbleData bubbleData = new BubbleData();
            BubbleTextData curBubbleTextData = new BubbleTextData();
            List<HtmlNode> activeModifiers = new List<HtmlNode>();
            int prevNewLineCount = 0;
            int curNewLineCount = 0;

            VisitNode(doc.DocumentNode, doc.DocumentNode, ref curBubbleTextData, activeModifiers, bubbleData, ref prevNewLineCount,
                ref curNewLineCount);

            nonHtmlOutput = doc.DocumentNode.InnerText;

            return bubbleData;
        }

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
                    curBubbleTextData.DynamicSize = result;
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
