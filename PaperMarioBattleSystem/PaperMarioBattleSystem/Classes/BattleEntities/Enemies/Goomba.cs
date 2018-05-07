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
    /// The standard Goomba enemy
    /// </summary>
    public class Goomba : BattleEnemy, ITattleableEntity
    {   
        public Goomba() : base(new Stats(1, 2, 0, 0, 0))
        {
            Name = "Goomba";

            AIBehavior = new GoombaAI(this);

            EntityProperties.AddStatusProperty(Enumerations.StatusTypes.Sleep, new StatusPropertyHolder(100, 0));
            EntityProperties.AddStatusProperty(Enumerations.StatusTypes.Stop, new StatusPropertyHolder(110, 0));
            EntityProperties.AddStatusProperty(Enumerations.StatusTypes.Dizzy, new StatusPropertyHolder(100, 0));
            EntityProperties.AddStatusProperty(Enumerations.StatusTypes.DEFDown, new StatusPropertyHolder(100, 0));
            EntityProperties.AddStatusProperty(Enumerations.StatusTypes.Poison, new StatusPropertyHolder(100, 0));
            EntityProperties.AddStatusProperty(Enumerations.StatusTypes.Confused, new StatusPropertyHolder(100, 0));

            LoadAnimations();
        }

        public override void LoadAnimations()
        {
            Texture2D spriteSheet = AssetManager.Instance.LoadRawTexture2D($"{ContentGlobals.SpriteRoot}/Enemies/Goomba.png");
            AnimManager.SetSpriteSheet(spriteSheet);

            AnimManager.AddAnimation(AnimationGlobals.IdleName, new ReverseAnimation(spriteSheet, AnimationGlobals.InfiniteLoop,
                new Animation.Frame(new Rectangle(67, 107, 26, 28), 1000d),
                new Animation.Frame(new Rectangle(35, 104, 26, 31), 150d, new Vector2(0, -3)),
                new Animation.Frame(new Rectangle(3, 5, 26, 34), 1000d, new Vector2(0, -6))));
            AnimManager.AddAnimation(AnimationGlobals.HurtName, new Animation(spriteSheet,
                new Animation.Frame(new Rectangle(65, 76, 29, 27), 80d),
                new Animation.Frame(new Rectangle(2, 109, 27, 26), 80d)));
            AnimManager.AddAnimation(AnimationGlobals.DeathName, new Animation(spriteSheet, new Animation.Frame(new Rectangle(2, 109, 27, 26), 300d)));

            AnimManager.AddAnimation(AnimationGlobals.RunningName, new ReverseAnimation(spriteSheet, AnimationGlobals.InfiniteLoop,
                new Animation.Frame(new Rectangle(129, 73, 28, 30), 150d, new Vector2(0, -2)),
                new Animation.Frame(new Rectangle(67, 107, 26, 28), 100d),
                new Animation.Frame(new Rectangle(99, 75, 28, 28), 150d)));
            AnimManager.AddAnimation(AnimationGlobals.JumpStartName, new Animation(spriteSheet,
                new Animation.Frame(new Rectangle(99, 107, 26, 28), 400d)));
        }

        public override void TurnUpdate()
        {
            base.TurnUpdate();
        }

        #region Tattle Info

        public bool CanBeTattled { get; set; } = true;

        public string[] GetTattleLogEntry()
        {
            return new string[]
            {
                $"HP: {BattleStats.MaxHP} Attack: {BattleStats.BaseAttack}\nDefense: {BattleStats.BaseDefense}",
                $"The underling of underlings.\nNo other distinguishing",
                "characteristics."
            };
        }

        public string GetTattleDescription()
        {
            return "That's a Goomba.\n<wait value=\"250\">Umm... <wait value=\"100\">Yeah, I'm one of those,\nin case you hadn't noticed.\n<k><p>" +
                   "Ahem... <wait value=\"100\">It says here: \"Goombas\nare underlings of underlings.\"\n<wait value=\"300\">...That is totally rude!\n<k><p>" +
                   "Their maximum HP is 2. <wait value=\"100\">They\nhave an Attack power of 1\nand a Defense of 0.<k>";
        }

        #endregion
    }
}
