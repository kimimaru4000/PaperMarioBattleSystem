using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The UI for Jump's Action Command.
    /// </summary>
    public class JumpActionCommandUI<T> : ActionCommandUI<T> where T : JumpCommand
    {
        public JumpActionCommandUI(T jumpCommand) : base(jumpCommand)
        {
            
        }

        public override void Draw()
        {
            if (ActionCmd.AcceptingInput == false) return;

            string text = "NO!";
            Color color = Color.Red;
            if (ActionCmd.WithinRange == true)
            {
                text = "OKAY!";
                color = Color.Green;
            }

            SpriteRenderer.Instance.DrawUIText(AssetManager.Instance.TTYDFont, text, new Vector2(300, 150), color, .7f);
        }
    }
}
