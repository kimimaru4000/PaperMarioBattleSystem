using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        /// <summary>
        /// Creates a Dialogue Bubble with a set of text.
        /// If a Dialogue Bubble already exists, the current one will be reset.
        /// </summary>
        /// <param name="textArray">An array of strings containing the text for the Dialogue Bubble to print.</param>
        public void CreateBubble(string[] textArray)
        {
            if (DialogBubble == null)
                DialogBubble = new DialogueBubble();

            DialogBubble.Reset();
            DialogBubble.SetText(textArray);
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
