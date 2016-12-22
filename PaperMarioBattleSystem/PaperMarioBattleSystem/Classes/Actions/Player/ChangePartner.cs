using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The ChangePartner MoveAction.
    /// </summary>
    public sealed class ChangePartner : MoveAction
    {
        /// <summary>
        /// The new Partner to switch to.
        /// </summary>
        private BattlePartner NewPartner = null;

        public ChangePartner(BattlePartner newPartner)
        {
            NewPartner = newPartner;

            Name = NewPartner.Name;

            MoveInfo = new MoveActionData(null, 0, NewPartner.PartnerDescription, TargetSelectionMenu.EntitySelectionType.Single,
                Enumerations.EntityTypes.Player, false, null);

            SetMoveSequence(new ChangePartnerSequence(this, NewPartner));
        }

        public override void OnMenuSelected()
        {
            //Check if the User is a Player and has Quick Change equipped.
            //This is done here in the menu because if Confusion causes a Player to swap Partners,
            //it's not affected by Quick Change and thus will cause the Player to use a turn whether it's equipped or not
            int quickChangeCount = User.GetEquippedBadgeCount(BadgeGlobals.BadgeTypes.QuickChange);
            
            //If Quick Change is equipped and Partners were swapped by a Player, don't use up a turn
            if (quickChangeCount > 0 && User.EntityType == Enumerations.EntityTypes.Player)
            {
                User.SetTurnsUsed(User.TurnsUsed - 1);
            }

            //Changing Partners doesn't require target selection, so go straight into the sequence
            base.OnMenuSelected();
        }
    }
}
