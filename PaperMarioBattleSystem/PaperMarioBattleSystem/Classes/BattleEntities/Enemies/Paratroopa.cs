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
    public sealed class Paratroopa : KoopaTroopa, ITattleableEntity, IWingedEntity
    {
        public IWingedBehavior WingedBehavior { get; private set; } = null;

        public Paratroopa()
        {
            Name = "Paratroopa";

            BattleStats.Level = 9;

            EntityProperties.SetVulnerableDamageEffects(EntityProperties.GetVulnerableDamageEffects() | Enumerations.DamageEffects.RemovesWings);

            ChangeHeightState(Enumerations.HeightStates.Airborne);

            EntityProperties.AddStatusProperty(Enumerations.StatusTypes.Dizzy, new StatusPropertyHolder(120d, 0));
            EntityProperties.AddStatusProperty(Enumerations.StatusTypes.Blown, new StatusPropertyHolder(110d, 0));
        }

        public override void LoadAnimations()
        {
            base.LoadAnimations();

            Texture2D spriteSheet = AssetManager.Instance.LoadRawTexture2D($"{ContentGlobals.SpriteRoot}/Enemies/Paratroopa.png");
            AnimManager.SetSpriteSheet(spriteSheet);

            //The Paratroopa uses the same in shell animation as a Koopa Troopa for Shell Shot, but rotates itself differently
            AnimManager.AddAnimation(AnimationGlobals.ParatroopaBattleAnimations.ShellShotName, new Animation(spriteSheet,
                new Animation.Frame(new Rectangle(2, 222, 28, 25), 1000d)));

            LoopAnimation wingedIdle = new LoopAnimation(spriteSheet, AnimationGlobals.InfiniteLoop,
                new Animation.Frame(new Rectangle(56, 4, 32, 48), 100d),
                new Animation.Frame(new Rectangle(56, 4, 32, 48), 100d, new Vector2(0, -1)),
                new Animation.Frame(new Rectangle(56, 4, 32, 48), 100d, new Vector2(0, -2)),
                new Animation.Frame(new Rectangle(56, 4, 32, 48), 100d, new Vector2(0, -1)));

            //This animation uses the same rectangle for every frame. However, the wings are different on those frames and the
            //Paratroopa has varying heights on each frame
            AnimManager.AddAnimation(AnimationGlobals.WingedBattleAnimations.WingedIdleName, wingedIdle);
            wingedIdle.SetChildFrames(
                new Animation.Frame(new Rectangle(72, 156, 19, 27), 100d, new Vector2(19, -13)),
                new Animation.Frame(new Rectangle(104, 220, 23, 23), 100d, new Vector2(21, -8)),
                new Animation.Frame(new Rectangle(40, 219, 18, 28), 100d, new Vector2(19, 8)),
                new Animation.Frame(new Rectangle(248, 190, 21, 23), 100d, new Vector2(20, 1)));

            //NOTE: Incomplete wing frames; the wings on the left of the Paratroopa will require more work to get in due to the way the wings are stored

            //Same story with this one
            LoopAnimation flyingName = new LoopAnimation(spriteSheet, AnimationGlobals.InfiniteLoop,
                new Animation.Frame(new Rectangle(103, 4, 33, 51), 80d),
                new Animation.Frame(new Rectangle(103, 4, 33, 51), 80d, new Vector2(0, -1)),
                new Animation.Frame(new Rectangle(103, 4, 33, 51), 80d, new Vector2(0, -2)),
                new Animation.Frame(new Rectangle(103, 4, 33, 51), 80d, new Vector2(0, -1)));
            AnimManager.AddAnimation(AnimationGlobals.WingedBattleAnimations.FlyingName, flyingName);
            flyingName.SetChildFrames(
                new Animation.Frame(new Rectangle(72, 156, 19, 27), 80d, new Vector2(26, -4)),
                new Animation.Frame(new Rectangle(104, 220, 23, 23), 80d, new Vector2(26, 4)),
                new Animation.Frame(new Rectangle(40, 219, 18, 28), 80d, new Vector2(26, 16)),
                new Animation.Frame(new Rectangle(248, 190, 21, 23), 80d, new Vector2(26, 13)));

            Animation hurtName = new Animation(spriteSheet,
                new Animation.Frame(new Rectangle(5, 59, 39, 44), 250d),
                new Animation.Frame(new Rectangle(200, 3, 36, 43), 250d));
            AnimManager.AddAnimation(AnimationGlobals.HurtName, hurtName);
            //NOTE: Not accurate - in PM, it looks like the wings are rotated slightly to match the Paratroopa's pose in its hurt animation
            hurtName.SetChildFrames(
                new Animation.Frame(new Rectangle(66, 190, 45, 26), 250d, new Vector2(3, -6), -.01f),
                new Animation.Frame(new Rectangle(66, 190, 45, 26), 250d, new Vector2(3, -6), -.01f));
        }

        protected override void SetFlippedBehavior()
        {
            FlippedBehavior = new ParatroopaFlippedBehavior(this, 2, EntityProperties.GetVulnerableDamageEffects(), BattleStats.BaseDefense);
        }

        public override void CleanUp()
        {
            base.CleanUp();

            WingedBehavior?.CleanUp();
        }

        public override void OnEnteredBattle()
        {
            base.OnEnteredBattle();

            WingedBehavior = new ParatroopaWingedBehavior(this, -1, Enumerations.DamageEffects.RemovesWings, new KoopaTroopa());

            AnimManager.PlayAnimation(GetIdleAnim());
        }

        public override string GetIdleAnim()
        {
            if (WingedBehavior.Grounded == false) return AnimationGlobals.WingedBattleAnimations.WingedIdleName;

            return base.GetIdleAnim();
        }
        
        #region Tattle Information

        public new string[] GetTattleLogEntry()
        {
            if (WingedBehavior.Grounded == true) return base.GetTattleLogEntry();   

            return new string[]
            {
                $"HP: {BattleStats.MaxHP} Attack: {BattleStats.BaseAttack}\nDefense: {BattleStats.BaseDefense}",
                $"A Koopa Troopa with wings\nthat stays airborne until",
                "you stomp on it and send\nit plunging to the ground."
            };
        }

        public new string GetTattleDescription()
        {
            if (WingedBehavior.Grounded == true) return base.GetTattleDescription();

            return "That's a Koopa Paratroopa.\n<wait value=\"250\">Well, umm<wait value=\"10\">.<wait value=\"10\">.<wait value=\"10\">.<wait value=\"300\"> It's basically\na Koopa Troopa with wings.\n<k><p>" +
                   "Max HP is 4, Attack is 2, and\nDefense is 1.<wait value=\"300\"> I kinda hate\nthat this guy gets to fly.\n<k><p>" +
                   "Of course, you can stomp on\nhim and he'll plunge down\nand be a plain Koopa Troopa.\n<k><p>" +
                   "Yeah, do that, and he's ours!\n<wait value=\"250\">Stomp again to flip him, and\n<wait value=\"250\">his arms and legs are useless!\n<k><p>" +
                   "<dynamic value=\"3\">Oops!</dynamic><wait value=\"100\"> Sorry, that's not true.\n<wait value=\"300\">It looks like he can still\nwiggle them around a bit...<k>";
        }

        #endregion
    }
}
