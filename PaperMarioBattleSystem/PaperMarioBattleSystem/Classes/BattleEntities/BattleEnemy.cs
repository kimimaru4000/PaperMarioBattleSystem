using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// Enemies in battle
    /// </summary>
    public abstract class BattleEnemy : BattleEntity
    {
        /// <summary>
        /// The spot index of the Enemy in battle
        /// </summary>
        public int BattleIndex { get; private set; } = -1;

        /// <summary>
        /// The Collectible the Enemy is holding
        /// </summary>
        public Collectible HeldCollectible { get; protected set; } = null;

        protected BattleEnemy(Stats stats) : base(stats)
        {
            Name = "Partner";

            EntityType = Enumerations.EntityTypes.Enemy;
        }

        public void SetBattleIndex(int battleIndex)
        {
            BattleIndex = battleIndex;
        }

        public override void OnBattleStart()
        {
            base.OnBattleStart();

            //Equip the held Badge, if one is held
            if (HeldCollectible?.CollectibleType == Enumerations.CollectibleTypes.Badge)
            {
                Badge heldBadge = (Badge)HeldCollectible;
                if (heldBadge.AffectedType == BadgeGlobals.AffectedTypes.Self || heldBadge.AffectedType == BadgeGlobals.AffectedTypes.Both)
                {
                    heldBadge.Equip(this);
                }
            }
        }

        public override int GetEquippedBadgeCount(BadgeGlobals.BadgeTypes badgeType)
        {
            if (HeldCollectible?.CollectibleType == Enumerations.CollectibleTypes.Badge)
            {
                Badge heldBadge = (Badge)HeldCollectible;
                if (heldBadge.BadgeType == badgeType && (heldBadge.AffectedType == BadgeGlobals.AffectedTypes.Self
                    || heldBadge.AffectedType == BadgeGlobals.AffectedTypes.Both))
                {
                    return 1;
                }
            }

            return 0;
        }
    }
}
