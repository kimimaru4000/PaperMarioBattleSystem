using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using static PaperMarioBattleSystem.Enumerations;
using static PaperMarioBattleSystem.BattleGlobals;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// Various utilities for the BattleManager.
    /// </summary>
    public static class BattleManagerUtils
    {
        /// <summary>
        /// Filters a set of entities by specified HeightStates.
        /// </summary>
        /// <typeparam name="T">A BattleEntity or derived type.</typeparam>
        /// <param name="entities">The list of BattleEntities to filter. This list is modified directly.</param>
        /// <param name="heightStates">The HeightStates to filter entities by. BattleEntities with any of the state will be included.
        /// If null or empty, will return the BattleEntities passed in</param>
        public static void FilterEntitiesByHeights<T>(List<T> entities, params HeightStates[] heightStates) where T : BattleEntity
        {
            //Return immediately if either input is null
            if (entities == null || heightStates == null || heightStates.Length == 0) return;

            for (int i = 0; i < entities.Count; i++)
            {
                BattleEntity entity = entities[i];

                //Remove the entity if it wasn't in any of the height states passed in
                if (heightStates.Contains(entity.HeightState) == false)
                {
                    entities.RemoveAt(i);
                    i--;
                }
            }
        }

        /// <summary>
        /// Filters a set of entities by specified HeightStates.
        /// </summary>
        /// <typeparam name="T">A BattleEntity or derived type.</typeparam>
        /// <param name="entities">The array of BattleEntities to filter.</param>
        /// <param name="heightStates">The HeightStates to filter BattleEntities by. BattleEntities with any of the state will be included.
        /// If null or empty, will return the BattleEntities passed in.</param>
        /// <returns>An array of BattleEntities filtered by HeightStates.</returns>
        public static T[] FilterEntitiesByHeights<T>(T[] entities, params HeightStates[] heightStates) where T : BattleEntity
        {
            if (entities == null || entities.Length == 0 || heightStates == null || heightStates.Length == 0) return entities;

            List<T> filteredEntities = new List<T>(entities);
            FilterEntitiesByHeights(filteredEntities, heightStates);

            return filteredEntities.ToArray();
        }

        /// <summary>
        /// Filters out BattleEntities marked as Untargetable from a set of BattleEntities.
        /// </summary>
        /// <typeparam name="T">A BattleEntity or derived type.</typeparam>
        /// <param name="entities">The list of BattleEntities to filter. The list is modified directly.</param>
        public static void FilterEntitiesByTargetable<T>(List<T> entities) where T : BattleEntity
        {
            //Return if the list is null
            if (entities == null) return;

            for (int i = 0; i < entities.Count; i++)
            {
                BattleEntity entity = entities[i];

                //Check if the entity has the Untargetable additional property
                bool untargetable = entity.EntityProperties.HasAdditionalProperty(AdditionalProperty.Untargetable);

                //If it's untargetable, remove it
                if (untargetable == true)
                {
                    entities.RemoveAt(i);
                    i--;
                }
            }
        }

        /// <summary>
        /// Filters out BattleEntities marked as Untargetable from a set of BattleEntities.
        /// </summary>
        /// <typeparam name="T">A BattleEntity or derived type.</typeparam>
        /// <param name="entities">The array of BattleEntities to filter.</param>
        /// <returns>An array of BattleEntities filtered by untargetable.</returns>
        public static T[] FilterEntitiesByTargetable<T>(T[] entities) where T : BattleEntity
        {
            if (entities == null || entities.Length == 0) return entities;

            List<T> filteredEntities = new List<T>(entities);
            FilterEntitiesByTargetable(filteredEntities);

            return filteredEntities.ToArray();
        }

        /// <summary>
        /// Filters out dead BattleEntities from a set.
        /// </summary>
        /// <typeparam name="T">A BattleEntity or derived type.</typeparam>
        /// <param name="entities">The BattleEntities to filter.</param>
        /// <returns>An array of all the alive BattleEntities.</returns>
        public static T[] FilterDeadEntities<T>(T[] entities) where T : BattleEntity
        {
            if (entities == null || entities.Length == 0) return entities;

            List<T> aliveEntities = new List<T>(entities);
            FilterDeadEntities(aliveEntities);

            return aliveEntities.ToArray();
        }

        /// <summary>
        /// Filters out dead BattleEntities from a set.
        /// </summary>
        /// <typeparam name="T">A BattleEntity or derived type.</typeparam>
        /// <param name="entities">The BattleEntities to filter.</param>
        public static void FilterDeadEntities<T>(List<T> entities) where T : BattleEntity
        {
            if (entities == null || entities.Count == 0) return;

            for (int i = 0; i < entities.Count; i++)
            {
                if (entities[i].IsDead == true)
                {
                    entities.RemoveAt(i);
                    i--;
                }
            }
        }

        /// <summary>
        /// Gets the position in front of a BattleEntity's battle position.
        /// </summary>
        /// <param name="entity">The BattleEntity to get the position in front of.</param>
        /// <param name="fromLeftSide">Whether the front refers to the left side of the BattleEntity.</param>
        /// <returns>A Vector2 with the position in front of the BattleEntity.</returns>
        public static Vector2 GetPositionInFront(BattleEntity entity, bool fromLeftSide)
        {
            Vector2 xdiff = new Vector2(PositionXDiff, 0f);
            if (fromLeftSide == true) xdiff.X = -xdiff.X;

            return (entity.BattlePosition + xdiff);
        }

        /// <summary>
        /// Tells whether one BattleEntity is in front of another.
        /// <para>If the BattleEntities being compared are of different <see cref="EntityTypes"/>, then this may not be accurate.</para>
        /// </summary>
        /// <param name="behindEntity">The BattleEntity that is supposedly behind <paramref name="frontEntity"/>.</param>
        /// <param name="frontEntity">The BattleEntity that is supposedly in front of <paramref name="behindEntity"/>.</param>
        /// <returns>true if <paramref name="behindEntity"/> has a higher BattleIndex than <paramref name="frontEntity"/>, otherwise false.</returns>
        public static bool IsEntityInFrontOf(BattleEntity behindEntity, BattleEntity frontEntity)
        {
            //Compare BattleIndex - the entity behind will have a higher BattleIndex
            return (behindEntity.BattleIndex > frontEntity.BattleIndex);
        }

        /// <summary>
        /// Finds all BattleIndex gaps in a list of BattleEntities.
        /// <para>If the BattleEntities in the list are of different <see cref="EntityTypes"/>, then this may not be accurate.</para>
        /// </summary>
        /// <typeparam name="T">A BattleEntity or derived type.</typeparam>
        /// <param name="entities">The BattleEntities to find the gaps for.</param>
        /// <returns>An int array containing the Battle Indices that have been skipped over. If none are found, an empty array.</returns>
        public static int[] FindBattleIndexGaps<T>(List<T> entities) where T : BattleEntity
        {
            //No BattleEntities are in this list, so nothing can be returned
            if (entities == null || entities.Count == 0)
            {
                return Array.Empty<int>();
            }

            List<int> gaps = null;
            int prevIndex = -1;

            for (int i = 0; i < entities.Count; i++)
            {
                int battleIndex = entities[i].BattleIndex;

                //Look for gaps in the index; if the difference is 2 or greater, then
                //the values in between prevIndex and battleIndex are gaps
                int diffIndex = battleIndex - prevIndex;

                for (int j = 1; j < diffIndex; j++)
                {
                    //Initialize if null
                    if (gaps == null)
                        gaps = new List<int>();

                    gaps.Add(prevIndex + j);
                }

                //Set previous index for comparison with the next BattleEntity
                prevIndex = battleIndex;
            }

            //Return empty array if there are no gaps
            if (gaps == null || gaps.Count == 0)
                return Array.Empty<int>();
            //Otherwise, return the gaps in a new array
            else return gaps.ToArray();
        }

        /// <summary>
        /// Swaps the BattleIndex and BattlePosition of two BattleEntities.
        /// <para>This is intended for BattleEntities of the same EntityType.
        /// Swapping BattleEntities of different EntityTypes may produce undesirable behavior.</para>
        /// </summary>
        /// <param name="firstEntity">The first BattleEntity to swap.</param>
        /// <param name="secondEntity">The second BattleEntity to swap.</param>
        /// <param name="preserveHeight">Whether to preserve the height of each BattleEntity or not.
        /// If true, BattleEntities will only swap X values of their BattlePositions.</param>
        public static void SwapEntityBattlePosAndIndex(BattleEntity firstEntity, BattleEntity secondEntity, bool preserveHeight)
        {
            //Store values
            Vector2 firstBattlePosition = firstEntity.BattlePosition;
            Vector2 secondBattlePosition = secondEntity.BattlePosition;

            int firstBattleIndex = firstEntity.BattleIndex;
            int secondBattleIndex = secondEntity.BattleIndex;

            //Swap positions
            //If we preserve height, use the BattleEntity's own Y
            if (preserveHeight == true)
            {
                firstEntity.SetBattlePosition(new Vector2(secondBattlePosition.X, firstBattlePosition.Y));
                secondEntity.SetBattlePosition(new Vector2(firstBattlePosition.X, secondBattlePosition.Y));
            }
            else
            {
                firstEntity.SetBattlePosition(secondBattlePosition);
                secondEntity.SetBattlePosition(firstBattlePosition);
            }

            //Swap BattleIndex; the lists will be automatically sorted
            firstEntity.SetBattleIndex(secondBattleIndex, true);
            secondEntity.SetBattleIndex(firstBattleIndex, true);
        }
    }
}
