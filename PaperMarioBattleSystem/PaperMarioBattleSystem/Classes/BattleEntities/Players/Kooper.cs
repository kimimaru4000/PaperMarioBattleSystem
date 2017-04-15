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

            AnimManager.AddAnimation(AnimationGlobals.PlayerBattleAnimations.StarSpecialName, new Animation(spriteSheet,
                new Animation.Frame(new Rectangle(233, 60, 34, 51), 700d)));
            AnimManager.AddAnimation(AnimationGlobals.PlayerBattleAnimations.StarWishName, new Animation(spriteSheet,
                new Animation.Frame(new Rectangle(193, 62, 30, 49), 700d)));

            AnimManager.AddAnimation(AnimationGlobals.ShelledBattleAnimations.ShellSpinName, new LoopAnimation(spriteSheet, AnimationGlobals.InfiniteLoop,
                new Animation.Frame(new Rectangle(162, 222, 28, 25), 250d),
                new Animation.Frame(new Rectangle(194, 222, 28, 25), 250d),
                new Animation.Frame(new Rectangle(225, 222, 30, 25), 250d),
                new Animation.Frame(new Rectangle(258, 222, 28, 25), 250d)));
            AnimManager.AddAnimation(AnimationGlobals.ShelledBattleAnimations.FlippedName, new LoopAnimation(spriteSheet, AnimationGlobals.InfiniteLoop,
                new Animation.Frame(new Rectangle(69, 221, 53, 26), 350d),
                new Animation.Frame(new Rectangle(5, 218, 54, 28), 350d)));
        }

        protected sealed override BattleMenu GetMainBattleMenu()
        {
            return new PartnerBattleMenu(new KooperSubMenu());
        }

        public override void OnTurnStart()
        {
            base.OnTurnStart();

            if (Flipped == true)
            {
                ManageFlippedTurn();
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
            if (Flipped == true) return AnimationGlobals.ShelledBattleAnimations.FlippedName;

            return base.GetIdleAnim();
        }

        protected override void HandleDamageEffects(Enumerations.DamageEffects damageEffects)
        {
            base.HandleDamageEffects(damageEffects);

            //Check whether any of the flags to flip are here
            if (UtilityGlobals.DamageEffectHasFlag(FlippedOnEffects, damageEffects) == true)
            {
                HandleFlipped();
            }
        }

        #region Interface Implementations

        public bool Flipped { get; private set; } = false;

        public int FlippedTurns { get; private set; } = 2;

        public int ElapsedFlippedTurns { get; private set; } = 0;

        public Enumerations.DamageEffects FlippedOnEffects => 
            (Enumerations.DamageEffects.FlipsShelled | Enumerations.DamageEffects.FlipsClefts);

        public int DefenseLoss => BattleStats.BaseDefense;

        public void HandleFlipped()
        {
            if (Flipped == false)
            {
                int immobile = EntityProperties.GetAdditionalProperty<int>(Enumerations.AdditionalProperty.Immobile) + 1;
                EntityProperties.AddAdditionalProperty(Enumerations.AdditionalProperty.Immobile, immobile);

                //Lower defense by an amount when flipped
                LowerDefense(DefenseLoss);
            }

            Flipped = true;

            AnimManager.PlayAnimation(AnimationGlobals.ShelledBattleAnimations.FlippedName);

            //Getting hit again while flipped refreshes the flip timer
            ElapsedFlippedTurns = 0;
        }

        #endregion

        private void ManageFlippedTurn()
        {
            ElapsedFlippedTurns++;

            if (ElapsedFlippedTurns >= FlippedTurns)
            {
                //Get up; this still uses up a turn
                UnFlip();
            }

            //Make Kooper do a NoAction instead of directly ending his turn
            BattleUIManager.Instance.ClearMenuStack();
            StartAction(new NoAction(), null);
        }

        private void UnFlip()
        {
            Flipped = false;
            AnimManager.PlayAnimation(GetIdleAnim(), true);

            int immobile = EntityProperties.GetAdditionalProperty<int>(Enumerations.AdditionalProperty.Immobile) - 1;
            EntityProperties.RemoveAdditionalProperty(Enumerations.AdditionalProperty.Immobile);
            if (immobile > 0)
            {
                EntityProperties.AddAdditionalProperty(Enumerations.AdditionalProperty.Immobile, immobile);
            }

            //Raise defense again after unflipping
            RaiseDefense(DefenseLoss);

            ElapsedFlippedTurns = 0;
        }
    }
}
