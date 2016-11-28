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
            ActionButtons.Add(new ActionButton("Jump", AssetManager.Instance.LoadAsset<Texture2D>("UI/Battle/JumpButton"),
                Enumerations.MoveCategories.Jump, new JumpSubMenu()));
            ActionButtons.Add(new ActionButton("Hammer", AssetManager.Instance.LoadAsset<Texture2D>("UI/Battle/JumpButton"),
                Enumerations.MoveCategories.Hammer, new HammerSubMenu()));
            
            Initialize(2);
        }
    }
}
