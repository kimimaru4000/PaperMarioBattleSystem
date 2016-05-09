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
    /// Mario in battle
    /// </summary>
    public class BattleMario : BattleEntity
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="marioStats">Mario's stats</param>
        public BattleMario(Stats marioStats) : base(marioStats)
        {
            Name = "Mario";
            EntityType = Enumerations.EntityTypes.Player;

            Texture2D spriteSheet = AssetManager.Instance.LoadAsset<Texture2D>($"{ContentGlobals.SpriteRoot}/Characters/Mario");
            AddAnimation(AnimationGlobals.IdleName, new Animation(spriteSheet, new Animation.Frame(new Rectangle(228, 918, 29, 51), 1000d)));
            AddAnimation(AnimationGlobals.HammerName, new Animation(spriteSheet, 
                new Animation.Frame(new Rectangle(341, 9, 36, 50), 100d),
                new Animation.Frame(new Rectangle(341, 64, 38, 50), 100d),
                new Animation.Frame(new Rectangle(341, 118, 32, 44), 100d),
                new Animation.Frame(new Rectangle(340, 166, 32, 44), 100d)));
            AddAnimation(AnimationGlobals.VictoryName, new Animation(spriteSheet, new Animation.Frame(new Rectangle(447, 281, 42, 50), 1000d)));
        }

        public override void OnTurnStart()
        {
            base.OnTurnStart();
            BattleUIManager.Instance.PushMenu(new MarioBattleMenu());
        }

        public override void TurnUpdate()
        {
            base.TurnUpdate();
        }

        public override void OnTurnEnd()
        {
            base.OnTurnEnd();
            BattleUIManager.Instance.ClearMenuStack();
        }

        public override void Draw()
        {
            base.Draw();

            //if (IsDead) return;
            //Rectangle rect = new Rectangle(228, 918, 29, 51);
            //SpriteRenderer.Instance.Draw(SpriteSheet, Position, rect, Color.White, new Vector2(0, 0), true, .1f);
        }
    }
}
