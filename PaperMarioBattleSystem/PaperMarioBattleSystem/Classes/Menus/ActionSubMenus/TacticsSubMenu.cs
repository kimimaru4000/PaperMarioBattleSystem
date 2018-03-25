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
    /// The various miscellaneous actions available to Mario and his Partner.
    /// </summary>
    public sealed class TacticsSubMenu : ActionSubMenu
    {
        public TacticsSubMenu()
        {
            Name = "Tactics";
            Position = new Vector2(230, 150);

            BattleEntity entity = BattleManager.Instance.EntityTurn;

            //int quickChangeCount = entity.GetEquippedBadgeCount(BadgeGlobals.BadgeTypes.QuickChange);
            //Enumerations.CostDisplayTypes costDisplayType = Enumerations.CostDisplayTypes.Shown;
            //if (quickChangeCount > 0) costDisplayType = Enumerations.CostDisplayTypes.Special;

            Texture2D battleTex = AssetManager.Instance.LoadRawTexture2D($"{ContentGlobals.BattleGFX}.png");

            //If no Partner is available (removed from battle or don't have one) then don't add the change partner action
            //Additionally, if only one partner is avaiable then don't add it either
            //This mimics TTYD behavior if you either have no Partner or if it's removed from battle via Gale Force or Fright
            bool addChangePartner = (BattleManager.Instance.GetPartner() != null && Inventory.Instance.partnerInventory.GetPartnerCount() > 1);
            if (addChangePartner == true)
            {
                Rectangle sourceRect = new Rectangle(30 + (((int)BattleManager.Instance.GetPartner().PartnerType - 1) * 32), 886, 32, 32);

                BattleActions.Add(new MenuAction("Change Partner", new CroppedTexture2D(battleTex, sourceRect),
                    "Change your current partner.", //costDisplayType, 
                    new ChangePartnerSubMenu()));
            }

            #region Charge Menu

            //Charge action if the Charge or Charge P Badge is equipped
            int chargeCount = entity.GetEquippedNPBadgeCount(BadgeGlobals.BadgeTypes.Charge);
            if (chargeCount > 0)
            {
                //Charge starts out at 2 then increases by 1 for each additional Badge
                int chargeAmount = 2 + (chargeCount - 1);

                BattleActions.Add(new MoveAction("Charge", new MoveActionData(new CroppedTexture2D(battleTex, new Rectangle(623, 807, 40, 40)),
                    "Save up strength to power up\nyour next attack",
                    Enumerations.MoveResourceTypes.FP, chargeCount, Enumerations.CostDisplayTypes.Shown, Enumerations.MoveAffectionTypes.None,
                    TargetSelectionMenu.EntitySelectionType.Single, false, null), new ChargeSequence(null, chargeAmount)));
            }

            #endregion

            //Defend action
            BattleActions.Add(new Defend());

            //Do nothing action
            BattleActions.Add(new NoAction());

            //Run away action
            BattleActions.Add(new RunAwayAction());
        }
    }
}
