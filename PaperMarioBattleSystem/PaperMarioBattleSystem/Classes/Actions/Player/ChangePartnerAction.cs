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
    /// The ChangePartner MoveAction.
    /// </summary>
    public sealed class ChangePartnerAction : MoveAction
    {
        /// <summary>
        /// The new Partner to switch to.
        /// </summary>
        private BattlePartner NewPartner = null;

        public ChangePartnerAction(BattleEntity user, BattlePartner newPartner) : base(user)
        {
            NewPartner = newPartner;

            Name = NewPartner.Name;

            CroppedTexture2D partnerIcon = null;

            //Icons exist only for any regular partners
            //Don't display anything for temporary or unused partners
            if (NewPartner.PartnerType <= Enumerations.PartnerTypes.MsMowz)
            {
                Rectangle sourceRect = new Rectangle(30 + (((int)NewPartner.PartnerType - 1) * 32), 886, 32, 32);

                //Pretty hackish for now; show the disabled icon instead if the Partner is the current one out
                if (NewPartner == User.BManager.Partner)
                {
                    sourceRect.Y += 32;
                }

                partnerIcon = new CroppedTexture2D(AssetManager.Instance.LoadRawTexture2D($"{ContentGlobals.UIRoot}/Battle/BattleGFX.png"),
                    sourceRect);
            }

            MoveInfo = new MoveActionData(partnerIcon, NewPartner.PartnerDescription, Enumerations.MoveResourceTypes.FP, 0,
                Enumerations.CostDisplayTypes.Hidden, Enumerations.MoveAffectionTypes.None, TargetSelectionMenu.EntitySelectionType.Single,
                false, null);

            SetMoveSequence(new ChangePartnerSequence(this, NewPartner));
        }

        public override void OnMenuSelected()
        {
            //Check if the User has Quick Change equipped.
            //This is done here in the menu because if Confusion causes a Player to swap Partners,
            //it's not affected by Quick Change and thus will cause the Player to use a turn whether it's equipped or not
            int quickChangeCount = User.GetPartyEquippedBadgeCount(BadgeGlobals.BadgeTypes.QuickChange);
            
            //If Quick Change is equipped, don't use up a turn
            if (quickChangeCount > 0)
            {
                User.SetTurnsUsed(User.TurnsUsed - 1);
            }

            //Changing Partners doesn't require target selection, so go straight into the sequence
            base.OnMenuSelected();
        }
    }
}
