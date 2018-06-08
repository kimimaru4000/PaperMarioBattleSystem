using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PaperMarioBattleSystem
{
    public sealed class Bow : BattlePartner
    {
        public Bow() : base(new PartnerStats(PartnerGlobals.PartnerRanks.Normal, 50, 0, 0))
        {
            Name = "Bow";
            PartnerDescription = "She can become transparent,\nand her specialty is slapping.";
            PartnerType = Enumerations.PartnerTypes.Bow;

            ChangeHeightState(Enumerations.HeightStates.Airborne);

            LoadAnimations();
        }

        public override void LoadAnimations()
        {
            Texture2D spriteSheet = AssetManager.Instance.LoadRawTexture2D($"{ContentGlobals.SpriteRoot}/Characters/Bow.png");
            AnimManager.SetSpriteSheet(spriteSheet);

            AnimManager.AddAnimation(AnimationGlobals.IdleName, new ReverseAnimation(null, AnimationGlobals.InfiniteLoop,
                new Animation.Frame(new Rectangle(150, 123, 41, 34), 100d, new Vector2(0, -1)),
                new Animation.Frame(new Rectangle(54, 163, 41, 34), 100d),
                new Animation.Frame(new Rectangle(54, 43, 41, 34), 100d, new Vector2(0, 1))));
            AnimManager.AddAnimation(AnimationGlobals.PlayerBattleAnimations.DangerName, new ReverseAnimation(null, AnimationGlobals.InfiniteLoop,
                new Animation.Frame(new Rectangle(150, 123, 41, 34), 100d, new Vector2(0, -1)),
                new Animation.Frame(new Rectangle(54, 163, 41, 34), 100d),
                new Animation.Frame(new Rectangle(54, 43, 41, 34), 100d, new Vector2(0, 1))));

            AnimManager.AddAnimation(AnimationGlobals.RunningName, new ReverseAnimation(null, AnimationGlobals.InfiniteLoop,
                new Animation.Frame(new Rectangle(150, 123, 41, 34), 60d, new Vector2(0, -1)),
                new Animation.Frame(new Rectangle(54, 163, 41, 34), 60d),
                new Animation.Frame(new Rectangle(54, 43, 41, 34), 60d, new Vector2(0, 1))));

            AnimManager.AddAnimation(AnimationGlobals.HurtName, new Animation(null,
                new Animation.Frame(new Rectangle(151, 44, 39, 33), 100d),
                new Animation.Frame(new Rectangle(199, 44, 39, 33), 100d),
                new Animation.Frame(new Rectangle(247, 44, 39, 33), 100d)));
            AnimManager.AddAnimation(AnimationGlobals.StatusBattleAnimations.DizzyName, new LoopAnimation(null, AnimationGlobals.InfiniteLoop,
                new Animation.Frame(new Rectangle(151, 44, 39, 33), 100d),
                new Animation.Frame(new Rectangle(199, 44, 39, 33), 100d),
                new Animation.Frame(new Rectangle(247, 44, 39, 33), 100d)));
            AnimManager.AddAnimation(AnimationGlobals.StatusBattleAnimations.ConfusedName, new LoopAnimation(null, AnimationGlobals.InfiniteLoop,
                new Animation.Frame(new Rectangle(151, 44, 39, 33), 100d),
                new Animation.Frame(new Rectangle(199, 44, 39, 33), 100d),
                new Animation.Frame(new Rectangle(247, 44, 39, 33), 100d)));

            AnimManager.AddAnimation(AnimationGlobals.DeathName, new Animation(null, new Animation.Frame(new Rectangle(199, 44, 39, 33), 1000d)));

            AnimManager.AddAnimation(AnimationGlobals.PlayerBattleAnimations.ChoosingActionName, new Animation(null, new Animation.Frame(new Rectangle(151, 4, 39, 33), 1000d)));
            AnimManager.AddAnimation(AnimationGlobals.PlayerBattleAnimations.DangerChoosingActionName, new Animation(null, new Animation.Frame(new Rectangle(151, 4, 39, 33), 1000d)));
        }

        protected override InputMenu GetMainBattleMenu()
        {
            return new PartnerBattleMenu(this, new BowSubMenu(this), Enumerations.PartnerTypes.Bow);
        }

        public override void OnTurnEnd()
        {
            base.OnTurnEnd();
            BattleUIManager.Instance.ClearMenuStack();
        }
    }
}
