using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// A grid that draws CroppedTexture2Ds.
    /// </summary>
    public class UIGrid : UIElement
    {
        /// <summary>
        /// The position of the grid. Elements are rendered in the top-left.
        /// </summary>
        public Vector2 Position = Vector2.Zero;

        /// <summary>
        /// The size of each cell in the grid.
        /// </summary>
        public Vector2 CellSize = new Vector2(32, 32);

        /// <summary>
        /// The number of columns in the grid.
        /// </summary>
        public int Columns = 2;

        /// <summary>
        /// The number of rows in the grid.
        /// </summary>
        public int Rows = 2;

        /// <summary>
        /// The CroppedTexture2Ds in the grid.
        /// </summary>
        protected List<CroppedTexture2D> GridElements = null;

        public UIGrid(int columns, int rows, Vector2 cellSize)
        {
            AdjustSize(columns, rows, cellSize);

            GridElements = new List<CroppedTexture2D>(Columns * Rows);
        }

        public void AddGridElement(CroppedTexture2D croppedTex)
        {
            if (croppedTex == null)
            {
                Debug.LogError($"Attempting to add null {nameof(CroppedTexture2D)} to the {nameof(UIGrid)}!");
                return;
            }

            GridElements.Add(croppedTex);
        }

        public void RemoveGridElement(CroppedTexture2D croppedTex)
        {
            GridElements.Remove(croppedTex);
        }

        public void AdjustSize(int columns, int rows, Vector2 cellSize)
        {
            Columns = columns;
            Rows = rows;
            CellSize = cellSize;
        }

        public override void Update()
        {

        }

        public override void Draw()
        {
            for (int i = 0; i < GridElements.Count; i++)
            {
                int xIndex = i % Columns;
                int yIndex = i / Rows;

                Vector2 posToDraw = Position + new Vector2(xIndex * CellSize.X, yIndex * CellSize.Y);

                SpriteRenderer.Instance.Draw(GridElements[i].Tex, posToDraw, GridElements[i].SourceRect, Color.White, false, false, .4f, true);
            }
        }
    }
}
