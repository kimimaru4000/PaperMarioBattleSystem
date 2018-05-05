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

        public override void CleanUp()
        {
            base.CleanUp();

            //Clear the held collectible to unequip any Badges on Enemies, as they're always completely removed from battle
            SetHeldCollectible(null);
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
                if (heldBadge.Equipped == false)
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

        /// <summary>
        /// Sets the held Collectible for the enemy. Enemies in TTYD can hold Items and Badges.
        /// <para>If setting a Badge, this will unequip the current Badge from the enemy, provided the held Collectible is a Badge, and equip the new Badge.</para>
        /// </summary>
        /// <param name="heldCollectible">The Collectible to hold.</param>
        public void SetHeldCollectible(Collectible heldCollectible)
        {
            //Unequip the current badge held, if one is held
            if (HeldCollectible?.CollectibleType == Enumerations.CollectibleTypes.Badge)
            {
                Badge heldBadge = (Badge)HeldCollectible;
                heldBadge.UnEquip();
            }

            //Set the collectible
            HeldCollectible = heldCollectible;

            //Equip the held Badge, if one is held
            if (HeldCollectible?.CollectibleType == Enumerations.CollectibleTypes.Badge)
            {
                Badge heldBadge = (Badge)HeldCollectible;
                heldBadge.Equip(this);
            }
        }

        protected override void DrawOther()
        {
            base.DrawOther();

            //Draw the collectible being held
            //Collectibles don't show if the enemy is taking a turn
            if (IsTurn == true) return;

            //Collectibles are drawn slightly to the right, behind enemies
            if (HeldCollectible != null && HeldCollectible.Icon != null && HeldCollectible.Icon.Tex != null)
            {
                //The position offset might need to be adjusted; for now this should work for a lot of enemies
                SpriteRenderer.Instance.Draw(HeldCollectible.Icon.Tex, new Vector2(Position.X + 6, Position.Y), HeldCollectible.Icon.SourceRect,
                    Color.White, false, false, Layer - .001f);
            }
        }
    }
}
