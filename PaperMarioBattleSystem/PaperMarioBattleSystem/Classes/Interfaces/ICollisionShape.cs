using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// An interface for a collision shape.
    /// </summary>
    public interface ICollisionShape
    {
        /// <summary>
        /// The type of collision shape this CollisionShape is.
        /// </summary>
        CollisionShapeTypes CollisionShapeType { get; }

        /// <summary>
        /// Tells if this collision shape collides with another collision shape.
        /// </summary>
        /// <param name="collisionShape">The collision shape.</param>
        /// <returns>true if the collision shape collides, otherwise false.</returns>
        bool CollidesWith(ICollisionShape collisionShape);
    }
}
