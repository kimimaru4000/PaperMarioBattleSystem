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
        /// The Collectible the Enemy is holding.
        /// </summary>
        public Collectible HeldCollectible { get; protected set; } = null;

        /// <summary>
        /// The Enemy's AI behavior.
        /// </summary>
        public EnemyAIBehavior AIBehavior { get; protected set; } = null;

        protected BattleEnemy(Stats stats) : base(stats)
        {
            Name = "Partner";

            EntityType = Enumerations.EntityTypes.Enemy;

            //Use the default AI behavior
            AIBehavior = new DefaultEnemyAI(this);
        }

        public override void OnBattleStart()
        {
            base.OnBattleStart();

            //Set battle position
            Vector2 battlepos = BattleManager.Instance.EnemyStartPos + new Vector2(BattleManager.Instance.PositionXDiff * BattleIndex, 0);
            if (HeightState == Enumerations.HeightStates.Airborne) battlepos.Y -= BattleManager.Instance.AirborneY;
            else if (HeightState == Enumerations.HeightStates.Ceiling) battlepos.Y -= BattleManager.Instance.CeilingY;

            SetBattlePosition(battlepos);
            Position = BattlePosition;

            //Equip the held Badge, if one is held
            if (HeldCollectible?.CollectibleType == Enumerations.CollectibleTypes.Badge)
            {
                Badge heldBadge = (Badge)HeldCollectible;
                if (heldBadge.AffectedType == BadgeGlobals.AffectedTypes.Self || heldBadge.AffectedType == BadgeGlobals.AffectedTypes.Both)
                {
                    heldBadge.Equip(this);
                }
            }

            //Check if the enemy has an entry in the Tattle table
            //If so, mark it to show its HP
            if (TattleDatabase.HasTattleDescription(Name) == true)
            {
                this.AddShowHPProperty();                
            }
        }

        public sealed override void OnTurnStart()
        {
            base.OnTurnStart();

            //Make the enemy perform an action on its turn
            AIBehavior.PerformAction();
        }

        public override Item GetItemOfType(Item.ItemTypes itemTypes)
        {
            return (HeldCollectible as Item);
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

        public void SetHeldCollectible(Collectible heldCollectible)
        {
            HeldCollectible = heldCollectible;
        }
    }
}
