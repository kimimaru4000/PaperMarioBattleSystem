using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The player's database for Tattles.
    /// It gets populated as the player Tattles more enemies.
    /// </summary>
    public static class TattleDatabase
    {
        /// <summary>
        /// The table that contains the images for enemies shown during a Tattle the player has recorded.
        /// The key is the enemy name while the value is their corresponding image.
        /// </summary>
        private static readonly Dictionary<string, Texture2D> TattleImageTable = new Dictionary<string, Texture2D>();

        /// <summary>
        /// The table that contains all the Tattle Logs the player has recorded.
        /// The key is the enemy name while the value is their Tattle Log, which is the out of battle description.
        /// </summary>
        private static readonly Dictionary<string, string[]> TattleLogTable = new Dictionary<string, string[]>();

        /// <summary>
        /// The table that contains all the Tattle descriptions the player has recorded.
        /// The key is the enemy name while the value is their Tattle description, which is the in-battle description.
        /// </summary>
        private static readonly Dictionary<string, string> TattleDescriptionTable = new Dictionary<string, string>();

        static TattleDatabase()
        {

        }

        #region Retrieval Methods

        /// <summary>
        /// Gets the Tattle image corresponding to an enemy.
        /// </summary>
        /// <param name="enemyName">The name of the enemy.</param>
        /// <returns>A Texture2D of the enemy's Tattle image if it exists in the table, otherwise null.</returns>
        public static Texture2D GetTattleImage(string enemyName)
        {
            Texture2D tex = null;
            TattleImageTable.TryGetValue(enemyName, out tex);

            return tex;
        }

        /// <summary>
        /// Tells if there is a Tattle image for a particular enemy.
        /// </summary>
        /// <param name="enemyName">The name of the enemy.</param>
        /// <returns>true if the enemy has a Tattle image entry, otherwise false.</returns>
        public static bool HasTattleImage(string enemyName)
        {
            return TattleImageTable.ContainsKey(enemyName);
        }

        /// <summary>
        /// Gets the Tattle Log corresponding to an enemy.
        /// </summary>
        /// <param name="enemyName">The name of the enemy.</param>
        /// <returns>A string[] array of the enemy's Tattle Log entry if it exists in the table, otherwise null.</returns>
        public static string[] GetTattleLog(string enemyName)
        {
            string[] log = null;
            TattleLogTable.TryGetValue(enemyName, out log);

            return log;
        }

        /// <summary>
        /// Tells if there is a Tattle Log for a particular enemy.
        /// </summary>
        /// <param name="enemyName">The name of the enemy.</param>
        /// <returns>true if the enemy has a Tattle Log entry, otherwise false.</returns>
        public static bool HasTattleLog(string enemyName)
        {
            return TattleLogTable.ContainsKey(enemyName);
        }

        /// <summary>
        /// Gets the Tattle Description corresponding to an enemy.
        /// </summary>
        /// <param name="enemyName">The name of the enemy.</param>
        /// <returns>A string of the enemy's in-battle Tattle Description entry if it exists in the table, otherwise null.</returns>
        public static string GetTattleDescription(string enemyName)
        {
            string description = null;
            TattleDescriptionTable.TryGetValue(enemyName, out description);

            return description;
        }

        /// <summary>
        /// Tells if there is a Tattle Description for a particular enemy.
        /// </summary>
        /// <param name="enemyName">The name of the enemy.</param>
        /// <returns>true if the enemy has a Tattle Description entry, otherwise false.</returns>
        public static bool HasTattleDescription(string enemyName)
        {
            return TattleDescriptionTable.ContainsKey(enemyName);
        }

        #endregion

        #region Add Methods

        /// <summary>
        /// Adds a Tattle image for a particular enemy if it doesn't exist.
        /// </summary>
        /// <param name="enemyName">The name of the enemy.</param>
        /// <param name="enemyImage">The image associated with the enemy.</param>
        public static void AddTattleImageEntry(string enemyName, Texture2D enemyImage)
        {
            if (HasTattleImage(enemyName) == true)
                return;

            TattleImageTable.Add(enemyName, enemyImage);
            Debug.Log($"Added Tattle Image entry for {enemyName}!");
        }

        /// <summary>
        /// Adds a Tattle Log for a particular enemy if it doesn't exist.
        /// </summary>
        /// <param name="enemyName">The name of the enemy.</param>
        /// <param name="logEntry">The log associated with the enemy. Each entry in the array is separated by a new line.</param>
        public static void AddTattleLogEntry(string enemyName, string[] logEntry)
        {
            if (HasTattleLog(enemyName) == true)
                return;

            TattleLogTable.Add(enemyName, logEntry);
            Debug.Log($"Added Tattle Log entry for {enemyName}!");
        }

        /// <summary>
        /// Adds a Tattle Description for a particular enemy if it doesn't exist.
        /// </summary>
        /// <param name="enemyName">The name of the enemy.</param>
        /// <param name="descriptionEntry">The description associated with the enemy.</param>
        public static void AddTattleDescriptionEntry(string enemyName, string descriptionEntry)
        {
            if (HasTattleDescription(enemyName) == true)
                return;

            TattleDescriptionTable.Add(enemyName, descriptionEntry);
            Debug.Log($"Added Tattle Description entry for {enemyName}!");
        }

        #endregion
    }
}
