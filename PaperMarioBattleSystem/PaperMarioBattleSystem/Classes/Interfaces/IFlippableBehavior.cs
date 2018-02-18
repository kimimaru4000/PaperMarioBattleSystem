using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PaperMarioBattleSystem.Enumerations;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// An interface for any object that handles flipping.
    /// <para>Examples include Koopa Troopas and Clefts.</para>
    /// </summary>
    public interface IFlippableBehavior : ICleanup
    {
        /// <summary>
        /// Whether the object is flipped or not.
        /// </summary>
        bool Flipped { get; }

        /// <summary>
        /// How many turns the object stays flipped.
        /// </summary>
        int FlippedTurns { get; }

        /// <summary>
        /// How many turns the object has been flipped.
        /// </summary>
        int ElapsedFlippedTurns { get; }

        /// <summary>
        /// The DamageEffects that cause the object to flip.
        /// </summary>
        DamageEffects FlippedOnEffects { get; }

        /// <summary>
        /// The amount of Defense lost by the object when flipped.
        /// In many cases this will be the object's BaseDefense.
        /// </summary>
        int DefenseLoss { get; }

        /// <summary>
        /// What happens when the object is flipped.
        /// </summary>
        void HandleFlipped();

        /// <summary>
        /// What happens when the object is unflipped.
        /// </summary>
        void UnFlip();

        /// <summary>
        /// Copies this behavior.
        /// </summary>
        /// <param name="entity">The BattleEntity this behavior is for.</param>
        /// <returns>A copy of this IFlippableBehavior.</returns>
        IFlippableBehavior CopyBehavior(BattleEntity entity);
    }
}
