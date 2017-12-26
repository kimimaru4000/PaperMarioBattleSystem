using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// A collision rectangle.
    /// </summary>
    public struct CollisionRect : ICollisionShape
    {
        private Rectangle Rect;

        public CollisionShapeTypes CollisionShapeType { get; private set; }

        public CollisionRect(Rectangle rect)
        {
            CollisionShapeType = CollisionShapeTypes.Rectangle;
            Rect = rect;
        }

        public void SetRect(Rectangle rect)
        {
            Rect = rect;
        }

        public bool CollidesWith(ICollisionShape collisionShape)
        {
            if (collisionShape != null)
            {
                if (collisionShape.CollisionShapeType == CollisionShapeTypes.Rectangle)
                {
                    CollisionRect collRect = (CollisionRect)collisionShape;
                    return (Rect.Intersects(collRect.Rect));
                }
            }

            return false;
        }
    }
}
