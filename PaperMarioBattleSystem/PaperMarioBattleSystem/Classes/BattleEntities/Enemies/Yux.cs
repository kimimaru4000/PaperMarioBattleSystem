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
    /// A Yux. They create 1 Mini-Yux per turn up to a max of 2; Mini-Yuxes put a shield around them that makes them invincible.
    /// </summary>
    public class Yux : BattleEnemy, ITattleableEntity
    {
        public Yux() : base(new Stats(12, 3, 0, 2, 0))
        {
            Name = "Yux";

            ChangeHeightState(Enumerations.HeightStates.Airborne);

            EntityProperties.AddStatusProperty(Enumerations.StatusTypes.Sleep, new StatusPropertyHolder(30, 0));
            EntityProperties.AddStatusProperty(Enumerations.StatusTypes.Dizzy, new StatusPropertyHolder(100, 0));
            EntityProperties.AddStatusProperty(Enumerations.StatusTypes.Confused, new StatusPropertyHolder(70, 0));
            EntityProperties.AddStatusProperty(Enumerations.StatusTypes.Tiny, new StatusPropertyHolder(90, 0));
            EntityProperties.AddStatusProperty(Enumerations.StatusTypes.Immobilized, new StatusPropertyHolder(80, 0));
            EntityProperties.AddStatusProperty(Enumerations.StatusTypes.DEFDown, new StatusPropertyHolder(95, 0));
            EntityProperties.AddStatusProperty(Enumerations.StatusTypes.Burn, new StatusPropertyHolder(0, 0));
            EntityProperties.AddStatusProperty(Enumerations.StatusTypes.Frozen, new StatusPropertyHolder(0, 0));
            EntityProperties.AddStatusProperty(Enumerations.StatusTypes.Fright, new StatusPropertyHolder(0, 0));
            EntityProperties.AddStatusProperty(Enumerations.StatusTypes.Blown, new StatusPropertyHolder(90, 0));
            EntityProperties.AddStatusProperty(Enumerations.StatusTypes.KO, new StatusPropertyHolder(95, 0));

            Texture2D spriteSheet = AssetManager.Instance.LoadAsset<Texture2D>($"{ContentGlobals.SpriteRoot}/Enemies/Koopatrol");
            AnimManager.SetSpriteSheet(spriteSheet);
        }

        #region Tattle Information

        public string[] GetTattleLogEntry()
        {
            return new string[]
            {
                "These pathetically ugly creatures were created in X-Naut laboratories." +
                "With Mini-Yux around them, they're impervious to all attacks."
            };
        }

        public string[] GetTattleDescription()
        {
            return new string[]
            {
                "That's a Yux. Says here it's a creature created in the X-Naut labs." +
                "Max HP is 3, Attack is 2, and Defense is 0. According to this, attacks and" +
                "items won't affect it if it has Mini-Yux around it. So, if any Mini-Yux appear, take those out first. Duh!"
            };
        }

        #endregion
    }
}
