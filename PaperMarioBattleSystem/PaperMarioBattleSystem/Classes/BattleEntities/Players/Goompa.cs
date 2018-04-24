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
    /// Goompa, Mario's first partner. He can't move and doesn't have any attacks.
    /// </summary>
    public class Goompa : BattlePartner
    {
        public Goompa() : base(new PartnerStats(PartnerGlobals.PartnerRanks.Normal, 50, 0, 0))
        {
            Name = "Goompa";
            PartnerDescription = "Goombario's wise grandpa.";
            PartnerType = Enumerations.PartnerTypes.Goompa;

            //Goompa doesn't take turns
            BaseTurns = -99;

            Texture2D spriteSheet = AssetManager.Instance.LoadRawTexture2D($"{ContentGlobals.SpriteRoot}/Characters/Goompa.png");
            AnimManager.SetSpriteSheet(spriteSheet);

            AnimManager.AddAnimation(AnimationGlobals.IdleName, new ReverseAnimation(null, AnimationGlobals.InfiniteLoop,
                new Animation.Frame(new Rectangle(3, 124, 26, 35), 1000),
                new Animation.Frame(new Rectangle(35, 123, 26, 36), 100, new Vector2(0, -1)),
                new Animation.Frame(new Rectangle(67, 121, 26, 38), 1000, new Vector2(0, -3))));

            AnimManager.AddAnimation(AnimationGlobals.PlayerBattleAnimations.ChoosingActionName, new Animation(null,
                new Animation.Frame(new Rectangle(3, 124, 26, 35), 100d)));
            AnimManager.AddAnimation(AnimationGlobals.PlayerBattleAnimations.DangerChoosingActionName, new Animation(null,
                new Animation.Frame(new Rectangle(3, 124, 26, 35), 100d)));

            AnimManager.AddAnimation(AnimationGlobals.HurtName, new Animation(null,
                new Animation.Frame(new Rectangle(129, 88, 30, 31), 100d)));
            AnimManager.AddAnimation(AnimationGlobals.DeathName, new Animation(null,
                new Animation.Frame(new Rectangle(129, 88, 30, 31), 1000d)));
            AnimManager.AddAnimation(AnimationGlobals.SpikedTipHurtName, new Animation(null,
                new Animation.Frame(new Rectangle(129, 88, 30, 31), 100d)));

            AnimManager.AddAnimation(AnimationGlobals.RunningName, new ReverseAnimation(null, AnimationGlobals.InfiniteLoop,
                new Animation.Frame(new Rectangle(35, 164, 28, 35), 100d),
                new Animation.Frame(new Rectangle(3, 124, 26, 35), 100d),
                new Animation.Frame(new Rectangle(161, 83, 29, 36), 100d)));

            AnimManager.AddAnimation(AnimationGlobals.PlayerBattleAnimations.DangerName, new ReverseAnimation(null, AnimationGlobals.InfiniteLoop,
                new Animation.Frame(new Rectangle(3, 124, 26, 35), 1000),
                new Animation.Frame(new Rectangle(35, 123, 26, 36), 100, new Vector2(0, -1)),
                new Animation.Frame(new Rectangle(67, 121, 26, 38), 1000, new Vector2(0, -3))));
            AnimManager.AddAnimation(AnimationGlobals.StatusBattleAnimations.InjuredName, new Animation(null,
                new Animation.Frame(new Rectangle(129, 88, 30, 31), 100d)));
            AnimManager.AddAnimation(AnimationGlobals.StatusBattleAnimations.SleepName, new Animation(null,
                new Animation.Frame(new Rectangle(129, 88, 30, 31), 100d)));
            AnimManager.AddAnimation(AnimationGlobals.StatusBattleAnimations.DizzyName, new Animation(null,
                new Animation.Frame(new Rectangle(129, 88, 30, 31), 100d)));
            AnimManager.AddAnimation(AnimationGlobals.StatusBattleAnimations.ConfusedName, new Animation(null,
                new Animation.Frame(new Rectangle(129, 88, 30, 31), 100d)));
            AnimManager.AddAnimation(AnimationGlobals.StatusBattleAnimations.StoneName, new Animation(null,
                new Animation.Frame(new Rectangle(3, 124, 26, 35), 1000)));

            AnimManager.AddAnimation(AnimationGlobals.TalkName, new ReverseAnimation(spriteSheet, AnimationGlobals.InfiniteLoop,
                new Animation.Frame(new Rectangle(3, 124, 26, 35), 32d),
                new Animation.Frame(new Rectangle(3, 163, 26, 36), 32d, new Vector2(0, -1)),
                new Animation.Frame(new Rectangle(163, 122, 26, 37), 32d, new Vector2(0, -2))));
        }

        protected override BattleMenu GetMainBattleMenu()
        {
            //Goompa doesn't have a submenu since he doesn't move
            //However, one nice idea can be to let him only use Tactics, Items, and Focus - though, most of the time he'd be doing nothing
            return null;
        }
    }
}
