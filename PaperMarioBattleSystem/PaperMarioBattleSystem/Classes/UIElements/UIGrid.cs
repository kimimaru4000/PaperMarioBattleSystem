//Comment this out to disable debug inputs and drawing for the grid in Debug builds
#if DEBUG
    //#define GRID_DEBUG
#endif

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
    /// A grid holding <see cref="PosUIElement"/>s.
    /// <para>By default the grid repositions the elements when it is modified in some way (Ex. Rows, Columns, CellSize).
    /// To change this behavior, set <see cref="AutomaticReposition"/> to false.
    /// In this case, the grid will need to be manually repositioned with <see cref="RepositionGridElements"/> after changes have been made.</para>
    /// </summary>
    public class UIGrid : PosUIElement
    {
        /// <summary>
        /// The types of pivots for the grid.
        /// </summary>
        public enum GridPivots
        {
            UpperLeft,
            UpperCenter,
            UpperRight,
            CenterLeft,
            Center,
            CenterRight,
            BottomLeft,
            BottomCenter,
            BottomRight
        }

        /// <summary>
        /// Whether to automatically reposition the elements after any changes or not
        /// </summary>
        public bool AutomaticReposition = true;

        public override Vector2 Position
        {
            set
            {
                Vector2 prevPos = Position;
                //Set position
                base.Position = value;

                //Reposition the grid after changing the position
                if (AutomaticReposition == true && prevPos != Position)
                    RepositionGridElements();
            }
        }

        /// <summary>
        /// A property for the size of each cell in the grid.
        /// </summary>
        public Vector2 CellSize
        {
            get => GridCellSize;
            set
            {
                Vector2 prevCellSize = GridCellSize;
                GridCellSize = value;

                //Reposition the grid if the value is different
                if (AutomaticReposition == true && prevCellSize != GridCellSize)
                {
                    RepositionGridElements();
                }
            }
        }

        /// <summary>
        /// A property for the number of columns in the grid.
        /// </summary>
        public int Columns
        {
            get => GridColumns;
            set
            {
                int prevCols = GridColumns;
                GridColumns = value;

                //Reposition the grid if the value is different
                if (AutomaticReposition == true && prevCols != GridColumns)
                {
                    RepositionGridElements();
                }
            }
        }

        /// <summary>
        /// A property for the number of rows in the grid.
        /// </summary>
        public int Rows
        {
            get => GridRows;
            set
            {
                int prevRows = GridRows;
                GridRows = value;

                //Reposition the grid if the value is different
                if (AutomaticReposition == true && prevRows != GridRows)
                {
                    RepositionGridElements();
                }
            }
        }

        /// <summary>
        /// The spacing of the grid elements.
        /// </summary>
        public Vector2 Spacing
        {
            get => GridElementSpacing;
            set
            {
                Vector2 prevSpacing = GridElementSpacing;
                GridElementSpacing = value;

                if (AutomaticReposition == true && prevSpacing != GridElementSpacing)
                {
                    RepositionGridElements();
                }
            }
        }

        /// <summary>
        /// The max number of elements that can be in the grid based on its size.
        /// </summary>
        public int MaxElementsInGrid => (Columns * Rows);

        /// <summary>
        /// The number of elements in the grid.
        /// </summary>
        public int NumElementsInGrid => (GridElements == null) ? 0 : GridElements.Count;

        /// <summary>
        /// The total size of the grid. This does not take into account the pivot.
        /// </summary>
        public Vector2 GridSize => new Vector2(Columns * CellSize.X, Rows * CellSize.Y);

        /// <summary>
        /// The bounds of the grid. This does not take into account the pivot.
        /// <para>This is primarily for debugging.</para>
        /// </summary>
        protected Rectangle GridBounds
        {
            get
            {
                Vector2 gridSize = GridSize;

                return new Rectangle((int)Position.X, (int)Position.Y, (int)gridSize.X, (int)gridSize.Y);
            }
        }

        /// <summary>
        /// The size of each cell in the grid.
        /// </summary>
        protected Vector2 GridCellSize = new Vector2(32, 32);

        /// <summary>
        /// The number of columns in the grid.
        /// </summary>
        protected int GridColumns = 2;

        /// <summary>
        /// The number of rows in the grid.
        /// </summary>
        protected int GridRows = 2;

        /// <summary>
        /// The grid pivot.
        /// </summary>
        public GridPivots GridPivot { get; protected set; } = GridPivots.UpperLeft;

        /// <summary>
        /// The pivot for the grid elements.
        /// </summary>
        public GridPivots ElementPivot { get; protected set; } = GridPivots.UpperLeft;

        /// <summary>
        /// The amount to space the grid elements away from each other.
        /// </summary>
        protected Vector2 GridElementSpacing = Vector2.Zero;

        /// <summary>
        /// The padding of the grid.
        /// </summary>
        protected Padding GridPadding = new Padding();

        /// <summary>
        /// The PosUIElements in the grid.
        /// This is a list for performance/usability reasons, as we can easily position a list in a grid-like manner.
        /// </summary>
        protected List<PosUIElement> GridElements = null;

        public UIGrid(int columns, int rows, Vector2 cellSize)
        {
            //Set the values directly instead of going through the properties
            //At this point there cannot be any elements in the grid, so bypass repositioning since it's unnecessary
            GridColumns = columns;
            GridRows = rows;
            GridCellSize = cellSize;

            GridElements = new List<PosUIElement>(Columns * Rows);
        }

        /// <summary>
        /// Adds an element to the grid.
        /// </summary>
        /// <param name="posUIElement">The PosUIElement to add to the grid.</param>
        public void AddGridElement(PosUIElement posUIElement)
        {
            if (posUIElement == null)
            {
                Debug.LogError($"Attempting to add null {nameof(PosUIElement)} to the {nameof(UIGrid)}!");
                return;
            }

            GridElements.Add(posUIElement);

            //Issue a warning saying to expand the grid if the number of elements is going over
            if (NumElementsInGrid > MaxElementsInGrid)
            {
                Debug.LogWarning($"The {nameof(UIGrid)} has {NumElementsInGrid} elements which exceeds the max of {MaxElementsInGrid}. "
                    + $"Please adjust the number of {nameof(Columns)} and {nameof(Rows)} when expanding the grid.");
            }

            if (AutomaticReposition == true)
            {
                RepositionGridElements();
            }
        }

        /// <summary>
        /// Removes an element from the grid.
        /// </summary>
        /// <param name="posUIElement">The PosUIElement to remove from the grid.</param>
        public void RemoveGridElement(PosUIElement posUIElement)
        {
            bool removed = GridElements.Remove(posUIElement);
            if (AutomaticReposition == true && removed == true)
            {
                RepositionGridElements();
            }
        }

        /// <summary>
        /// Removes an element from the grid.
        /// </summary>
        /// <param name="index">The index of the element to remove from the grid.</param>
        public void RemoveGridElement(int index)
        {
            RemoveGridElement(GetGridElement(index));
        }

        /// <summary>
        /// Removes an element from the grid.
        /// </summary>
        /// <param name="column">The zero-based column number of the element.</param>
        /// <param name="row">The zero-based row number of the element.</param>
        public void RemoveGridElement(int column, int row)
        {
            RemoveGridElement(GetGridElement(column, row));
        }

        /// <summary>
        /// Clears the grid by removing all elements from it.
        /// </summary>
        public void ClearGrid()
        {
            GridElements.Clear();
        }

        /// <summary>
        /// Repositions the elements in the grid.
        /// </summary>
        public void RepositionGridElements()
        {
            //Check for null - this should only be possible in the constructor
            if (GridElements == null)
                return;

            for (int i = 0; i < GridElements.Count; i++)
            {
                GridElements[i].Position = GetPositionAtIndex(i);
            }
        }

        /// <summary>
        /// Returns an index in the grid from column and row numbers.
        /// </summary>
        /// <param name="column">The zero-based column of the grid.</param>
        /// <param name="row">The zero-based row of the grid.</param>
        /// <returns>-1 if the column or row is out of the grid's range, otherwise an index</returns>
        public int GetIndex(int column, int row)
        {
            if (column < 0 || column >= Columns || row < 0 || row >= Rows)
            {
                Debug.LogWarning($"Column {column} or Row {row} is out of the grid's range!");
                return -1;
            }

            //Return the row times the total number of Columns and offset by the supplied column
            int index = (row * Columns) + column;
            return index;
        }

        /// <summary>
        /// Returns zero-based column and row numbers from an index in the grid.
        /// </summary>
        /// <param name="index">The index to retrieve the zero-based column and row numbers for.</param>
        /// <param name="column">An out integer that will be the zero-based column number. -1 if the grid has 0 or fewer Columns.</param>
        /// <param name="row">An out integer that will be the zero-based row number. -1 if the grid has 0 or fewer Columns.</param>
        public void GetColumnRowFromIndex(int index, out int column, out int row)
        {
            if (Columns <= 0)
            {
                Debug.LogWarning($"Max grid columns is {Columns} which is less than or equal to 0!");

                column = -1;
                row = -1;
                return;
            }

            //Perform Modulo to obtain the column number and division to obtain the row number
            column = index % Columns;
            row = index / Columns;
        }

        /// <summary>
        /// Returns the grid element at an index.
        /// </summary>
        /// <param name="index">The index to retrieve the element for.</param>
        /// <returns>null if the index is out of the grid's range, otherwise the element at the index.</returns>
        public PosUIElement GetGridElement(int index)
        {
            if (index < 0 || index >= GridElements.Count)
            {
                Debug.LogWarning($"index {index} is out of the grid's range!");
                return null;
            }

            return GridElements[index];
        }

        /// <summary>
        /// Returns the grid element at a particular column and row number.
        /// </summary>
        /// <param name="column">The zero-based column number.</param>
        /// <param name="row">The zero-based row number.</param>
        /// <returns>null if the column or index are out of the grid's range, otherwise the element at the index.</returns>
        public PosUIElement GetGridElement(int column, int row)
        {
            return GetGridElement(GetIndex(column, row));
        }

        /// <summary>
        /// Gets the position of a grid element at a particular index relative to the grid's position.
        /// This overload is used for convenience.
        /// </summary>
        /// <param name="index">The index of the grid element. This can be outside of the grid's range.</param>
        /// <returns>The relative position of the grid element.</returns>
        protected Vector2 GetRelativePositionAtIndex(int index)
        {
            return GetRelativePositionAtIndex(index, GridPivots.UpperLeft);
        }

        /// <summary>
        /// Gets the position of a grid element at a particular index relative to the grid's position.
        /// </summary>
        /// <param name="index">The index of the grid element. This can be outside of the grid's range.</param>
        /// <param name="pivot">The pivot used to offset the elements in the grid.</param>
        /// <returns>The relative position of the grid element.</returns>
        protected Vector2 GetRelativePositionAtIndex(int index, GridPivots pivot)
        {
            GetColumnRowFromIndex(index, out int xIndex, out int yIndex);

            Vector2 elementPivotPos = GetElementOffsetForPivot(pivot);

            //Use the GridPivot for the Spacing since it applies to the entire grid
            Vector2 spacingOffset = GetSpacingAtColumnRow(xIndex, yIndex, GridPivot);

            //Apply the padding
            Vector2 paddingOffset = GridPadding.TotalPadding;

            Vector2 relativePos = new Vector2(xIndex * CellSize.X, yIndex * CellSize.Y) - elementPivotPos + spacingOffset + paddingOffset;
            return relativePos;
        }

        /// <summary>
        /// Gets the position a grid element would be at a particular index.
        /// </summary>
        /// <param name="index">The index of the grid element. This can be outside of the grid's range.</param>
        /// <returns>The position of the grid element.</returns>
        public Vector2 GetPositionAtIndex(int index)
        {
            return GetPositionAtIndex(index, GridPivot, ElementPivot);
        }

        /// <summary>
        /// Gets the position a grid element would be at a particular index with a particular element and grid pivot.
        /// </summary>
        /// <param name="index">The index of the grid element. This can be outside of the grid's range.</param>
        /// <param name="elementPivot">The pivot used to offset the elements in the grid.</param>
        /// <param name="gridPivot">The pivot used to offset the grid.</param>
        /// <returns>The position of the grid element with the designated element and grid pivots.</returns>
        public Vector2 GetPositionAtIndex(int index, GridPivots gridPivot, GridPivots elementPivot)
        {
            //Add the grid's Position with the relative position of the element at the index
            //Then subtract from the pivot offset
            Vector2 relativePos = GetRelativePositionAtIndex(index, elementPivot);
            Vector2 offsetPos = GetOffsetFromPivot(gridPivot);
            Vector2 posToDraw = (Position + relativePos) - offsetPos;

            return posToDraw;
        }

        /// <summary>
        /// Changes the GridPivot of the grid.
        /// </summary>
        /// <param name="pivot">The GridPivot to change to.</param>
        public void ChangeGridPivot(GridPivots pivot)
        {
            GridPivots prevGridPivot = GridPivot;
            GridPivot = pivot;

            if (AutomaticReposition == true && prevGridPivot != GridPivot)
            {
                RepositionGridElements();
            }
        }

        /// <summary>
        /// Changes the ElementPivot of the grid.
        /// </summary>
        /// <param name="pivot">The GridPivot to change to.</param>
        public void ChangeElementPivot(GridPivots pivot)
        {
            GridPivots prevElementPivot = ElementPivot;
            ElementPivot = pivot;

            if (AutomaticReposition == true && prevElementPivot != ElementPivot)
            {
                RepositionGridElements();
            }
        }

        /// <summary>
        /// Changes the padding of the grid.
        /// </summary>
        /// <param name="left">The left padding.</param>
        /// <param name="right">The right padding.</param>
        /// <param name="top">The top padding.</param>
        /// <param name="bottom">The bottom padding.</param>
        public void ChangeGridPadding(int left, int right, int top, int bottom)
        {
            GridPadding.Left = left;
            GridPadding.Right = right;
            GridPadding.Top = top;
            GridPadding.Bottom = bottom;

            if (AutomaticReposition == true)
            {
                RepositionGridElements();
            }
        }

        /// <summary>
        /// Changes the padding of the grid relative to its original padding.
        /// </summary>
        /// <param name="left">The left padding.</param>
        /// <param name="right">The right padding.</param>
        /// <param name="top">The top padding.</param>
        /// <param name="bottom">The bottom padding.</param>
        public void ChangeGridPaddingRelative(int left, int right, int top, int bottom)
        {
            ChangeGridPadding(GridPadding.Left + left, GridPadding.Right + right, GridPadding.Top + top, GridPadding.Bottom + bottom);
        }

        /// <summary>
        /// Gets the offset of the grid at a particular pivot.
        /// </summary>
        /// <param name="pivot">The pivot to get the grid offset for.</param>
        /// <returns>A Vector2 of the grid offset for the pivot.</returns>
        protected Vector2 GetOffsetFromPivot(GridPivots pivot)
        {
            //Store these quick-access values, as this makes it more readable and easier to modify
            //The indices are converted to be zero-based
            float colRight = GridSize.X;
            float colCenter = (colRight / 2f);
            float rowBottom = GridSize.Y;
            float rowCenter = (rowBottom / 2f);

            switch(pivot)
            {
                case GridPivots.UpperCenter: return new Vector2(colCenter, 0f);
                case GridPivots.UpperRight: return new Vector2(colRight, 0f);
                case GridPivots.CenterLeft: return new Vector2(0f, rowCenter);
                case GridPivots.Center: return new Vector2(colCenter, rowCenter);
                case GridPivots.CenterRight: return new Vector2(colRight, rowCenter);
                case GridPivots.BottomLeft: return new Vector2(0f, rowBottom);
                case GridPivots.BottomCenter: return new Vector2(colCenter, rowBottom);
                case GridPivots.BottomRight: return new Vector2(colRight, rowBottom);
                case GridPivots.UpperLeft:
                default: return Vector2.Zero;
            }
        }

        /// <summary>
        /// Gets the offset for the grid elements at a particular pivot.
        /// </summary>
        /// <param name="pivot">The pivot to get the grid elements offset for.</param>
        /// <returns>A Vector2 of the grid elements offset for the pivot.</returns>
        protected Vector2 GetElementOffsetForPivot(GridPivots pivot)
        {
            //Store these quick-access values, as this makes it more readable and easier to modify
            float xRight = CellSize.X;
            float xMid = xRight / 2f;
            float yBottom = CellSize.Y;
            float yMid = yBottom / 2f;

            switch(pivot)
            {
                case GridPivots.UpperCenter: return new Vector2(xMid, 0f);
                case GridPivots.UpperRight: return new Vector2(xRight, 0f);
                case GridPivots.CenterLeft: return new Vector2(0f, yMid);
                case GridPivots.Center: return new Vector2(xMid, yMid);
                case GridPivots.CenterRight: return new Vector2(xRight, yMid);
                case GridPivots.BottomLeft: return new Vector2(0f, yBottom);
                case GridPivots.BottomCenter: return new Vector2(xMid, yBottom);
                case GridPivots.BottomRight: return new Vector2(xRight, yBottom);
                case GridPivots.UpperLeft:
                default: return Vector2.Zero;
            }
        }

        /// <summary>
        /// Gets the spacing of a grid element at a column and row number for a pivot.
        /// </summary>
        /// <param name="column">The zero-based column number.</param>
        /// <param name="row">The zero-based row number.</param>
        /// <param name="pivot">The pivot to get the spacing for.</param>
        /// <returns>A Vector2 of the spacing of the grid element for the pivot.</returns>
        protected Vector2 GetSpacingAtColumnRow(int column, int row, GridPivots pivot)
        {
            //Return on invalid input
            if (Columns <= 0 || Rows <= 0)
            {
                Debug.LogWarning($"{nameof(Columns)}:{Columns} or {nameof(Rows)}:{Rows} is less than or equal to 0!");
                return Vector2.Zero;
            }
            else if (column < 0 || row < 0)
            {
                Debug.LogWarning($"{nameof(column)}:{column} or {nameof(row)}:{row} is less than 0!");
                return Vector2.Zero;
            }

            Vector2 finalSpacing = Vector2.Zero;

            //The pivot column and pivot row
            //We offset the column and row by these to get the spacing
            //The further an element is from the pivot, the greater it will multiply the spacing value by
            float pivotCol = 0;
            float pivotRow = 0;
            
            //The column pivot for the left grid pivots is the leftmost element
            if (pivot == GridPivots.UpperLeft || pivot == GridPivots.CenterLeft || pivot == GridPivots.BottomLeft)
            {
                pivotCol = 0;
            }
            //The row pivot for the upper grid pivots is the uppermost element
            if (pivot == GridPivots.UpperLeft || pivot == GridPivots.UpperCenter || pivot == GridPivots.UpperRight)
            {
                pivotRow = 0;
            }

            //The column pivot for the right grid pivots is the rightmost element
            if (pivot == GridPivots.UpperRight || pivot == GridPivots.CenterRight || pivot == GridPivots.BottomRight)
            {
                pivotCol = (Columns - 1);
            }
            //The row pivot for the bottom grid pivots is the bottommost element
            if (pivot == GridPivots.BottomLeft || pivot == GridPivots.BottomCenter || pivot == GridPivots.BottomRight)
            {
                pivotRow = (Rows - 1);
            }

            //The column pivot for the center grid pivots is the centered element
            //In the event of an even number of columns, this will be halfway between the two middle columns (Ex: .5 for 4 columns)
            if (pivot == GridPivots.UpperCenter || pivot == GridPivots.Center || pivot == GridPivots.BottomCenter)
            {
                pivotCol = ((Columns - 1) / 2f);
            }
            //The row pivot for the center grid pivots is the centered element
            //In the event of an even number of rows, this will be halfway between the two middle rows (Ex: .5 for 4 rows)
            if (pivot == GridPivots.CenterLeft || pivot == GridPivots.Center || pivot == GridPivots.CenterRight)
            {
                pivotRow = ((Rows - 1) / 2f);
            }

            //Subtract from the pivot
            //The center two elements for even numbers of columns/rows will be separated from each other by half for centered pivots
            float pivotColDiff = (column - pivotCol);
            float pivotRowDiff = (row - pivotRow);

            finalSpacing.X = pivotColDiff * Spacing.X;
            finalSpacing.Y = pivotRowDiff * Spacing.Y;

            return finalSpacing;
        }

        public override void Update()
        {
            //Debug commands for testing

            #if GRID_DEBUG
            
                //Grid pivots
                if (Input.GetKeyDown(Keys.Q)) ChangeGridPivot(GridPivots.UpperLeft);
                else if (Input.GetKeyDown(Keys.W)) ChangeGridPivot(GridPivots.UpperCenter);
                else if (Input.GetKeyDown(Keys.E)) ChangeGridPivot(GridPivots.UpperRight);
                else if (Input.GetKeyDown(Keys.A)) ChangeGridPivot(GridPivots.CenterLeft);
                else if (Input.GetKeyDown(Keys.S)) ChangeGridPivot(GridPivots.Center);
                else if (Input.GetKeyDown(Keys.D)) ChangeGridPivot(GridPivots.CenterRight);
                else if (Input.GetKeyDown(Keys.Z)) ChangeGridPivot(GridPivots.BottomLeft);
                else if (Input.GetKeyDown(Keys.X)) ChangeGridPivot(GridPivots.BottomCenter);
                else if (Input.GetKeyDown(Keys.C)) ChangeGridPivot(GridPivots.BottomRight);

                //Element pivots
                if (Input.GetKeyDown(Keys.R)) ChangeElementPivot(GridPivots.UpperLeft);
                else if (Input.GetKeyDown(Keys.T)) ChangeElementPivot(GridPivots.UpperCenter);
                else if (Input.GetKeyDown(Keys.Y)) ChangeElementPivot(GridPivots.UpperRight);
                else if (Input.GetKeyDown(Keys.F)) ChangeElementPivot(GridPivots.CenterLeft);
                else if (Input.GetKeyDown(Keys.G)) ChangeElementPivot(GridPivots.Center);
                else if (Input.GetKeyDown(Keys.H)) ChangeElementPivot(GridPivots.CenterRight);
                else if (Input.GetKeyDown(Keys.V)) ChangeElementPivot(GridPivots.BottomLeft);
                else if (Input.GetKeyDown(Keys.B)) ChangeElementPivot(GridPivots.BottomCenter);
                else if (Input.GetKeyDown(Keys.N)) ChangeElementPivot(GridPivots.BottomRight);

                float spacingVal = 10f;

                //Spacing
                //if (Input.GetKeyDown(Keys.U)) Spacing = new Vector2(-spacingVal, -spacingVal);
                if (Input.GetKeyDown(Keys.I)) Spacing += new Vector2(0f, -spacingVal);
                //else if (Input.GetKeyDown(Keys.O)) Spacing = new Vector2(spacingVal, -spacingVal);
                else if (Input.GetKeyDown(Keys.J)) Spacing += new Vector2(-spacingVal, 0f);
                else if (Input.GetKeyDown(Keys.K)) Spacing = new Vector2(0f, 0f);
                else if (Input.GetKeyDown(Keys.L)) Spacing += new Vector2(spacingVal, 0f);
                else if (Input.GetKeyDown(Keys.M)) Spacing += new Vector2(0f, spacingVal);
                //else if (Input.GetKeyDown(Keys.OemComma)) Spacing = new Vector2(0f, spacingVal);
                //else if (Input.GetKeyDown(Keys.OemPeriod)) Spacing = new Vector2(spacingVal, spacingVal);

                if (AutomaticReposition == false)
                {
                    RepositionGridElements();
                }

            #endif
        }

        public override void Draw()
        {
            for (int i = 0; i < GridElements.Count; i++)
            {
                GridElements[i].Draw();
            }

            #if GRID_DEBUG
                DrawGridBounds();
            #endif
        }

        //NOTE: Use for debugging only
        private void DrawGridBounds()
        {
            Texture2D tex = AssetManager.Instance.LoadRawTexture2D($"{ContentGlobals.UIRoot}/Box.png");
            Rectangle rect = GridBounds;

            SpriteRenderer.Instance.Draw(tex, new Vector2(rect.X, rect.Y), null, Color.Blue, 0f,Vector2.Zero, new Vector2(rect.Width, rect.Height), false, false, .3f, true);
        }

        /// <summary>
        /// Padding used for the grid.
        /// </summary>
        public struct Padding
        {
            public int Left;
            public int Right;
            public int Top;
            public int Bottom;

            /// <summary>
            /// The total padding as a Vector2.
            /// </summary>
            public Vector2 TotalPadding => new Vector2(Right - Left, Bottom - Top);

            public Padding(int left, int right, int top, int bottom)
            {
                Left = left;
                Right = right;
                Top = top;
                Bottom = bottom;
            }
        }
    }
}
