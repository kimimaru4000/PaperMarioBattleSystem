using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// Handles dialogue with dialogue bubbles.
    /// <para>This is a Singleton</para>
    /// </summary>
    public sealed class DialogueManager : ICleanup, IUpdateable
    {
        #region Singleton Fields

        public static DialogueManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new DialogueManager();
                }

                return instance;
            }
        }

        public static bool HasInstance => (instance != null);

        private static DialogueManager instance = null;

        #endregion

        private DialogueBubble DialogBubble = null;

        public DialogueBubble CurDialogueBubble => DialogBubble;

        private DialogueManager()
        {

        }

        public void CleanUp()
        {
            DialogBubble?.CleanUp();
        }

        ///// <summary>
        ///// Creates a Dialogue Bubble from an array of text.
        ///// This is method is only for compatibility with current Tattles, which use an array of strings.
        ///// </summary>
        ///// <param name="textArray">The array of text.</param>
        //public void CreateBubble(string[] textArray, BattleEntity speaker)
        //{
        //    string text = string.Empty;
        //
        //    for (int i = 0; i < textArray.Length; i++)
        //    {
        //        text += textArray[i] + "\n<k>";
        //
        //        //Add the paragraph tag before the 
        //        if (i != (textArray.Length - 1))
        //        {
        //            text += "<p>";
        //        }
        //    }
        //
        //    CreateBubble(text, speaker);
        //}

        /// <summary>
        /// Creates a Dialogue Bubble with text.
        /// If a Dialogue Bubble already exists, the current one will be reset.
        /// </summary>
        /// <param name="text">A string containing the text for the Dialogue Bubble to parse and print.</param>
        public void CreateBubble(string text)
        {
            CreateBubble(text, null);
        }

        /// <summary>
        /// Creates a Dialogue Bubble with text.
        /// If a Dialogue Bubble already exists, the current one will be reset.
        /// </summary>
        /// <param name="text">A string containing the text for the Dialogue Bubble to parse and print.</param>
        /// <param name="speaker">An optional BattleEntity that is set as the designated speaker of the Dialogue Bubble.</param>
        public void CreateBubble(string text, BattleEntity speaker)
        {
            CreateBubble(text, AssetManager.Instance.TTYDFont, speaker);
        }

        /// <summary>
        /// Creates a Dialogue Bubble with text.
        /// If a Dialogue Bubble already exists, the current one will be reset.
        /// </summary>
        /// <param name="text">A string containing the text for the Dialogue Bubble to parse and print.</param>
        /// <param name="spriteFont">The SpriteFont for the Dialogue Bubble to use when printing text.</param>
        /// <param name="speaker">An optional BattleEntity that is set as the designated speaker of the Dialogue Bubble.</param>
        public void CreateBubble(string text, SpriteFont spriteFont, BattleEntity speaker)
        {
            if (DialogBubble == null)
                DialogBubble = new DialogueBubble();

            DialogBubble.Reset();
            DialogBubble.SetFont(spriteFont);
            DialogBubble.SetText(text);
            DialogBubble.AttachSpeaker(speaker);
        }

        public void Update()
        {
            if (DialogBubble != null)
            {
                DialogBubble.Update();
                if (DialogBubble.IsDone == true)
                {
                    DialogBubble.CleanUp();
                    DialogBubble = null;
                }
            }
        }
    }
}
