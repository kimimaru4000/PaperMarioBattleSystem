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
    /// <para>The grid adjusts the elements whenever it is changed in some way.</para>
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
        /// The PosUIElements in the grid.
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

        public void RemoveGridElement(PosUIElement posUIElement)
        {
            bool removed = GridElements.Remove(posUIElement);
            if (removed == true)
            {
                RepositionGridElements();
            }
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
        /// Gets the position a grid element would be at a particular index.
        /// </summary>
        /// <param name="index">The index of the grid element.</param>
        /// <returns>The position of the grid at the element.</returns>
        protected Vector2 GetPositionAtIndex(int index)
        {
            int xIndex = index % Columns;
            int yIndex = index / Rows;

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
