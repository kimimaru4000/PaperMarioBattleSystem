using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// Enum utility class.
    /// </summary>
    public static class EnumUtility
    {
        /// <summary>
        /// Gets the values for an Enum of a particular type in an array and caches it.
        /// </summary>
        /// <typeparam name="T">The Enum type.</typeparam>
        public static class GetValues<T> where T: Enum
        {
            /// <summary>
            /// The cached enum array containing all the values for the Enum type.
            /// </summary>
            public static T[] EnumValues { get; private set; } = null;

            static GetValues()
            {
                EnumValues = (T[])Enum.GetValues(typeof(T));
            }
        }

        /// <summary>
        /// Gets the names for an Enum of a particular type in an array and caches it.
        /// </summary>
        /// <typeparam name="T">The Enum type.</typeparam>
        public static class GetNames<T> where T: Enum
        {
            /// <summary>
            /// The cached string array containing all the names in the Enum type.
            /// </summary>
            public static string[] EnumNames { get; private set; } = null;

            static GetNames()
            {
                EnumNames = Enum.GetNames(typeof(T));
            }
        }
    }
}
