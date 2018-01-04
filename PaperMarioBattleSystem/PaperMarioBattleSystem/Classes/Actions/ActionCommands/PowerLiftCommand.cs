using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// Move the cursor across the 3x3 grid and select the red and blue arrows to boost your stats.
    /// Hitting a Poison Mushroom halves the cursor's speed for a short time.
    /// </summary>
    //NOTE: Clean this up - the code is pretty good performance-wise, but it can be more readable after trimming and commenting it
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
        private const int AttackBoostReq = 5;

        /// <summary>
        /// The number of Defense selections required to boost your Defense by 1.
        /// </summary>
        private const int DefenseBoostReq = 5;

        /// <summary>
        /// The number of Attack boosts obtained.
        /// </summary>
        private int AttackBoosts = 0;

        /// <summary>
        /// The number of Defense boosts obtained.
        /// </summary>
        private int DefenseBoosts = 0;

        /// <summary>
        /// The number of Attack icons selected.
        /// </summary>
        private int AttackSelections = 0;

        /// <summary>
        /// The numbber of Defense icons selected.
        /// </summary>
        private int DefenseSelections = 0;

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
        private double LastAttackBoost = 0d;

        /// <summary>
        /// The last time a Defense boost was obtained.
        /// </summary>
        private double LastDefenseBoost = 0d;

        private int NumColumns = 3;
        private int NumRows = 3;
        private Vector2 LiftGridCellSize = new Vector2(26, 24);
        private Vector2 LiftGridSpacing = new Vector2(52, 48);

        /// <summary>
        /// How long Power Lift lasts.
        /// </summary>
        private double CommandTime = 15000d;
        private double ElapsedCommandTime = 0d;

        private double CursorSpeedDur = 150d;
        private Vector2 PrevCursorPos = Vector2.Zero;
        private Vector2 CurrentCursorPos { get => Cursor.Position; set => Cursor.Position = value; }
        private Vector2 DestinationCursorPos = Vector2.Zero;
        private Color CursorColor = Color.White;
        private Color MovingColor = Color.Blue;
        private Color SelectedColor = Color.Red;
        private double ElapsedMoveTime = 0d;

        private double PoisonSpeedDur = 500d;
        private double PoisonDur = 2000d;
        private double ElapsedPoisonTime = 0d;
        private bool IsPoisoned = false;

        private CroppedTexture2D BigCursor = null;
        private CroppedTexture2D SmallCursor = null;
        private CroppedTexture2D ArrowIcon = null;
        private CroppedTexture2D BarEdge = null;
        private CroppedTexture2D Bar = null;
        private CroppedTexture2D BarFill = null;

        private UIFourPiecedTex Cursor = null;
        private int CurColumn = 0;
        private int CurRow = 0;

        private double IconFadeTime = 500d;
        private double IconStayTime = 2300d;

        private readonly double[] IconCreationTimes = new double[] { 100d, 250d, 350d, 500d, 800d };
        private double PrevCreationTime = 0d;

        private bool SelectedIcon = false;

        /// <summary>
        /// The grid used for laying out the objects.
        /// </summary>
        private UIGrid PowerLiftGrid = null;

        /// <summary>
        /// The internal grid used for tracking the icons.
        /// </summary>
        private PowerLiftIconElement[][] IconGrid = null;

        /// <summary>
        /// Tells whether the player can select an arrow with the cursor.
        /// The cursor cannot select while it is moving to another spot on the grid.
        /// </summary>
        private bool CanSelect => (CurrentCursorPos == DestinationCursorPos);

        public PowerLiftCommand(IActionCommandHandler commandHandler, double commandTime) : base(commandHandler)
        {
            CommandTime = commandTime;
        }

        public override void StartInput(params object[] values)
        {
            base.StartInput(values);

            Texture2D battleGFX = AssetManager.Instance.LoadRawTexture2D($"{ContentGlobals.UIRoot}/Battle/BattleGFX.png");

            BigCursor = new CroppedTexture2D(battleGFX, new Rectangle(14, 273, 46, 46));
            SmallCursor = new CroppedTexture2D(battleGFX, new Rectangle(10, 330, 13, 12));//new CroppedTexture2D(AssetManager.Instance.LoadRawTexture2D($"{ContentGlobals.UIRoot}/DebugAssets/BoxOutline2.png"), null);
            ArrowIcon = new CroppedTexture2D(battleGFX, new Rectangle(5, 353, 50, 61));
            BarEdge = new CroppedTexture2D(battleGFX, new Rectangle(514, 245, 7, 28));
            Bar = new CroppedTexture2D(battleGFX, new Rectangle(530, 245, 1, 28));
            BarFill = new CroppedTexture2D(battleGFX, new Rectangle(541, 255, 1, 1));

            Cursor = new UIFourPiecedTex(BigCursor, BigCursor.WidthHeightToVector2(), .6f, CursorColor);

            ElapsedCommandTime = ElapsedPoisonTime = 0d;
            SelectedIcon = false;
            IsPoisoned = false;

            //Set up the grid
            SetUpGrid();

            BattleUIManager.Instance.AddUIElement(PowerLiftGrid);
            BattleUIManager.Instance.AddUIElement(Cursor);

            //Center the cursor in the middle
            CurColumn = PowerLiftGrid.Columns / 2;
            CurRow = PowerLiftGrid.Rows / 2;

            int centerIndex = PowerLiftGrid.GetIndex(CurColumn, CurRow);
            PrevCursorPos = CurrentCursorPos = DestinationCursorPos = PowerLiftGrid.GetPositionAtIndex(centerIndex);
        }

        public override void EndInput()
        {
            base.EndInput();

            ElapsedCommandTime = ElapsedPoisonTime = 0d;
            SelectedIcon = true;
            IsPoisoned = false;

            BattleUIManager.Instance.RemoveUIElement(PowerLiftGrid);
            BattleUIManager.Instance.RemoveUIElement(Cursor);

            CurColumn = CurRow = 0;

            AttackBoosts = DefenseBoosts = 0;
            LastAttackBoost = LastDefenseBoost = 0d;
            PrevCreationTime = 0d;

            UtilityGlobals.ClearJaggedArray(ref IconGrid);
            IconGrid = null;

            PowerLiftGrid.ClearGrid();
            PowerLiftGrid = null;
        }

        private void SetUpGrid()
        {
            //Create the grid
            PowerLiftGrid = new UIGrid(NumColumns, NumRows, LiftGridCellSize);

            //Populate the grid based on how many columns and rows it has
            int totalSize = PowerLiftGrid.MaxElementsInGrid;
            for (int i = 0; i < totalSize; i++)
            {
                //Small cursors are on the grid
                PowerLiftGrid.AddGridElement(new UIFourPiecedTex(SmallCursor.Copy(), SmallCursor.WidthHeightToVector2(), .5f, Color.White));
            }

            //Although the grid is drawn on the UI layer, we center it using the sprite layer's (0, 0) position for ease
            PowerLiftGrid.Position = Camera.Instance.SpriteToUIPos(Vector2.Zero);
            PowerLiftGrid.ChangeGridPivot(UIGrid.GridPivots.Center);
            //PowerLiftGrid.ChangeElementPivot(UIGrid.GridPivots.Center);

            Vector2 paddingSize = LiftGridCellSize / 2;

            PowerLiftGrid.ChangeGridPadding(0, (int)paddingSize.X, 0, (int)paddingSize.Y);

            PowerLiftGrid.Spacing = LiftGridSpacing;

            //Initialize the icon grid
            UtilityGlobals.InitializeJaggedArray(ref IconGrid, PowerLiftGrid.Columns, PowerLiftGrid.Rows);
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
                    Cursor.TintColor = CursorColor;
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
                CurrentCursorPos = Vector2.Lerp(PrevCursorPos, DestinationCursorPos, (float)(ElapsedMoveTime / speed));

                //We're done moving to our destination
                if (ElapsedMoveTime >= speed)
                {
                    Cursor.TintColor = CursorColor;
                    CurrentCursorPos = DestinationCursorPos;
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
            if (newCol != CurColumn && newCol >= 0 && newCol < PowerLiftGrid.Columns)
            {
                CurColumn = newCol;

                PrevCursorPos = CurrentCursorPos;
                DestinationCursorPos = PowerLiftGrid.GetPositionAtIndex(PowerLiftGrid.GetIndex(CurColumn, CurRow));
                Cursor.TintColor = MovingColor;

                ElapsedMoveTime = 0d;
            }
            else if (newRow != CurRow && newRow >= 0 && newRow < PowerLiftGrid.Rows)
            {
                CurRow = newRow;

                PrevCursorPos = CurrentCursorPos;
                DestinationCursorPos = PowerLiftGrid.GetPositionAtIndex(PowerLiftGrid.GetIndex(CurColumn, CurRow));
                Cursor.TintColor = MovingColor;

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
            Cursor.TintColor = SelectedColor;
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
            Vector2 position = PowerLiftGrid.GetPositionAtIndex(PowerLiftGrid.GetIndex(gridCol, gridRow));

            //Create the icon element
            PowerLiftIconElement element = new PowerLiftIconElement((PowerLiftIcons)randIcon, position, IconFadeTime, IconStayTime, .45f);
            IconGrid[gridCol][gridRow] = element;
        }

        protected override void OnDraw()
        {
            //Draw the grid
            for (int i = 0; i < IconGrid.Length; i++)
            {
                for (int j = 0; j < IconGrid[i].Length; j++)
                {
                    PowerLiftIconElement iconElement = IconGrid[i][j];

                    //Draw the icon elements
                    if (iconElement != null)
                    {
                        iconElement.Draw();
                    }
                }
            }

            //Draw the boosts
            DrawBoosts();

            //Draw time remaining (debug)
            SpriteRenderer.Instance.DrawText(AssetManager.Instance.TTYDFont, Math.Round(CommandTime - ElapsedCommandTime, 0).ToString(), new Vector2(250, 130), Color.White, .7f);
        }

        private void DrawBoosts()
        {
            //Set up variables
            int cellSize = (int)(PowerLiftGrid.CellSize.Y);
            Vector2 arrowScale = new Vector2(.75f, .65f);
            Vector2 barScale = new Vector2(80, 1);

            //Depth values
            float arrowDepth = .4f;
            float barDepth = .41f;
            float barFillDepth = .42f;
            float boostTextDepth = .43f;

            float attackArrowY = PowerLiftGrid.Position.Y - cellSize;
            float defenseArrowY = PowerLiftGrid.Position.Y + cellSize;

            Vector2 fillScaleOffset = new Vector2(4, 18);

            bool drawAttackArrow = true;
            bool drawDefenseArrow = true;

            #region Arrow Blinking Interval Logic

            //See if the arrows should blink
            //First check if we recently obtained an Attack and/or Defense boost
            const double blinkTimesTwo = ArrowBlinkInterval * 2d;
            double attackBlink = LastAttackBoost + ArrowBlinkTotalTime;
            double defenseBlink = LastDefenseBoost + ArrowBlinkTotalTime;

            //We recently obtained an Attack boost
            if (attackBlink > Time.ActiveMilliseconds)
            {
                //Get the difference in time, then mod it with 2 times the interval (first half is the blinking, second half is visible)
                double timeDiff = Time.ActiveMilliseconds - LastAttackBoost;
                double attackBlinkMod = timeDiff % blinkTimesTwo;

                //We're at the blinking part of the interval, so don't draw the arrow
                if (attackBlinkMod < ArrowBlinkInterval)
                {
                    drawAttackArrow = false;
                }
            }

            //We recently obtained a Defense boost
            if (defenseBlink > Time.ActiveMilliseconds)
            {
                //Get the difference in time, then mod it with 2 times the interval (first half is the blinking, second half is visible)
                double timeDiff = Time.ActiveMilliseconds - LastDefenseBoost;
                double defenseBlinkMod = timeDiff % blinkTimesTwo;

                //We're at the blinking part of the interval, so don't draw the arrow
                if (defenseBlinkMod < ArrowBlinkInterval)
                {
                    drawDefenseArrow = false;
                }
            }

            #endregion

            //Draw the stat arrows - Attack is on top while Defense is on the bottom
            if (drawAttackArrow == true)
            {
                SpriteRenderer.Instance.Draw(ArrowIcon.Tex, new Vector2(50, attackArrowY), ArrowIcon.SourceRect, Color.Red, 0f, Vector2.Zero, arrowScale, false, false, arrowDepth, true);
                SpriteRenderer.Instance.DrawText(AssetManager.Instance.TTYDFont, $"+{AttackBoosts}", new Vector2(50, attackArrowY + 15f), Color.White, 0f, Vector2.Zero, 1f, boostTextDepth, true);
            }

            if (drawDefenseArrow == true)
            {
                SpriteRenderer.Instance.Draw(ArrowIcon.Tex, new Vector2(50, defenseArrowY), ArrowIcon.SourceRect, Color.Blue, 0f, Vector2.Zero, arrowScale, false, false, arrowDepth, true);
                SpriteRenderer.Instance.DrawText(AssetManager.Instance.TTYDFont, $"+{DefenseBoosts}", new Vector2(50, defenseArrowY + 15f), Color.White, 0f, Vector2.Zero, 1f, boostTextDepth, true);
            }

            //Draw the bars - one for each stat arrow
            //Attack bar
            Vector2 attackBarPos = new Vector2(110, attackArrowY + 5);
            float attackFillScale = AttackSelections / (float)AttackBoostReq;

            SpriteRenderer.Instance.Draw(BarEdge.Tex, attackBarPos, BarEdge.SourceRect, Color.White, false, false, barDepth, true);
            SpriteRenderer.Instance.Draw(Bar.Tex, attackBarPos + new Vector2(BarEdge.SourceRect.Value.Width, 0f), Bar.SourceRect, Color.White, 0f, Vector2.Zero, barScale, false, false, barDepth, true);
            SpriteRenderer.Instance.Draw(BarEdge.Tex, attackBarPos + new Vector2(barScale.X + BarEdge.SourceRect.Value.Width, 0f), BarEdge.SourceRect, Color.White, true, false, barDepth, true);
            SpriteRenderer.Instance.Draw(BarFill.Tex, attackBarPos + new Vector2(5f, 5f), BarFill.SourceRect, Color.Pink, 0f, Vector2.Zero, new Vector2(attackFillScale * (barScale.X + fillScaleOffset.X), fillScaleOffset.Y), false, false, barFillDepth, true);

            //Defense bar
            Vector2 defenseBarPos = new Vector2(110, defenseArrowY + 5);
            float defenseFillScale = DefenseSelections / (float)DefenseBoostReq;
            Color defenseFillColor = new Color(50, 170, 255);

            SpriteRenderer.Instance.Draw(BarEdge.Tex, defenseBarPos, BarEdge.SourceRect, Color.White, false, false, barDepth, true);
            SpriteRenderer.Instance.Draw(Bar.Tex, defenseBarPos + new Vector2(BarEdge.SourceRect.Value.Width, 0f), Bar.SourceRect, Color.White, 0f, Vector2.Zero, barScale, false, false, barDepth, true);
            SpriteRenderer.Instance.Draw(BarEdge.Tex, defenseBarPos + new Vector2(barScale.X + BarEdge.SourceRect.Value.Width, 0f), BarEdge.SourceRect, Color.White, true, false, barDepth, true);
            SpriteRenderer.Instance.Draw(BarFill.Tex, defenseBarPos + new Vector2(5f, 5f), BarFill.SourceRect, defenseFillColor, 0f, Vector2.Zero, new Vector2(defenseFillScale * (barScale.X + fillScaleOffset.X), fillScaleOffset.Y), false, false, barFillDepth, true);
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
