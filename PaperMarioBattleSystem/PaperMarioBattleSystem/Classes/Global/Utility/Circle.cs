using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// A structure representing a Circle.
    /// </summary>
    public struct Circle
    {
        /// <summary>
        /// The center point of the Circle.
        /// </summary>
        public Vector2 Center;

        /// <summary>
        /// The radius of the Circle.
        /// </summary>
        public double Radius;

        /// <summary>
        /// Tells if the Circle is empty.
        /// </summary>
        public bool IsEmpty => (Center == Vector2.Zero && Radius == 0f);

        /// <summary>
        /// Returns an empty Circle at the origin with a radius of 0.
        /// </summary>
        public static Circle Empty => new Circle(Vector2.Zero, 0f);

        /// <summary>
        /// Creates a new instance of a Circle with a specified center point and radius.
        /// </summary>
        /// <param name="x">The X point of the Circle.</param>
        /// <param name="y">The Y point of the Circle.</param>
        /// <param name="radius">The radius of the Circle.</param>
        public Circle(float x, float y, double radius) : this(new Vector2(x, y), radius)
        {

        }

        /// <summary>
        /// Creates a new instance of a Circle with a specified center point and radius.
        /// </summary>
        /// <param name="center">The center of the Circle.</param>
        /// <param name="radius">The radius of the Circle.</param>
        public Circle(Vector2 center, double radius)
        {
            Center = center;
            Radius = radius;
        }

        /// <summary>
        /// Tells if this Circle intersects another Circle.
        /// </summary>
        /// <param name="other">The Circle to test intersection with.</param>
        /// <returns>true if the sum of the radii squared is less than or equal to the distance between the circles squared.</returns>
        public bool Intersects(Circle other)
        {
            double radiusSquared = Math.Pow(Radius + other.Radius, 2);
            double distance = Vector2.DistanceSquared(Center, other.Center);

            return (distance <= radiusSquared);
        }

        /// <summary>
        /// Tells if the Circle contains a Vector2.
        /// </summary>
        /// <param name="value">The Vector2 to test.</param>
        /// <returns>true if <paramref name="value"/> is contained in the Circle, otherwise false.</returns>
        public bool Contains(Vector2 value)
        {
            return (Intersects(new Circle(value, 0d)));
        }

        /// <summary>
        /// Tells if the Circle contains a Point.
        /// </summary>
        /// <param name="value">The Point to test.</param>
        /// <returns>true if <paramref name="value"/> is contained in the Circle, otherwise false.</returns>
        public bool Contains(Point value)
        {
            return Contains(new Vector2(value.X, value.Y));
        }

        /// <summary>
        /// Gets a point around the Circle at a particular angle.
        /// </summary>
        /// <param name="angle">The angle, in radians.</param>
        /// <returns>A Vector2 with the X and Y components at the location around the circle.</returns>
        public Vector2 GetPointAround(double angle)
        {
            float x = (float)(Math.Cos(angle) * Radius) + Center.X;
            float y = (float)(Math.Sin(angle) * Radius) + Center.Y;

            return new Vector2(x, y);
        }

        #region Comparison and Operator Overloading

        public override bool Equals(object obj)
        {
            return (obj is Circle) && (this == (Circle)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 18;
                hash = (hash * 37) + Center.GetHashCode();
                hash = (hash * 37) + Radius.GetHashCode();
                return hash;
            }
        }

        public static bool operator ==(Circle a, Circle b)
        {
            return (a.Center == b.Center && a.Radius == b.Radius);
        }

        public static bool operator !=(Circle a, Circle b)
        {
            return (a.Center != b.Center || a.Radius != b.Radius);
        }

        public override string ToString()
        {
            return Center.ToString() + " Radius: " + Radius;
        }

        #endregion
    }
}
