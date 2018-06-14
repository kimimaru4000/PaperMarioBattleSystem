using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Lucky Start Badge - Grants Mario a positive Status Effect at the start of battle for 3 turns.
    /// <para>The Status Effects that can be granted are: HPRegen (+2), FPRegen (+2 FP), Electrified, Dodgy.</para>
    /// </summary>
    public class LuckyStartBadge : Badge
    {
        public const int StatusDuration = 3;

        private bool AddedWithEvents = false;

        public LuckyStartBadge()
        {
            Name = "Lucky Start";
            Description = "Make something good happen when you first enter battle.";

            BPCost = 4;
            PriceValue = 70;

            BadgeType = BadgeGlobals.BadgeTypes.LuckyStart;
            AffectedType = BadgeGlobals.AffectedTypes.Self;
        }

        protected override void OnEquip()
        {
            HandleSubscribe(EntityEquipped.BManager);

            EntityEquipped.ChangedBattleManagerEvent -= OnEntityChangedBattle;
            EntityEquipped.ChangedBattleManagerEvent += OnEntityChangedBattle;
        }

        protected override void OnUnequip()
        {
            HandleUnsubscribe(EntityEquipped.BManager);

            EntityEquipped.ChangedBattleManagerEvent -= OnEntityChangedBattle;

            AddedWithEvents = false;
        }

        private StatusEffect[] GetPossibleGrantedStatuses()
        {
            return new StatusEffect[]
            {
                new HPRegenStatus(2, StatusDuration),
                new FPRegenStatus(2, StatusDuration),
                new ElectrifiedStatus(StatusDuration),
                new DodgyStatus(StatusDuration)
            };
        }

        private void OnEntityEnteredBattle(BattleEntity entity)
        {
            //Check if the BattleEntity this Badge is equipped to is added to battle
            //If so, grant it one of the Status Effects
            if (EntityEquipped == entity)
            {
                HandleUnsubscribe(EntityEquipped.BManager);

                //Get the statuses and choose a random one
                StatusEffect[] statuses = GetPossibleGrantedStatuses();
                int randStatus = RandomGlobals.Randomizer.Next(0, statuses.Length);

                //Despite the badge's effects, the Status Effect isn't guaranteed to be inflicted
                //If you have Feeling Fine equipped in TTYD and get Electrified with Lucky Start,
                //it's not inflicted but the "LUCKY" text is still displayed and the sound still plays
                if (EntityEquipped.EntityProperties.TryAfflictStatus(100d, statuses[randStatus].StatusType) == true)
                {
                    EntityEquipped.EntityProperties.AfflictStatus(statuses[randStatus]);
                }
            }
        }

        //Check if this BattleEntity changed battles
        private void OnEntityChangedBattle(in BattleManager prevBManager, in BattleManager newBManager)
        {
            HandleUnsubscribe(prevBManager);

            HandleSubscribe(newBManager);
        }

        /// <summary>
        /// Handles subscribing to a BattleManager's events.
        /// If this Lucky Start badge has already done this, it won't do it again until removed.
        /// </summary>
        /// <param name="bManager">The BattleManager.</param>
        private void HandleSubscribe(in BattleManager bManager)
        {
            if (AddedWithEvents == true || bManager == null) return;

            bManager.EntityAddedEvent -= OnEntityEnteredBattle;
            bManager.EntityAddedEvent += OnEntityEnteredBattle;

            AddedWithEvents = true;
        }

        /// <summary>
        /// Handles unsubscribing from a BattleManager's events.
        /// If this Lucky Start badge has already done this, it won't do it again until added.
        /// </summary>
        /// <param name="bManager">The BattleManager.</param>
        private void HandleUnsubscribe(in BattleManager bManager)
        {
            if (AddedWithEvents == false || bManager == null) return;

            bManager.EntityAddedEvent -= OnEntityEnteredBattle;

            AddedWithEvents = false;
        }
    }
}
