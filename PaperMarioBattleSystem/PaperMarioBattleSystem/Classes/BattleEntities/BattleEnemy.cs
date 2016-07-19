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

        public override bool HasBadge(BadgeGlobals.BadgeTypes badgeType)
        {
            return (HeldCollectible?.CollectibleType == Enumerations.CollectibleTypes.Badge);
        }
    }
}
