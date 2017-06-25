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
    public sealed class PowerLiftCommand : ActionCommand
    {
        #region Enums

        private enum PowerLiftIcons
        {
            None = 0,
            Poison = 1,
            Attack = 2,
            Defense = 3
        }

        #endregion

        private int NumColumns = 3;
        private int NumRows = 3;
        private Vector2 LiftGridCellSize = new Vector2(26, 24);
        private Vector2 LiftGridSpacing = new Vector2(52, 48);

        private double CommandTime = 30000d;
        private int CursorSpeed = 3;
        private Vector2 CurrentCursorPos { get => Cursor.Position; set => Cursor.Position = value; }
        private Vector2 DestinationCursorPos = Vector2.Zero;
        private Color CursorColor = Color.White;
        private Color MovingColor = Color.Blue;

        private float PoisonFactor = .5f;
        private double PoisonDur = 2000d;

        private CroppedTexture2D BigCursor = null;
        private CroppedTexture2D SmallCursor = null;

        private UIFourPiecedTex Cursor = null;

        /// <summary>
        /// The grid used for laying out the objects.
        /// </summary>
        private UIGrid PowerLiftGrid = null;

        /// <summary>
        /// The internal grid used for tracking the icons.
        /// </summary>
        private PowerLiftIcons[][] IconGrid = null;

        /// <summary>
        /// Tells whether the player can select an arrow with the cursor.
        /// The cursor cannot select while it is moving to another spot on the grid.
        /// </summary>
        private bool CanSelect => (CurrentCursorPos == DestinationCursorPos);

        public PowerLiftCommand(IActionCommandHandler commandHandler, double commandTime) : base(commandHandler)
        {
            Texture2D battleGFX = AssetManager.Instance.LoadAsset<Texture2D>($"{ContentGlobals.UIRoot}/Battle/BattleGFX");

            BigCursor = new CroppedTexture2D(battleGFX, new Rectangle(14, 273, 46, 46));
            SmallCursor = new CroppedTexture2D(battleGFX, new Rectangle(10, 330, 13, 12));//new CroppedTexture2D(AssetManager.Instance.LoadAsset<Texture2D>($"{ContentGlobals.UIRoot}/Debug/BoxOutline2"), null);

            Cursor = new UIFourPiecedTex(BigCursor, LiftGridCellSize / 2, .6f, CursorColor);

            CommandTime = commandTime;
        }

        public override void StartInput(params object[] values)
        {
            base.StartInput(values);

            //Set up the grid
            SetUpGrid();

            BattleUIManager.Instance.AddUIElement(PowerLiftGrid);
            BattleUIManager.Instance.AddUIElement(Cursor);

            //Center the cursor in the middle
            int centerIndex = PowerLiftGrid.GetIndex(PowerLiftGrid.Columns / 2, PowerLiftGrid.Rows / 2);
            CurrentCursorPos = DestinationCursorPos = PowerLiftGrid.GetPositionAtIndex(centerIndex);
        }

        public override void EndInput()
        {
            base.EndInput();

            BattleUIManager.Instance.RemoveUIElement(PowerLiftGrid);
            BattleUIManager.Instance.RemoveUIElement(Cursor);

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
                //Offset their position since they're being drawn in 4 pieces centered about an origin
                PowerLiftGrid.AddGridElement(new UIFourPiecedTex(SmallCursor.Copy(), LiftGridCellSize / 2, .5f, Color.White));
            }

            //Although the grid is drawn on the UI layer, we center it using the sprite layer's (0, 0) position for ease
            PowerLiftGrid.Position = Camera.Instance.SpriteToUIPos(Vector2.Zero);
            PowerLiftGrid.ChangeGridPivot(UIGrid.GridPivots.Center);

            PowerLiftGrid.Spacing = LiftGridSpacing;

            //Initialize the icon grid
            UtilityGlobals.InitializeJaggedArray(IconGrid, PowerLiftGrid.Columns, PowerLiftGrid.Rows);
        }

        protected override void ReadInput()
        {
            
        }

        protected override void OnDraw()
        {
            
        }
    }
}
