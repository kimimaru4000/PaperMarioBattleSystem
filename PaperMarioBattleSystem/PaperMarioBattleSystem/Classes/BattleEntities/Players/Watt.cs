using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static PaperMarioBattleSystem.Enumerations;

namespace PaperMarioBattleSystem
{
    public sealed class Watt : BattlePartner
    {
        /// <summary>
        /// Watt's Electric Payback.
        /// </summary>
        public StatusGlobals.PaybackHolder ElectricPayback { get; private set; } = new StatusGlobals.PaybackHolder(StatusGlobals.PaybackTypes.Constant, PhysicalAttributes.Electrified,
            Elements.Electric, new ContactTypes[] { ContactTypes.Latch, ContactTypes.SideDirect, ContactTypes.TopDirect },
            new ContactProperties[] { ContactProperties.None },
            ContactResult.PartialSuccess, ContactResult.Success, 1, null);

        public Watt() : base(new PartnerStats(PartnerGlobals.PartnerRanks.Normal, 50, 0, 0))
        {
            Name = "Watt";
            PartnerDescription = "";
            PartnerType = Enumerations.PartnerTypes.Watt;

            ChangeHeightState(HeightStates.Airborne);

            //Watt is Electrified
            EntityProperties.AddPhysAttribute(Enumerations.PhysicalAttributes.Electrified);

            //Add Watt's Electrified Payback
            EntityProperties.AddPayback(ElectricPayback);

            //Watt is a light source that fully lights any battle
            EntityProperties.AddAdditionalProperty(AdditionalProperty.LightSource, 10000d);

            LoadAnimations();
        }

        public override void LoadAnimations()
        {
            Texture2D spriteSheet = AssetManager.Instance.LoadRawTexture2D($"{ContentGlobals.SpriteRoot}/Characters/Watt.png");
            AnimManager.SetSpriteSheet(spriteSheet);

            AnimManager.AddAnimation(AnimationGlobals.IdleName, new LoopAnimation(null, AnimationGlobals.InfiniteLoop,
                new Animation.Frame(new Rectangle(102, 149, 34, 33), 70d, new Vector2(0, 0)),
                new Animation.Frame(new Rectangle(151, 149, 33, 34), 70d, new Vector2(1, 1)),
                new Animation.Frame(new Rectangle(103, 230, 34, 31), 70d, new Vector2(1, 0))));
            AnimManager.AddAnimation(AnimationGlobals.PlayerBattleAnimations.DangerName, new LoopAnimation(null, AnimationGlobals.InfiniteLoop,
                new Animation.Frame(new Rectangle(102, 149, 34, 33), 70d, new Vector2(0, 0)),
                new Animation.Frame(new Rectangle(151, 149, 33, 34), 70d, new Vector2(1, 1)),
                new Animation.Frame(new Rectangle(103, 230, 34, 31), 70d, new Vector2(1, 0))));

            AnimManager.AddAnimation(AnimationGlobals.HurtName, new Animation(null,
                new Animation.Frame(new Rectangle(199, 189, 33, 34), 70d),
                new Animation.Frame(new Rectangle(6, 230, 36, 32), 70d),
                new Animation.Frame(new Rectangle(53, 227, 36, 36), 70d, new Vector2(-1, -1))));
            AnimManager.AddAnimation(AnimationGlobals.SpikedTipHurtName, new Animation(null,
                new Animation.Frame(new Rectangle(199, 189, 33, 34), 70d),
                new Animation.Frame(new Rectangle(6, 230, 36, 32), 70d),
                new Animation.Frame(new Rectangle(53, 227, 36, 36), 70d, new Vector2(-1, -1))));

            AnimManager.AddAnimation(AnimationGlobals.DeathName, new Animation(null,
                new Animation.Frame(new Rectangle(6, 230, 36, 32), 1000d)));

            AnimManager.AddAnimation(AnimationGlobals.RunningName, new LoopAnimation(null, AnimationGlobals.InfiniteLoop,
                new Animation.Frame(new Rectangle(102, 149, 34, 33), 50d, new Vector2(0, 0)),
                new Animation.Frame(new Rectangle(151, 149, 33, 34), 50d, new Vector2(1, 1)),
                new Animation.Frame(new Rectangle(103, 230, 34, 31), 50d, new Vector2(1, 0))));

            AnimManager.AddAnimation(AnimationGlobals.PlayerBattleAnimations.ChoosingActionName, new LoopAnimation(null, AnimationGlobals.InfiniteLoop,
                new Animation.Frame(new Rectangle(7, 269, 33, 34), 70d, new Vector2(1, 0)),
                new Animation.Frame(new Rectangle(150, 229, 34, 33), 70d, new Vector2(0, -1)),
                new Animation.Frame(new Rectangle(55, 270, 34, 31), 70d, new Vector2(1, -1))));
            AnimManager.AddAnimation(AnimationGlobals.PlayerBattleAnimations.DangerChoosingActionName, new LoopAnimation(null, AnimationGlobals.InfiniteLoop,
                new Animation.Frame(new Rectangle(7, 269, 33, 34), 70d, new Vector2(1, 0)),
                new Animation.Frame(new Rectangle(150, 229, 34, 33), 70d, new Vector2(0, -1)),
                new Animation.Frame(new Rectangle(55, 270, 34, 31), 70d, new Vector2(1, -1))));

            AnimManager.AddAnimation(AnimationGlobals.StatusBattleAnimations.DizzyName, new LoopAnimation(null, AnimationGlobals.InfiniteLoop,
                new Animation.Frame(new Rectangle(103, 112, 34, 28), 70d),
                new Animation.Frame(new Rectangle(199, 111, 33, 31), 70d),
                new Animation.Frame(new Rectangle(7, 152, 34, 28), 70d),
                new Animation.Frame(new Rectangle(150, 111, 34, 30), 70d, new Vector2(-1, 0))));
            AnimManager.AddAnimation(AnimationGlobals.StatusBattleAnimations.ConfusedName, new LoopAnimation(null, AnimationGlobals.InfiniteLoop,
                new Animation.Frame(new Rectangle(103, 112, 34, 28), 70d),
                new Animation.Frame(new Rectangle(199, 111, 33, 31), 70d),
                new Animation.Frame(new Rectangle(7, 152, 34, 28), 70d),
                new Animation.Frame(new Rectangle(150, 111, 34, 30), 70d, new Vector2(-1, 0))));
            AnimManager.AddAnimation(AnimationGlobals.StatusBattleAnimations.InjuredName, new LoopAnimation(null, AnimationGlobals.InfiniteLoop,
                new Animation.Frame(new Rectangle(103, 112, 34, 28), 70d),
                new Animation.Frame(new Rectangle(199, 111, 33, 31), 70d),
                new Animation.Frame(new Rectangle(7, 152, 34, 28), 70d),
                new Animation.Frame(new Rectangle(150, 111, 34, 30), 70d, new Vector2(-1, 0))));
            AnimManager.AddAnimation(AnimationGlobals.StatusBattleAnimations.SleepName, new LoopAnimation(null, AnimationGlobals.InfiniteLoop,
                new Animation.Frame(new Rectangle(199, 230, 34, 31), 150d),
                new Animation.Frame(new Rectangle(150, 189, 34, 33), 150d, new Vector2(-1, 0))));
        }

        protected override BattleMenu GetMainBattleMenu()
        {
            return new PartnerBattleMenu(new WattSubMenu(), PartnerTypes.Watt);
        }

        public override void OnTurnEnd()
        {
            base.OnTurnEnd();
            BattleUIManager.Instance.ClearMenuStack();
        }
    }
}
