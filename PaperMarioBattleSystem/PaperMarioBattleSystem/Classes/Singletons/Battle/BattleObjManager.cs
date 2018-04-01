using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// Handles any non-BattleEntity objects in battle.
    /// This can be used for anything ranging from a collision checker to a visual that needs to be rendered.
    /// <para>This is a Singleton.</para>
    /// </summary>
    public class BattleObjManager : IUpdateable, IDrawable, ICleanup
    {
        #region Singleton Fields

        public static BattleObjManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new BattleObjManager();
                }

                return instance;
            }
        }

        private static BattleObjManager instance = null;

        #endregion

        /// <summary>
        /// The list of BattleObjects.
        /// </summary>
        private readonly List<BattleObject> BattleObjects = new List<BattleObject>();

        private BattleObjManager()
        {

        }

        public void CleanUp()
        {
            ClearAllBattleObjects();

            instance = null;
        }

        /// <summary>
        /// Adds a BattleObject.
        /// </summary>
        /// <param name="battleObj">The BattleObject to add.</param>
        public void AddBattleObject(BattleObject battleObj)
        {
            if (battleObj == null)
            {
                Debug.LogWarning($"Trying to add null {nameof(BattleObject)} to the {nameof(BattleObjManager)}!");
                return;
            }

            BattleObjects.Add(battleObj);
        }

        /// <summary>
        /// Removes a BattleObject.
        /// </summary>
        /// <param name="battleObj">The BattleObject to remove.</param>
        public void RemoveBattleObject(BattleObject battleObj)
        {
            if (battleObj == null) return;

            bool removed = BattleObjects.Remove(battleObj);
            if (removed == true)
            {
                battleObj.CleanUp();
            }
        }

        /// <summary>
        /// Removes a BattleObject by index.
        /// </summary>
        /// <param name="index">The index of the BattleObject to remove from the list.</param>
        public void RemoveBattleObject(int index)
        {
            if (index < 0 || index >= BattleObjects.Count)
            {
                Debug.LogError($"{index} is out of range for the {nameof(BattleObjects)} list, which has a count of {BattleObjects.Count}");
                return;
            }

            BattleObjects[index].CleanUp();
            BattleObjects.RemoveAt(index);
        }

        /// <summary>
        /// Returns all BattleObjects in a new list.
        /// </summary>
        /// <returns>A new list containing all the BattleObjects.</returns>
        public List<BattleObject> GetAllBattleObjects()
        {
            return new List<BattleObject>(BattleObjects);
        }

        /// <summary>
        /// Removes all BattleObjects.
        /// </summary>
        public void ClearAllBattleObjects()
        {
            for (int i = 0; i < BattleObjects.Count; i++)
            {
                RemoveBattleObject(i);
                i--;
            }
        }

        public void Update()
        {
            for (int i = 0; i < BattleObjects.Count; i++)
            {
                BattleObjects[i].Update();

                if (BattleObjects[i].ReadyForRemoval == true)
                {
                    RemoveBattleObject(i);
                    i--;
                }
            }
        }

        public void Draw()
        {
            for (int i = 0; i < BattleObjects.Count; i++)
            {
                BattleObjects[i].Draw();
            }
        }
    }
}
