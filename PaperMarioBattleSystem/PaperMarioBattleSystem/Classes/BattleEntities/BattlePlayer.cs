using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PaperMarioBattleSystem.Enumerations;
using static PaperMarioBattleSystem.BattlePlayerGlobals;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The base class for playable characters in battle (Ex. Mario and his Partners)
    /// </summary>
    public abstract class BattlePlayer : BattleEntity
    {
        public PlayerTypes PlayerType { get; protected set; } = PlayerTypes.Mario;

        public static readonly GlobalPlayerProperties PlayerProperties = new GlobalPlayerProperties();

        public BattlePlayer(Stats stats) : base(stats)
        {
            DefensiveActions.Add(new Guard(this));
            DefensiveActions.Add(new Superguard(this));
        }

        /// <summary>
        /// Returns Mario's FP for BattlePlayers, as they share the same FP pool for Mario.
        /// </summary>
        public override int CurFP => BattleManager.Instance.GetMario().BattleStats.FP;

        /// <summary>
        /// While any BattleEntity can have FP, only Mario actually uses it.
        /// Both Partners and Mario add to Mario's FP pool.
        /// </summary>
        /// <param name="fp"></param>
        public override void HealFP(int fp)
        {
            BattleMario mario = BattleManager.Instance.GetMario();
            mario.BattleStats.FP = UtilityGlobals.Clamp(mario.BattleStats.FP + fp, 0, mario.BattleStats.MaxFP);
            Debug.Log($"{mario.Name} healed {fp} FP!");
        }

        /// <summary>
        /// While any BattleEntity can have FP, only Mario actually uses it.
        /// Both Partners and Mario subtract from Mario's FP pool.
        /// </summary>
        public override void LoseFP(int fp)
        {
            BattleMario mario = BattleManager.Instance.GetMario();
            mario.BattleStats.FP = UtilityGlobals.Clamp(mario.BattleStats.FP - fp, 0, mario.BattleStats.MaxFP);
            Debug.Log($"{mario.Name} healed {fp} FP!");
        }

        public override int GetEquippedBadgeCount(BadgeGlobals.BadgeTypes badgeType)
        {
            //NOTE: This isn't entity-specific right now, so it technically doesn't work properly.
            //For example, if a Partner had Mario's Jump, it could use Power Bounce if Mario had
            //the Badge equipped even if the Partner didn't

            return Inventory.Instance.GetActiveBadgeCount(badgeType);
        }

        /// <summary>
        /// Global properties relating to all Players. Only one instance should exist and be in the BattlePlayer class.
        /// <para>This includes effects from Badges like Quick Change, Simplifier, and Unsimplifier.</para>
        /// </summary>
        public class GlobalPlayerProperties
        {
            private readonly Dictionary<PlayerProperties, object> Properties = new Dictionary<PlayerProperties, object>();

            /// <summary>
            /// Adds a player property, replacing the current value if one already exists.
            /// </summary>
            /// <param name="property">The PlayerProperties to add.</param>
            /// <param name="value">An object of the value corresponding to the PlayerProperties.</param>
            public void AddProperty(PlayerProperties property, object value)
            {
                if (HasProperty(property) == true)
                {
                    RemoveProperty(property);
                }

                Properties.Add(property, value);

                Debug.Log($"Added player property {property} with value {value}");
            }

            /// <summary>
            /// Removes a property.
            /// </summary>
            /// <param name="property">The PlayerProperties to remove.</param>
            public void RemoveProperty(PlayerProperties property)
            {
                if (HasProperty(property) == false)
                {
                    Debug.LogWarning($"Player property {property} cannot be removed as it doesn't have an entry");
                    return;
                }

                bool removed = Properties.Remove(property);

                if (removed == true)
                {
                    //Debug.Log($"Removed player property {property}");
                }
            }

            /// <summary>
            /// Gets a property, returning a default value if none were found.
            /// </summary>
            /// <typeparam name="T">The type of property to get.</typeparam>
            /// <param name="property">The PlayerProperties to get the value for.</param>
            /// <returns>The value corresponding to the property passed in. If no value was found, returns the default value of type T.</returns>
            public T GetProperty<T>(PlayerProperties property)
            {
                if (HasProperty(property) == false)
                {
                    return default(T);
                }

                return (T)Properties[property];
            }

            /// <summary>
            /// Tells if a property exists.
            /// </summary>
            /// <param name="property">The PlayerProperties to check a value for.</param>
            /// <returns>true if a value corresponding to the PlayerProperties exists, otherwise false.</returns>
            public bool HasProperty(PlayerProperties property)
            {
                return Properties.ContainsKey(property);
            }
        }
    }
}
