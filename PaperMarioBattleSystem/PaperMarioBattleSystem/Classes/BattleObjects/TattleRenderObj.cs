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
    /// The object rendering the box and a portion of the screen during a PM Tattle.
    /// </summary>
    public sealed class TattleRenderObj : BattleObject, IPosition
    {
        /// <summary>
        /// The states of the tattle box.
        /// </summary>
        public enum States
        {
            MovingDown, Opening, Waiting, Closing, MovingUp
        }

        /// <summary>
        /// The size of the render region.
        /// </summary>
        private readonly Vector2 RenderRegionSize = new Vector2(140, 100);

        /// <summary>
        /// The size of the tattle box surrounding the render region.
        /// </summary>
        private readonly Vector2 TattleBoxSize = new Vector2(150, 110);

        public Vector2 Position { get; set; } = Vector2.Zero;

        /// <summary>
        /// The position of the BattleEntity that was tattled.
        /// </summary>
        private Vector2 TattledEntityPos = Vector2.Zero;

        /// <summary>
        /// The region of the screen to render.
        /// </summary>
        private readonly Rectangle ScreenRegion = Rectangle.Empty;

        /// <summary>
        /// The current region of the screen that's being rendered.
        /// </summary>
        private Rectangle CurRegion = Rectangle.Empty;

        /// <summary>
        /// The scale to draw the region at.
        /// </summary>
        private Vector2 DrawScale = new Vector2(2f, 2f);

        /// <summary>
        /// The start position of the tattle box.
        /// </summary>
        private Vector2 StartPos = Vector2.Zero;

        /// <summary>
        /// The end position of the tattle box.
        /// </summary>
        private Vector2 EndPos = new Vector2(540, 380);

        /// <summary>
        /// How long it takes the tattle box to move.
        /// </summary>
        private double MoveTime = 600d;

        /// <summary>
        /// How long it takes the tattle box to open and close, showing the contents of that region of the screen.
        /// </summary>
        private double OpenCloseTime = 300d;

        /// <summary>
        /// The sliced tattle box Texture.
        /// </summary>
        private NineSlicedTexture2D TattleWindow = null;

        /// <summary>
        /// The current state of the tattle box.
        /// </summary>
        private States CurState = States.Waiting;
        private double ElapsedTime = 0d;

        /// <summary>
        /// The start Y value of the tattle box.
        /// This applies to the value the current region starts at and goes to, respectively, when the tattle box is opening and closing.
        /// </summary>
        private int StartRegionY => ScreenRegion.Y + (ScreenRegion.Height / 2);

        public TattleRenderObj(Vector2 tattledPos)
        {
            TattledEntityPos = tattledPos;

            Vector2 windowSize = SpriteRenderer.Instance.WindowSize;

            int regionWidthOver2 = (int)RenderRegionSize.X / 2;
            int regionHeightOver2 = (int)RenderRegionSize.Y / 2;

            //Set the region of the screen to draw from, and ensure it doesn't go outside the screen's bounds
            ScreenRegion = new Rectangle((int)TattledEntityPos.X - regionWidthOver2, (int)TattledEntityPos.Y - regionHeightOver2, (int)RenderRegionSize.X, (int)RenderRegionSize.Y);
            if (ScreenRegion.X < 0) ScreenRegion.X = 0;
            if (ScreenRegion.Y < 0) ScreenRegion.Y = 0;
            if (ScreenRegion.Right > windowSize.X) ScreenRegion.Width = (int)windowSize.X - ScreenRegion.X;
            if (ScreenRegion.Bottom > windowSize.Y) ScreenRegion.Height = (int)windowSize.Y - ScreenRegion.Y;

            StartPos = new Vector2(EndPos.X, -ScreenRegion.Height - Math.Abs(RenderRegionSize.Y - TattleBoxSize.Y));
            Position = StartPos;

            //Start out closed
            CurRegion = ScreenRegion;
            CurRegion.Y = StartRegionY;
            CurRegion.Height = 0;

            TattleWindow = new NineSlicedTexture2D(AssetManager.Instance.LoadRawTexture2D($"{ContentGlobals.BattleGFX}.png"),
                new Rectangle(457, 812, 32, 16), 7, 6, 7, 9);

            SpriteRenderer.Instance.FullyConcludedDrawingEvent -= OnDrawingFullyConcluded;
            SpriteRenderer.Instance.FullyConcludedDrawingEvent += OnDrawingFullyConcluded;
        }

        public override void CleanUp()
        {
            SpriteRenderer.Instance.FullyConcludedDrawingEvent -= OnDrawingFullyConcluded;

            TattleWindow = null;
        }

        /// <summary>
        /// Starts the tattle box, moving downwards.
        /// </summary>
        /// <param name="moveTime">The amount of time to move down in.</param>
        public void Start(double moveTime)
        {
            CurState = States.MovingDown;
            Position = StartPos;
            ElapsedTime = 0d;
            MoveTime = moveTime;
        }

        /// <summary>
        /// Opens the tattle box, revealing the contents of the portion of the screen rendered.
        /// </summary>
        /// <param name="moveTime">The amount of time to open the box in.</param>
        public void Open(double openTime)
        {
            CurState = States.Opening;
            ElapsedTime = 0d;
            OpenCloseTime = openTime;

            CurRegion.Y = StartRegionY;
            CurRegion.Height = 0;
        }

        /// <summary>
        /// Closes the tattle box, obscuring the contents of the portion of the screen rendered.
        /// </summary>
        /// <param name="moveTime">The amount of time to close the box in.</param>
        public void Close(double closeTime)
        {
            CurState = States.Closing;
            ElapsedTime = 0d;
            OpenCloseTime = closeTime;

            CurRegion.Y = ScreenRegion.Y;
            CurRegion.Height = ScreenRegion.Height;
        }

        /// <summary>
        /// Ends the tattle box, moving upwards offscreen.
        /// </summary>
        /// <param name="moveTime">The amount of time to move up in.</param>
        public void End(double moveTime)
        {
            CurState = States.MovingUp;
            Position = EndPos;
            ElapsedTime = 0d;
            MoveTime = moveTime;
        }

        public override void Update()
        {
            //Increment time if we're not waiting
            if (CurState != States.Waiting)
            {
                ElapsedTime += Time.ElapsedMilliseconds;
            }

            //If we're moving down or moving up, move the box
            if (CurState == States.MovingDown || CurState == States.MovingUp)
            {
                Vector2 start = StartPos;
                Vector2 end = EndPos;
                if (CurState == States.MovingUp)
                {
                    UtilityGlobals.Swap(ref start, ref end);
                }

                Position = Interpolation.Interpolate(start, end, ElapsedTime / MoveTime,
                    CurState == States.MovingDown ? Interpolation.InterpolationTypes.CubicOut : Interpolation.InterpolationTypes.CubicIn);

                if (ElapsedTime > MoveTime)
                {
                    CurState = States.Waiting;
                    ElapsedTime = 0d;
                }
            }
            //If we're opening or closing the box, do the appropriate action
            else if (CurState == States.Opening || CurState == States.Closing)
            {
                if (CurState == States.Opening)
                {
                    CurRegion.Y = Interpolation.Interpolate(StartRegionY, ScreenRegion.Y, ElapsedTime / OpenCloseTime, Interpolation.InterpolationTypes.Linear);
                    CurRegion.Height = Interpolation.Interpolate(0, ScreenRegion.Height, ElapsedTime / OpenCloseTime, Interpolation.InterpolationTypes.Linear);
                }
                else
                {
                    CurRegion.Y = Interpolation.Interpolate(ScreenRegion.Y, StartRegionY, ElapsedTime / OpenCloseTime, Interpolation.InterpolationTypes.Linear);
                    CurRegion.Height = Interpolation.Interpolate(ScreenRegion.Height, 0, ElapsedTime / OpenCloseTime, Interpolation.InterpolationTypes.Linear);
                }

                if (ElapsedTime > OpenCloseTime)
                {
                    CurState = States.Waiting;
                    ElapsedTime = 0d;
                }
            }
        }

        private void OnDrawingFullyConcluded(in RenderTarget2D finalRenderTarget)
        {
            //After finishing all drawing, render this to the backbuffer
            //This renders on top of the final RenderTarget, which prevents us from rendering over that portion of the screen in the RenderTarget
            
            SpriteRenderer.Instance.BeginBatch(SpriteRenderer.Instance.uiBatch, null, SamplerState.PointClamp, null, null);
            SpriteRenderer.Instance.DrawUISliced(TattleWindow, new Rectangle((int)Position.X - (int)(TattleBoxSize.X * (DrawScale.X / 2f)), (int)Position.Y - (int)(TattleBoxSize.Y * (DrawScale.Y / 2f)), (int)TattleBoxSize.X * (int)DrawScale.X, (int)TattleBoxSize.Y * (int)DrawScale.Y), Color.Blue, .9f);
            
            //Don't render this part of the screen if we're not displaying it yet
            if (CurRegion.Width != 0 && CurRegion.Height != 0)
            {
                SpriteRenderer.Instance.DrawUI(finalRenderTarget, Position, CurRegion, Color.White, 0f, new Vector2(.5f, .5f), DrawScale, false, false, 1f);
            }

            SpriteRenderer.Instance.EndBatch(SpriteRenderer.Instance.uiBatch);
        }
    }
}
