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
    public sealed class Paragoomba : Goomba, ITattleableEntity, IWingedEntity
    {
        protected override MoveAction ActionUsed => WingedBehavior.Grounded == false ? new DiveKick() : base.ActionUsed;

        public IWingedBehavior WingedBehavior { get; private set; } = null;

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

        public override void CleanUp()
        {
            base.CleanUp();

            WingedBehavior?.CleanUp();
        }

        public override void OnBattleStart()
        {
            base.OnBattleStart();

            WingedBehavior = new ParagoombaWingedBehavior(this, -1, EntityProperties.GetVulnerableDamageEffects(), new Goomba());

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
                $"A Goomba with wings. Can't\nreach it with a hammer while",
                "it's in the air, but once\n it's damaged, its wings get",
                $"clipped. It's kind of sad,\nreally."
            };
        }

        public new string[] GetTattleDescription()
        {
            if (WingedBehavior.Grounded == true) return base.GetTattleDescription();

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
