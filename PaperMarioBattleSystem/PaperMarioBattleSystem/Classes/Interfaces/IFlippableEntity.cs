using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PaperMarioBattleSystem.Enumerations;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// An interface for any BattleEntity that can be flipped.
    /// <para>Examples include Koopa Troopas and Clefts.</para>
    /// </summary>
    public interface IFlippableEntity
    {
        /// <summary>
        /// Whether the BattleEntity is flipped or not.
        /// </summary>
        bool Flipped { get; }

        /// <summary>
        /// How many turns the BattleEntity stays flipped.
        /// </summary>
        int FlippedTurns { get; }

        /// <summary>
        /// How many turns the BattleEntity has been flipped.
        /// </summary>
        int ElapsedFlippedTurns { get; }

        /// <summary>
        /// The DamageEffects that cause the BattleEntity to flip.
        /// <para>This should only be FlipsShelled and/or FlipsClefts.</para>
        /// </summary>
        DamageEffects FlippedOnEffects { get; }

        /// <summary>
        /// The amount of Defense lost by the BattleEntity when flipped.
        /// In many cases this will be the BattleEntity's BaseDefense.
        /// </summary>
        int DefenseLoss { get; }

        /// <summary>
        /// What happens when the BattleEntity is flipped.
        /// </summary>
        void HandleFlipped();
    }
}
