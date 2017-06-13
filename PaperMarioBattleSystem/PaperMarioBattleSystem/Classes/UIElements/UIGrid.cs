using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

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
                //Set position
                base.Position = value;

                //Reposition the grid after changing the position
                if (AutomaticReposition == true)
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
        /// The max number of elements that can be in the grid based on its size.
        /// </summary>
        public int MaxElementsInGrid => (Columns * Rows);

        /// <summary>
        /// The number of elements in the grid.
        /// </summary>
        public int NumElementsInGrid => (GridElements == null) ? 0 : GridElements.Count;

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
        /// The offset to render the grid at.
        /// This is set when changing the GridPivot.
        /// </summary>
        protected Vector2 PivotOffset = Vector2.Zero;

        /// <summary>
        /// The grid pivot.
        /// </summary>
        protected GridPivots GridPivot = GridPivots.UpperLeft;

        ///// <summary>
        ///// The pivot for the grid elements.
        ///// </summary>
        //protected GridPivots ElementPivot = GridPivots.UpperLeft;

        /// <summary>
        /// The PosUIElements in the grid. This is a list for performance reasons, as we can easily position a list in a grid-like manner.
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
            GetColumnRowFromIndex(index, out int xIndex, out int yIndex);

            Vector2 relativePos = new Vector2(xIndex * CellSize.X, yIndex * CellSize.Y);
            return relativePos;
        }

        /*/// <summary>
        /// Gets the position of a grid element at a particular index relative to the grid's position.
        /// </summary>
        /// <param name="index">The index of the grid element. This can be outside of the grid's range.</param>
        /// <param name="pivot">The pivot used to offset the elements in the grid.</param>
        /// <returns>The relative position of the grid element.</returns>
        protected Vector2 GetRelativePositionAtIndex(int index, GridPivots pivot)
        {
            GetColumnRowFromIndex(index, out int xIndex, out int yIndex);

            Vector2 elementPivotPos = GetElementOffsetForPivot(pivot);

            Vector2 relativePos = new Vector2(xIndex * CellSize.X, yIndex * CellSize.Y) - elementPivotPos;
            return relativePos;
        }*/

        /// <summary>
        /// Gets the position a grid element would be at a particular index.
        /// </summary>
        /// <param name="index">The index of the grid element. This can be outside of the grid's range.</param>
        /// <returns>The position of the grid element.</returns>
        public Vector2 GetPositionAtIndex(int index)
        {
            //Add the grid's Position with the relative position of the element at the index
            //Then subtract from the pivot offset
            Vector2 posToDraw = (Position + GetRelativePositionAtIndex(index)) - PivotOffset;

            return posToDraw;
        }

        /// <summary>
        /// Changes the GridPivot of the grid.
        /// </summary>
        /// <param name="pivot">The GridPivot to change to.</param>
        public void ChangeGridPivot(GridPivots pivot)
        {
            GridPivot = pivot;

            //Update the offset
            PivotOffset = GetOffsetFromPivot(GridPivot);
            if (AutomaticReposition == true)
            {
                RepositionGridElements();
            }
        }

        /*/// <summary>
        /// Changes the ElementPivot of the grid.
        /// </summary>
        /// <param name="pivot">The GridPivot to change to.</param>
        public void ChangeElementPivot(GridPivots pivot)
        {
            ElementPivot = pivot;
            if (AutomaticReposition == true)
            {
                RepositionGridElements();
            }
        }*/

        /// <summary>
        /// Gets the offset at a particular pivot.
        /// </summary>
        /// <param name="pivot">The pivot to get the offset for.</param>
        /// <returns>A Vector2 of the offset for the pivot.</returns>
        protected Vector2 GetOffsetFromPivot(GridPivots pivot)
        {
            //Store these quick-access values, as this makes it more readable and easier to modify
            //The indices are convereted to be zero-based
            int colRight = (Columns - 1);
            int colCenter = colRight / 2;
            int rowBottom = (Rows - 1);
            int rowCenter = rowBottom / 2;

            switch(pivot)
            {
                case GridPivots.UpperCenter: return GetRelativePositionAtIndex(GetIndex(colCenter, 0));
                case GridPivots.UpperRight: return GetRelativePositionAtIndex(GetIndex(colRight, 0));
                case GridPivots.CenterLeft: return GetRelativePositionAtIndex(GetIndex(0, rowCenter));
                case GridPivots.Center: return GetRelativePositionAtIndex(GetIndex(colCenter, rowCenter));
                case GridPivots.CenterRight: return GetRelativePositionAtIndex(GetIndex(colRight, rowCenter));
                case GridPivots.BottomLeft: return GetRelativePositionAtIndex(GetIndex(0, rowBottom));
                case GridPivots.BottomCenter: return GetRelativePositionAtIndex(GetIndex(colCenter, rowBottom));
                case GridPivots.BottomRight: return GetRelativePositionAtIndex(GetIndex(colRight, rowBottom));
                case GridPivots.UpperLeft:
                default: return GetRelativePositionAtIndex(GetIndex(0, 0));
            }
        }

        /*protected Vector2 GetElementOffsetForPivot(GridPivots pivot)
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
                default: return new Vector2(0f, 0f);
            }
        }*/

        public override void Update()
        {

        }

        public override void Draw()
        {
            for (int i = 0; i < GridElements.Count; i++)
            {
                GridElements[i].Draw();
            }
        }
    }
}
