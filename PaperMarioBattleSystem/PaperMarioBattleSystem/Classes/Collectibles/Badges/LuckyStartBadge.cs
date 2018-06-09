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
            EntityEquipped.BManager.EntityAddedEvent -= OnEntityEnteredBattle;
            EntityEquipped.BManager.EntityAddedEvent += OnEntityEnteredBattle;
        }

        protected override void OnUnequip()
        {
            EntityEquipped.BManager.EntityAddedEvent -= OnEntityEnteredBattle;
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
                EntityEquipped.BManager.EntityAddedEvent -= OnEntityEnteredBattle;

                //Get the statuses and choose a random one
                StatusEffect[] statuses = GetPossibleGrantedStatuses();
                int randStatus = RandomGlobals.Randomizer.Next(0, statuses.Length);

                //Despite the badge's effects, the Status Effect isn't guaranteed to be inflicted
                //If you have Feeling Fine equipped in TTYD and get Electrified with Lucky Start,
                //it's not inflicted yet the "LUCKY" text is displayed and the sound plays
                if (EntityEquipped.EntityProperties.TryAfflictStatus(100d, statuses[randStatus]) == true)
                {
                    EntityEquipped.EntityProperties.AfflictStatus(statuses[randStatus]);
                }
            }
        }
    }
}
