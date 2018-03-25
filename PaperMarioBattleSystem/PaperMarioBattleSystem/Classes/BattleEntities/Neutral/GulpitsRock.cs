using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static PaperMarioBattleSystem.Enumerations;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// A Gulpit's Rock. It is a neutral entity that is used by Gulpit in its Rock Spit move.
    /// </summary>
    public sealed class GulpitsRock : BattleEntity, ITattleableEntity, IUsableEntity
    {
        /// <summary>
        /// The amount of additional damage a small Gulpit's Rock adds to a Gulpit's Rock Spit move.
        /// </summary>
        private const int SmallRockAdditionalDamage = 3;

        /// <summary>
        /// The amount of additional damage a big Gulpit's Rock adds to a Gulpit's Rock Spit move.
        /// </summary>
        private const int BigRockAdditionalDamage = 5;

        /// <summary>
        /// The amount of damage the Gulpit's Rock adds to a Gulpit's Rock Spit move.
        /// </summary>
        public int UsableValue { get; private set; } = 3;

        /// <summary>
        /// Whether the Gulpit's Rock is a big or small one.
        /// </summary>
        public bool BigRock = false;

        public GulpitsRock(bool bigRock) : base(new Stats(1, 1, 0, 0, 0))
        {
            Name = "Gulpits' Rock";

            EntityType = Enumerations.EntityTypes.Neutral;

            SetStatusProperties();

            //Gulpits' Rocks do not move at all, so their base turns are less than 0
            BaseTurns = -99;

            BigRock = bigRock;

            //If the rock is big, make it deal more damage than the small one
            UsableValue = (BigRock == true) ? BigRockAdditionalDamage : SmallRockAdditionalDamage;

            Texture2D spriteSheet = AssetManager.Instance.LoadRawTexture2D($"{ContentGlobals.SpriteRoot}/Enemies/Gulpit.png");
            AnimManager.SetSpriteSheet(spriteSheet);

            //The animations differ between the different sizes of rocks
            //They have only idle and death animations
            if (BigRock == false)
            {
                AnimManager.AddAnimation(AnimationGlobals.IdleName, new Animation(spriteSheet, new Animation.Frame(new Rectangle(59, 378, 26, 21), 1500d)));
                AnimManager.AddAnimation(AnimationGlobals.DeathName, new Animation(spriteSheet,
                    new Animation.Frame(new Rectangle(1, 370, 45, 37), 80d),
                    new Animation.Frame(new Rectangle(340, 299, 42, 32), 80d),
                    new Animation.Frame(new Rectangle(392, 299, 32, 30), 80d)));
            }
            else
            {
                AnimManager.AddAnimation(AnimationGlobals.IdleName, new Animation(spriteSheet, new Animation.Frame(new Rectangle(534, 301, 34, 29), 1500d)));
                AnimManager.AddAnimation(AnimationGlobals.DeathName, new Animation(spriteSheet,
                    new Animation.Frame(new Rectangle(483, 298, 44, 37), 80d),
                    new Animation.Frame(new Rectangle(437, 300, 40, 35), 80d),
                    new Animation.Frame(new Rectangle(1, 370, 45, 37), 80d),
                    new Animation.Frame(new Rectangle(340, 299, 42, 32), 80d),
                    new Animation.Frame(new Rectangle(392, 299, 32, 30), 80d)));
            }
        }

        public override void OnBattleStart()
        {
            base.OnBattleStart();

            Vector2 startPos = new Vector2(-20, 85);

            //Small rocks are slightly in front
            if (BigRock == false)
                startPos.Y += 15f;

            //Set battle position
            Vector2 battlepos = startPos + new Vector2((BattleManager.Instance.PositionXDiff / 2) * BattleIndex, 0);

            SetBattlePosition(battlepos);
            Position = BattlePosition;
        }

        public override void OnTurnStart()
        {
            //In the event a Gulpit's Rock does start its turn, simply do nothing
            base.OnTurnStart();

            StartAction(new NoAction(), false, null);
        }

        public override Item GetItemOfType(Item.ItemTypes itemTypes)
        {
            return null;
        }

        private void SetStatusProperties()
        {
            //Gulpits' Rocks are immune to all Status Effects
            StatusTypes[] statustypes = UtilityGlobals.GetEnumValues<StatusTypes>();
            for (int i = 0; i < statustypes.Length; i++)
            {
                EntityProperties.AddStatusProperty(statustypes[i], new StatusPropertyHolder(0d, 0, 1));
            }
        }

        #region Tattle Information

        public bool CanBeTattled { get; set; } = true;

        public string[] GetTattleLogEntry()
        {
            return new string[] { "N/A" };
        }

        public string[] GetTattleDescription()
        {
            //The tattle is the same whether it's a small or a big rock
            return new string[]
            {
                "These are Gulpits' Rocks. Gulpits\ngulp them and then spit 'em at\npeople.\nThere are two sizes of rocks.",
                "The big rocks do more damage\nthan the small rocks do.\n...Big surprise, huh?"
            };
        }

        #endregion
    }
}
