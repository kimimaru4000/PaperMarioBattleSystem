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

        public override int GetEquippedBadgeCount(BadgeGlobals.BadgeTypes badgeType)
        {
            return Inventory.Instance.GetActiveBadgeCount(badgeType);
        }

        /// <summary>
        /// Global properties relating to all Players. Only one instance should exist and be in the BattlePlayer class.
        /// <para>This includes effects from Badges like Quick Change, Simplifier, and Unsimplifier.</para>
        /// </summary>
        public class GlobalPlayerProperties
        {
            private readonly Dictionary<PlayerProperties, object> Properties = new Dictionary<PlayerProperties, object>();

            public void AddProperty<T>(PlayerProperties property, T value)
            {
                if (HasProperty(property) == true)
                {
                    RemoveProperty(property);
                }

                Properties.Add(property, value);

                Debug.Log($"Added player property {property} with value {value}");
            }

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

            public T GetProperty<T>(PlayerProperties property)
            {
                if (HasProperty(property) == false)
                {
                    return default(T);
                }

                return (T)Properties[property];
            }

            public bool HasProperty(PlayerProperties property)
            {
                return Properties.ContainsKey(property);
            }
        }
    }
}
