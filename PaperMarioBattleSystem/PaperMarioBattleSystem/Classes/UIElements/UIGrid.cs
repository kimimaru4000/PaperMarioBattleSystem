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
    /// <para>The grid repositions the elements when it is modified in some way (Ex. Rows, Columns, CellSize).</para>
    /// </summary>
    public class UIGrid : PosUIElement
    {
        //NOTE: Elements are rendered starting from the top-left at the moment

        public override Vector2 Position
        {
            set
            {
                //Set position
                base.Position = value;

                //Reposition the grid after changing the position
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
                if (prevCellSize != GridCellSize)
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
                if (prevCols != GridColumns)
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
                if (prevRows != GridRows)
                {
                    RepositionGridElements();
                }
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
            RepositionGridElements();
        }

        /// <summary>
        /// Removes an element from the grid.
        /// </summary>
        /// <param name="posUIElement">The PosUIElement to remove from the grid.</param>
        public void RemoveGridElement(PosUIElement posUIElement)
        {
            bool removed = GridElements.Remove(posUIElement);
            if (removed == true)
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
        /// Repositions the elements in the grid.
        /// </summary>
        protected void RepositionGridElements()
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
        /// Gets the position a grid element would be at a particular index.
        /// </summary>
        /// <param name="index">The index of the grid element. This can be outside of the grid's range.</param>
        /// <returns>The position of the grid at the element.</returns>
        public Vector2 GetPositionAtIndex(int index)
        {
            GetColumnRowFromIndex(index, out int xIndex, out int yIndex);

            Vector2 posToDraw = Position + new Vector2(xIndex * CellSize.X, yIndex * CellSize.Y);

            return posToDraw;
        }

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
