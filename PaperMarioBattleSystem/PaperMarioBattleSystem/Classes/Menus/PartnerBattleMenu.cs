using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PaperMarioBattleSystem.Extensions;
using static PaperMarioBattleSystem.Enumerations;

namespace PaperMarioBattleSystem
{
    public class PartnerBattleMenu : PlayerBattleMenu
    {
        protected ActionSubMenu PartnerSubMenu { get; private set; } = null;
        protected PartnerTypes PartnerType { get; private set; } = PartnerTypes.None;

        public PartnerBattleMenu(BattleEntity user, ActionSubMenu partnerSubMenu, PartnerTypes partnerType) : base(user)
        {
            PartnerSubMenu = partnerSubMenu;
            PartnerType = partnerType;

            Texture2D abilityTex = AssetManager.Instance.LoadRawTexture2D($"{ContentGlobals.BattleGFX}.png");

            CroppedTexture2D partnerIcon = new CroppedTexture2D(abilityTex,
                new Rectangle(30 + (((int)PartnerType - 1) * 32), 886, 32, 32));

            ActionButtons.Add(new ActionButton("Abilities", partnerIcon, MoveCategories.Partner, PartnerSubMenu));

            //Add Focus to the Partner battle menu if the Group Focus badge is equipped
            int groupFocusCount = User.GetEquippedBadgeCount(BadgeGlobals.BadgeTypes.GroupFocus);
            if (groupFocusCount > 0)
            {
                ActionSubMenu focusMenu = new ActionSubMenu(user, "Focus", new FocusAction(user));
                focusMenu.AutoSelectSingle = true;

                CroppedTexture2D starPower = new CroppedTexture2D(abilityTex, new Rectangle(182, 812, 24, 24));
                ActionButtons.Add(new ActionButton("Focus", starPower, MoveCategories.Special, focusMenu));
            }

            Initialize(2);
        }
    }
}
