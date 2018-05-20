using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace PaperMarioBattleSystem
{
    public sealed class TidalWaveActionCommandUI : ActionCommandUI<TidalWaveCommand>
    {
        public TidalWaveActionCommandUI(TidalWaveCommand tidalWaveCmd) : base(tidalWaveCmd)
        {

        }

        public override void Draw()
        {
            if (ActionCmd?.AcceptingInput == false) return;

            Vector2 startDrawLoc = new Vector2(250, 150);
            int xPosDiff = 20;
            Vector2 nextPos = startDrawLoc + new Vector2(xPosDiff * ActionCmd.ButtonsPressed.Count, 0);

            //Show the buttons pressed
            for (int i = 0; i < ActionCmd.ButtonsPressed.Count; i++)
            {
                Keys button = ActionCmd.ButtonsPressed[i];
                SpriteRenderer.Instance.DrawUIText(AssetManager.Instance.TTYDFont, button.ToString(), startDrawLoc + new Vector2(xPosDiff * i, 0), Color.Black, .7f);
            }

            //Show the button that should be pressed next, unless the input limit was reached
            if (ActionCmd.ButtonsPressed.Count < ActionCmd.InputLimit)
            {
                SpriteRenderer.Instance.DrawUIText(AssetManager.Instance.TTYDFont, ActionCmd.NextButtonToPress.ToString(), nextPos, Color.White, .7f);
            }

            SpriteRenderer.Instance.DrawUIText(AssetManager.Instance.TTYDFont, Math.Round(ActionCmd.StartTime - Time.ActiveMilliseconds, 2).ToString(), startDrawLoc + new Vector2(0, -20), Color.White, .7f);
        }
    }
}
