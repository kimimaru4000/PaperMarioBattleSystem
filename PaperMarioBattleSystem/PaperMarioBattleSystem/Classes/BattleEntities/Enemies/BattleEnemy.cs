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

            //Check if the enemy has an entry in the Tattle table
            //If so, mark it to show its HP
            if (TattleDatabase.HasTattleDescription(Name) == true)
            {
                this.AddShowHPProperty();                
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

        public void SetHeldCollectible(Collectible heldCollectible)
        {
            HeldCollectible = heldCollectible;
        }

        public override void Draw()
        {
            base.Draw();

            if (BattleManager.Instance.ShouldShowPlayerTurnUI == true)
            {
                int showHP = EntityProperties.GetAdditionalProperty<int>(Enumerations.AdditionalProperty.ShowHP);

                if (showHP > 0)
                {
                    //Show HP
                    SpriteRenderer.Instance.DrawText(AssetManager.Instance.TTYDFont, $"{CurHP}/{BattleStats.MaxHP}", Position + new Vector2(0, 40), Color.White, .2f, false);
                }
            }
        }
    }
}
