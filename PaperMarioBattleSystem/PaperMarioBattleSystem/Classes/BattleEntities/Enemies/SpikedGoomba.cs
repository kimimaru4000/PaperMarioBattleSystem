using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PaperMarioBattleSystem
{
    public class SpikedGoomba : Goomba, ITattleableEntity
    {
        public SpikedGoomba()
        {
            Name = "Spiked Goomba";
            BattleStats = new Stats(1, 2, 0, 1, 0);

            EntityProperties.AddPhysAttribute(Enumerations.PhysicalAttributes.Spiked);
            EntityProperties.AddPayback(new StatusGlobals.PaybackHolder(StatusGlobals.PaybackTypes.Constant, Enumerations.PhysicalAttributes.Spiked,
                Enumerations.Elements.Sharp, new Enumerations.ContactTypes[] { Enumerations.ContactTypes.TopDirect },
                new Enumerations.ContactProperties[] { Enumerations.ContactProperties.None },
                Enumerations.ContactResult.Failure, Enumerations.ContactResult.Failure, 1, null));
        }

        public override void LoadAnimations()
        {
            base.LoadAnimations();

            Texture2D spriteSheet = AssetManager.Instance.LoadRawTexture2D($"{ContentGlobals.SpriteRoot}/Enemies/SpikedGoomba.png");
            AnimManager.SetSpriteSheet(spriteSheet);

            AnimManager.AddAnimation(AnimationGlobals.IdleName, new ReverseAnimation(spriteSheet, AnimationGlobals.InfiniteLoop,
                new Animation.Frame(new Rectangle(34, 153, 28, 39), 1000d),
                new Animation.Frame(new Rectangle(67, 152, 26, 40), 150d),
                new Animation.Frame(new Rectangle(67, 110, 26, 42), 1000d, new Vector2(0, -1))));
            AnimManager.AddAnimation(AnimationGlobals.HurtName, new Animation(spriteSheet,
                new Animation.Frame(new Rectangle(32, 68, 31, 36), 80d),
                new Animation.Frame(new Rectangle(128, 108, 31, 36), 80d)));
            AnimManager.AddAnimation(AnimationGlobals.DeathName, new Animation(spriteSheet, new Animation.Frame(new Rectangle(128, 108, 31, 36), 300d)));

            AnimManager.AddAnimation(AnimationGlobals.RunningName, new ReverseAnimation(spriteSheet, AnimationGlobals.InfiniteLoop,
                new Animation.Frame(new Rectangle(98, 153, 29, 39), 150d),
                new Animation.Frame(new Rectangle(34, 153, 28, 39), 100d),
                new Animation.Frame(new Rectangle(34, 110, 30, 42), 150d, new Vector2(0, -1))));
            AnimManager.AddAnimation(AnimationGlobals.JumpStartName, new Animation(spriteSheet,
                new Animation.Frame(new Rectangle(98, 113, 28, 39), 400d)));
        }

        #region Tattle Information

        public new string[] GetTattleLogEntry()
        {
            return new string[]
            {
                $"HP: {BattleStats.MaxHP} Attack: {BattleStats.BaseAttack}\nDefense: {BattleStats.BaseDefense}",
                $"A Goomba that wears a\nspiked helmet. Slightly",
                "higher attack power than\n a typical Goomba."
            };
        }

        public new string GetTattleDescription()
        {
            return "That's a Spiky Goomba.\n<wait value=\"250\">...A spiky-headed Goomba.\n<wait value=\"250\">What a creative name.\n<k><p>" +
                   "That spike is super-pointy, so\nit's better to hit it with a\nhammer than jump on it.\n<k><p>" +
                   "Maximum HP is 2, Attack is 2,\nand Defense is 0.\n<k><p>" +
                   "The addition of the spike\nmeans you'll hurt your feet\nif you jump on it. <wait value=\"100\">Duh!<k>";
        }

        #endregion
    }
}
