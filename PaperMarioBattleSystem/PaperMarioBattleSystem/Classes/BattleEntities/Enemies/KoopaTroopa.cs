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
    /// A Koopa Troopa
    /// </summary>
    public class KoopaTroopa : BattleEnemy, IFlippableEntity, ITattleableEntity
    {
        //NOTE: Temporary until we get a simple enemy AI system in
        protected virtual MoveAction ActionUsed => new ShellToss();

        public KoopaTroopa() : base(new Stats(8, 4, 0, 1, 1))
        {
            Name = "Koopa Troopa";

            EntityProperties.SetVulnerableDamageEffects(Enumerations.DamageEffects.FlipsShelled | Enumerations.DamageEffects.FlipsClefts);

            EntityProperties.AddStatusProperty(Enumerations.StatusTypes.Sleep, new StatusPropertyHolder(95d, 0));
            EntityProperties.AddStatusProperty(Enumerations.StatusTypes.Dizzy, new StatusPropertyHolder(90d, 0));
            EntityProperties.AddStatusProperty(Enumerations.StatusTypes.Confused, new StatusPropertyHolder(90d, 0));
            EntityProperties.AddStatusProperty(Enumerations.StatusTypes.Tiny, new StatusPropertyHolder(95d, 0));
            EntityProperties.AddStatusProperty(Enumerations.StatusTypes.Immobilized, new StatusPropertyHolder(100d, 0));
            EntityProperties.AddStatusProperty(Enumerations.StatusTypes.DEFDown, new StatusPropertyHolder(95d, 0));
            EntityProperties.AddStatusProperty(Enumerations.StatusTypes.Burn, new StatusPropertyHolder(100d, 0));
            EntityProperties.AddStatusProperty(Enumerations.StatusTypes.Frozen, new StatusPropertyHolder(90d, 0));
            EntityProperties.AddStatusProperty(Enumerations.StatusTypes.Fright, new StatusPropertyHolder(95d, 0));
            EntityProperties.AddStatusProperty(Enumerations.StatusTypes.Blown, new StatusPropertyHolder(90d, 0));
            EntityProperties.AddStatusProperty(Enumerations.StatusTypes.KO, new StatusPropertyHolder(100d, 0));

            Texture2D spriteSheet = AssetManager.Instance.LoadAsset<Texture2D>($"{ContentGlobals.SpriteRoot}/Enemies/KoopaTroopa");
            AnimManager.SetSpriteSheet(spriteSheet);

            AnimManager.AddAnimation(AnimationGlobals.IdleName, new Animation(spriteSheet, new Animation.Frame(new Rectangle(9, 117, 33, 50), 1000d)));
            AnimManager.AddAnimation(AnimationGlobals.RunningName, new ReverseAnimation(spriteSheet, AnimationGlobals.InfiniteLoop,
                new Animation.Frame(new Rectangle(247, 4, 40, 48), 200d),
                new Animation.Frame(new Rectangle(248, 61, 34, 49), 200d),
                new Animation.Frame(new Rectangle(201, 62, 33, 49), 200d)));

            AnimManager.AddAnimation(AnimationGlobals.HurtName, new Animation(spriteSheet,
                new Animation.Frame(new Rectangle(102, 115, 39, 44), 250d),
                new Animation.Frame(new Rectangle(200, 115, 36, 43), 250d)));
            AnimManager.AddAnimation(AnimationGlobals.DeathName, new Animation(spriteSheet,
                new Animation.Frame(new Rectangle(244, 117, 40, 42), 1000d)));

            AnimManager.AddAnimation(AnimationGlobals.ShelledBattleAnimations.EnterShellName, new Animation(spriteSheet,
                new Animation.Frame(new Rectangle(193, 245, 27, 26), 70d),
                new Animation.Frame(new Rectangle(34, 278, 26, 24), 70d)));
            AnimManager.AddAnimation(AnimationGlobals.ShelledBattleAnimations.ShellSpinName, new LoopAnimation(spriteSheet, AnimationGlobals.InfiniteLoop,
                new Animation.Frame(new Rectangle(2, 246, 28, 25), 100d),
                new Animation.Frame(new Rectangle(33, 246, 30, 25), 100d),
                new Animation.Frame(new Rectangle(2, 278, 28, 25), 100d),
                new Animation.Frame(new Rectangle(162, 246, 28, 25), 100d),
                new Animation.Frame(new Rectangle(129, 246, 30, 25), 100d),
                new Animation.Frame(new Rectangle(322, 214, 28, 25), 100d)));
            AnimManager.AddAnimation(AnimationGlobals.ShelledBattleAnimations.ExitShellName, new Animation(spriteSheet,
                new Animation.Frame(new Rectangle(34, 278, 26, 24), 70d),
                new Animation.Frame(new Rectangle(193, 245, 27, 26), 70d)));
            AnimManager.AddAnimation(AnimationGlobals.ShelledBattleAnimations.FlippedName, new LoopAnimation(spriteSheet, AnimationGlobals.InfiniteLoop,
                new Animation.Frame(new Rectangle(5, 216, 53, 23), 300d),
                new Animation.Frame(new Rectangle(68, 209, 54, 30), 300d)));
        }

        public override void OnTurnStart()
        {
            base.OnTurnStart();

            if (Flipped == true)
            {
                ManageFlippedTurn();
            }
            else
            {
                StartAction(ActionUsed, BattleManager.Instance.GetFrontPlayer());
            }
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

        #region Flippable Implementation

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

            //Don't play this animation if dead
            if (IsDead == false)
            {
                BattleEventManager.Instance.QueueBattleEvent((int)BattleGlobals.StartEventPriorities.Damage - 2,
                    new BattleManager.BattleState[] { BattleManager.BattleState.Turn, BattleManager.BattleState.TurnEnd },
                    new PlayAnimBattleEvent(this, GetIdleAnim(), false));
            }

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

            //Make the Koopa Troopa do a NoAction instead of directly ending its turn
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

        #region Tattle Information

        public string[] GetTattleLogEntry()
        {
            return new string[]
            {
                $"HP: {BattleStats.MaxHP} Attack: {BattleStats.BaseAttack}\nDefense: {BattleStats.BaseDefense}",
                $"Koopa Troopas have been around forever. Jump on them to flip them over and drop their Defense to 0."
            };
        }

        public string[] GetTattleDescription()
        {
            return new string[]
            {
                "That's a Koopa Troopa. They've been around forever! Gotta respect the longevity!",
                "Their shells are hard, but flip them over and their Defense drops to zero.",
                "And you know how to flip them over, right? Just jump on their heads!"
            };
        }

        #endregion
    }
}
