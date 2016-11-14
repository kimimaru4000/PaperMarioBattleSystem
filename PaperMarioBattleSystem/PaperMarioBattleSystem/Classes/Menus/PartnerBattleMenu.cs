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
    public class PartnerBattleMenu : PlayerBattleMenu
    {
        protected ActionSubMenu PartnerSubMenu { get; private set; } = null;

        public PartnerBattleMenu(ActionSubMenu partnerSubMenu) : base(Enumerations.PlayerTypes.Partner)
        {
            PartnerSubMenu = partnerSubMenu;

            ActionButtons.Add(new ActionButton("Abilities", AssetManager.Instance.LoadAsset<Texture2D>("UI/Battle/JumpButton"),
                new Vector2(-170, 50), PartnerSubMenu));
        }
    }
}
