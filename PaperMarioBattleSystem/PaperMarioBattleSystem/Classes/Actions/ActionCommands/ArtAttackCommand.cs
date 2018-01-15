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
    /// Move the star around to draw. Draw around enemies to damage them based on how much of their hurtbox is covered.
    /// <para>This is likely the most complex Action Command in the entire Paper Mario series.
    /// It only increases the Command Rank when you encircle all enemies with it, even if they're dead from the attack.</para>
    /// <para>Overlapping another line while it doesn't close causes it to clear the drawing. The color of the lines cycle
    /// constantly.</para>
    /// </summary>
    public sealed class ArtAttackCommand : ActionCommand
    {
        #region Color Fields

        /// <summary>
        /// The colors the lines cycle through.
        /// </summary>
        private readonly Color[] CycleColors = new Color[] { Color.White, Color.Brown, Color.Silver, Color.Gold, Color.Purple };

        /// <summary>
        /// The time it takes to fully lerp to the next color.
        /// </summary>
        private const double ColorDuration = 1000d;

        /// <summary>
        /// The amount of time elapsed
        /// </summary>
        private double ColorElapsedTime = 0d;

        /// <summary>
        /// The current color to lerp from. This is compared with the next color and wraps around to the first color upon reaching the end.
        /// </summary>
        private int ColorIndex = 0;

        #endregion

        /// <summary>
        /// How many frames max before a Line is created.
        /// </summary>
        private const int CreateLineFrames = 3;

        /// <summary>
        /// The speed at which the Star cursor moves when drawing.
        /// </summary>
        private const int StarSpeed = 3;

        /// <summary>
        /// The thickness of each line when drawing.
        /// </summary>
        private const float LineThickness = 1;

        /// <summary>
        /// The color of the lines.
        /// </summary>
        private Color LineColor = Color.White;

        /// <summary>
        /// The current number of elapsed frames.
        /// </summary>
        private int CurFrames = 0;

        /// <summary>
        /// The position of the Star cursor, or where you're currently drawing.
        /// </summary>
        private Vector2 StarPos = Vector2.Zero;
        
        /// <summary>
        /// The current velocity of the Star cursor.
        /// </summary>
        private Vector2 StarVelocity = Vector2.Zero;

        /// <summary>
        /// The previous position of the Star cursor.
        /// </summary>
        private Vector2 PrevStarPos = Vector2.Zero;
        
        /// <summary>
        /// The previous velocity of the Star cursor.
        /// </summary>
        private Vector2 PrevStarVelocity = Vector2.Zero;

        /// <summary>
        /// The point at which the next line starts.
        /// </summary>
        private Vector2 StartPoint = Vector2.Zero;

        /// <summary>
        /// The start point at which the next line is created.
        /// </summary>
        private Vector2 StartLinePoint = Vector2.Zero;

        /// <summary>
        /// Tells whether at least one line was added or not.
        /// </summary>
        private bool AddedLine = false;

        /// <summary>
        /// The amount of time to draw.
        /// This is strictly the draw time, so it excludes when you're unable to draw for a brief period after damaging enemies.
        /// </summary>
        public double DrawTime = 20000d;

        /// <summary>
        /// The amount of elapsed draw time.
        /// </summary>
        public double ElapsedDrawTime = 0d;

        /// <summary>
        /// The Lines to draw.
        /// </summary>
        private readonly List<Line> Lines = new List<Line>();

        /// <summary>
        /// The texture to display the lines.
        /// </summary>
        private Texture2D LineTexture = null;

        private Line? LastLine => Lines.Count == 0 ? null : (Line?)Lines[Lines.Count - 1];

        public ArtAttackCommand(IActionCommandHandler commandHandler, Vector2 startPos, double drawTime) : base(commandHandler)
        {
            StarPos = startPos;
            DrawTime = drawTime;
        }

        public override void StartInput(params object[] values)
        {
            base.StartInput(values);

            if (LineTexture == null)
            {
                LineTexture = AssetManager.Instance.LoadRawTexture2D($"{ContentGlobals.UIRoot}/Box.png");
            }

            AddedLine = false;

            StarVelocity = Vector2.Zero;
            PrevStarPos = StartPoint = StartLinePoint = StarPos;
            PrevStarVelocity = Vector2.Zero;
            Lines.Clear();
        }

        public override void EndInput()
        {
            base.EndInput();

            AddedLine = false;

            Lines.Clear();
        }

        /* 1. Track position, previous position, velocity, and previous velocity of the Star cursor
         * 2. If the velocity isn't 0, create a new Line from the start of the Star cursor after a few frames or if the velocity changed
         * 3. When this new Line is created, check for intersection with any of the other lines. If true, end the Action Command
         */
        protected override void ReadInput()
        {
            //End the Action Command with a Failure when time is up
            if (ElapsedDrawTime >= DrawTime)
            {
                OnComplete(CommandResults.Failure);
                return;
            }

            //Update the color of the lines
            CycleLineColors();

            //Increment time afterwards; this is how it seems to work in the game if you complete a circle right before it ends
            ElapsedDrawTime += Time.ElapsedMilliseconds;

            PrevStarVelocity = StarVelocity;
            PrevStarPos = StarPos;

            StarVelocity = Vector2.Zero;

            //Use any of the keys to move
            //NOTE: Place bounds on moving the cursor so you can't draw off the edge of the screen
            if (Input.GetKey(Keys.Up) == true)
            {
                StarVelocity.Y -= StarSpeed;
            }
            if (Input.GetKey(Keys.Down) == true)
            {
                StarVelocity.Y += StarSpeed;
            }
            if (Input.GetKey(Keys.Left) == true)
            {
                StarVelocity.X -= StarSpeed;
            }
            if (Input.GetKey(Keys.Right) == true)
            {
                StarVelocity.X += StarSpeed;
            }

            StarPos += StarVelocity;

            CurFrames++;
            if (CurFrames >= CreateLineFrames || (PrevStarVelocity != StarVelocity && PrevStarVelocity != Vector2.Zero))
            {
                CurFrames = 0;

                //Don't add new lines if the star cursor isn't moving
                if (StartPoint != PrevStarPos)
                {
                    //If we added at least one line, offset the start point of the next line so the lines don't automatically intersect
                    if (AddedLine == true)
                    {
                        //Get the previous velocity, as that indicates where the line is heading before it's created
                        Point change = new Point((int)PrevStarVelocity.X, (int)PrevStarVelocity.Y);
                        if (change.X != 0) change.X = (change.X < 0) ? -1 : 1;
                        if (change.Y != 0) change.Y = (change.Y < 0) ? -1 : 1;

                        StartLinePoint = new Vector2((int)StartPoint.X + change.X, (int)StartPoint.Y + change.Y);
                    }

                    AddNewLine();
                    AddedLine = true;
                }

                StartPoint = PrevStarPos;

                //Check for a completed shape
                int collisionIndex = CheckCompletedShape();
                if (collisionIndex >= 0)
                {
                    Rectangle rect = GetShapeBoundingBox(collisionIndex);
                    SendResponse(new ActionCommandGlobals.ArtAttackResponse(rect));
                    OnComplete(CommandResults.Success);
                }
            }
        }

        /// <summary>
        /// Adds a new Line to the player's drawing.
        /// </summary>
        private void AddNewLine()
        {
            Line line = new Line((int)StartLinePoint.X, (int)StartLinePoint.Y, (int)PrevStarPos.X, (int)PrevStarPos.Y);
            Lines.Add(line);

            //Debug.Log($"line {line} start = {StartPoint} starpos = {PrevStarPos} prevstarvel = {PrevStarVelocity} starvel = {StarVelocity}");
        }

        /// <summary>
        /// Checks if a shape is completed, based on whether the new line intersected with an existing line or not.
        /// </summary>
        /// <returns>The index of the Line that was intersected to completed the shape, otherwise -1.</returns>
        private int CheckCompletedShape()
        {
            //After an intersection is true, get the index of the line it intersected with
            //Then, create a bounding box around those lines and see if any enemies are in them

            if (Lines.Count < 2) return -1;
            
            Line lastLine = Lines[Lines.Count - 1];
            
            for (int i = 0; i < (Lines.Count - 1); i++)
            {
                if (lastLine.Intersects(Lines[i]) == true)
                    return i;
            }

            return -1;
        }

        /// <summary>
        /// Gets the bounding box of the shape the player completed as a Rectangle.
        /// </summary>
        /// <param name="collisionIndex">The index of the Line the last drawn Line intersected with.</param>
        /// <returns>A Rectangle of the bounding box corresponding to the shape the player completed.</returns>
        private Rectangle GetShapeBoundingBox(int collisionIndex)
        {
            int minX = int.MaxValue;
            int minY = int.MaxValue;
            int maxX = int.MinValue;
            int maxY = int.MinValue;

            //Check for the min and max X and Y positions
            //The min is the start of the rectangle, and the max is used to determine the width and height of the rectangle
            for (int i = collisionIndex; i < Lines.Count; i++)
            {
                Line line = Lines[i];
                Point p1 = line.P1;
                Point p2 = line.P2;

                //Check for all min and max values
                if (p1.X < minX) minX = p1.X;
                if (p1.Y < minY) minY = p1.Y;
                if (p1.X > maxX) maxX = p1.X;
                if (p1.Y > maxY) maxY = p1.Y;

                if (p2.X < minX) minX = p2.X;
                if (p2.Y < minY) minY = p2.Y;
                if (p2.X > maxX) maxX = p2.X;
                if (p2.Y > maxY) maxY = p2.Y;
            }

            Rectangle boundingBox = new Rectangle(minX, minY, Math.Abs(maxX - minX), Math.Abs(maxY - minY));

            return boundingBox;
        }

        /// <summary>
        /// Gets the scale of a Line.
        /// </summary>
        /// <param name="line">The Line to get the scale of.</param>
        /// <returns>A Vector2 of the Line's scale.</returns>
        private Vector2 GetLineScale(Line line)
        {
            float lineLength = (float)line.GetLength();
            
            //Add one because the scale counts the start pixel
            //For example, a Line with points (0, 0) to (10, 0) will have a scale of 10 but only render from (0, 0) to (9, 0)
            lineLength += 1f;

            //Use only the X for the scale; the rotation will determine where the line points
            Vector2 scale = new Vector2(lineLength, LineThickness);

            return scale;
        }

        protected override void OnDraw()
        {
            for (int i = 0; i < Lines.Count; i++)
            {
                Line line = Lines[i];
                float angle = line.GetLineAngle(false);
                Vector2 scale = GetLineScale(line);
                Vector2 origin = Vector2.Zero;
                Vector2 direction = line.GetDirection();
                Vector2 position = line.P1.ToVector2();

                //Due to the way drawing a single pixel rotated and scaled at an origin of (0, 0) works
                //we need to offset specific cases so lines are drawn in their correct locations
                //These offsets allow the lines to match up one-to-one with their actual positions
                AdjustLineOffset(line, ref scale, ref position);

                SpriteRenderer.Instance.DrawUI(LineTexture, position, null, LineColor, angle, origin, scale, false, false, .8f);
            }
        }

        /// <summary>
        /// Adjusts a Line to render it correctly.
        /// </summary>
        /// <param name="line">The Line to render.</param>
        /// <param name="scale">The current scale to render the Line with.</param>
        /// <param name="position">The current position the Line is drawn at.</param>
        private void AdjustLineOffset(Line line, ref Vector2 scale, ref Vector2 position)
        {
            //Get the direction the line is pointing
            Vector2 direction = line.GetDirection();

            //Offset lines going down
            if (direction.Y < 0)
            {
                position.Y += 1;

                //Lines facing up-left are short by 1, likely due to rotation issues
                if (direction.X < 0)
                {
                    position.X += 1;
                    position.Y += 1;
                    scale.X += 1;
                }
            }
            //Offset lines going up
            else if (direction.Y > 0)
            {
                position.X += 1;

                //Lines facing down-left are short by 1, likely due to rotation issues
                if (direction.X < 0)
                {
                    scale.X += 1;
                    position.X += 1;
                }
            }
            //Offset lines going straight left
            else if (direction.X < 0f && direction.Y == 0f)
            {
                position.X += 1;
                position.Y += 1;
            }
        }

        /// <summary>
        /// Updates the color of the lines by cycling through the array of defined colors.
        /// </summary>
        private void CycleLineColors()
        {
            //Increment elapsed time
            ColorElapsedTime += Time.ElapsedMilliseconds;

            //Compare the current color with the next, and wrap around
            int nextColorIndex = UtilityGlobals.Wrap(ColorIndex + 1, 0, CycleColors.Length - 1);

            //Lerp the colors
            LineColor = Color.Lerp(CycleColors[ColorIndex], CycleColors[nextColorIndex], (float)(ColorElapsedTime / ColorDuration));

            //Move onto the next color and reset the elapsed time
            if (ColorElapsedTime >= ColorDuration)
            {
                ColorIndex = nextColorIndex;
                ColorElapsedTime = 0d;
            }
        }
    }
}
