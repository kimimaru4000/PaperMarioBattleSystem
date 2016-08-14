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

        public BattlePartner(Stats stats) : base(stats)
        {
            Name = "Partner";

            EntityType = Enumerations.EntityTypes.Player;
            PlayerType = Enumerations.PlayerTypes.Partner;
        }

        /// <summary>
        /// Swaps Badges from one Partner to another new Partner to apply the effects to that Partner
        /// </summary>
        /// <param name="partnerEquipped">The Partner currently equipped with the Badges</param>
        /// <param name="newPartner">The Partner to equip the Badges to</param>
        public static void SwapPartnerBadges(BattlePartner partnerEquipped, BattlePartner newPartner)
        {
            List<Badge> partnerBadges = Inventory.Instance.GetActivePartnerBadges();

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
