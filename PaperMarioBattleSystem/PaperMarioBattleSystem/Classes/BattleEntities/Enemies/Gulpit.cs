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

            ChangeHeightState(Enumerations.HeightStates.Grounded);

            //NOTE: Approximations based on the data we have from MarioWiki
            //"Good" is substantially more than 50%, "Fair" is approximately 50%, "Poor" is less than 50%
            EntityProperties.AddStatusProperty(Enumerations.StatusTypes.Sleep, new StatusPropertyHolder(90, 0));
            EntityProperties.AddStatusProperty(Enumerations.StatusTypes.Dizzy, new StatusPropertyHolder(100d, 0));
            EntityProperties.AddStatusProperty(Enumerations.StatusTypes.Tiny, new StatusPropertyHolder(95d, 0));
            EntityProperties.AddStatusProperty(Enumerations.StatusTypes.Immobilized, new StatusPropertyHolder(85d, 0));
            EntityProperties.AddStatusProperty(Enumerations.StatusTypes.Paralyzed, new StatusPropertyHolder(95, 0));
            EntityProperties.AddStatusProperty(Enumerations.StatusTypes.DEFDown, new StatusPropertyHolder(100d, 0));
            EntityProperties.AddStatusProperty(Enumerations.StatusTypes.Fright, new StatusPropertyHolder(80d, 0));
            EntityProperties.AddStatusProperty(Enumerations.StatusTypes.Blown, new StatusPropertyHolder(50, 0));
            EntityProperties.AddStatusProperty(Enumerations.StatusTypes.Lifted, new StatusPropertyHolder(55, 0));

            Texture2D spriteSheet = AssetManager.Instance.LoadAsset<Texture2D>($"{ContentGlobals.SpriteRoot}/Enemies/Gulpit");
            AnimManager.SetSpriteSheet(spriteSheet);

            AnimManager.AddAnimation(AnimationGlobals.IdleName, new Animation(spriteSheet,
                new Animation.Frame(new Rectangle(376, 2, 64, 69), 1500d)));
            AnimManager.AddAnimation(AnimationGlobals.RunningName, new ReverseAnimation(spriteSheet, AnimationGlobals.InfiniteLoop,
                new Animation.Frame(new Rectangle(372, 225, 69, 68), 250d),
                new Animation.Frame(new Rectangle(376, 2, 64, 69), 250d),
                new Animation.Frame(new Rectangle(485, 155, 66, 68), 250d)));
            AnimManager.AddAnimation(AnimationGlobals.HurtName, new Animation(spriteSheet,
                new Animation.Frame(new Rectangle(146, 164, 71, 59), 200d),
                new Animation.Frame(new Rectangle(480, 7, 70, 64), 200d)));
            AnimManager.AddAnimation(AnimationGlobals.DeathName, new Animation(spriteSheet,
                new Animation.Frame(new Rectangle(480, 7, 70, 64), 1000d)));
        }

        public override void OnTurnStart()
        {
            base.OnTurnStart();

            //If any IUsableEntities are found in the Neutral BattleEntity list, perform Rock Spit with it
            //Otherwise, perform Lick
            List<IUsableEntity> usableEntities = new List<IUsableEntity>();

            int chosenIndex = -1;

            //Get all Neutral entities
            BattleEntity[] neutralEntities = BattleManager.Instance.GetEntities(Enumerations.EntityTypes.Neutral, Enumerations.HeightStates.Grounded);
            for (int i = 0; i < neutralEntities.Length; i++)
            {
                IUsableEntity usableEntity = neutralEntities[i] as IUsableEntity;
                //Add to the list if this entity is usable
                if (usableEntity != null)
                {
                    usableEntities.Add(usableEntity);
                }
            }

            //Choose a random entity out of the ones we found
            //if (usableEntities.Count > 0)
            //{
            //    chosenIndex = GeneralGlobals.Randomizer.Next(0, usableEntities.Count);
            //}

            //If we found and chose a usable entity, use Rock Spit with the entity
            //Otherwise, use Lick
            MoveAction action = null;
            if (chosenIndex >= 0)
                action = new Hammer();//new RockSpitAction(usableEntities[chosenIndex]);
            else action = new LickAction();

            StartAction(action, false, BattleManager.Instance.GetFrontPlayer());
        }

        #region Tattle Information

        public string[] GetTattleLogEntry()
        {
            //Log entries don't exist in the original PM and Gulpits are PM-only enemies, so they don't have one
            return new string[] { "N/A" };
        }

        public string[] GetTattleDescription()
        {
            return new string[]
            {
                "This is a Gulpit.\nWhoa! Gulpits look pretty\nburly, don't they?",
                "Max HP: 12, Attack Power: 2,\nDefense Power: 0",
                "Gulpits attack by picking rocks\nup in their mouths and then\nspitting 'em back out.",
                "The bigger the rock they spit,\nthe more damage you'll take.",
                "Big rocks take 7 HP in damage,\nand small ones take 5 HP.",
                "They can't do much damage\nwithout ammo, so you should\nconcentrate on the rocks first."
            };
        }

        #endregion
    }
}
