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
    /// A Shy Guy.
    /// </summary>
    public class ShyGuy : BattleEnemy, ITattleableEntity
    {
        public ShyGuy() : base(new Stats(14, 7, 0, 0, 0))
        {
            Name = "Shy Guy";

            EntityProperties.AddStatusProperty(Enumerations.StatusTypes.Sleep, new StatusPropertyHolder(70d, 0));
            EntityProperties.AddStatusProperty(Enumerations.StatusTypes.Stop, new StatusPropertyHolder(90d, 0));
            EntityProperties.AddStatusProperty(Enumerations.StatusTypes.Dizzy, new StatusPropertyHolder(90d, 0));
            EntityProperties.AddStatusProperty(Enumerations.StatusTypes.Fright, new StatusPropertyHolder(80d, 0));
            EntityProperties.AddStatusProperty(Enumerations.StatusTypes.Paralyzed, new StatusPropertyHolder(90d, 1));
            EntityProperties.AddStatusProperty(Enumerations.StatusTypes.Tiny, new StatusPropertyHolder(75d, 0));
            EntityProperties.AddStatusProperty(Enumerations.StatusTypes.Lifted, new StatusPropertyHolder(85d, 0));
            EntityProperties.AddStatusProperty(Enumerations.StatusTypes.Blown, new StatusPropertyHolder(80d, 0));

            LoadAnimations();
        }

        public override void LoadAnimations()
        {
            Texture2D spriteSheet = AssetManager.Instance.LoadRawTexture2D($"{ContentGlobals.SpriteRoot}/Enemies/ShyGuy.png");
            AnimManager.SetSpriteSheet(spriteSheet);

            AnimManager.AddAnimation(AnimationGlobals.IdleName, new LoopAnimation(null, AnimationGlobals.InfiniteLoop,
                new Animation.Frame(new Rectangle(3, 7, 27, 32), 150d),
                new Animation.Frame(new Rectangle(35, 6, 26, 33), 150d, new Vector2(0, -1))));
        }

        #region Tattle Data

        public bool CanBeTattled { get; set; } = true;

        public string[] GetTattleLogEntry()
        {
            return null;
        }

        public string GetTattleDescription()
        {
            return "This is a Shy Guy.\nThey're ne'er-do-wells.\nBig time rascals.\nNo good...\n<k><p>" +
                   "Max HP: 7, Attack Power: 2,\nDefense Power: 0\n<k><p>" +
                   "They occasionally do acrobatic\nattacks that have an attack\npower of 3.\n<k><p>" +
                   "Who knows what lies in the\nhearts of these troublemakers?\n<wait value=\"250\">I guess they're dangerous, but\nthey're pretty small time.<k>";
        }

        #endregion
    }
}
