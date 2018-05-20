using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace PaperMarioBattleSystem
{
    public class RallyWinkActionCommandUI : FillBarActionCommandUI<RallyWinkCommand>
    {
        public RallyWinkActionCommandUI(RallyWinkCommand rallyWink) : base(rallyWink, new Vector2(250, 200), new Vector2(200, 1), null)
        {

        }

        public override void Draw()
        {
            if (ActionCmd?.AcceptingInput == false) return;

            base.Draw();

            //Show the buttons you're supposed to press, with the current button highlighted
            for (int i = 0; i < ActionCmd.ButtonsToPress.Length; i++)
            {
                Keys button = ActionCmd.ButtonsToPress[i];
                Color color = Color.Black;
                if (button == ActionCmd.CurButton) color = Color.White;

                SpriteRenderer.Instance.DrawUIText(AssetManager.Instance.TTYDFont, button.ToString(), StartPos + new Vector2(i * 15, -30f), color, .5f);
            }
        }
    }
}
