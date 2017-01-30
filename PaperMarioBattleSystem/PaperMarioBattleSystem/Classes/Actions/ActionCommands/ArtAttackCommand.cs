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
    /// <para>Overlapping another line while it doesn't close causes it to clear the drawing.</para>
    /// </summary>
    public sealed class ArtAttackCommand : ActionCommand
    {
        /// <summary>
        /// The speed at which the Star cursor moves when drawing.
        /// </summary>
        private const int StarSpeed = 3;

        /// <summary>
        /// The position of the Star, or where you're currently drawing.
        /// </summary>
        private Vector2 StarPos = Vector2.Zero;

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
        /// The points to draw.
        /// </summary>
        private readonly Dictionary<Vector2, bool> Points = new Dictionary<Vector2, bool>();

        /// <summary>
        /// The texture to display the points.
        /// </summary>
        private Texture2D PointTexture = null;

        public ArtAttackCommand(IActionCommandHandler commandHandler, Vector2 startPos, double drawTime) : base(commandHandler)
        {
            StarPos = startPos;
            DrawTime = drawTime;

            PointTexture = AssetManager.Instance.LoadAsset<Texture2D>($"{ContentGlobals.UIRoot}/Box");
        }

        public override void StartInput()
        {
            base.StartInput();

            ElapsedDrawTime = 0d;
            Points.Clear();
        }

        public override void EndInput()
        {
            base.EndInput();

            ElapsedDrawTime = 0d;
            Points.Clear();
        }

        protected override void ReadInput()
        {
            if (ElapsedDrawTime >= DrawTime)
            {
                OnComplete(CommandResults.Failure);
                return;
            }

            //Increment time afterwards; this is how it seems to work in the game if you complete a circle right before it ends
            ElapsedDrawTime += Time.ElapsedMilliseconds;

            Vector2 moveAmt = Vector2.Zero;

            //Use any of the keys to move
            if (Input.GetKey(Keys.Up) == true)
            {
                moveAmt.Y -= StarSpeed;
            }
            if (Input.GetKey(Keys.Down) == true)
            {
                moveAmt.Y += StarSpeed;
            }
            if (Input.GetKey(Keys.Left) == true)
            {
                moveAmt.X -= StarSpeed;
            }
            if (Input.GetKey(Keys.Right) == true)
            {
                moveAmt.X += StarSpeed;
            }

            PlotPoints(moveAmt);

            StarPos += moveAmt;

            //If a circle is completed, send a response with the position of the star and the elapsed draw time, then end with a Success
            if (CheckCompletedShape() == true)
            {
                SendResponse(new ActionCommandGlobals.ArtAttackResponse(StarPos, ElapsedDrawTime));
                OnComplete(CommandResults.Success);
            }
        }

        //NOTE: Use the Line struct and create a new Line every few frames the user is drawing, or if the direction was changed
        //Consider using a List instead of a Dictionary, and put Lines in it instead of Vector2s
        private void PlotPoints(Vector2 pointOffset)
        {
            float multiplierX = (pointOffset.X < 0) ? -1 : (pointOffset.X > 0) ? 1 : 0;
            float multiplierY = (pointOffset.Y < 0) ? -1 : (pointOffset.Y > 0) ? 1 : 0;

            if (multiplierX == 0 && multiplierY == 0) return;

            for (int i = 0; i <= StarSpeed; i++)
            {
                Vector2 starPos = StarPos + new Vector2(i * multiplierX, i * multiplierY);
                if (Points.ContainsKey(starPos) == false)
                {
                    Points.Add(starPos, true);
                }
            }
        }

        /// <summary>
        /// Checks if a shape is completed, based on whether the new line intersected with an existing line or not.
        /// </summary>
        /// <returns></returns>
        private bool CheckCompletedShape()
        {
            return false;
        }

        protected override void OnDraw()
        {
            foreach (KeyValuePair<Vector2, bool> point in Points)
            {
                SpriteRenderer.Instance.Draw(PointTexture, point.Key, Color.White, false, .8f, true);
            }
        }
    }
}
