using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PaperMarioBattleSystem
{
    public sealed class Goombario : BattlePartner
    {
        public Goombario() : base(new PartnerStats(PartnerGlobals.PartnerRanks.Normal, 50, 0, 0))
        {
            Name = "Goombario";
            PartnerDescription = "He can Headbonk on enemies!";
            PartnerType = Enumerations.PartnerTypes.Goombario;

            Texture2D spriteSheet = AssetManager.Instance.LoadRawTexture2D($"{ContentGlobals.SpriteRoot}/Characters/Goombario.png");
            AnimManager.SetSpriteSheet(spriteSheet);

            AnimManager.AddAnimation(AnimationGlobals.IdleName, new ReverseAnimation(spriteSheet, AnimationGlobals.InfiniteLoop,
                new Animation.Frame(new Rectangle(67, 89, 26, 30), 1000d),
                new Animation.Frame(new Rectangle(131, 88, 26, 31), 100d, new Vector2(0, -1)),
                new Animation.Frame(new Rectangle(99, 86, 26, 33), 1000d, new Vector2(0, -2))));

            AnimManager.AddAnimation(AnimationGlobals.RunningName, new ReverseAnimation(spriteSheet, AnimationGlobals.InfiniteLoop,
                new Animation.Frame(new Rectangle(3, 9, 28, 30), 100d),
                new Animation.Frame(new Rectangle(67, 89, 26, 30), 100d),
                new Animation.Frame(new Rectangle(129, 46, 30, 33), 100d)));

            AnimManager.AddAnimation(AnimationGlobals.PlayerBattleAnimations.ChoosingActionName, new Animation(spriteSheet,
            new Animation.Frame(new Rectangle(2, 46, 26, 33), 1000d, new Vector2(0, -2))));

            AnimManager.AddAnimation(AnimationGlobals.JumpStartName, new Animation(spriteSheet,
                new Animation.Frame(new Rectangle(130, 5, 28, 31), 400d)));

            AnimManager.AddAnimation(AnimationGlobals.HurtName, new Animation(spriteSheet,
                new Animation.Frame(new Rectangle(49, 164, 30, 27), 1000d)));
            AnimManager.AddAnimation(AnimationGlobals.DeathName, new Animation(spriteSheet,
                new Animation.Frame(new Rectangle(49, 164, 30, 27), 1000d)));

            AnimManager.AddAnimation(AnimationGlobals.StatusBattleAnimations.InjuredName, new LoopAnimation(spriteSheet, AnimationGlobals.InfiniteLoop,
                new Animation.Frame(new Rectangle(81, 163, 30, 28), 650d),
                new Animation.Frame(new Rectangle(7, 162, 32, 29), 650d)));
        }

        protected sealed override BattleMenu GetMainBattleMenu()
        {
            return new PartnerBattleMenu(new GoombarioSubMenu(), Enumerations.PartnerTypes.Goombario);
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
            //Rectangle rect = new Rectangle(67, 89, 26, 30);
            //SpriteRenderer.Instance.Draw(SpriteSheet, Position, rect, Color.White, new Vector2(0, 0), true, .1f);
        }
    }
}
