using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static PaperMarioBattleSystem.SweetTreatGlobals;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// An element correlating to an icon for the <see cref="SweetTreatCommand"/>.
    /// <para>The icon moves down from the top of the screen and disappears when it hits the stage.
    /// If hit by the thrown star, it performs its effect and disappears.</para>
    /// </summary>
    public class SweetTreatRestorationElement : UICroppedTexture2D, ICollisionHandler
    {
        public RestoreTypes RestorationType = RestoreTypes.None;

        /// <summary>
        /// The start position.
        /// </summary>
        protected Vector2 StartPosition = Vector2.Zero;

        /// <summary>
        /// The end position.
        /// </summary>
        protected Vector2 EndPosition = Vector2.Zero;

        /// <summary>
        /// How long it takes the element to move to the bottom.
        /// </summary>
        protected double MovementDur = 1200d;

        private double ElapsedTime = 0d;

        /// <summary>
        /// Tells if the restoration element is done moving.
        /// </summary>
        public bool DoneMoving => (ElapsedTime >= MovementDur);

        /// <summary>
        /// Gets the collision rectangle of the restoration element.
        /// </summary>
        public Rectangle IconRect
        {
            get
            {
                Vector2 widthHeight = CroppedTex2D.WidthHeightToVector2() * Scale;
                Vector2 pos = Position - (widthHeight.Halve());

                return new Rectangle((int)pos.X, (int)pos.Y, (int)widthHeight.X, (int)widthHeight.Y);
            }
        }

        public SweetTreatRestorationElement(RestoreTypes restorationType, double movementDur, Vector2 startPosition, Vector2 endPosition)
        {
            RestorationType = restorationType;
            MovementDur = movementDur;

            StartPosition = startPosition;
            EndPosition = endPosition;

            //Initialize
            Initialize();
        }

        private void Initialize()
        {
            Position = StartPosition;

            CroppedTex2D = new CroppedTexture2D(AssetManager.Instance.LoadRawTexture2D($"{ContentGlobals.BattleGFX}.png"),
                        new Rectangle(228, 413, 40, 40));

            //Get the correct icon based on the RestoreTypes
            switch (RestorationType)
            {
                case RestoreTypes.MarioHP:
                    CroppedTex2D.SetRect(new Rectangle(135, 415, 36, 40));
                    break;
                case RestoreTypes.BigMarioHP:
                    CroppedTex2D.SetRect(new Rectangle(135, 415, 36, 40));
                    Scale = new Vector2(2f, 2f);
                    break;
                case RestoreTypes.PartnerHP:
                    CroppedTex2D.SetRect(new Rectangle(84, 414, 40, 38));
                    break;
                case RestoreTypes.BigPartnerHP:
                    CroppedTex2D.SetRect(new Rectangle(84, 414, 40, 38));
                    Scale = new Vector2(2f, 2f);
                    break;
                case RestoreTypes.FP:
                    CroppedTex2D.SetRect(new Rectangle(179, 416, 40, 39));
                    break;
                case RestoreTypes.BigFP:
                    CroppedTex2D.SetRect(new Rectangle(179, 416, 40, 39));
                    Scale = new Vector2(2f, 2f);
                    break;
                case RestoreTypes.PoisonMushroom:
                default:
                    CroppedTex2D.SetRect(new Rectangle(228, 413, 40, 40));
                    break;
            }
        }

        /// <summary>
        /// Gets the restoration element's behavior data on hit.
        /// </summary>
        /// <returns>A <see cref="RestoreBehaviorData"/> containing this element's behavior when hit.
        /// If this element's <see cref="RestorationType"/> doesn't have an entry entry in the global table, then a default value.</returns>
        public RestoreBehaviorData GetBehaviorDataOnHit()
        {
            //Return a default value if an entry doesn't exist
            if (RestorationTable.ContainsKey(RestorationType) == false)
            {
                return RestoreBehaviorData.Default;
            }

            return RestorationTable[RestorationType];
        }

        public override void Update()
        {
            //Don't move if we're done moving
            if (DoneMoving == true) return;

            ElapsedTime += Time.ElapsedMilliseconds;

            //Interpolate
            Position = Interpolation.Interpolate(StartPosition, EndPosition, ElapsedTime / MovementDur, Interpolation.InterpolationTypes.Linear);

            //Set the position to the end if we're done moving
            if (DoneMoving == true)
            {
                Position = EndPosition;
            }
        }

        public override void Draw()
        {
            //This is a UI element, so always render it on the UI layer
            SpriteRenderer.Instance.DrawUI(CroppedTex2D.Tex, Position, CroppedTex2D.SourceRect, TintColor, Rotation, new Vector2(.5f, .5f), Scale, FlipX, FlipY, Depth);

            Debug.DebugDrawHollowRect(IconRect, Color.Red, .9f, 1, true);
        }

        #region Interface Implementations

        /// <summary>
        /// The type of collision shape.
        /// </summary>
        public ICollisionShape collisionShape
        {
            get
            {
                return new CollisionRect(IconRect);
            }
        }

        /// <summary>
        /// Handles a collision with another object.
        /// </summary>
        /// <param name="collisionResponse">The collision data.</param>
        public void HandleCollision(CollisionResponseHolder collisionResponse)
        {
            BattleUIManager.Instance.RemoveUIElement(this);
        }

        public CollisionResponseHolder GetCollisionResponse(ICollisionHandler collisionObject)
        {
            return new CollisionResponseHolder(this, GetBehaviorDataOnHit());
        }

        #endregion
    }
}
