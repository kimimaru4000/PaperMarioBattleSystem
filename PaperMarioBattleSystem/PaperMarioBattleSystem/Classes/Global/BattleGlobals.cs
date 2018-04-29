using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// Class for global values dealing with Battles
    /// </summary>
    public static class BattleGlobals
    {
        #region Enums

        /// <summary>
        /// The settings battles can take place in.
        /// </summary>
        public enum BattleSettings
        {
            /// <summary>
            /// Normal battles.
            /// </summary>
            Normal,

            /// <summary>
            /// Dark battles. Enemy and Neutral BattleEntities are untargetable if there is no light source that illuminates them.
            /// </summary>
            Dark
        }

        /// <summary>
        /// Priority values for various types of Battle Events.
        /// </summary>
        public enum BattleEventPriorities
        {
            Message = 0, Stage = 500, YuxArrange = 600, BobberyBomb = 750, Status = 1000,
            HealHP = 1250, HealFP = 1251, Dialogue = 1500, Fright = 1700, BlownAway = 1800, Death = 2000, Damage = 2500
        }

        #endregion

        #region Constants

        /// <summary>
        /// Anything below or equal to this value is an invalid Battle Index.
        /// </summary>
        public const int InvalidBattleIndex = -1;
        public const int DefaultTurnCount = 1;

        public const int MinDamage = 0;
        public const int MaxDamage = 99;

        public const int MaxPowerBounces = 100;

        public const int MinDangerHP = 2;
        public const int MaxDangerHP = 5;
        public const int PerilHP = 1;
        public const int DeathHP = 0;

        /// <summary>
        /// Values less than or equal to this for succession attacks, such as a Fuzzy's Kissy-Kissy move, indicate that this attack
        /// won't end until the Action Command is successfully performed.
        /// </summary>
        public const int InfiniteSuccessionAttacks = 0;

        public const string NoRunMessage = "Can't flee this fight!";

        #endregion

        #region Structs

        /// <summary>
        /// Holds information containing various properties for battle.
        /// </summary>
        public struct BattleProperties
        {
            public BattleSettings BattleSetting;
            public bool Runnable;

            public BattleProperties(BattleSettings battleSetting, bool runnable)
            {
                BattleSetting = battleSetting;
                Runnable = runnable;
            }
        }

        /// <summary>
        /// Holds information about a MoveAction being used and the BattleEntities it targets
        /// </summary>
        public struct ActionHolder
        {
            /// <summary>
            /// The MoveAction being used.
            /// </summary>
            public MoveAction Action { get; private set; }

            /// <summary>
            /// The BattleEntities the action targets.
            /// </summary>
            public BattleEntity[] Targets { get; private set; }

            public ActionHolder(MoveAction action, params BattleEntity[] targets)
            {
                Action = action;
                Targets = targets;
            }
        }

        public struct DefensiveActionHolder
        {
            /// <summary>
            /// The final damage, influenced by the Defensive Action
            /// </summary>
            public int Damage { get; private set; }

            /// <summary>
            /// A filtered set of StatusEffects, influenced by the Defensive Action
            /// </summary>
            public StatusChanceHolder[] Statuses { get; private set; }

            /// <summary>
            /// The filtered DamageEffects, influenced by the Defensive Action.
            /// <para>For example, Koops won't get flipped if he Guards or Superguards a Goomba's bonk.</para>
            /// </summary>
            public Enumerations.DamageEffects DamageEffect { get; private set; }

            /// <summary>
            /// The type of DefensiveAction that was used.
            /// </summary>
            public Enumerations.DefensiveActionTypes DefensiveActionType { get; private set; }

            /// <summary>
            /// The type and amount of damage dealt to the attacker.
            /// If none, set to null.
            /// </summary>
            //public ElementDamageHolder? ElementHolder { get; private set; }
            public StatusGlobals.PaybackHolder? Payback { get; private set; }

            public DefensiveActionHolder(int damage, StatusChanceHolder[] statuses, Enumerations.DamageEffects damageEffect,
                Enumerations.DefensiveActionTypes defensiveActionType)
                : this(damage, statuses, damageEffect, defensiveActionType, null)
            {
            }

            public DefensiveActionHolder(int damage, StatusChanceHolder[] statuses, Enumerations.DamageEffects damageEffect,
                Enumerations.DefensiveActionTypes defensiveActionType, StatusGlobals.PaybackHolder? payback)//ElementDamageHolder? elementHolder)
            {
                Damage = damage;
                Statuses = statuses;
                DamageEffect = damageEffect;
                DefensiveActionType = defensiveActionType;
                //ElementHolder = elementHolder;
                Payback = payback;
            }
        }

        /// <summary>
        /// Holds a pending Battle Event with its priority and the Battle States it should be added in.
        /// The fields in this struct are immutable.
        /// </summary>
        public struct PendingBattleEventHolder
        {
            public int Priority { get; private set; }
            public BattleManager.BattleState[] States { get; private set; }
            public BattleEvent PendingBattleEvent { get; private set; }

            public PendingBattleEventHolder(int priority, BattleManager.BattleState[] battleStates, BattleEvent battleEvent)
            {
                Priority = priority;
                States = battleStates;
                PendingBattleEvent = battleEvent;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// A Comparison method used to sort BattleEntities by their BattleIndex.
        /// </summary>
        /// <param name="entity1">The first BattleEntity whose BattleIndex to compare.</param>
        /// <param name="entity2">The second BattleEntity whose BattleIndex to compare.</param>
        /// <returns>-1 if entity1 has a lower BattleIndex, 1 if entity2 has a lower BattleIndex, and 0 if they have the same BattleIndex.</returns>
        public static int EntityBattleIndexSort(BattleEntity entity1, BattleEntity entity2)
        {
            //Check for null
            if (entity1 == null)
                return 1;
            if (entity2 == null)
                return -1;

            //Compare BattleIndex; lower ones are favored
            if (entity1.BattleIndex < entity2.BattleIndex)
                return -1;
            else if (entity1.BattleIndex > entity2.BattleIndex)
                return 1;

            return ResolveSameBattleIndex(entity1, entity2);
        }

        /// <summary>
        /// Handles resolving the sorting of BattleEntities if their BattleIndices are the same.
        /// </summary>
        /// <param name="entity1"></param>
        /// <param name="entity2"></param>
        /// <returns>-1 if entity1 has a lower X position, 1 if entity2 has a lower X position. If players, higher X positions are favored instead.
        /// If X positions are equal, -1 if entity1 has a lower Y position and 1 if entity2 has a lower Y position.
        /// If X and Y positions are equal, 0.</returns>
        private static int ResolveSameBattleIndex(BattleEntity entity1, BattleEntity entity2)
        {
            //Check if they have the same X position
            //If so, compare the Y - lower Y values are favored
            if (entity1.BattlePosition.X == entity2.BattlePosition.X)
            {
                if (entity1.BattlePosition.Y < entity2.BattlePosition.Y)
                    return -1;
                else if (entity1.BattlePosition.Y < entity2.BattlePosition.Y)
                    return 1;
            }
            //If not, compare X positions
            else
            {
                //Sorting occurs between same BattleEntities with the same EntityType
                BattleEntity leftEntity = entity1;
                BattleEntity rightEntity = entity2;

                //Swap if they're players, as Players go from right to left
                if (entity1.EntityType == Enumerations.EntityTypes.Player)
                {
                    UtilityGlobals.Swap(ref leftEntity, ref rightEntity);
                }

                //Compare X position; favor the lower for enemies and the higher for players
                if (leftEntity.BattlePosition.X < rightEntity.BattlePosition.X)
                    return -1;
                else if (leftEntity.BattlePosition.X > rightEntity.BattlePosition.X)
                    return 1;
            }

            return 0;
        }

        /// <summary>
        /// Tells if a BattleIndex is valid.
        /// </summary>
        /// <param name="battleIndex">The BattleIndex to test for validity.</param>
        /// <returns>true if <paramref name="battleIndex"/> is greater than <see cref="InvalidBattleIndex"/>, otherwise false.</returns>
        public static bool IsValidBattleIndex(int battleIndex) => (battleIndex > InvalidBattleIndex);

        #endregion
    }
}
