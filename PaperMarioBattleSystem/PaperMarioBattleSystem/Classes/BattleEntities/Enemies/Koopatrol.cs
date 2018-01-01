﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// A Koopatrol. They're powerful armored Koopas.
    /// </summary>
    public class Koopatrol : KoopaTroopa, ITattleableEntity
    {
        private readonly StatusGlobals.PaybackHolder SpikedPayback = new StatusGlobals.PaybackHolder(StatusGlobals.PaybackTypes.Constant, Enumerations.PhysicalAttributes.Spiked,
                Enumerations.Elements.Sharp, new Enumerations.ContactTypes[] { Enumerations.ContactTypes.TopDirect },
                new Enumerations.ContactProperties[] { Enumerations.ContactProperties.None, Enumerations.ContactProperties.Protected },
                Enumerations.ContactResult.Failure, Enumerations.ContactResult.Failure, 1, null);

        public Koopatrol()
        {
            Name = "Koopatrol";

            //Using their TTYD stats
            BattleStats = new Stats(26, 6, 0, 4, 2);

            EntityProperties.AddPayback(SpikedPayback);
            EntityProperties.AddPhysAttribute(Enumerations.PhysicalAttributes.Spiked);

            EntityProperties.AddStatusProperty(Enumerations.StatusTypes.Sleep, new StatusPropertyHolder(70d, -1));
            EntityProperties.AddStatusProperty(Enumerations.StatusTypes.Dizzy, new StatusPropertyHolder(105d, 0));
            EntityProperties.AddStatusProperty(Enumerations.StatusTypes.Confused, new StatusPropertyHolder(75d, 0));
            EntityProperties.AddStatusProperty(Enumerations.StatusTypes.Tiny, new StatusPropertyHolder(90d, 0));
            EntityProperties.AddStatusProperty(Enumerations.StatusTypes.Stop, new StatusPropertyHolder(75d, -1));
            EntityProperties.AddStatusProperty(Enumerations.StatusTypes.DEFDown, new StatusPropertyHolder(95d, 0));
            EntityProperties.AddStatusProperty(Enumerations.StatusTypes.Burn, new StatusPropertyHolder(100d, 0));
            EntityProperties.AddStatusProperty(Enumerations.StatusTypes.Frozen, new StatusPropertyHolder(70d, 0));
            EntityProperties.AddStatusProperty(Enumerations.StatusTypes.Fright, new StatusPropertyHolder(70d, 0));
            EntityProperties.AddStatusProperty(Enumerations.StatusTypes.Blown, new StatusPropertyHolder(70d, 0));
            EntityProperties.AddStatusProperty(Enumerations.StatusTypes.KO, new StatusPropertyHolder(95d, 0));
            EntityProperties.AddStatusProperty(Enumerations.StatusTypes.Electrified, new StatusPropertyHolder(80d, 0));

            Texture2D spriteSheet = AssetManager.Instance.LoadRawTexture2D($"{ContentGlobals.SpriteRoot}/Enemies/Koopatrol.png");
            AnimManager.SetSpriteSheet(spriteSheet);

            AnimManager.AddAnimation(AnimationGlobals.IdleName, new LoopAnimation(spriteSheet, AnimationGlobals.InfiniteLoop,
                new Animation.Frame(new Rectangle(1, 388, 42, 59), 500d)));
            AnimManager.AddAnimation(AnimationGlobals.RunningName, new ReverseAnimation(spriteSheet, AnimationGlobals.InfiniteLoop,
                new Animation.Frame(new Rectangle(1, 388, 42, 59), 100d),
                new Animation.Frame(new Rectangle(49, 387, 43, 60), 100d, new Vector2(-1, -1)),
                new Animation.Frame(new Rectangle(98, 386, 45, 60), 100d, new Vector2(-2, -2))));
            AnimManager.AddAnimation(AnimationGlobals.HurtName, new Animation(spriteSheet,
                new Animation.Frame(new Rectangle(5, 325, 40, 50), 250d),
                new Animation.Frame(new Rectangle(56, 325, 38, 49), 250d)));
            AnimManager.AddAnimation(AnimationGlobals.DeathName, new Animation(spriteSheet,
                new Animation.Frame(new Rectangle(99, 327, 43, 48), 1000d)));

            AnimManager.AddAnimation(AnimationGlobals.ShelledBattleAnimations.EnterShellName, new Animation(spriteSheet,
                new Animation.Frame(new Rectangle(99, 262, 39, 49), 70d),
                new Animation.Frame(new Rectangle(152, 260, 35, 27), 70d),
                new Animation.Frame(new Rectangle(202, 261, 33, 25), 70d)));
            AnimManager.AddAnimation(AnimationGlobals.ShelledBattleAnimations.ShellSpinName, new LoopAnimation(spriteSheet, AnimationGlobals.InfiniteLoop,
                new Animation.Frame(new Rectangle(34, 449, 28, 30), 100d),
                new Animation.Frame(new Rectangle(66, 449, 28, 30), 100d),
                new Animation.Frame(new Rectangle(97, 449, 30, 30), 100d),
                new Animation.Frame(new Rectangle(130, 449, 28, 30), 100d),
                new Animation.Frame(new Rectangle(162, 449, 28, 30), 100d),
                new Animation.Frame(new Rectangle(1, 449, 30, 30), 100d)));
            AnimManager.AddAnimation(AnimationGlobals.ShelledBattleAnimations.ExitShellName, new Animation(spriteSheet,
                new Animation.Frame(new Rectangle(202, 261, 33, 25), 70d),
                new Animation.Frame(new Rectangle(152, 260, 35, 27), 70d),
                new Animation.Frame(new Rectangle(99, 262, 39, 49), 70d)));
            AnimManager.AddAnimation(AnimationGlobals.ShelledBattleAnimations.FlippedName, new LoopAnimation(spriteSheet, AnimationGlobals.InfiniteLoop,
                new Animation.Frame(new Rectangle(337, 57, 58, 35), 300d),
                new Animation.Frame(new Rectangle(337, 97, 58, 36), 300d)));
        }

        protected override MoveAction ActionUsed
        {
            get
            {
                //Check if any other Koopatrols are available
                //If there are less than a certain number, summon one if a random check succeeds
                bool shouldSummon = UtilityGlobals.TestRandomCondition(35d);
                if (shouldSummon == true)
                {
                    //Since the Koopatrol excludes itself, this number should be 1 less than the minimum number required for this action
                    const int minCountSummon = 2;

                    List<BattleEntity> koopatrolsList = new List<BattleEntity>(BattleManager.Instance.GetEntityAllies(this, HeightState));
                    Type koopatrolType = this.GetType();
                    for (int i = 0; i < koopatrolsList.Count; i++)
                    {
                        //Check exact types. Remove BattleEntities that aren't Koopatrols
                        Type enemyType = koopatrolsList[i].GetType();

                        if (koopatrolType != enemyType)
                        {
                            koopatrolsList.RemoveAt(i);
                            i--;
                        }
                    }

                    //Summon another Koopatrol
                    if (koopatrolsList.Count < minCountSummon)
                    {
                        return new SummonKoopatrolAction();
                    }
                }

                return base.ActionUsed;
            }
        }

        public override void HandleFlipped()
        {
            base.HandleFlipped();

            EntityProperties.RemovePayback(SpikedPayback);
            EntityProperties.RemovePhysAttribute(Enumerations.PhysicalAttributes.Spiked);
        }

        protected override void UnFlip()
        {
            base.UnFlip();

            EntityProperties.AddPayback(SpikedPayback);
            EntityProperties.AddPhysAttribute(Enumerations.PhysicalAttributes.Spiked);
        }

        #region Tattle Information

        public new string[] GetTattleLogEntry()
        {
            return new string[]
            {
                $"HP: {BattleStats.MaxHP} Attack: {BattleStats.BaseAttack}\nDefense: {BattleStats.BaseDefense}",
                $"Occasionally uses an attack\ncalled Charge that saves up",
                "energy, and can call in\nbackup for support if you",
                "don't defeat it quickly."
            };
        }

        public new string[] GetTattleDescription()
        {
            return new string[]
            {
                "That's a Koopatrol.\nA Koopa Troopa who protects\nhimself with spiked armor.",
                $"Max HP is {BattleStats.MaxHP}, Attack is {BattleStats.BaseAttack},\nand Defense is {BattleStats.BaseDefense}.",
                "It attacks with its shell and\nwith its head, then sometimes\ncharges up for a fierce move.",
                "Plus, if you take too long to\nwin, it'll call reinforcements.\nYeah, sorta gnarly, huh?",
                "It's one of the worst of\nBowser's guys. Koopa Troopas\ndream of being Koopatrols.",
                "...Hey, and by the way, what\ndo you think Bowser's doing\nnow, anyway? Eating?"
            };
        }

        #endregion
    }
}