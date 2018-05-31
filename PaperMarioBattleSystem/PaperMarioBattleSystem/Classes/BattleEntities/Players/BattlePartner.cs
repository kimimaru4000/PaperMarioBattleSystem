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
        public override int CurFP => (IsInBattle == false || BManager.Mario == null) ? 0 : BManager.Mario.BattleStats.FP;

        /// <summary>
        /// Partners and Mario add to Mario's FP pool.
        /// </summary>
        /// <param name="fp"></param>
        public override void HealFP(int fp)
        {
            BattleMario mario = BManager.Mario;
            mario.HealFP(fp);
        }

        /// <summary>
        /// Partners and Mario subtract from Mario's FP pool.
        /// </summary>
        public override void LoseFP(int fp)
        {
            BattleMario mario = BManager.Mario;
            mario.LoseFP(fp);
        }

        public override void RaiseMaxFP(int fp)
        {
            BattleMario mario = BManager.Mario;
            mario.RaiseMaxFP(fp);
        }

        public override void LowerMaxFP(int fp)
        {
            BattleMario mario = BManager.Mario;
            mario.LowerMaxFP(fp);
        }

        public override void OnBattleStart()
        {
            this.ActivateAndEquipBadge(Inventory.Instance.GetBadge(BadgeGlobals.BadgeTypes.ChargeP, BadgeGlobals.BadgeFilterType.UnEquipped));
            this.ActivateAndEquipBadge(Inventory.Instance.GetBadge(BadgeGlobals.BadgeTypes.FlowerSaverP, BadgeGlobals.BadgeFilterType.UnEquipped));
            this.ActivateAndEquipBadge(Inventory.Instance.GetBadge(BadgeGlobals.BadgeTypes.DoubleDipP, BadgeGlobals.BadgeFilterType.UnEquipped));
            this.ActivateAndEquipBadge(Inventory.Instance.GetBadge(BadgeGlobals.BadgeTypes.DoubleDipP, BadgeGlobals.BadgeFilterType.UnEquipped));
            this.ActivateAndEquipBadge(Inventory.Instance.GetBadge(BadgeGlobals.BadgeTypes.GroupFocus, BadgeGlobals.BadgeFilterType.UnEquipped));
            //this.ActivateAndEquipBadge(Inventory.Instance.GetBadge(BadgeGlobals.BadgeTypes.FeelingFineP, BadgeGlobals.BadgeFilterType.UnEquipped));
            //this.ActivateAndEquipBadge(Inventory.Instance.GetBadge(BadgeGlobals.BadgeTypes.HPPlusP, BadgeGlobals.BadgeFilterType.UnEquipped));
            //this.ActivateAndEquipBadge(Inventory.Instance.GetBadge(BadgeGlobals.BadgeTypes.RightOn, BadgeGlobals.BadgeFilterType.UnEquipped));
            //this.ActivateAndEquipBadge(Inventory.Instance.GetBadge(BadgeGlobals.BadgeTypes.Peekaboo, BadgeGlobals.BadgeFilterType.UnEquipped));
            
            base.OnBattleStart();
        }

        public override void OnPhaseCycleStart()
        {
            base.OnPhaseCycleStart();

            //Set the number of max turns each Partner should have to the number of max turns this one does
            PartnerMaxTurns = MaxTurns;
        }

        public sealed override StarPowerBase GetStarPower(StarPowerGlobals.StarPowerTypes starPowerType)
        {
            return BManager.Mario.MStats.GetStarPowerFromType(starPowerType);
        }

        /// <summary>
        /// Swaps Badges from one Partner to another new Partner to apply the effects to that Partner.
        /// </summary>
        /// <param name="partnerEquipped">The Partner currently equipped with the Badges.</param>
        /// <param name="newPartner">The Partner to equip the Badges to.</param>
        public static void SwapPartnerBadges(BattlePartner partnerEquipped, BattlePartner newPartner)
        {
            Badge[] partnerBadges = partnerEquipped.EntityProperties.GetEquippedBadges();
            
            //Go through all the Badges
            for (int i = 0; i < partnerBadges.Length; i++)
            {
                //Unequip them from the current Partner
                partnerBadges[i].UnEquip();

                //Equip them onto the new Partner
                partnerBadges[i].Equip(newPartner);
            }
        }
    }
}
