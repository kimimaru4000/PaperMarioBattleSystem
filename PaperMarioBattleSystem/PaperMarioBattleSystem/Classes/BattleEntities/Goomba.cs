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
    /// The standard Goomba enemy
    /// </summary>
    public class Goomba : BattleEnemy
    {
        public Goomba() : base(new Stats(1, 20, 0, 1, 0))
        {
            Name = "Goomba";

            SpriteSheet = AssetManager.Instance.LoadAsset<Texture2D>($"{ContentGlobals.SpriteRoot}/Enemies/Goomba");
        }

        public override void OnTurnStart()
        {
            base.OnTurnStart();

            StartAction(new Jump(), BattleManager.Instance.GetEntities(Enumerations.EntityTypes.Player)[0]);
        }

        public override void TurnUpdate()
        {
            base.TurnUpdate();
        }

        public override void Draw()
        {
            base.Draw();

            Rectangle rect = new Rectangle(67, 107, 26, 28);
            SpriteRenderer.Instance.Draw(SpriteSheet, Position, rect, Color.White, new Vector2(0, 0), false, .1f);
        }
    }
}
