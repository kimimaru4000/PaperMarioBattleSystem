using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace PaperMarioBattleSystem
{
    public sealed class TornadoJumpActionCommandUI : JumpActionCommandUI<TornadoJumpCommand>
    {
        public TornadoJumpActionCommandUI(TornadoJumpCommand tornadoJumpCommand) : base(tornadoJumpCommand)
        {

        }

        public override void Draw()
        {
            if (ActionCmd?.AcceptingInput == false) return;

            if (ActionCmd.Step == TornadoJumpCommand.Steps.Jump)
            {
                base.Draw();
            }
            else
            {
                Vector2 startDrawLoc = new Vector2(250, 150);
                int xPosDiff = 20;

                //Show the buttons to press; if the button was pressed, show it black
                for (int i = 0; i < ActionCmd.ButtonsToPress.Length; i++)
                {
                    Keys button = ActionCmd.ButtonsToPress[i];
                    Color buttonColor = Color.White;
                    if (ActionCmd.ButtonIndex > i) buttonColor = Color.Black;

                    SpriteRenderer.Instance.DrawUIText(AssetManager.Instance.TTYDFont, button.ToString(), startDrawLoc + new Vector2(xPosDiff * i, 0), buttonColor, .7f);
                }

                SpriteRenderer.Instance.DrawUIText(AssetManager.Instance.TTYDFont, Math.Round(ActionCmd.TornadoEndTime - Time.ActiveMilliseconds, 2).ToString(), startDrawLoc + new Vector2(0, -20), Color.White, .7f);
            }
        }
    }
}
