using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PaperMarioBattleSystem.Utilities;

namespace PaperMarioBattleSystem
{
    public sealed class ArtAttackActionCommandUI : ActionCommandUI<ArtAttackCommand>
    {
        /// <summary>
        /// The thickness of each line when drawing.
        /// </summary>
        private const float LineThickness = 1;

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

        /// <summary>
        /// The color of the lines.
        /// </summary>
        private Color LineColor = Color.White;

        /// <summary>
        /// The texture to display the lines.
        /// </summary>
        private Texture2D LineTexture = null;

        public ArtAttackActionCommandUI(ArtAttackCommand artAttackCommand) : base(artAttackCommand)
        {
            LineTexture = AssetManager.Instance.LoadRawTexture2D($"{ContentGlobals.UIRoot}/Box.png");
        }

        public override void Update()
        {
            CycleLineColors();
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

        public override void Draw()
        {
            if (ActionCmd?.AcceptingInput == false) return;

            for (int i = 0; i < ActionCmd.Lines.Count; i++)
            {
                Line line = ActionCmd.Lines[i];
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
    }
}
