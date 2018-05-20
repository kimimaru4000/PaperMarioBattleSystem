using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace PaperMarioBattleSystem
{
    public class MashButtonRangeActionCommandUI : FillBarActionCommandUI<MashButtonRangeCommand>
    {
        public MashButtonRangeActionCommandUI(MashButtonRangeCommand mashButtonRangeCmd) : base(mashButtonRangeCmd, new Vector2(250, 150), new Vector2(100f, 1f), null)
        {

        }

        public override void Update()
        {
            base.Update();
            BarFillColor = Color.White;
        }

        public override void Draw()
        {
            base.Draw();
            SpriteRenderer.Instance.DrawUIText(AssetManager.Instance.TTYDFont, ActionCmd.CurrentValue.ToString(), new Vector2(355, 150), Color.White, .71f);
        }
    }
}
