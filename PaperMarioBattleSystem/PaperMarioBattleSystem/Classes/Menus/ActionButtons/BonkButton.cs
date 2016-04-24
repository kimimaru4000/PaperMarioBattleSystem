using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// TEMPORARY; Goombario's Bonk action will be under the "Abilities" menu,
    /// which exists for all Partners, albeit with a different image for each partner
    /// </summary>
    public class BonkButton : ActionButton
    {
        public BonkButton() : base("Bonk")
        {
            ButtonImage = AssetManager.Instance.LoadAsset<Texture2D>("UI/Battle/JumpButton");
        }

        public override void OnSelected()
        {
            BattleUIManager.Instance.PushMenu(new BonkSubMenu());
        }

        public override void Draw(bool selected)
        {
            Color color = Color.White * .75f;
            if (selected == true) color = Color.White;

            SpriteRenderer.Instance.Draw(ButtonImage, Camera.Instance.SpriteToUIPos(new Vector2(-190, 50)), color, false, .4f, true);
            SpriteRenderer.Instance.DrawText(AssetManager.Instance.Font, Name, new Vector2(210, 320), color, .45f);
        }
    }
}
