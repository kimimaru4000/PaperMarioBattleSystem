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

            Texture2D spriteSheet = AssetManager.Instance.LoadRawTexture2D($"{ContentGlobals.SpriteRoot}/Enemies/SpikedGoomba.png");
            AnimManager.SetSpriteSheet(spriteSheet);

            AnimManager.AddAnimation(AnimationGlobals.IdleName, new ReverseAnimation(spriteSheet, AnimationGlobals.InfiniteLoop,
                new Animation.Frame(new Rectangle(34, 153, 28, 39), 1000d),
                new Animation.Frame(new Rectangle(67, 152, 26, 40), 150d, new Vector2(0, -1)),
                new Animation.Frame(new Rectangle(67, 110, 26, 42), 1000d, new Vector2(0, -2))));
            AnimManager.AddAnimation(AnimationGlobals.HurtName, new Animation(spriteSheet,
                new Animation.Frame(new Rectangle(32, 68, 31, 36), 500d),
                new Animation.Frame(new Rectangle(128, 108, 31, 36), 500d)));
            AnimManager.AddAnimation(AnimationGlobals.DeathName, new Animation(spriteSheet, new Animation.Frame(new Rectangle(128, 108, 31, 36), 1000d)));

            AnimManager.AddAnimation(AnimationGlobals.RunningName, new ReverseAnimation(spriteSheet, AnimationGlobals.InfiniteLoop,
                new Animation.Frame(new Rectangle(98, 153, 29, 39), 150d),
                new Animation.Frame(new Rectangle(34, 153, 28, 39), 100d),
                new Animation.Frame(new Rectangle(34, 110, 30, 42), 150d)));
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

        public new string[] GetTattleDescription()
        {
            return new string[]
            {
                "That's a Spiky Goomba. ...A spiky-headed Goomba. What a creative name.",
                "That spike is super-pointy, so it's better to hit it with a hammer than jump on it.",
                $"Maximum HP is {BattleStats.MaxHP}, Attack is {BattleStats.BaseAttack}, and Defense is {BattleStats.BaseDefense}.",
                "The addition of the spike means you'll hurt your feet if you jump on it. Duh!"
            };
        }

        #endregion
    }
}
