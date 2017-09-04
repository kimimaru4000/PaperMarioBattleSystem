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

            Texture2D spriteSheet = AssetManager.Instance.LoadAsset<Texture2D>($"{ContentGlobals.SpriteRoot}/Characters/Bow");
            AnimManager.SetSpriteSheet(spriteSheet);

            AnimManager.AddAnimation(AnimationGlobals.IdleName, new Animation(null, new Animation.Frame(new Rectangle(151, 4, 39, 33), 1000d)));
            AnimManager.AddAnimation(AnimationGlobals.HurtName, new Animation(null,
                new Animation.Frame(new Rectangle(151, 44, 39, 33), 100d),
                new Animation.Frame(new Rectangle(199, 44, 39, 33), 100d),
                new Animation.Frame(new Rectangle(247, 44, 39, 33), 100d)));

            AnimManager.AddAnimation(AnimationGlobals.DeathName, new Animation(null, new Animation.Frame(new Rectangle(199, 44, 39, 33), 1000d)));
        }

        protected override BattleMenu GetMainBattleMenu()
        {
            return new PartnerBattleMenu(new BowSubMenu());
        }

        public override void OnTurnEnd()
        {
            base.OnTurnEnd();
            BattleUIManager.Instance.ClearMenuStack();
        }
    }
}
