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
        /// Tells if the string is a valid text modifier.
        /// </summary>
        /// <param name="text">The string to test.</param>
        /// <returns>true if so, otherwise false.</returns>
        public static bool IsValidModifier(in string text)
        {
            return (IsColorMod(text) == true || IsDynamicMod(text) == true || IsShakeMod(text) == true || IsWaveMod(text) == true);
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

        #region Classes

        /// <summary>
        /// Represents Text Modifier data for a Dialogue Bubble.
        /// </summary>
        public class BubbleTextData
        {
            public int StartIndex = 0;
            public int EndIndex = 0;
            public Color TextColor = Color.Black;
            public float DynamicSize = 1f;
            public bool Shake = false;
            public bool Wave = false;
        }

        #endregion

        #region Parsing Methods

        public static List<BubbleTextData> ParseText(in string bubbleText, out string nonHtmlOutput)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(bubbleText);

            BubbleTextData curBubbleData = new BubbleTextData();
            List<BubbleTextData> bubbleData = new List<BubbleTextData>();
            List<HtmlNode> activeModifiers = new List<HtmlNode>();

            VisitNode(doc.DocumentNode, doc.DocumentNode, ref curBubbleData, activeModifiers, bubbleData);

            nonHtmlOutput = doc.DocumentNode.InnerText;

            return bubbleData;
        }

        private static void VisitNode(HtmlNode root, HtmlNode node, ref BubbleTextData curBubbleData, List<HtmlNode> activeModifiers,
            List<BubbleTextData> bubbleData)
        {
            if (node == null) return;

            //Check node; if not ignorable, do logic
            if (IsIgnorableNode(node.Name) == false)
            {
                //If it's not a text node, check what it is 
                if (IsTextNode(node.Name) == false)
                {
                    //If it's a valid modifier, add this node to the list
                    //Start up a new bubble data too
                    if (IsValidModifier(node.Name) == true)
                    {
                        activeModifiers.Add(node);
                    }
                }
                //It's a text node; see where it lies
                else
                {
                    //Set start and end indices
                    curBubbleData.StartIndex = root.InnerText.IndexOf(node.InnerText);
                    curBubbleData.EndIndex = curBubbleData.StartIndex + node.InnerText.Length;

                    //Apply all attributes
                    for (int i = 0; i < activeModifiers.Count; i++)
                    {
                        HtmlNode mod = activeModifiers[i];

                        int startPos = mod.StreamPosition;
                        int endPos = mod.EndNode.StreamPosition;

                        //Check if the node is inside this modifier
                        if (node.StreamPosition > startPos && node.StreamPosition < endPos)
                        {
                            string name = mod.Name;

                            //It is, so apply effects
                            if (IsColorMod(name) == true)
                            {
                                //Get color as hex
                                string color = mod.Attributes[0].Value;

                                uint result = uint.Parse(color, System.Globalization.NumberStyles.HexNumber);

                                curBubbleData.TextColor = new Color(result);
                            }
                            else if (IsDynamicMod(name) == true)
                            {
                                //Parse the result as a float
                                if (float.TryParse(mod.Attributes[0].Value, out float result) == true)
                                {
                                    curBubbleData.DynamicSize = result;
                                }
                            }
                            else if (IsShakeMod(name) == true) curBubbleData.Shake = true;
                            else if (IsWaveMod(name) == true) curBubbleData.Wave = true;
                        }
                        //It's out of range; remove this mod
                        else
                        {
                            activeModifiers.RemoveAt(i);
                            i--;
                        }
                    }

                    //Add the bubble data and start a new one
                    bubbleData.Add(curBubbleData);
                    curBubbleData = new BubbleTextData();
                }
            }

            //Visit all children of this node
            if (node.HasChildNodes == true)
            {
                for (int i = 0; i < node.ChildNodes.Count; i++)
                {
                    VisitNode(root, node.ChildNodes[i], ref curBubbleData, activeModifiers, bubbleData);
                }
            }
        }

        #endregion
    }
}
