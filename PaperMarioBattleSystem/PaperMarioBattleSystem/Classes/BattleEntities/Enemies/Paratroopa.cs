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
    /// A Paratroopa - A Koopa Troopa with wings.
    /// </summary>
    public sealed class Paratroopa : KoopaTroopa, IWingedEntity
    {
        public Paratroopa()
        {
            Name = "Paratroopa";

            BattleStats.Level = 9;

            EntityProperties.SetVulnerableDamageEffects(EntityProperties.GetVulnerableDamageEffects() | Enumerations.DamageEffects.RemovesWings);

            ChangeHeightState(Enumerations.HeightStates.Airborne);

            EntityProperties.AddStatusProperty(Enumerations.StatusTypes.Dizzy, new StatusPropertyHolder(120d, 0));
            EntityProperties.AddStatusProperty(Enumerations.StatusTypes.Blown, new StatusPropertyHolder(110d, 0));

            Texture2D spriteSheet = AssetManager.Instance.LoadAsset<Texture2D>($"{ContentGlobals.SpriteRoot}/Enemies/Paratroopa");
            AnimManager.SetSpriteSheet(spriteSheet);

            //The Paratroopa uses the same in shell animation as a Koopa Troopa for Shell Shot, but rotates itself differently
            AnimManager.AddAnimation(AnimationGlobals.ParatroopaBattleAnimations.ShellShotName, new Animation(spriteSheet,
                new Animation.Frame(new Rectangle(2, 222, 28, 25), 1000d)));

            //This animation uses the same rectangle for every frame. However, the wings are different on those frames and the
            //Paratroopa has varying heights on each frame
            AnimManager.AddAnimation(AnimationGlobals.WingedBattleAnimations.WingedIdleName, new LoopAnimation(spriteSheet, AnimationGlobals.InfiniteLoop,
                new Animation.Frame(new Rectangle(56, 4, 32, 48), 200d)));
            //Same story with this one
            AnimManager.AddAnimation(AnimationGlobals.WingedBattleAnimations.FlyingName, new LoopAnimation(spriteSheet, AnimationGlobals.InfiniteLoop,
                new Animation.Frame(new Rectangle(103, 4, 33, 51), 200d)));
        }

        public override string GetIdleAnim()
        {
            if (Grounded == false) return AnimationGlobals.WingedBattleAnimations.WingedIdleName;

            return base.GetIdleAnim();
        }

        protected override void HandleDamageEffects(Enumerations.DamageEffects damageEffects)
        {
            //Prioritize winged first
            //This allows the Paratroopa to be flipped after landing if hit with two Jump-like moves in one turn
            if (UtilityGlobals.DamageEffectHasFlag(damageEffects, Enumerations.DamageEffects.RemovesWings) == true
                && EntityProperties.IsVulnerableToDamageEffect(Enumerations.DamageEffects.RemovesWings) == true)
            {
                HandleGrounded();
            }
            else
            {
                base.HandleDamageEffects(damageEffects);
            }
        }

        #region Winged Implementation

        public bool Grounded { get; private set; } = false;

        public BattleEntity GroundedEntity { get; private set; } = new KoopaTroopa();

        public int GroundedTurns => -1;

        public int ElapsedGroundedTurns { get; private set; } = 0;

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

        public void RemoveWings()
        {
            //Add VFX for the wings disappearing
            Texture2D spriteSheet = AssetManager.Instance.LoadAsset<Texture2D>($"{ContentGlobals.SpriteRoot}/Enemies/Paratroopa");
            CroppedTexture2D wingSprite = new CroppedTexture2D(spriteSheet, new Rectangle(66, 190, 45, 26));

            //Put the wings in the same spot as they were in the Paratroopa's last animation
            WingsDisappearVFX wingsDisappear = new WingsDisappearVFX(wingSprite, BattlePosition + new Vector2(-4, -1),
                EntityType != Enumerations.EntityTypes.Enemy, .1f - .01f, 500d, 500d, (1d / 30d) * Time.MsPerS);

            BattleVFXManager.Instance.AddVFXElement(wingsDisappear);
        }

        #endregion

        private void ChangeToGroundedEntity()
        {
            Name = GroundedEntity.Name;

            //Set the vulnerability to the same as the grounded entity. The grounded entity shouldn't have a winged vulnerabilty
            EntityProperties.SetVulnerableDamageEffects(GroundedEntity.EntityProperties.GetVulnerableDamageEffects());

            //Change HeightState
            ChangeHeightState(Enumerations.HeightStates.Grounded);
        }
    }
}
