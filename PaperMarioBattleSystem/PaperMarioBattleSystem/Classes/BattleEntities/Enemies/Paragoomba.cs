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
    public sealed class Paragoomba : Goomba, IWingedEntity
    {
        protected override MoveAction ActionUsed => Grounded == false ? new DiveKick() : base.ActionUsed;

        private CroppedTexture2D idleOneWings = null;// new CroppedTexture2D(spriteSheet, new Rectangle(3, 166, 41, 18));

        public Paragoomba()
        {
            Name = "Paragoomba";

            EntityProperties.SetVulnerableDamageEffects(Enumerations.DamageEffects.RemovesWings);

            ChangeHeightState(Enumerations.HeightStates.Airborne);

            Texture2D spriteSheet = AssetManager.Instance.LoadAsset<Texture2D>($"{ContentGlobals.SpriteRoot}/Enemies/Paragoomba");
            AnimManager.SetSpriteSheet(spriteSheet);

            AnimManager.AddAnimation(AnimationGlobals.IdleName, new LoopAnimation(spriteSheet, AnimationGlobals.InfiniteLoop,
                new Animation.Frame(new Rectangle(129, 45, 27, 28), 250d),
                new Animation.Frame(new Rectangle(2, 7, 26, 30), 250d)));
            AnimManager.AddAnimation(AnimationGlobals.HurtName, new Animation(spriteSheet,
                new Animation.Frame(new Rectangle(97, 48, 29, 27), 500d),
                new Animation.Frame(new Rectangle(65, 88, 29, 27), 500d),
                new Animation.Frame(new Rectangle(98, 89, 27, 26), 500d)));
            AnimManager.AddAnimation(AnimationGlobals.DeathName, new Animation(spriteSheet, new Animation.Frame(new Rectangle(98, 89, 27, 26), 1000d)));

            AnimManager.AddAnimation(AnimationGlobals.ParagoombaBattleAnimations.DiveKickName, new Animation(spriteSheet,
                new Animation.Frame(new Rectangle(33, 89, 27, 30), 1000d)));

            //Wings (for the first idle frame, at least) are offset (-7, -1 (or left 7, up 1)) from the Paragoomba's body
            //Both Wings for each frame are in a single cropped texture
            //The wings are rendered underneath the Paragoomba's body
            //CroppedTexture2D idleOneWings = new CroppedTexture2D(spriteSheet, new Rectangle(3, 166, 41, 18));
            idleOneWings = new CroppedTexture2D(spriteSheet, new Rectangle(3, 166, 41, 18));
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

        public override void Draw()
        {
            base.Draw();

            //Draw wings if winged
            if (Grounded == false)
            {
                //Draw wings
                SpriteRenderer.Instance.Draw(idleOneWings.Tex, Position + new Vector2(-7, -1), idleOneWings.SourceRect, Color.White, false, .09f, false);
            }
        }

        #region Interface Implementation

        public bool Grounded { get; private set; } = false;

        public BattleEntity GroundedEntity { get; private set; } = new Goomba();

        public int GroundedTurns => -1;

        public int ElapsedGroundedTurns { get; private set; }

        public void HandleGrounded()
        {
            Grounded = true;

            ChangeToGroundedEntity();

            //After all this set the GroundedEntity to null, as we don't need its information anymore
            GroundedEntity = null;
        }

        #endregion

        private void ChangeToGroundedEntity()
        {
            Name = GroundedEntity.Name;

            //Set the vulnerability to the same as the grounded entity. The grounded entity shouldn't have a winged vulnerabilty
            EntityProperties.SetVulnerableDamageEffects(GroundedEntity.EntityProperties.GetVulnerableDamageEffects());

            //Change HeightState and battle position
            ChangeHeightState(Enumerations.HeightStates.Grounded);
        }
    }
}
