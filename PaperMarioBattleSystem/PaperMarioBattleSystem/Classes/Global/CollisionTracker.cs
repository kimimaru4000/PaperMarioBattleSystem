using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// An object that tracks collisions among a group of <see cref="ICollisionHandler"/> objects.
    /// </summary>
    /// <typeparam name="T">A type that implements <see cref="ICollisionHandler"/>.</typeparam>
    public class CollisionTracker<T> : ICleanup where T : ICollisionHandler
    {
        public delegate void CollisionHandler(CollisionResponseHolder collisionResponse);

        /// <summary>
        /// An event for handling collisions with an object.
        /// </summary>
        public event CollisionHandler CollisionHandlerEvent = null;

        public T CollisionObj = default(T);

        /// <summary>
        /// The list of objects that collision should be handled with.
        /// </summary>
        public List<T> CollisionObjects = null;

        public CollisionTracker(T handler, List<T> collisionObjects)
        {
            CollisionObj = handler;
            SetCollisionData(collisionObjects);
        }

        public void SetCollisionData(List<T> collisionObjects)
        {
            ClearTrackedObjects();
            CollisionObjects = collisionObjects;
        }

        public void ClearTrackedObjects()
        {
            CollisionObjects.Clear();
        }

        public void CleanUp()
        {
            ClearTrackedObjects();

            CollisionHandlerEvent = null;
        }

        /// <summary>
        /// Checks for collisions with all the collision shapes.
        /// </summary>
        /// <param name="collisionObj">The collision shape to check with collision.</param>
        public void CheckCollisions(ICollisionHandler collisionObj)
        {
            for (int i = 0; i < CollisionObjects.Count; i++)
            {
                //Check for collision with the objects
                if (collisionObj.collisionShape.CollidesWith(CollisionObjects[i].collisionShape) == true)
                {
                    //Invoke the collision event
                    CollisionHandlerEvent?.Invoke(CollisionObjects[i].GetCollisionResponse(collisionObj));
                }
            }
        }
    }
}
