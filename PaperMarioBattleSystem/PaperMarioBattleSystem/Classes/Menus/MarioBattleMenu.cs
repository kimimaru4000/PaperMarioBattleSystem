using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// Mario's battle menu. It contains Strategies, Items, Jump, Hammer, and Star Spirits
    /// </summary>
    public sealed class MarioBattleMenu : PlayerBattleMenu
    {
        public MarioBattleMenu() : base(Enumerations.PlayerTypes.Mario)
        {
            Texture2D tex = AssetManager.Instance.LoadRawTexture2D("UI/Battle/JumpButton.png");

            ActionButtons.Add(new ActionButton("Jump", tex,
                Enumerations.MoveCategories.Jump, new JumpSubMenu()));
            ActionButtons.Add(new ActionButton("Hammer", tex,
                Enumerations.MoveCategories.Hammer, new HammerSubMenu()));
            ActionButtons.Add(new ActionButton("Special", tex,
                Enumerations.MoveCategories.Special, new SpecialSubMenu()));

            Initialize(2);
        }
    }
}
