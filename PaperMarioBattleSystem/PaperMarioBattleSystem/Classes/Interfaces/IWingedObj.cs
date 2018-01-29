using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// An interface for any Winged object that can be knocked to the ground.
    /// <para>Examples include Paragoombas and Stilt Guys.
    /// Parakarry is excluded, as he cannot be knocked to the ground.</para>
    /// </summary>
    public interface IWingedObj : ICleanup
    {
        /// <summary>
        /// Whether the object is Grounded or not.
        /// This is set to true when the object is knocked to the ground and loses its wings.
        /// </summary>
        bool Grounded { get; }

        /// <summary>
        /// How many turns the object stays Grounded.
        /// <para>This is only used by Kammy Koopa in the PM games.</para>
        /// </summary>
        int GroundedTurns { get; }

        /// <summary>
        /// How many turns the object has been Grounded.
        /// <para>This is only used by Kammy Koopa in the PM games.</para>
        /// </summary>
        int ElapsedGroundedTurns { get; }

        /// <summary>
        /// The BattleEntity to become when grounded. It should not have a vulnerability to losing wings.
        /// Winged BattleEntities should use only the information from this and clear it after falling, unless they go airborne again.
        /// </summary>
        BattleEntity GroundedEntity { get; }

        /// <summary>
        /// The DamageEffects the object is grounded from.
        /// </summary>
        Enumerations.DamageEffects GroundedOnEffects { get; }

        /// <summary>
        /// Removes the Winged object's wings, performing any other logic required to transition it to the GroundedEntity.
        /// </summary>
        void RemoveWings();

        /// <summary>
        /// What happens when the object is hit by a move that grounds it.
        /// </summary>
        void HandleGrounded();
    }
}
