using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PaperMarioBattleSystem
{
    public sealed class Kooper : BattlePartner
    {
        public Kooper() : base(new PartnerStats(PartnerGlobals.PartnerRanks.Normal, 50, 0, 1))
        {
            Name = "Kooper";
            PartnerDescription = "He can throw his shell at enemies!";
            PartnerType = Enumerations.PartnerTypes.Kooper;

            //As Kooper is a Koopa, he can be flipped
            EntityProperties.SetVulnerableDamageEffects(Enumerations.DamageEffects.FlipsShelled | Enumerations.DamageEffects.FlipsClefts);

            Texture2D spriteSheet = AssetManager.Instance.LoadAsset<Texture2D>($"{ContentGlobals.SpriteRoot}/Characters/Kooper");
            AnimManager.SetSpriteSheet(spriteSheet);

            AnimManager.AddAnimation(AnimationGlobals.IdleName, new Animation(spriteSheet, new Animation.Frame(new Rectangle(97, 117, 30, 50), 1000d)));

            AnimManager.AddAnimation(AnimationGlobals.HurtName, new Animation(spriteSheet,
                new Animation.Frame(new Rectangle(4, 170, 42, 45), 500d),
                new Animation.Frame(new Rectangle(52, 173, 41, 42), 500d)));
            AnimManager.AddAnimation(AnimationGlobals.DeathName, new Animation(spriteSheet, new Animation.Frame(new Rectangle(52, 173, 41, 42), 1000d)));

            AnimManager.AddAnimation(AnimationGlobals.RunningName, new LoopAnimation(spriteSheet, AnimationGlobals.InfiniteLoop,
                new Animation.Frame(new Rectangle(97, 117, 30, 50), 100d),
                new Animation.Frame(new Rectangle(89, 3, 34, 48), 100d)));

            AnimManager.AddAnimation(AnimationGlobals.KooperBattleAnimations.ShellSpinName, new LoopAnimation(spriteSheet, AnimationGlobals.InfiniteLoop,
                new Animation.Frame(new Rectangle(162, 222, 28, 25), 250d),
                new Animation.Frame(new Rectangle(194, 222, 28, 25), 250d),
                new Animation.Frame(new Rectangle(225, 222, 30, 25), 250d),
                new Animation.Frame(new Rectangle(258, 222, 28, 25), 250d)));
        }

        protected sealed override BattleMenu GetMainBattleMenu()
        {
            return new PartnerBattleMenu(new KooperSubMenu());
        }

        public override void OnTurnEnd()
        {
            base.OnTurnEnd();
            BattleUIManager.Instance.ClearMenuStack();
        }
    }
}
