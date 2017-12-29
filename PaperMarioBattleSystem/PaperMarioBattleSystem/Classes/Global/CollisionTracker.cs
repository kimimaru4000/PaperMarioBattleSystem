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
    /// <typeparam name="T">A type for the main collision object that implements <see cref="ICollisionHandler"/>.</typeparam>
    /// <typeparam name="U">A type for the group of collision objects that implements <see cref="ICollisionHandler"/>.</typeparam>
    public class CollisionTracker<T, U> : ICleanup where T : ICollisionHandler where U : ICollisionHandler
    {
        /// <summary>
        /// A delegate for handling collisions.
        /// </summary>
        /// <param name="collisionTracker">The <see cref="CollisionTracker{T, U}"/> that sent the event.</param>
        /// <param name="collisionResponse">The <see cref="CollisionResponseHolder"/> that was sent.</param>
        public delegate void CollisionHandler(CollisionTracker<T,U> collisionTracker, CollisionResponseHolder collisionResponse);

        /// <summary>
        /// An event for handling collisions with an object.
        /// </summary>
        public event CollisionHandler CollisionHandlerEvent = null;

        public T CollisionObj = default(T);

        /// <summary>
        /// The list of objects that collision should be handled with.
        /// </summary>
        public List<U> CollisionObjects = null;

        public CollisionTracker(T handler, List<U> collisionObjects)
        {
            CollisionObj = handler;
            SetCollisionData(collisionObjects);
        }

        public void SetCollisionData(List<U> collisionObjects)
        {
            ClearTrackedObjects();
            CollisionObjects = collisionObjects;
        }

        public void ClearTrackedObjects()
        {
            //Set to null, as this reference might be used elsewhere
            CollisionObjects = null;
        }

        public void CleanUp()
        {
            ClearTrackedObjects();
            CollisionObj = default(T);

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
                    CollisionHandlerEvent?.Invoke(this, CollisionObjects[i].GetCollisionResponse(collisionObj));
                }
            }
        }
    }
}
