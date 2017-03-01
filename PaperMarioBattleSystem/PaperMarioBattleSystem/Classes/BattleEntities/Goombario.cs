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

            Texture2D spriteSheet = AssetManager.Instance.LoadAsset<Texture2D>($"{ContentGlobals.SpriteRoot}/Characters/Goombario");
            AnimManager.SetSpriteSheet(spriteSheet);

            AnimManager.AddAnimation(AnimationGlobals.IdleName, new Animation(spriteSheet, new Animation.Frame(new Rectangle(67, 89, 26, 30), 1000d)));
            AnimManager.AddAnimation(AnimationGlobals.HurtName, new Animation(spriteSheet, new Animation.Frame(new Rectangle(49, 164, 30, 27), 1000d)));
            AnimManager.AddAnimation(AnimationGlobals.DeathName, new Animation(spriteSheet, new Animation.Frame(new Rectangle(81, 163, 30, 28), 1000d)));
        }

        protected sealed override BattleMenu GetMainBattleMenu()
        {
            return new PartnerBattleMenu(new GoombarioSubMenu());
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
