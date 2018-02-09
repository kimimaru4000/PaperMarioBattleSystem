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
    /// VFX for performing a Stylish Move.
    /// </summary>
    public sealed class StylishMoveVFX : VFXElement, IPosition, IScalable
    {
        private enum StylishVFXState
        {
            Moving,
            Waiting,
            Scaling
        }

        private readonly Vector2 PosDiff = new Vector2(1000, 0);
        private const double MoveTime = 500d;
        private const double WaitTime = 700d;
        private const double ScaleTime = 500d;

        public Vector2 Position { get; set; }
        public Vector2 Scale { get; set; } = new Vector2(.5f, .5f);

        private Vector2 StartPos = Vector2.Zero;
        private Vector2 EndPos = Vector2.Zero;

        private Vector2 StartScale = Vector2.Zero;

        private double ElapsedTime = 0d;

        private StylishVFXState State = StylishVFXState.Moving;

        private CroppedTexture2D[] StylishTextures = null;

        public StylishMoveVFX(Vector2 endPos)
        {
            EndPos = endPos;
            StartPos = EndPos + PosDiff;

            Position = StartPos;

            InitTextures();
        }

        private void InitTextures()
        {
            StylishTextures = new CroppedTexture2D[8];

            Texture2D battleGFX = AssetManager.Instance.LoadRawTexture2D($"{ContentGlobals.BattleGFX}.png");

            StylishTextures[0] = new CroppedTexture2D(battleGFX, new Rectangle(471, 947, 48, 48));
            StylishTextures[1] = new CroppedTexture2D(battleGFX, new Rectangle(519, 947, 48, 48));
            StylishTextures[2] = new CroppedTexture2D(battleGFX, new Rectangle(567, 947, 48, 48));
            StylishTextures[3] = new CroppedTexture2D(battleGFX, new Rectangle(615, 947, 48, 48));
            StylishTextures[4] = new CroppedTexture2D(battleGFX, new Rectangle(663, 947, 48, 48));
            StylishTextures[5] = StylishTextures[0];
            StylishTextures[6] = new CroppedTexture2D(battleGFX, new Rectangle(711, 947, 48, 48));
            StylishTextures[7] = new CroppedTexture2D(battleGFX, new Rectangle(759, 947, 48, 48));
        }

        public override void CleanUp()
        {
            if (StylishTextures != null)
            {
                for (int i = 0; i < StylishTextures.Length; i++)
                {
                    StylishTextures[i] = null;
                }

                StylishTextures = null;
            }
        }

        private void DoMoving()
        {
            ElapsedTime += Time.ElapsedMilliseconds;

            Position = Interpolation.Interpolate(StartPos, EndPos, ElapsedTime / MoveTime, Interpolation.InterpolationTypes.QuadOut);

            if (ElapsedTime >= MoveTime)
            {
                Position = EndPos;
                ElapsedTime = 0d;
                State = StylishVFXState.Waiting;
            }
        }

        private void DoWaiting()
        {
            ElapsedTime += Time.ElapsedMilliseconds;

            if (ElapsedTime >= WaitTime)
            {
                ElapsedTime = 0d;
                StartScale = Scale;
                State = StylishVFXState.Scaling;
            }
        }

        private void DoScaling()
        {
            ElapsedTime += Time.ElapsedMilliseconds;

            Scale = Interpolation.Interpolate(StartScale, Vector2.Zero, ElapsedTime / ScaleTime, Interpolation.InterpolationTypes.Linear);

            if (ElapsedTime >= ScaleTime)
            {
                ElapsedTime = 0d;
                ReadyForRemoval = true;
            }
        }

        public override void Update()
        {
            if (State == StylishVFXState.Moving)
            {
                DoMoving();
            }
            else if (State == StylishVFXState.Waiting)
            {
                DoWaiting();
            }
            else if (State == StylishVFXState.Scaling)
            {
                DoScaling();
            }
        }

        public override void Draw()
        {
            if (StylishTextures == null) return;

            for (int i = 0; i < StylishTextures.Length; i++)
            {
                float width = StylishTextures[i].SourceRect.Value.Width * Scale.X;
                Vector2 pos = Position + new Vector2(i * width, 0);

                SpriteRenderer.Instance.Draw(StylishTextures[i].Tex, pos, StylishTextures[i].SourceRect, Color.White, 0f, new Vector2(.5f, .5f), Scale, false, false, .8f);
            }
        }
    }
}
