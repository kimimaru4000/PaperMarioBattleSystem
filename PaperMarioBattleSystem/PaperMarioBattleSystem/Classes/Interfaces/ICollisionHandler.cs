using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// An interface for objects that handle collisions.
    /// </summary>
    public interface ICollisionHandler
    {
        /// <summary>
        /// The collision shape the object has.
        /// </summary>
        ICollisionShape collisionShape { get; }
        
        /// <summary>
        /// Handles a collision with another object.
        /// </summary>
        /// <param name="collisionResponse">The collision data.</param>
        void HandleCollision(CollisionResponseHolder collisionResponse);

        /// <summary>
        /// Gets a collision response from the object.
        /// </summary>
        /// <param name="collisionObj">The object that collided with this one.</param>
        /// <returns>A <see cref="CollisionResponseHolder"/> with the collision data.</returns>
        CollisionResponseHolder GetCollisionResponse(ICollisionHandler collisionObj);
    }
}
