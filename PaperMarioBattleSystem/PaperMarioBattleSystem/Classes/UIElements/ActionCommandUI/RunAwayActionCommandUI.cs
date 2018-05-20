using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PaperMarioBattleSystem
{
    public sealed class RunAwayActionCommandUI : FillBarActionCommandUI<RunAwayCommand>
    {
        private CroppedTexture2D MovingCursor = null;

        public RunAwayActionCommandUI(RunAwayCommand runAwayCmd) : base(runAwayCmd, new Vector2(250, 150), new Vector2(100f, 1f), null)
        {
            Texture2D battleGFX = AssetManager.Instance.LoadRawTexture2D($"{ContentGlobals.BattleGFX}.png");

            MovingCursor = new CroppedTexture2D(battleGFX, new Rectangle(498, 304, 46, 38));
        }

        public override void Draw()
        {
            base.Draw();

            //Draw the cursor
            //Regardless of MaxBarValue, needs to be rendered within the range
            float barValScaleFactor = BarSize.X / (float)ActionCmd.MaxBarValue;
            SpriteRenderer.Instance.DrawUI(MovingCursor.Tex, StartPos + new Vector2((float)ActionCmd.CurCursorVal * barValScaleFactor, 0f), MovingCursor.SourceRect, Color.White, 0f, new Vector2(.5f, 1f), Vector2.One, false, false, .4f);
        }
    }
}
