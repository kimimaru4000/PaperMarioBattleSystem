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

            Texture2D abilityTex = AssetManager.Instance.LoadRawTexture2D($"{ContentGlobals.UIRoot}/Battle/JumpButton.png");

            ActionButtons.Add(new ActionButton("Abilities", new CroppedTexture2D(abilityTex, null),
                Enumerations.MoveCategories.Partner, PartnerSubMenu));

            //Add Focus to the Partner battle menu if the Group Focus badge is equipped
            int groupFocusCount = BattleManager.Instance.EntityTurn.GetEquippedBadgeCount(BadgeGlobals.BadgeTypes.GroupFocus);
            if (groupFocusCount > 0)
            {
                ActionSubMenu focusMenu = new ActionSubMenu(new Focus());
                focusMenu.AutoSelectSingle = true;

                CroppedTexture2D starPower = new CroppedTexture2D(AssetManager.Instance.LoadRawTexture2D($"{ContentGlobals.UIRoot}/Battle/BattleGFX.png"), new Rectangle(182, 812, 24, 24));
                ActionButtons.Add(new ActionButton("Focus", starPower,
                    Enumerations.MoveCategories.Special, focusMenu));
            }

            Initialize(2);
        }
    }
}
