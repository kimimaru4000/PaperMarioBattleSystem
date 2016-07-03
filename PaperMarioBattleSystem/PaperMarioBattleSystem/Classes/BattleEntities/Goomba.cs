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
        public Goomba() : base(new Stats(1, 2, 0, 1, 0))
        {
            Name = "Goomba";

            AddStatusPercentage(Enumerations.StatusTypes.Sleep, 100);
            AddStatusPercentage(Enumerations.StatusTypes.Immobilized, 110);
            AddStatusPercentage(Enumerations.StatusTypes.Dizzy, 100);
            AddStatusPercentage(Enumerations.StatusTypes.Soft, 100);
            AddStatusPercentage(Enumerations.StatusTypes.Poison, 100);

            Texture2D spriteSheet = AssetManager.Instance.LoadAsset<Texture2D>($"{ContentGlobals.SpriteRoot}/Enemies/Goomba");
            AddAnimation(AnimationGlobals.IdleName, new Animation(spriteSheet, new Animation.Frame(new Rectangle(67, 107, 26, 28), 1000d)));
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

            //Rectangle rect = new Rectangle(67, 107, 26, 28);
            //SpriteRenderer.Instance.Draw(SpriteSheet, Position, rect, Color.White, new Vector2(0, 0), false, .1f);
        }
    }
}
