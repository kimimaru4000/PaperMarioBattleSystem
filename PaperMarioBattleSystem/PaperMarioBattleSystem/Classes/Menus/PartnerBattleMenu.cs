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

            ActionButtons.Add(new ActionButton("Abilities", AssetManager.Instance.LoadRawTexture2D("UI/Battle/JumpButton.png"),
                Enumerations.MoveCategories.Partner, PartnerSubMenu));

            //Add Focus to the Partner battle menu if the Group Focus badge is equipped
            int groupFocusCount = BattleManager.Instance.EntityTurn.GetEquippedBadgeCount(BadgeGlobals.BadgeTypes.GroupFocus);
            if (groupFocusCount > 0)
            {
                ActionSubMenu focusMenu = new ActionSubMenu(new Focus());
                focusMenu.AutoSelectSingle = true;
                ActionButtons.Add(new ActionButton("Focus", AssetManager.Instance.LoadRawTexture2D("UI/Battle/JumpButton.png"),
                    Enumerations.MoveCategories.Special, focusMenu));
            }

            Initialize(2);
        }
    }
}
