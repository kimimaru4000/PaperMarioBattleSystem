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
    /// The star you throw in Sweet Treat.
    /// </summary>
    public sealed class SweetTreatThrownStar : UICroppedTexture2D, ICollisionHandler
    {
        public Vector2 Speed = Vector2.Zero;

        /// <summary>
        /// Gets the collision rectangle of the star.
        /// </summary>
        public Rectangle CollRect
        {
            get
            {
                Vector2 widthHeight = CroppedTex2D.WidthHeightToVector2() * Scale;
                Vector2 pos = Position - (widthHeight.Halve());

                return new Rectangle((int)pos.X, (int)pos.Y, (int)widthHeight.X, (int)widthHeight.Y);
            }
        }

        public SweetTreatThrownStar(Vector2 startPosition, Vector2 speed)
        {
            Position = startPosition;
            Origin = new Vector2(.5f, .5f);

            CroppedTex2D = new CroppedTexture2D(AssetManager.Instance.LoadRawTexture2D($"{ContentGlobals.BattleGFX}.png"),
                new Rectangle(245, 986, 40, 37));

            Speed = speed;
        }

        public ICollisionShape collisionShape => new CollisionRect(CollRect);

        public CollisionResponseHolder GetCollisionResponse(ICollisionHandler collisionObject)
        {
            return new CollisionResponseHolder(this, null);
        }

        public void HandleCollision(CollisionResponseHolder collisionResponse)
        {
            BattleUIManager.Instance.RemoveUIElement(this);
        }

        public override void Update()
        {
            Position += Speed;
        }

        public override void Draw()
        {
            base.Draw();

            Debug.DebugDrawHollowRect(CollRect, Color.Blue, .8f, 1, true);
        }
    }
}
