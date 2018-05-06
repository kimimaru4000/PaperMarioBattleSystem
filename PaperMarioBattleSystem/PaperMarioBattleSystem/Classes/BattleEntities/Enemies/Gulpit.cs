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
    /// A Gulpit. It uses Gulpits' Rocks as an attack.
    /// </summary>
    public sealed class Gulpit : BattleEnemy, ITattleableEntity
    {
        public Gulpit() : base(new Stats(22, 12, 0, 2, 0))
        {
            Name = "Gulpit";

            AIBehavior = new GulpitAI(this);

            ChangeHeightState(Enumerations.HeightStates.Grounded);

            //These values are ripped from the battle scripts for Gulpit
            EntityProperties.AddStatusProperty(Enumerations.StatusTypes.Sleep, new StatusPropertyHolder(60d, 0));
            EntityProperties.AddStatusProperty(Enumerations.StatusTypes.Poison, new StatusPropertyHolder(80d, 0));
            EntityProperties.AddStatusProperty(Enumerations.StatusTypes.Dizzy, new StatusPropertyHolder(90d, 0));
            EntityProperties.AddStatusProperty(Enumerations.StatusTypes.Tiny, new StatusPropertyHolder(75d, 0));
            EntityProperties.AddStatusProperty(Enumerations.StatusTypes.Stop, new StatusPropertyHolder(85d, 0));
            EntityProperties.AddStatusProperty(Enumerations.StatusTypes.Paralyzed, new StatusPropertyHolder(80, 1));
            EntityProperties.AddStatusProperty(Enumerations.StatusTypes.DEFDown, new StatusPropertyHolder(100d, 0));
            EntityProperties.AddStatusProperty(Enumerations.StatusTypes.Fright, new StatusPropertyHolder(0d, 0));
            EntityProperties.AddStatusProperty(Enumerations.StatusTypes.Blown, new StatusPropertyHolder(50, 0));
            EntityProperties.AddStatusProperty(Enumerations.StatusTypes.Lifted, new StatusPropertyHolder(50, 0));
            EntityProperties.AddStatusProperty(Enumerations.StatusTypes.Frozen, new StatusPropertyHolder(0d, 0));
            EntityProperties.AddStatusProperty(Enumerations.StatusTypes.Electrified, new StatusPropertyHolder(10d, 0));

            LoadAnimations();
        }

        public override void LoadAnimations()
        {
            Texture2D spriteSheet = AssetManager.Instance.LoadRawTexture2D($"{ContentGlobals.SpriteRoot}/Enemies/Gulpit.png");
            AnimManager.SetSpriteSheet(spriteSheet);

            AnimManager.AddAnimation(AnimationGlobals.IdleName, new LoopAnimation(spriteSheet, AnimationGlobals.InfiniteLoop,
                new Animation.Frame(new Rectangle(152, 225, 64, 70), 600d),
                new Animation.Frame(new Rectangle(376, 2, 64, 69), 600d)));
            AnimManager.AddAnimation(AnimationGlobals.RunningName, new ReverseAnimation(spriteSheet, AnimationGlobals.InfiniteLoop,
                new Animation.Frame(new Rectangle(372, 225, 69, 68), 250d),
                new Animation.Frame(new Rectangle(376, 2, 64, 69), 250d),
                new Animation.Frame(new Rectangle(485, 155, 66, 68), 250d)));
            AnimManager.AddAnimation(AnimationGlobals.HurtName, new Animation(spriteSheet,
                new Animation.Frame(new Rectangle(146, 164, 71, 59), 200d),
                new Animation.Frame(new Rectangle(480, 7, 70, 64), 200d)));
            AnimManager.AddAnimation(AnimationGlobals.DeathName, new Animation(spriteSheet,
                new Animation.Frame(new Rectangle(480, 7, 70, 64), 1000d)));

            AnimManager.AddAnimation(AnimationGlobals.GulpitBattleAnimations.LickName, new Animation(spriteSheet,
                new Animation.Frame(new Rectangle(370, 155, 70, 68), 300d),
                new Animation.Frame(new Rectangle(247, 229, 84, 66), 300d),
                new Animation.Frame(new Rectangle(449, 226, 105, 69), 300d),
                new Animation.Frame(new Rectangle(37, 1, 68, 78), 300d)));
            AnimManager.AddAnimation(AnimationGlobals.GulpitBattleAnimations.SpitRockName, new Animation(spriteSheet,
                new Animation.Frame(new Rectangle(246, 305, 83, 62), 200d, new Vector2(0, 4)),
                new Animation.Frame(new Rectangle(8, 298, 96, 69), 200d, new Vector2(-7, 0)),
                new Animation.Frame(new Rectangle(151, 297, 65, 70), 200d, new Vector2(8, 0)),
                new Animation.Frame(new Rectangle(41, 224, 63, 71), 1000d, new Vector2(9, -1)),
                new Animation.Frame(new Rectangle(148, 4, 69, 67), 300d, new Vector2(7, 1))));
        }

        #region Tattle Information

        public bool CanBeTattled { get; set; } = true;

        public string[] GetTattleLogEntry()
        {
            //Log entries don't exist in the original PM and Gulpits are PM-only enemies, so they don't have one
            return new string[] { "N/A" };
        }

        public string GetTattleDescription()
        {
            return "This is a Gulpit.\nWhoa! Gulpits look pretty\nburly, don't they?\n<k><p>" +
                   "Max HP: 12, Attack Power: 2,\nDefense Power: 0\n<k><p>" +
                   "Gulpits attack by picking rocks\nup in their mouths and then\nspitting 'em back out.\n<k><p>" +
                   "The bigger the rock they spit,\nthe more damage you'll take.\n<k><p>" +
                   "Big rocks take 7 HP in damage,\nand small ones take 5 HP.\n<k><p>" +
                   "They can't do much damage\nwithout ammo, so you should\nconcentrate on the rocks first.<k>";
        }

        #endregion
    }
}
