using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static PaperMarioBattleSystem.SweetTreatGlobals;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// Handles spawning elements in Sweet Treat.
    /// The elements correspond to HP, FP, or Poison Mushrooms.
    /// <para>The spawner randomly chooses from the <see cref="AllowedRestorationTypes"/> list, but it will not go over the specified count for each.</para>
    /// </summary>
    public class SweetTreatElementSpawner : BattleObject
    {
        #region Enums

        /// <summary>
        /// The type of distribution to use when spawning restoration types.
        /// </summary>
        public enum RestoreDistributionTypes
        {
            Custom
        }

        #endregion

        /// <summary>
        /// The list of restoration elements currently active.
        /// </summary>
        public List<SweetTreatRestorationElement> RestorationElements = new List<SweetTreatRestorationElement>();

        /// <summary>
        /// The number of columns or rows for the elements.
        /// </summary>
        public int ColumnRows = 1;

        /// <summary>
        /// The X or Y difference position between each column or row.
        /// </summary>
        public float ColumnRowDiffVal = 35f;

        /// <summary>
        /// How long it takes the elements to finish moving.
        /// </summary>
        public double MovementDur = 5000d;

        /// <summary>
        /// The start position the elements spawn.
        /// </summary>
        public Vector2 StartSpawnPos = Vector2.Zero;

        /// <summary>
        /// The end position the elements spawn. The direction the elements move is related to the difference of this value and <see cref="StartSpawnPos"/>.
        /// </summary>
        public Vector2 EndSpawnPos = Vector2.Zero;

        /// <summary>
        /// The amount of each restoration type to spawn.
        /// </summary>
        protected Dictionary<RestoreTypes, int> RestorationTypeTracker = new Dictionary<RestoreTypes, int>();

        /// <summary>
        /// The permitted restoration types to spawn. This list will be reduced as more icons spawn their max values.
        /// </summary>
        protected List<RestoreTypes> AllowedRestorationTypes = new List<RestoreTypes>();

        /// <summary>
        /// The time to wait before spawning the next element.
        /// </summary>
        public double TimeBetweenElements = 750d;

        private double ElapsedTime = 0d;

        /// <summary>
        /// Tells if the spawner spawns the elements vertically or not.
        /// </summary>
        protected bool IsVertical => (StartSpawnPos.X == EndSpawnPos.X);

        /// <summary>
        /// Tells if the spawner is done spawning elements.
        /// </summary>
        protected bool DoneSpawning => (RestorationTypeTracker.Count == 0);

        /// <summary>
        /// Tells if the spawner is completely done.
        /// </summary>
        public bool CompletelyDone => (DoneSpawning == true && RestorationElements.Count == 0);

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="columnRows">The number of columns or rows to spawn elements on.</param>
        /// <param name="columnRowDiffVal">The X or Y position difference between each column or row.</param>
        /// <param name="movementDur">How long it takes the elements to move. This also represents how long they're on screen.</param>
        /// <param name="timeBetweenElements">How much time to wait before each element.</param>
        /// <param name="startSpawnPos">The start position the elements spawn.</param>
        /// <param name="endSpawnPos">The end position of the elements.</param>
        /// <param name="allowedRestorationTypes">The restoration types the spawner will create.</param>
        /// <param name="restorationTypeCounts">The counts for each restoration type. Define them in the same order as the permitted restoration types.</param>
        public SweetTreatElementSpawner(int columnRows, float columnRowDiffVal, double movementDur, double timeBetweenElements,
            Vector2 startSpawnPos, Vector2 endSpawnPos, IList<RestoreTypes> allowedRestorationTypes, IList<int> restorationTypeCounts)
        {
            ColumnRows = columnRows;
            ColumnRowDiffVal = columnRowDiffVal;
            MovementDur = movementDur;
            TimeBetweenElements = timeBetweenElements;

            StartSpawnPos = startSpawnPos;
            EndSpawnPos = endSpawnPos;

            AllowedRestorationTypes.AddRange(allowedRestorationTypes);

            SetupRestorationDict(restorationTypeCounts);
        }

        public override void CleanUp()
        {
            for (int i = 0; i < RestorationElements.Count; i++)
            {
                BattleUIManager.Instance.RemoveUIElement(RestorationElements[i]);
                RestorationElements.RemoveAt(i);
                i--;
            }

            RestorationTypeTracker.Clear();
            AllowedRestorationTypes.Clear();
        }

        public void RemoveElement(SweetTreatRestorationElement restorationElement)
        {
            if (restorationElement == null) return;

            BattleUIManager.Instance.RemoveUIElement(restorationElement);
            RestorationElements.Remove(restorationElement);
        }

        private void SetupRestorationDict(IList<int> restorationTypeCounts)
        {
            for (int i = 0; i < AllowedRestorationTypes.Count; i++)
            {
                //Add the type and count
                RestoreTypes restoreType = AllowedRestorationTypes[i];
                if (RestorationTypeTracker.ContainsKey(restoreType) == false)
                {
                    RestorationTypeTracker.Add(restoreType, restorationTypeCounts[i]);
                }
            }
        }

        protected void SpawnElement()
        {
            //Get an element to spawn
            RestoreTypes restoreType = ChooseElementToSpawn();

            //If we get this value, we shouldn't be spawning
            if (restoreType == RestoreTypes.None)
            {
                Debug.LogError($"{nameof(SweetTreatElementSpawner)} is trying to spawn an element when it shouldn't be!");
                return;
            }

            int randColRow = GeneralGlobals.Randomizer.Next(0, ColumnRows);

            float valAdd = randColRow * ColumnRowDiffVal;

            //Create the element
            Vector2 startPos = StartSpawnPos;
            Vector2 endPos = EndSpawnPos;
            if (IsVertical == true)
            {
                startPos.X += valAdd;
                endPos.X += valAdd;
            }
            else
            {
                startPos.Y += valAdd;
                endPos.Y += valAdd;
            }

            SweetTreatRestorationElement element = new SweetTreatRestorationElement(restoreType, MovementDur, startPos, endPos);

            RestorationElements.Add(element);

            BattleUIManager.Instance.AddUIElement(element);

            //Remove from this element
            RestorationTypeTracker[restoreType]--;

            //Remove the entry if we shouldn't spawn any more of them
            if (RestorationTypeTracker[restoreType] <= 0)
            {
                RestorationTypeTracker.Remove(restoreType);
                AllowedRestorationTypes.Remove(restoreType);
            }
        }

        protected RestoreTypes ChooseElementToSpawn()
        {
            //Don't bother if done spawning
            if (DoneSpawning == true || AllowedRestorationTypes == null || AllowedRestorationTypes.Count == 0)
                return RestoreTypes.None;

            int randVal = GeneralGlobals.Randomizer.Next(0, AllowedRestorationTypes.Count);

            return AllowedRestorationTypes[randVal];
        }

        public override void Update()
        {
            //Keep spawning if we're not done yet
            if (DoneSpawning == false)
            {
                ElapsedTime += Time.ElapsedMilliseconds;
                if (ElapsedTime >= TimeBetweenElements)
                {
                    //Spawn the next element
                    SpawnElement();

                    ElapsedTime = 0d;
                }
            }

            //Remove them if they're done moving
            for (int i = 0; i < RestorationElements.Count; i++)
            {
                if (RestorationElements[i].DoneMoving == true)
                {
                    BattleUIManager.Instance.RemoveUIElement(RestorationElements[i]);

                    RestorationElements.RemoveAt(i);
                    i--;
                }
            }
        }
    }
}
