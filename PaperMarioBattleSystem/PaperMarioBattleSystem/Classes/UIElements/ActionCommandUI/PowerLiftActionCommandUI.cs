using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// Handles the UI aspect of Power Lift.
    /// </summary>
    public sealed class PowerLiftActionCommandUI : ActionCommandUI<PowerLiftCommand>
    {
        /// <summary>
        /// The total time the Attack/Defense boost display arrows spend in the blinking interval after gaining a boost.
        /// </summary>
        private const double ArrowBlinkTotalTime = 1000d;

        /// <summary>
        /// The interval at which the Attack/Defense boost display arrows blink after gaining a boost.
        /// </summary>
        private const double ArrowBlinkInterval = ArrowBlinkTotalTime / 10d;

        private Color CursorColor = Color.White;

        private CroppedTexture2D BigCursor = null;
        private CroppedTexture2D SmallCursor = null;
        private CroppedTexture2D ArrowIcon = null;
        private CroppedTexture2D BarEdge = null;
        private CroppedTexture2D Bar = null;
        private CroppedTexture2D BarFill = null;

        private UIFourPiecedTex Cursor = null;

        private Dictionary<PowerLiftCommand.PowerLiftIcons, CroppedTexture2D> IconGraphics = new Dictionary<PowerLiftCommand.PowerLiftIcons, CroppedTexture2D>();

        /// <summary>
        /// The grid used for laying out the objects.
        /// </summary>
        private UIGrid PowerLiftGrid = null;

        private Vector2 PrevCursorPos = Vector2.Zero;

        public PowerLiftActionCommandUI(PowerLiftCommand powerLiftCommand) : base(powerLiftCommand)
        {
            Texture2D battleGFX = AssetManager.Instance.LoadRawTexture2D($"{ContentGlobals.UIRoot}/Battle/BattleGFX.png");

            BigCursor = new CroppedTexture2D(battleGFX, new Rectangle(14, 273, 46, 46));
            SmallCursor = new CroppedTexture2D(battleGFX, new Rectangle(10, 330, 13, 12));
            ArrowIcon = new CroppedTexture2D(battleGFX, new Rectangle(5, 353, 50, 61));
            BarEdge = new CroppedTexture2D(battleGFX, new Rectangle(514, 245, 7, 28));
            Bar = new CroppedTexture2D(battleGFX, new Rectangle(530, 245, 1, 28));
            BarFill = new CroppedTexture2D(battleGFX, new Rectangle(541, 255, 1, 1));

            Cursor = new UIFourPiecedTex(BigCursor, BigCursor.WidthHeightToVector2(), .6f, CursorColor);

            SetUpGrid();
            InitIconGraphics();
        }

        public override void Update()
        {
            if (ActionCmd.AcceptingInput == false) return;

            //If we can select it, set and store the current position of the cursor
            if (ActionCmd.CanSelect == true)
            {
                PrevCursorPos = Cursor.Position = PowerLiftGrid.GetPositionAtIndex(PowerLiftGrid.GetIndex(ActionCmd.CurColumn, ActionCmd.CurRow));
            }
            else
            {
                //Lerp the cursor to the destination
                double speed = ActionCmd.CursorSpeedDur;
                if (ActionCmd.IsPoisoned == true)
                {
                    speed = ActionCmd.PoisonSpeedDur;
                }

                Vector2 destPos = PowerLiftGrid.GetPositionAtIndex(PowerLiftGrid.GetIndex(ActionCmd.DestColumn, ActionCmd.DestRow));
                Cursor.Position = Vector2.Lerp(PrevCursorPos, destPos, (float)(ActionCmd.ElapsedMoveTime / speed));
            }

            Cursor.TintColor = ActionCmd.CursorColor;

            PowerLiftGrid.Update();
        }

        private void SetUpGrid()
        {
            //Create the grid
            PowerLiftGrid = new UIGrid(ActionCmd.NumColumns, ActionCmd.NumRows, ActionCmd.LiftGridCellSize);

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

            Vector2 paddingSize = ActionCmd.LiftGridCellSize / 2;

            PowerLiftGrid.ChangeGridPadding(0, (int)paddingSize.X, 0, (int)paddingSize.Y);

            PowerLiftGrid.Spacing = ActionCmd.LiftGridSpacing;
        }

        private void InitIconGraphics()
        {
            Texture2D battleGFX = AssetManager.Instance.LoadRawTexture2D($"{ContentGlobals.UIRoot}/Battle/BattleGFX.png");

            CroppedTexture2D arrowTex = new CroppedTexture2D(battleGFX, new Rectangle(5, 353, 50, 61));

            IconGraphics.Add(PowerLiftCommand.PowerLiftIcons.None, null);
            IconGraphics.Add(PowerLiftCommand.PowerLiftIcons.Poison, new CroppedTexture2D(battleGFX, new Rectangle(90, 270, 106, 108)));
            IconGraphics.Add(PowerLiftCommand.PowerLiftIcons.Attack, arrowTex);
            IconGraphics.Add(PowerLiftCommand.PowerLiftIcons.Defense, arrowTex);
        }

        public override void Draw()
        {
            if (ActionCmd.AcceptingInput == false) return;

            PowerLiftGrid.Draw();
            Cursor.Draw();

            DrawGrid();

            //Draw the boosts
            DrawBoosts();

            //Draw time remaining (debug)
            SpriteRenderer.Instance.DrawUIText(AssetManager.Instance.TTYDFont, Math.Round(ActionCmd.CommandTime - ActionCmd.ElapsedCommandTime, 0).ToString(), new Vector2(250, 130), Color.White, .7f);
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
            double attackBlink = ActionCmd.LastAttackBoost + ArrowBlinkTotalTime;
            double defenseBlink = ActionCmd.LastDefenseBoost + ArrowBlinkTotalTime;

            //We recently obtained an Attack boost
            if (attackBlink > Time.ActiveMilliseconds)
            {
                //Get the difference in time, then mod it with 2 times the interval (first half is the blinking, second half is visible)
                double timeDiff = Time.ActiveMilliseconds - ActionCmd.LastAttackBoost;
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
                double timeDiff = Time.ActiveMilliseconds - ActionCmd.LastDefenseBoost;
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
                SpriteRenderer.Instance.DrawUI(ArrowIcon.Tex, new Vector2(50, attackArrowY), ArrowIcon.SourceRect, Color.Red, 0f, Vector2.Zero, arrowScale, false, false, arrowDepth);
                SpriteRenderer.Instance.DrawUIText(AssetManager.Instance.TTYDFont, $"+{ActionCmd.AttackBoosts}", new Vector2(50, attackArrowY + 15f), Color.White, 0f, Vector2.Zero, 1f, boostTextDepth);
            }

            if (drawDefenseArrow == true)
            {
                SpriteRenderer.Instance.DrawUI(ArrowIcon.Tex, new Vector2(50, defenseArrowY), ArrowIcon.SourceRect, Color.Blue, 0f, Vector2.Zero, arrowScale, false, false, arrowDepth);
                SpriteRenderer.Instance.DrawUIText(AssetManager.Instance.TTYDFont, $"+{ActionCmd.DefenseBoosts}", new Vector2(50, defenseArrowY + 15f), Color.White, 0f, Vector2.Zero, 1f, boostTextDepth);
            }

            //Draw the bars - one for each stat arrow
            //Attack bar
            Vector2 attackBarPos = new Vector2(110, attackArrowY + 5);
            float attackFillScale = ActionCmd.AttackSelections / (float)PowerLiftCommand.AttackBoostReq;

            SpriteRenderer.Instance.DrawUI(BarEdge.Tex, attackBarPos, BarEdge.SourceRect, Color.White, false, false, barDepth);
            SpriteRenderer.Instance.DrawUI(Bar.Tex, attackBarPos + new Vector2(BarEdge.SourceRect.Value.Width, 0f), Bar.SourceRect, Color.White, 0f, Vector2.Zero, barScale, false, false, barDepth);
            SpriteRenderer.Instance.DrawUI(BarEdge.Tex, attackBarPos + new Vector2(barScale.X + BarEdge.SourceRect.Value.Width, 0f), BarEdge.SourceRect, Color.White, true, false, barDepth, true);
            SpriteRenderer.Instance.DrawUI(BarFill.Tex, attackBarPos + new Vector2(5f, 5f), BarFill.SourceRect, Color.Pink, 0f, Vector2.Zero, new Vector2(attackFillScale * (barScale.X + fillScaleOffset.X), fillScaleOffset.Y), false, false, barFillDepth);

            //Defense bar
            Vector2 defenseBarPos = new Vector2(110, defenseArrowY + 5);
            float defenseFillScale = ActionCmd.DefenseSelections / (float)PowerLiftCommand.DefenseBoostReq;
            Color defenseFillColor = new Color(50, 170, 255);

            SpriteRenderer.Instance.DrawUI(BarEdge.Tex, defenseBarPos, BarEdge.SourceRect, Color.White, false, false, barDepth);
            SpriteRenderer.Instance.DrawUI(Bar.Tex, defenseBarPos + new Vector2(BarEdge.SourceRect.Value.Width, 0f), Bar.SourceRect, Color.White, 0f, Vector2.Zero, barScale, false, false, barDepth);
            SpriteRenderer.Instance.DrawUI(BarEdge.Tex, defenseBarPos + new Vector2(barScale.X + BarEdge.SourceRect.Value.Width, 0f), BarEdge.SourceRect, Color.White, true, false, barDepth);
            SpriteRenderer.Instance.DrawUI(BarFill.Tex, defenseBarPos + new Vector2(5f, 5f), BarFill.SourceRect, defenseFillColor, 0f, Vector2.Zero, new Vector2(defenseFillScale * (barScale.X + fillScaleOffset.X), fillScaleOffset.Y), false, false, barFillDepth);
        }

        private void DrawGrid()
        {
            //Draw the grid
            for (int i = 0; i < ActionCmd.IconGrid.Length; i++)
            {
                for (int j = 0; j < ActionCmd.IconGrid[i].Length; j++)
                {
                    PowerLiftIconElement iconElement = ActionCmd.IconGrid[i][j];

                    //Draw the icon elements
                    if (iconElement != null)
                    {
                        CroppedTexture2D croppedTex = IconGraphics[iconElement.PowerliftIcon];

                        if (croppedTex != null)
                        {
                            Vector2 position = PowerLiftGrid.GetPositionAtIndex(PowerLiftGrid.GetIndex(i, j));
                            SpriteRenderer.Instance.DrawUI(croppedTex.Tex, position, croppedTex.SourceRect, iconElement.TintColor, 0f, new Vector2(.5f, .5f), iconElement.Scale, false, false, iconElement.Depth);
                        }
                    }
                }
            }
        }
    }
}
