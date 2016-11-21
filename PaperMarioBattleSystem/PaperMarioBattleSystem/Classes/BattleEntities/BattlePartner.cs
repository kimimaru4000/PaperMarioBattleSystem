using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// Mario's partners in battle
    /// </summary>
    public abstract class BattlePartner : BattlePlayer
    {
        public Enumerations.PartnerTypes PartnerType { get; protected set; } = Enumerations.PartnerTypes.None;

        public string PartnerDescription { get; protected set; } = string.Empty;

        /// <summary>
        /// The number of max turns all Partners have this phase cycle. All Partners share turns.
        /// </summary>
        public static int PartnerMaxTurns { get; private set; } = 0;

        public BattlePartner(Stats stats) : base(stats)
        {
            Name = "Partner";

            EntityType = Enumerations.EntityTypes.Player;
            PlayerType = Enumerations.PlayerTypes.Partner;
        }

        public override void OnBattleStart()
        {
            base.OnBattleStart();

            Inventory.Instance.GetBadge(BadgeGlobals.BadgeTypes.ChargeP, BadgeGlobals.BadgeFilterType.UnEquipped)?.Equip(this);
        }

        public override void OnPhaseCycleStart()
        {
            base.OnPhaseCycleStart();

            //Set the number of max turns each Partner should have to the number of max turns this one does
            PartnerMaxTurns = MaxTurns;
        }

        /// <summary>
        /// Swaps Badges from one Partner to another new Partner to apply the effects to that Partner
        /// </summary>
        /// <param name="partnerEquipped">The Partner currently equipped with the Badges</param>
        /// <param name="newPartner">The Partner to equip the Badges to</param>
        public static void SwapPartnerBadges(BattlePartner partnerEquipped, BattlePartner newPartner)
        {
            List<Badge> partnerBadges = Inventory.Instance.GetActivePartnerBadges(true);

            //Go through all the Badges
            for (int i = 0; i < partnerBadges.Count; i++)
            {
                //Unequip them from the current Partner
                partnerBadges[i].UnEquip();

                //Equip them onto the new Partner
                partnerBadges[i].Equip(newPartner);
            }
        }
    }
}
