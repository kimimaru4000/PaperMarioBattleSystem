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
    /// A Paragoomba - A Goomba with wings.
    /// </summary>
    public sealed class Paragoomba : Goomba, IWingedEntity, ITattleableEntity
    {
        protected override MoveAction ActionUsed => Grounded == false ? new DiveKick() : base.ActionUsed;

        public Paragoomba()
        {
            Name = "Paragoomba";

            EntityProperties.SetVulnerableDamageEffects(Enumerations.DamageEffects.RemovesWings);

            ChangeHeightState(Enumerations.HeightStates.Airborne);

            Texture2D spriteSheet = AssetManager.Instance.LoadRawTexture2D($"{ContentGlobals.SpriteRoot}/Enemies/Paragoomba.png");
            AnimManager.SetSpriteSheet(spriteSheet);

            AnimManager.AddAnimation(AnimationGlobals.WingedBattleAnimations.WingedIdleName, new LoopAnimation(spriteSheet, AnimationGlobals.InfiniteLoop,
                new Animation.Frame(new Rectangle(129, 45, 27, 28), 200d),
                new Animation.Frame(new Rectangle(1, 7, 27, 30), 200d)));
            AnimManager.AddAnimation(AnimationGlobals.WingedBattleAnimations.FlyingName, new LoopAnimation(spriteSheet, AnimationGlobals.InfiniteLoop,
                new Animation.Frame(new Rectangle(129, 45, 27, 28), 100d),
                new Animation.Frame(new Rectangle(1, 7, 27, 30), 100d)));
            AnimManager.AddAnimation(AnimationGlobals.HurtName, new Animation(spriteSheet,
                new Animation.Frame(new Rectangle(97, 48, 29, 27), 80d),
                new Animation.Frame(new Rectangle(98, 89, 27, 26), 80d)));
            AnimManager.AddAnimation(AnimationGlobals.DeathName, new Animation(spriteSheet, new Animation.Frame(new Rectangle(98, 89, 27, 26), 1000d)));

            AnimManager.AddAnimation(AnimationGlobals.ParagoombaBattleAnimations.DiveKickName, new Animation(spriteSheet,
                new Animation.Frame(new Rectangle(33, 89, 27, 30), 1000d)));

            //Wings (for the first idle frame, at least) are offset (-7, -1 (or left 7, up 1)) from the Paragoomba's body
            //Both Wings for each frame are in a single cropped texture
            //The wings are rendered underneath the Paragoomba's body

            AnimManager.AddAnimationChildFrames(AnimationGlobals.WingedBattleAnimations.WingedIdleName,
                new Animation.Frame(new Rectangle(3, 166, 41, 18), 200d, new Vector2(-7, -1), -.01f),
                new Animation.Frame(new Rectangle(50, 161, 41, 14), 200d, new Vector2(-7, 13), -.01f));
            AnimManager.AddAnimationChildFrames(AnimationGlobals.WingedBattleAnimations.FlyingName,
                new Animation.Frame(new Rectangle(3, 166, 41, 18), 100d, new Vector2(-7, -1), -.01f),
                new Animation.Frame(new Rectangle(50, 161, 41, 14), 100d, new Vector2(-7, 13), -.01f));
            AnimManager.AddAnimationChildFrames(AnimationGlobals.HurtName,
                new Animation.Frame(new Rectangle(3, 166, 41, 18), 80d, new Vector2(-4, -1), -.01f),
                new Animation.Frame(new Rectangle(3, 166, 41, 18), 80d, new Vector2(-4, -1), -.01f));
            AnimManager.AddAnimationChildFrames(AnimationGlobals.DeathName,
                new Animation.Frame(new Rectangle(3, 166, 41, 18), 1000d, new Vector2(-4, -1), -.01f));

            AnimManager.AddAnimationChildFrames(AnimationGlobals.ParagoombaBattleAnimations.DiveKickName,
                new Animation.Frame(new Rectangle(120, 121, 31, 21), 1000d, new Vector2(-1, -9), -.01f));
        }

        public override void OnBattleStart()
        {
            base.OnBattleStart();

            AnimManager.PlayAnimation(GetIdleAnim());
        }

        protected override void HandleDamageEffects(Enumerations.DamageEffects damageEffects)
        {
            base.HandleDamageEffects(damageEffects);

            if (UtilityGlobals.DamageEffectHasFlag(damageEffects, Enumerations.DamageEffects.RemovesWings) == true
                && EntityProperties.IsVulnerableToDamageEffect(Enumerations.DamageEffects.RemovesWings) == true)
            {
                HandleGrounded();
            }
        }

        public override string GetIdleAnim()
        {
            if (Grounded == false) return AnimationGlobals.WingedBattleAnimations.WingedIdleName;

            return base.GetIdleAnim();
        }

        #region Interface Implementation

        public bool Grounded { get; private set; } = false;

        public BattleEntity GroundedEntity { get; private set; } = new Goomba();

        public int GroundedTurns => -1;

        public int ElapsedGroundedTurns { get; private set; }

        public void RemoveWings()
        {
            //Remove the wings from the hurt and death animations
            Animation[] animations = AnimManager.GetAnimations(AnimationGlobals.HurtName, AnimationGlobals.DeathName);

            //Clear all child frames with wings
            for (int i = 0; i < animations.Length; i++)
            {
                animations[i].SetChildFrames(null);
            }

            //Add VFX for the wings disappearing
            Texture2D spriteSheet = AssetManager.Instance.LoadRawTexture2D($"{ContentGlobals.SpriteRoot}/Enemies/Paragoomba.png");
            CroppedTexture2D wingSprite = new CroppedTexture2D(spriteSheet, new Rectangle(3, 166, 41, 18));

            //Put the wings in the same spot as they were in the Paragoomba's last animation
            WingsDisappearVFX wingsDisappear = new WingsDisappearVFX(wingSprite, BattlePosition + new Vector2(-4, -1),
                EntityType != Enumerations.EntityTypes.Enemy, .1f - .01f, 500d, 500d, (1d / 30d) * Time.MsPerS);

            BattleVFXManager.Instance.AddVFXElement(wingsDisappear);
        }

        public void HandleGrounded()
        {
            Grounded = true;

            ChangeToGroundedEntity();

            //Queue the BattleEvent to move the entity down
            BattleEventManager.Instance.QueueBattleEvent((int)BattleGlobals.StartEventPriorities.Damage - 1,
                new BattleManager.BattleState[] { BattleManager.BattleState.Turn, BattleManager.BattleState.TurnEnd },
                new GroundedBattleEvent(this, new Vector2(BattlePosition.X, BattleManager.Instance.EnemyStartPos.Y)));

            //Queue the BattleEvent to remove the wings
            BattleEventManager.Instance.QueueBattleEvent((int)BattleGlobals.StartEventPriorities.Damage - 1,
                new BattleManager.BattleState[] { BattleManager.BattleState.Turn, BattleManager.BattleState.TurnEnd },
                new RemoveWingsBattleEvent(this));

            //After all this set the GroundedEntity to null, as we don't need its information anymore
            GroundedEntity = null;
        }

        #endregion

        private void ChangeToGroundedEntity()
        {
            //Check if the winged entity and its grounded version have entries in the Tattle database
            bool wingedInTattle = TattleDatabase.HasTattleDescription(Name);
            bool groundedInTattle = TattleDatabase.HasTattleDescription(GroundedEntity.Name);

            //If the winged entity has an entry and the grounded version doesn't, remove its ShowHP property
            if (wingedInTattle == true && groundedInTattle == false)
            {
                this.SubtractShowHPProperty();
            }
            //If the winged entity doesn't have an entry and the grounded version does, add its ShowHP property
            else if (wingedInTattle == false && groundedInTattle == true)
            {
                this.AddShowHPProperty();
            }

            Name = GroundedEntity.Name;

            //Set the vulnerability to the same as the grounded entity. The grounded entity shouldn't have a winged vulnerabilty
            EntityProperties.SetVulnerableDamageEffects(GroundedEntity.EntityProperties.GetVulnerableDamageEffects());

            //Change HeightState
            ChangeHeightState(Enumerations.HeightStates.Grounded);
        }

        #region Tattle Information

        public new string[] GetTattleLogEntry()
        {
            return new string[]
            {
                $"HP: {BattleStats.MaxHP} Attack: {BattleStats.BaseAttack}\nDefense: {BattleStats.BaseDefense}",
                $"A Goomba with wings. Can't\nreach it with a hammer while",
                "it's in the air, but once\n it's damaged, its wings get",
                $"clipped. It's kind of sad,\nreally."
            };
        }

        public new string[] GetTattleDescription()
        {
            return new string[]
            {
                "That's a Paragoomba. Basically a Goomba with wings. I'm jealous!",
                $"Maximum HP is {BattleStats.MaxHP}, Attack is {BattleStats.BaseAttack}, and Defense is {BattleStats.BaseDefense}.",
                "You can't hammer it while it's flying, but rough it up and it'll totally plummet!"
            };
        }

        #endregion
    }
}
