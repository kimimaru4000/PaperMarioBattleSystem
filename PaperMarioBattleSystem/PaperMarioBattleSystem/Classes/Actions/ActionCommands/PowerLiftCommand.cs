using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PaperMarioBattleSystem.Utilities;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// Move the cursor across the 3x3 grid and select the red and blue arrows to boost your stats.
    /// Hitting a Poison Mushroom halves the cursor's speed for a short time.
    /// </summary>
    public sealed class PowerLiftCommand : ActionCommand
    {
        #region Enums

        public enum PowerLiftIcons
        {
            None = 0,
            Poison = 1,
            Attack = 2,
            Defense = 3
        }

        #endregion

        /// <summary>
        /// The number of Attack selections required to boost your Attack by 1.
        /// </summary>
        public const int AttackBoostReq = 5;

        /// <summary>
        /// The number of Defense selections required to boost your Defense by 1.
        /// </summary>
        public const int DefenseBoostReq = 5;

        /// <summary>
        /// The number of Attack boosts obtained.
        /// </summary>
        public int AttackBoosts { get; private set; } = 0;

        /// <summary>
        /// The number of Defense boosts obtained.
        /// </summary>
        public int DefenseBoosts { get; private set; } = 0;

        /// <summary>
        /// The number of Attack icons selected.
        /// </summary>
        public int AttackSelections { get; private set; } = 0;

        /// <summary>
        /// The numbber of Defense icons selected.
        /// </summary>
        public int DefenseSelections { get; private set; } = 0;

        /// <summary>
        /// The total time the Attack/Defense boost display arrows spend in the blinking interval after gaining a boost.
        /// </summary>
        private const double ArrowBlinkTotalTime = 1000d;

        /// <summary>
        /// The interval at which the Attack/Defense boost display arrows blink after gaining a boost.
        /// </summary>
        private const double ArrowBlinkInterval = ArrowBlinkTotalTime / 10d;

        /// <summary>
        /// The last time an Attack boost was obtained.
        /// </summary>
        public double LastAttackBoost { get; private set; } = 0d;

        /// <summary>
        /// The last time a Defense boost was obtained.
        /// </summary>
        public double LastDefenseBoost { get; private set; } = 0d;

        public int NumColumns { get; private set; } = 3;
        public int NumRows { get; private set; } = 3;
        public Vector2 LiftGridCellSize { get; private set; } = new Vector2(26, 24);
        public Vector2 LiftGridSpacing { get; private set; } = new Vector2(52, 48);

        /// <summary>
        /// How long Power Lift lasts.
        /// </summary>
        public double CommandTime { get; private set; } = 15000d;
        public double ElapsedCommandTime { get; private set; } = 0d;

        public double CursorSpeedDur { get; private set; } = 150d;
        public Color CursorColor { get; private set; } = Color.White;
        private Color NormalColor = Color.White;
        private Color MovingColor = Color.Blue;
        private Color SelectedColor = Color.Red;
        public double ElapsedMoveTime { get; private set; } = 0d;

        public double PoisonSpeedDur { get; private set; } = 500d;
        private double PoisonDur = 2000d;
        private double ElapsedPoisonTime = 0d;
        public bool IsPoisoned { get; private set; } = false;

        public int CurColumn { get; private set; } = 0;
        public int CurRow { get; private set; } = 0;
        public int DestColumn { get; private set; } = 0;
        public int DestRow { get; private set; } = 0;

        private double IconFadeTime = 500d;
        private double IconStayTime = 2300d;

        private readonly double[] IconCreationTimes = new double[] { 100d, 250d, 350d, 500d, 800d };
        private double PrevCreationTime = 0d;

        private bool SelectedIcon = false;

        /// <summary>
        /// The grid used for tracking the icons.
        /// </summary>
        public PowerLiftIconElement[][] IconGrid { get; private set; } = null;

        /// <summary>
        /// Tells whether the player can select an arrow with the cursor.
        /// The cursor cannot select while it is moving to another spot on the grid.
        /// </summary>
        public bool CanSelect => (CurColumn == DestColumn && CurRow == DestRow);

        public PowerLiftCommand(IActionCommandHandler commandHandler, double commandTime) : base(commandHandler)
        {
            CommandTime = commandTime;
        }

        public override void StartInput(params object[] values)
        {
            base.StartInput(values);

            ElapsedCommandTime = ElapsedPoisonTime = 0d;
            SelectedIcon = false;
            IsPoisoned = false;

            //Set up the grid
            SetUpGrid();

            //Center the cursor in the middle
            CurColumn = NumColumns / 2;
            CurRow = NumRows / 2;

            DestColumn = CurColumn;
            DestRow = CurRow;
        }

        public override void EndInput()
        {
            base.EndInput();

            ElapsedCommandTime = ElapsedPoisonTime = 0d;
            SelectedIcon = true;
            IsPoisoned = false;

            CurColumn = CurRow = DestColumn = DestRow = 0;

            AttackBoosts = DefenseBoosts = 0;
            LastAttackBoost = LastDefenseBoost = 0d;
            PrevCreationTime = 0d;

            for (int i = 0; i < IconGrid.Length; i++)
            {
                IconGrid[i] = null;
            }

            IconGrid = null;
        }

        private void SetUpGrid()
        {
            //Initialize the icon grid
            IconGrid = new PowerLiftIconElement[NumColumns][];
            for (int i = 0; i < IconGrid.Length; i++)
            {
                IconGrid[i] = new PowerLiftIconElement[NumRows];
            }
        }

        protected override void ReadInput()
        {
            if (ElapsedCommandTime >= CommandTime)
            {
                //If any stat was boosted by at least 1, it's a Success
                if (AttackBoosts > 0 || DefenseBoosts > 0)
                {
                    OnComplete(CommandResults.Success);
                }
                else
                {
                    OnComplete(CommandResults.Failure);
                }
                return;
            }

            ElapsedCommandTime += Time.ElapsedMilliseconds;

            HandlePoisoned();
            HandleCursorInput();
            
            UpdateIconGrid();

            HandleIconCreation();
        }

        private void HandleCursorInput()
        {
            if (CanSelect == true)
            {
                //Wait a frame after having just selected an icon (this matches the behavior in the game)
                if (SelectedIcon == true)
                {
                    CursorColor = NormalColor;
                    SelectedIcon = false;
                    return;
                }

                //Check for selecting the icon
                if (Input.GetKeyDown(Keys.Z) == true)
                {
                    HandleIconSelection(IconGrid[CurColumn][CurRow]);
                }

                //Don't allow cursor movement if an icon was just selected
                if (SelectedIcon == false)
                {
                    HandleCursorMovement();
                }
            }
            //Handle moving the cursor
            else
            {
                //Progress the amount of time spent moving
                ElapsedMoveTime += Time.ElapsedMilliseconds;

                //Choose the speed; if the player hit a Poison Mushroom, use the slower speed until it expires
                double speed = CursorSpeedDur;
                if (IsPoisoned == true)
                {
                    speed = PoisonSpeedDur;
                }

                //Lerp to the destination
                //CurrentCursorPos = Vector2.Lerp(PrevCursorPos, DestinationCursorPos, (float)(ElapsedMoveTime / speed));

                //We're done moving to our destination
                if (ElapsedMoveTime >= speed)
                {
                    CurColumn = DestColumn;
                    CurRow = DestRow;

                    CursorColor = NormalColor;
                    //CurrentCursorPos = DestinationCursorPos;
                }
            }
        }

        private void HandleCursorMovement()
        {
            int newCol = CurColumn;
            int newRow = CurRow;

            if (Input.GetKeyDown(Keys.Up) == true)
            {
                newRow -= 1;
            }
            else if (Input.GetKeyDown(Keys.Down) == true)
            {
                newRow += 1;
            }
            else if (Input.GetKeyDown(Keys.Left) == true)
            {
                newCol -= 1;
            }
            else if (Input.GetKeyDown(Keys.Right) == true)
            {
                newCol += 1;
            }

            //Check if we moved at all and make sure we're in bounds
            if (newCol != CurColumn && newCol >= 0 && newCol < NumColumns)
            {
                DestColumn = newCol;
                CursorColor = MovingColor;

                ElapsedMoveTime = 0d;
            }
            else if (newRow != CurRow && newRow >= 0 && newRow < NumRows)
            {
                DestRow = newRow;
                CursorColor = MovingColor;

                ElapsedMoveTime = 0d;
            }
        }

        private void HandleIconSelection(PowerLiftIconElement iconSelected)
        {
            if (iconSelected != null)
            {
                switch (iconSelected.PowerliftIcon)
                {
                    case PowerLiftIcons.Poison:
                        IsPoisoned = true;
                        ElapsedPoisonTime = 0d;

                        IconGrid[CurColumn][CurRow] = null;
                        break;
                    case PowerLiftIcons.Attack:
                        AttackSelections++;
                        if (AttackSelections >= AttackBoostReq)
                        {
                            AttackBoosts++;
                            AttackSelections = 0;
                            LastAttackBoost = Time.ActiveMilliseconds;

                            //Send the response with the new number of boosts
                            SendResponse(new ActionCommandGlobals.PowerLiftResponse(AttackBoosts, DefenseBoosts));
                        }
                        
                        IconGrid[CurColumn][CurRow] = null;
                        break;
                    case PowerLiftIcons.Defense:
                        DefenseSelections++;
                        if (DefenseSelections >= DefenseBoostReq)
                        {
                            DefenseBoosts++;
                            DefenseSelections = 0;
                            LastDefenseBoost = Time.ActiveMilliseconds;

                            //Send the response with the new number of boosts
                            SendResponse(new ActionCommandGlobals.PowerLiftResponse(AttackBoosts, DefenseBoosts));
                        }

                        IconGrid[CurColumn][CurRow] = null;
                        break;
                    default:
                        break;
                }
            }

            //Pressing A to select causes the cursor to turn red for 1 frame even if you don't hit an icon
            CursorColor = SelectedColor;
            SelectedIcon = true;
        }

        private void HandlePoisoned()
        {
            //Check if the player is poisoned, which occurs after hitting a Poison Mushroom
            if (IsPoisoned == true)
            {
                //Check if the poisoned timer is done and remove the poison if the player isn't currently moving the cursor
                //The latter check prevents issues with the cursor snapping since the timing changed during the movement
                if (ElapsedPoisonTime >= PoisonDur && CanSelect == true)
                {
                    IsPoisoned = false;
                    ElapsedPoisonTime = 0d;
                }
                //Otherwise progress the timer
                else
                {
                    ElapsedPoisonTime += Time.ElapsedMilliseconds;
                }
            }
        }

        /// <summary>
        /// Updates the icon grid.
        /// </summary>
        private void UpdateIconGrid()
        {
            for (int i = 0; i < IconGrid.Length; i++)
            {
                for (int j = 0; j < IconGrid[i].Length; j++)
                {
                    //Get the icon element
                    PowerLiftIconElement iconElement = IconGrid[i][j];
                    
                    //If there's no icon element here, continue
                    if (iconElement == null)
                    {
                        continue;
                    }

                    //Update the icon element
                    iconElement.Update();

                    //If the icon is completely done, clear it
                    if (iconElement.IsDone == true)
                    {
                        IconGrid[i][j] = null;
                    }
                }
            }
        }

        private void HandleIconCreation()
        {
            //Check if we surpassed the creation time
            if (Time.ActiveMilliseconds >= PrevCreationTime)
            {
                GridIndexHolder nextIndex = FindRandomAvailableGridIndex();
                if (nextIndex.Column >= 0 && nextIndex.Row >= 0)
                {
                    CreateNextIconElement(nextIndex.Column, nextIndex.Row);
                }

                PrevCreationTime = Time.ActiveMilliseconds + IconCreationTimes[GeneralGlobals.Randomizer.Next(0, IconCreationTimes.Length)];
            }
        }

        /// <summary>
        /// Finds the next available index on the grid to place a <see cref="PowerLiftIconElement"/>.
        /// </summary>
        /// <returns>A <see cref="GridIndexHolder"/> with the available column and row indices.
        /// If none are available, the column and row indices will be -1.</returns>
        private GridIndexHolder FindRandomAvailableGridIndex()
        {
            List <GridIndexHolder> availableSpots = new List<GridIndexHolder>();

            for (int i = 0; i < IconGrid.Length; i++)
            {
                for (int j = 0; j < IconGrid[i].Length; j++)
                {
                    //We found an available spot
                    if (IconGrid[i][j] == null)
                    {
                        availableSpots.Add(new GridIndexHolder(i, j));
                    }
                }
            }

            //There are no available spots on the grid, so return invalid data
            if (availableSpots.Count == 0)
            {
                return new GridIndexHolder(-1, -1);
            }
            //Choose a random spot on the grid
            else
            {
                //Get a random value and return it
                int randSpot = GeneralGlobals.Randomizer.Next(0, availableSpots.Count);

                return availableSpots[randSpot];
            }
        }

        /// <summary>
        /// Chooses an icon and creates an element at a particular grid index.
        /// </summary>
        private void CreateNextIconElement(int gridCol, int gridRow)
        {
            //Choose a random icon among Poison Mushrooms, Attack, and Defense
            int randIcon = GeneralGlobals.Randomizer.Next((int)PowerLiftIcons.Poison, (int)PowerLiftIcons.Defense + 1);

            //Create the icon element
            PowerLiftIconElement element = new PowerLiftIconElement((PowerLiftIcons)randIcon, IconFadeTime, IconStayTime, .45f);
            IconGrid[gridCol][gridRow] = element;
        }

        /// <summary>
        /// Holds zero-based column and row values corresponding to a grid index.
        /// </summary>
        private struct GridIndexHolder
        {
            public int Column;
            public int Row;

            public GridIndexHolder(int column, int row)
            {
                Column = column;
                Row = row;
            }
        }
    }
}
