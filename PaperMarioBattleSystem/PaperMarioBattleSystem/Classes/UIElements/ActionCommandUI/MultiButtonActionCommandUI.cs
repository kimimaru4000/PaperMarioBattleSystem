using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace PaperMarioBattleSystem
{
    public class MultiButtonActionCommandUI : ActionCommandUI<MultiButtonCommand>
    {
        private double ElapsedTime = 0d;

        public MultiButtonActionCommandUI(MultiButtonCommand multiButtonCommand) : base(multiButtonCommand)
        {

        }

        public override void Update()
        {
            ElapsedTime += Time.ElapsedMilliseconds;
        }

        public override void Draw()
        {
            if (ActionCmd.AcceptingInput == false) return;

            Vector2 startDrawLoc = new Vector2(250, 150);
            int xPosDiff = 20;

            //Show the buttons pressed
            for (int i = 0; i < ActionCmd.ButtonSequence.Length; i++)
            {
                Keys button = ActionCmd.ButtonSequence[i];
                Color color = Color.White;
                //If the button was pressed already, show it black
                if (i < ActionCmd.CurButtonIndex)
                    color = Color.Black;

                SpriteRenderer.Instance.DrawUIText(AssetManager.Instance.TTYDFont, button.ToString(), startDrawLoc + new Vector2(xPosDiff * i, 0), color, .7f);
            }

            //Show time remaining
            SpriteRenderer.Instance.DrawUIText(AssetManager.Instance.TTYDFont, Math.Round(ActionCmd.InputDuration - ElapsedTime, 2).ToString(), startDrawLoc + new Vector2(0, -20), Color.White, .7f);
        }
    }
}
