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
    /// A Pokey enemy. It has 3 segments.
    /// </summary>
    public class Pokey : BattleEnemy, ITattleableEntity
    {
        public Pokey() : base(new Stats(11, 4, 0, 2, 0))
        {
            Name = "Pokey";

            #region Entity Property Setup

            EntityProperties.AddStatusProperty(Enumerations.StatusTypes.Sleep, new StatusPropertyHolder(95, 0));
            EntityProperties.AddStatusProperty(Enumerations.StatusTypes.Dizzy, new StatusPropertyHolder(80, 0));
            EntityProperties.AddStatusProperty(Enumerations.StatusTypes.Confused, new StatusPropertyHolder(90, 0));
            EntityProperties.AddStatusProperty(Enumerations.StatusTypes.Tiny, new StatusPropertyHolder(90, 0));
            EntityProperties.AddStatusProperty(Enumerations.StatusTypes.Stop, new StatusPropertyHolder(80, 0));
            EntityProperties.AddStatusProperty(Enumerations.StatusTypes.DEFDown, new StatusPropertyHolder(95, 0));
            EntityProperties.AddStatusProperty(Enumerations.StatusTypes.Burn, new StatusPropertyHolder(100, 0));
            EntityProperties.AddStatusProperty(Enumerations.StatusTypes.Frozen, new StatusPropertyHolder(60, 0));
            EntityProperties.AddStatusProperty(Enumerations.StatusTypes.Fright, new StatusPropertyHolder(100, 0));
            EntityProperties.AddStatusProperty(Enumerations.StatusTypes.Blown, new StatusPropertyHolder(90, 0));
            EntityProperties.AddStatusProperty(Enumerations.StatusTypes.KO, new StatusPropertyHolder(100, 0));

            EntityProperties.AddPhysAttribute(Enumerations.PhysicalAttributes.Spiked);
            EntityProperties.AddPayback(new StatusGlobals.PaybackHolder(StatusGlobals.PaybackTypes.Constant, Enumerations.PhysicalAttributes.Spiked,
                Enumerations.Elements.Sharp, new Enumerations.ContactTypes[] { Enumerations.ContactTypes.Latch, Enumerations.ContactTypes.TopDirect, Enumerations.ContactTypes.SideDirect },
                new Enumerations.ContactProperties[] { Enumerations.ContactProperties.None },
                Enumerations.ContactResult.Failure, Enumerations.ContactResult.Failure, 1, null));

            EntityProperties.SetVulnerableDamageEffects(Enumerations.DamageEffects.RemovesSegment);

            #endregion

            Texture2D spriteSheet = AssetManager.Instance.LoadRawTexture2D($"{ContentGlobals.SpriteRoot}/Enemies/Pokey.png");
            AnimManager.SetSpriteSheet(spriteSheet);
        }

        #region Tattle Info

        public bool CanBeTattled { get; set; } = true;

        public string[] GetTattleLogEntry()
        {
            return new string[]
            {
                $"HP: {BattleStats.MaxHP} Attack: {BattleStats.BaseAttack}\nDefense: {BattleStats.BaseDefense}",
                $"A cactus ghoul covered from\nhead to base in nasty spines.",
                "It attacks by lobbing sections\nof itself at you, and can even",
                "call other Pokeys to come\nfight alongside it."
            };
        }

        public string[] GetTattleDescription()
        {
            return new string[]
            {
                "That's a Pokey. It's a cactus ghoul that's got nasty spines all over its body.",
                $"Max HP is {BattleStats.MaxHP}, Attack is {BattleStats.BaseAttack}, and Defense is {BattleStats.BaseDefense}.",
                "Look at those spines... Those would TOTALLY hurt. If you stomp on it, you'll regret it.",
                "Pokeys attack by lobbing parts of their bodies and by charging at you...",
                "They can even call friends in for help, so be quick about taking them out."
            };
        }

        #endregion
    }
}
