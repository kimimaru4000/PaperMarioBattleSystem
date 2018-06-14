using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PaperMarioBattleSystem.Extensions;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Peekaboo Badge - Allows Mario and his Partner to see enemy HP.
    /// </summary>
    public sealed class PeekabooBadge : Badge
    {
        private bool AddedWithEvents = false;

        public PeekabooBadge()
        {
            Name = "Peekaboo";
            Description = "Make enemy HP visible.";

            BPCost = 2;
            PriceValue = 50;

            BadgeType = BadgeGlobals.BadgeTypes.Peekaboo;
            AffectedType = BadgeGlobals.AffectedTypes.Both;
        }

        protected override void OnEquip()
        {
            HandleAdded(EntityEquipped.BManager);

            EntityEquipped.ChangedBattleManagerEvent -= OnEntityBattleChanged;
            EntityEquipped.ChangedBattleManagerEvent += OnEntityBattleChanged;
        }

        protected override void OnUnequip()
        {
            HandleRemoved(EntityEquipped.BManager);

            EntityEquipped.ChangedBattleManagerEvent -= OnEntityBattleChanged;

            AddedWithEvents = false;
        }

        private void OnEntityAdded(BattleEntity entity)
        {
            if (entity == EntityEquipped)
            {
                HandleAdded(EntityEquipped.BManager);
            }
            else if (entity.EntityType == Enumerations.EntityTypes.Enemy)
            {
                //Tell the enemy to show its HP. Note that we have an integer in case they have been tattled
                entity.AddShowHPProperty();
            }
        }

        private void OnEntityRemoved(BattleEntity entity)
        {
            //If the BattleEntity removed is the one Peekaboo is equipped to, unsubscribe from the events
            if (entity == EntityEquipped)
            {
                HandleRemoved(EntityEquipped.BManager);
            }
            else if (entity.EntityType == Enumerations.EntityTypes.Enemy)
            {
                //Tell the enemy to stop its HP from being shown
                entity.SubtractShowHPProperty();
            }
        }

        private void OnEntityBattleChanged(in BattleManager prevBManager, in BattleManager newBManager)
        {
            //Unsubscribe from the previous BattleManager's events
            HandleRemoved(prevBManager);

            //Subscribe to the new BattleManager's events
            HandleAdded(newBManager);
        }

        /// <summary>
        /// Handles subscribing to a BattleManager's events and adding Peekaboo's effects to all Enemies in that BattleManager.
        /// If this Peekaboo badge has already done this, it won't do it again.
        /// </summary>
        /// <param name="bManager">The BattleManager.</param>
        private void HandleAdded(in BattleManager bManager)
        {
            //Return if null or we shouldn't add
            if (AddedWithEvents == true || bManager == null) return;

            bManager.EntityAddedEvent -= OnEntityAdded;
            bManager.EntityAddedEvent += OnEntityAdded;

            bManager.EntityRemovedEvent -= OnEntityRemoved;
            bManager.EntityRemovedEvent += OnEntityRemoved;

            //For all current enemies, show their HP
            BattleEntity[] enemies = bManager.GetEntities(Enumerations.EntityTypes.Enemy, null);
            for (int i = 0; i < enemies.Length; i++)
            {
                enemies[i].AddShowHPProperty();
            }

            AddedWithEvents = true;
        }

        /// <summary>
        /// Handles unsubscribing from a BattleManager's events and removing Peekaboo's effects from all Enemies in that BattleManager.
        /// If this Peekaboo badge has already done this, it won't do it again.
        /// </summary>
        /// <param name="bManager">The BattleManager.</param>
        private void HandleRemoved(in BattleManager bManager)
        {
            //Return if null or we haven't added
            if (AddedWithEvents == false || bManager == null) return;

            bManager.EntityAddedEvent -= OnEntityAdded;
            bManager.EntityRemovedEvent -= OnEntityRemoved;

            //For all current enemies, remove showing their HP
            BattleEntity[] enemies = bManager.GetEntities(Enumerations.EntityTypes.Enemy, null);
            for (int i = 0; i < enemies.Length; i++)
            {
                enemies[i].SubtractShowHPProperty();
            }

            AddedWithEvents = false;
        }
    }
}
