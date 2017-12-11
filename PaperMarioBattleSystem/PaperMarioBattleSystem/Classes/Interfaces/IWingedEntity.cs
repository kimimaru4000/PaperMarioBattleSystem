using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// An interface for any Winged BattleEntity that can be knocked to the ground.
    /// <para>Examples include Paragoombas and Stilt Guys.
    /// Parakarry is excluded as he cannot be knocked to the ground.</para>
    /// </summary>
    public interface IWingedEntity
    {
        /// <summary>
        /// Whether the entity is Grounded or not.
        /// This is set to true when the BattleEntity is knocked to the ground and loses its wings.
        /// </summary>
        bool Grounded { get; }

        /// <summary>
        /// How many turns the BattleEntity stays Grounded.
        /// This is only used by Kammy Koopa in the PM games.
        /// </summary>
        int GroundedTurns { get; }

        /// <summary>
        /// How many turns the BattleEntity has been Grounded.
        /// <para>This is only used by Kammy Koopa in the PM games.</para>
        /// </summary>
        int ElapsedGroundedTurns { get; }

        /// <summary>
        /// The BattleEntity to become when grounded. It should not have a vulnerability to losing wings.
        /// Winged BattleEntities should use only the information from this and clear it after falling, unless they go airborne again.
        /// </summary>
        BattleEntity GroundedEntity { get; }

        /// <summary>
        /// Removes the Winged BattleEntity's wings.
        /// </summary>
        void RemoveWings();

        /// <summary>
        /// What happens when the BattleEntity is Grounded.
        /// </summary>
        void HandleGrounded();
    }
}
