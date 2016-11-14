using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PaperMarioBattleSystem
{
    public sealed class Kooper : BattlePartner
    {
        public Kooper() : base(new Stats(1, 50, 0, 0, 1))
        {
            Name = "Kooper";
            PartnerType = Enumerations.PartnerTypes.Kooper;
            
            Texture2D spriteSheet = AssetManager.Instance.LoadAsset<Texture2D>($"{ContentGlobals.SpriteRoot}/Characters/Kooper");
            AddAnimation(AnimationGlobals.IdleName, new Animation(spriteSheet, new Animation.Frame(new Rectangle(97, 117, 30, 50), 1000d)));

            AddAnimation(AnimationGlobals.RunningName, new LoopAnimation(spriteSheet, AnimationGlobals.InfiniteLoop,
                new Animation.Frame(new Rectangle(97, 117, 30, 50), 100d),
                new Animation.Frame(new Rectangle(89, 3, 34, 48), 100d)));

            AddAnimation(AnimationGlobals.KooperBattleAnimations.ShellSpinName, new LoopAnimation(spriteSheet, AnimationGlobals.InfiniteLoop,
                new Animation.Frame(new Rectangle(162, 222, 28, 25), 250d),
                new Animation.Frame(new Rectangle(194, 222, 28, 25), 250d),
                new Animation.Frame(new Rectangle(225, 222, 30, 25), 250d),
                new Animation.Frame(new Rectangle(258, 222, 28, 25), 250d)));
        }

        public override void OnTurnStart()
        {
            base.OnTurnStart();
            BattleUIManager.Instance.PushMenu(new PartnerBattleMenu(new KooperSubMenu()));
        }

        public override void OnTurnEnd()
        {
            base.OnTurnEnd();
            BattleUIManager.Instance.ClearMenuStack();
        }
    }
}
