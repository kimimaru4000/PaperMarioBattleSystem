using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PaperMarioBattleSystem
{
    public sealed class Kooper : BattlePartner, IFlippableEntity
    {
        public IFlippableBehavior FlippedBehavior { get; private set; } = null;

        public Kooper() : base(new PartnerStats(PartnerGlobals.PartnerRanks.Normal, 50, 0, 1))
        {
            Name = "Kooper";
            PartnerDescription = "He can throw his shell at enemies!";
            PartnerType = Enumerations.PartnerTypes.Kooper;

            //As Kooper is a Koopa, he can be flipped
            EntityProperties.SetVulnerableDamageEffects(Enumerations.DamageEffects.FlipsShelled | Enumerations.DamageEffects.FlipsClefts);

            FlippedBehavior = new KoopaFlippedBehavior(this, 2, EntityProperties.GetVulnerableDamageEffects(), BattleStats.BaseDefense);

            Texture2D spriteSheet = AssetManager.Instance.LoadRawTexture2D($"{ContentGlobals.SpriteRoot}/Characters/Kooper.png");
            AnimManager.SetSpriteSheet(spriteSheet);

            AnimManager.AddAnimation(AnimationGlobals.IdleName, new Animation(spriteSheet, new Animation.Frame(new Rectangle(97, 117, 30, 50), 1000d)));
            AnimManager.AddAnimation(AnimationGlobals.PlayerBattleAnimations.DangerName, new Animation(spriteSheet, new Animation.Frame(new Rectangle(97, 117, 30, 50), 1000d)));

            AnimManager.AddAnimation(AnimationGlobals.HurtName, new Animation(spriteSheet,
                new Animation.Frame(new Rectangle(4, 170, 42, 45), 500d),
                new Animation.Frame(new Rectangle(52, 173, 41, 42), 500d)));
            AnimManager.AddAnimation(AnimationGlobals.DeathName, new Animation(spriteSheet, new Animation.Frame(new Rectangle(52, 173, 41, 42), 1000d)));

            AnimManager.AddAnimation(AnimationGlobals.RunningName, new LoopAnimation(spriteSheet, AnimationGlobals.InfiniteLoop,
                new Animation.Frame(new Rectangle(97, 117, 30, 50), 100d, new Vector2(-2, 0)),
                new Animation.Frame(new Rectangle(89, 3, 34, 48), 100d)));

            AnimManager.AddAnimation(AnimationGlobals.PlayerBattleAnimations.ChoosingActionName, new Animation(spriteSheet, new Animation.Frame(new Rectangle(225, 117, 30, 50), 1000d)));
            AnimManager.AddAnimation(AnimationGlobals.PlayerBattleAnimations.DangerChoosingActionName, new Animation(spriteSheet, new Animation.Frame(new Rectangle(225, 117, 30, 50), 1000d)));

            AnimManager.AddAnimation(AnimationGlobals.PlayerBattleAnimations.StarSpecialName, new Animation(spriteSheet,
                new Animation.Frame(new Rectangle(233, 60, 34, 51), 700d)));
            AnimManager.AddAnimation(AnimationGlobals.PlayerBattleAnimations.StarWishName, new Animation(spriteSheet,
                new Animation.Frame(new Rectangle(193, 62, 30, 49), 700d)));

            AnimManager.AddAnimation(AnimationGlobals.StatusBattleAnimations.DizzyName, new Animation(spriteSheet, new Animation.Frame(new Rectangle(97, 117, 30, 50), 1000d)));
            AnimManager.AddAnimation(AnimationGlobals.StatusBattleAnimations.ConfusedName, new Animation(spriteSheet, new Animation.Frame(new Rectangle(97, 117, 30, 50), 1000d)));
            AnimManager.AddAnimation(AnimationGlobals.StatusBattleAnimations.SleepName, new Animation(spriteSheet, new Animation.Frame(new Rectangle(97, 117, 30, 50), 1000d)));
            AnimManager.AddAnimation(AnimationGlobals.StatusBattleAnimations.InjuredName, new LoopAnimation(spriteSheet, AnimationGlobals.InfiniteLoop,
                new Animation.Frame(new Rectangle(4, 170, 42, 45), 500d),
                new Animation.Frame(new Rectangle(52, 173, 41, 42), 500d)));

            AnimManager.AddAnimation(AnimationGlobals.ShelledBattleAnimations.ShellSpinName, new LoopAnimation(spriteSheet, AnimationGlobals.InfiniteLoop,
                new Animation.Frame(new Rectangle(162, 222, 28, 25), 250d),
                new Animation.Frame(new Rectangle(194, 222, 28, 25), 250d),
                new Animation.Frame(new Rectangle(225, 222, 30, 25), 250d),
                new Animation.Frame(new Rectangle(258, 222, 28, 25), 250d)));
            AnimManager.AddAnimation(AnimationGlobals.ShelledBattleAnimations.FlippedName, new LoopAnimation(spriteSheet, AnimationGlobals.InfiniteLoop,
                new Animation.Frame(new Rectangle(69, 221, 53, 26), 350d),
                new Animation.Frame(new Rectangle(5, 218, 54, 28), 350d)));
        }

        public override void CleanUp()
        {
            base.CleanUp();

            FlippedBehavior?.CleanUp();
        }

        protected sealed override BattleMenu GetMainBattleMenu()
        {
            return new PartnerBattleMenu(new KooperSubMenu(), Enumerations.PartnerTypes.Kooper);
        }

        public override void OnTurnStart()
        {
            base.OnTurnStart();

            if (FlippedBehavior.Flipped == true)
            {
                //Make Kooper do a NoAction instead of directly ending his turn
                BattleUIManager.Instance.ClearMenuStack();
                StartAction(new NoAction(), true, null);
            }
        }

        public override void OnTurnEnd()
        {
            base.OnTurnEnd();
            BattleUIManager.Instance.ClearMenuStack();
        }

        public override string GetIdleAnim()
        {
            //If Flipped, return the Flipped animation
            if (FlippedBehavior.Flipped == true) return AnimationGlobals.ShelledBattleAnimations.FlippedName;

            return base.GetIdleAnim();
        }
    }
}
