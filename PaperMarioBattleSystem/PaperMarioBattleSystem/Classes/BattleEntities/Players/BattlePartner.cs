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

        public BattlePartner(PartnerStats stats) : base(stats)
        {
            Name = "Partner";

            EntityType = Enumerations.EntityTypes.Player;
            PlayerType = Enumerations.PlayerTypes.Partner;
        }

        /// <summary>
        /// Returns Mario's FP for BattlePartners, as they share the same FP pool for Mario.
        /// </summary>
        public override int CurFP => BattleManager.Instance.GetMario().BattleStats.FP;

        /// <summary>
        /// Partners and Mario add to Mario's FP pool.
        /// </summary>
        /// <param name="fp"></param>
        public override void HealFP(int fp)
        {
            BattleMario mario = BattleManager.Instance.GetMario();
            mario.BattleStats.FP = UtilityGlobals.Clamp(mario.BattleStats.FP + fp, 0, mario.BattleStats.MaxFP);
            Debug.Log($"{mario.Name} healed {fp} FP!");
        }

        /// <summary>
        /// Partners and Mario subtract from Mario's FP pool.
        /// </summary>
        public override void LoseFP(int fp)
        {
            BattleMario mario = BattleManager.Instance.GetMario();
            mario.BattleStats.FP = UtilityGlobals.Clamp(mario.BattleStats.FP - fp, 0, mario.BattleStats.MaxFP);
            Debug.Log($"{mario.Name} lost {fp} FP!");
        }

        public override void OnBattleStart()
        {
            base.OnBattleStart();

            Inventory.Instance.GetBadge(BadgeGlobals.BadgeTypes.ChargeP, BadgeGlobals.BadgeFilterType.UnEquipped)?.Equip(this);
            Inventory.Instance.GetBadge(BadgeGlobals.BadgeTypes.FlowerSaverP, BadgeGlobals.BadgeFilterType.UnEquipped)?.Equip(this);
            Inventory.Instance.GetBadge(BadgeGlobals.BadgeTypes.DoubleDipP, BadgeGlobals.BadgeFilterType.UnEquipped)?.Equip(this);
            Inventory.Instance.GetBadge(BadgeGlobals.BadgeTypes.DoubleDipP, BadgeGlobals.BadgeFilterType.UnEquipped)?.Equip(this);
            Inventory.Instance.GetBadge(BadgeGlobals.BadgeTypes.GroupFocus, BadgeGlobals.BadgeFilterType.UnEquipped)?.Equip(this);
            //Inventory.Instance.GetBadge(BadgeGlobals.BadgeTypes.FeelingFineP, BadgeGlobals.BadgeFilterType.UnEquipped)?.Equip(this);
        }

        public override void OnPhaseCycleStart()
        {
            base.OnPhaseCycleStart();

            //Set the number of max turns each Partner should have to the number of max turns this one does
            PartnerMaxTurns = MaxTurns;
        }

        public sealed override int GetEquippedBadgeCount(BadgeGlobals.BadgeTypes badgeType)
        {
            BadgeGlobals.BadgeTypes newBadgeType = badgeType;

            //Find the Partner version of the Badge
            BadgeGlobals.BadgeTypes? tempBadgeType = BadgeGlobals.GetPartnerBadgeType(badgeType);
            if (tempBadgeType != null)
            {
                newBadgeType = tempBadgeType.Value;
            }
            else
            {
                //If there is no Partner version, get the Badge and check if it affects Partners
                Badge badge = Inventory.Instance.GetBadge(newBadgeType, BadgeGlobals.BadgeFilterType.Equipped);
                //If the Badge isn't equipped or doesn't affect Both or the Partner, none are equipped to this Partner
                if (badge == null || badge.AffectedType == BadgeGlobals.AffectedTypes.Self) return 0;
            }

            return Inventory.Instance.GetActiveBadgeCount(newBadgeType);
        }

        public sealed override StarPowerBase GetStarPower(StarPowerGlobals.StarPowerTypes starPowerType)
        {
            return BattleManager.Instance.GetMario().MStats.GetStarPowerFromType(starPowerType);
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
