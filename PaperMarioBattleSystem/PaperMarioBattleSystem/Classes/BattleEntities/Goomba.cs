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
        public Goomba() : base(new Stats(1, 2, 0, 0, 0))
        {
            Name = "Goomba";

            EntityProperties.AddStatusProperty(Enumerations.StatusTypes.Sleep, new StatusPropertyHolder(100, 0));
            EntityProperties.AddStatusProperty(Enumerations.StatusTypes.Immobilized, new StatusPropertyHolder(110, 0));
            EntityProperties.AddStatusProperty(Enumerations.StatusTypes.Dizzy, new StatusPropertyHolder(100, 0));
            EntityProperties.AddStatusProperty(Enumerations.StatusTypes.Soft, new StatusPropertyHolder(100, 0));
            EntityProperties.AddStatusProperty(Enumerations.StatusTypes.Poison, new StatusPropertyHolder(100, 0));
            EntityProperties.AddStatusProperty(Enumerations.StatusTypes.Confused, new StatusPropertyHolder(100, 0));

            Texture2D spriteSheet = AssetManager.Instance.LoadAsset<Texture2D>($"{ContentGlobals.SpriteRoot}/Enemies/Goomba");
            AddAnimation(AnimationGlobals.IdleName, new Animation(spriteSheet, new Animation.Frame(new Rectangle(67, 107, 26, 28), 1000d)));
            AddAnimation(AnimationGlobals.HurtName, new Animation(spriteSheet, 
                new Animation.Frame(new Rectangle(65, 76, 29, 27), 500d),
                new Animation.Frame(new Rectangle(2, 109, 27, 26), 500d)));
            AddAnimation(AnimationGlobals.DeathName, new Animation(spriteSheet, new Animation.Frame(new Rectangle(2, 109, 27, 26), 1000d)));
        }

        public override void OnTurnStart()
        {
            base.OnTurnStart();

            StartAction(new Jump(), BattleManager.Instance.GetFrontPlayer());
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
