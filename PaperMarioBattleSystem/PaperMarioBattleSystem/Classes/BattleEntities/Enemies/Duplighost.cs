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
    /// A Duplighost. It can disguise as Mario's active Partner and use its moves.
    /// </summary>
    public class Duplighost : BattleEnemy, ITattleableEntity
    {
        /// <summary>
        /// The Partner the Duplighost is disguised as.
        /// </summary>
        public Enumerations.PartnerTypes PartnerTypeDisguise = Enumerations.PartnerTypes.None;

        /// <summary>
        /// The flippable behavior for the Duplighost for when it's disguised as Kooper.
        /// </summary>
        public IFlippableBehavior FlippableBehavior = null;

        public Duplighost() : base(new Stats(23, 15, 0, 0, 0))
        {
            Name = "Duplighost";
            
            EntityProperties.AddStatusProperty(Enumerations.StatusTypes.Sleep, new StatusPropertyHolder(60, -1));
            EntityProperties.AddStatusProperty(Enumerations.StatusTypes.Stop, new StatusPropertyHolder(80, -1));
            EntityProperties.AddStatusProperty(Enumerations.StatusTypes.Dizzy, new StatusPropertyHolder(75, -1));
            EntityProperties.AddStatusProperty(Enumerations.StatusTypes.DEFDown, new StatusPropertyHolder(100, 0));
            EntityProperties.AddStatusProperty(Enumerations.StatusTypes.Poison, new StatusPropertyHolder(100, 0));
            EntityProperties.AddStatusProperty(Enumerations.StatusTypes.Confused, new StatusPropertyHolder(100, 0));
            EntityProperties.AddStatusProperty(Enumerations.StatusTypes.Fright, new StatusPropertyHolder(0, 0));
            EntityProperties.AddStatusProperty(Enumerations.StatusTypes.Paralyzed, new StatusPropertyHolder(75, 1));
            EntityProperties.AddStatusProperty(Enumerations.StatusTypes.Lifted, new StatusPropertyHolder(0, 0));
            EntityProperties.AddStatusProperty(Enumerations.StatusTypes.Tiny, new StatusPropertyHolder(75, 0));
            EntityProperties.AddStatusProperty(Enumerations.StatusTypes.Blown, new StatusPropertyHolder(0, 0));

            Texture2D spriteSheet = AssetManager.Instance.LoadRawTexture2D($"{ContentGlobals.SpriteRoot}/Enemies/Duplighost.png");
            AnimManager.SetSpriteSheet(spriteSheet);

            AnimManager.AddAnimation(AnimationGlobals.IdleName, new LoopAnimation(null, AnimationGlobals.InfiniteLoop,
                new Animation.Frame(new Rectangle(4, 4, 51, 43), 110d),
                new Animation.Frame(new Rectangle(196, 101, 51, 42), 110d, new Vector2(0, 1)),
                new Animation.Frame(new Rectangle(260, 98, 50, 45), 110d, new Vector2(0, -2)),
                new Animation.Frame(new Rectangle(324, 97, 50, 46), 110d, new Vector2(0, -3))));
            AnimManager.AddAnimation(AnimationGlobals.HurtName, new Animation(null,
                new Animation.Frame(new Rectangle(1, 151, 50, 40), 110d),
                new Animation.Frame(new Rectangle(65, 151, 51, 39), 110d, new Vector2(0, 1))));
            AnimManager.AddAnimation(AnimationGlobals.DeathName, new Animation(null,
                new Animation.Frame(new Rectangle(1, 151, 50, 40), 1000d)));
            AnimManager.AddAnimation(AnimationGlobals.RunningName, new ReverseAnimation(null, AnimationGlobals.InfiniteLoop,
                new Animation.Frame(new Rectangle(4, 98, 52, 45), 60d),
                new Animation.Frame(new Rectangle(68, 98, 51, 45), 60d),
                new Animation.Frame(new Rectangle(132, 100, 50, 43), 60d, new Vector2(0, 1))));

            AnimManager.AddAnimation(AnimationGlobals.DuplighostBattleAnimations.HeadbuttStartName, new Animation(null,
                new Animation.Frame(new Rectangle(195, 149, 51, 42), 1000d)));
            AnimManager.AddAnimation(AnimationGlobals.DuplighostBattleAnimations.HeadbuttName, new LoopAnimation(null, AnimationGlobals.InfiniteLoop,
                new Animation.Frame(new Rectangle(5, 198, 47, 37), 110d),
                new Animation.Frame(new Rectangle(69, 197, 47, 39), 110d, new Vector2(0, 1))));

            AnimManager.AddAnimation(AnimationGlobals.DuplighostBattleAnimations.DisguiseStartName, new Animation(null,
                new Animation.Frame(new Rectangle(67, 251, 52, 44), 110d),
                new Animation.Frame(new Rectangle(193, 244, 51, 51), 110d, new Vector2(0, -7))));
            AnimManager.AddAnimation(AnimationGlobals.DuplighostBattleAnimations.DisguiseName, new LoopAnimation(null, AnimationGlobals.InfiniteLoop,
                new Animation.Frame(new Rectangle(129, 247, 51, 48), 110d),
                new Animation.Frame(new Rectangle(257, 248, 51, 47), 110d, new Vector2(0, 1))));

            //Use the same animation for Confused since Confused isn't in PM
            LoopAnimation dizzyAnim = new LoopAnimation(null, AnimationGlobals.InfiniteLoop,
                new Animation.Frame(new Rectangle(132, 198, 52, 41), 110d),
                new Animation.Frame(new Rectangle(196, 198, 52, 41), 110d),
                new Animation.Frame(new Rectangle(260, 198, 52, 41), 110d),
                new Animation.Frame(new Rectangle(324, 198, 52, 41), 110d));

            AnimManager.AddAnimation(AnimationGlobals.StatusBattleAnimations.DizzyName, dizzyAnim);
            AnimManager.AddAnimation(AnimationGlobals.StatusBattleAnimations.ConfusedName, dizzyAnim);
        }

        public override void CleanUp()
        {
            if (IsDead == false)
            {
                RemoveDisguise();
            }
        }

        public override void OnTurnStart()
        {
            base.OnTurnStart();

            StartAction(new HeadbuttAction(), false, BattleManager.Instance.GetFrontPlayer());
        }

        protected override void OnTakeDamage(InteractionHolder damageInfo)
        {
            //If a Duplighost is inflicted with Paralyzed or takes Electric damage, its disguise is removed
            if (damageInfo.Hit == true && damageInfo.DamageElement == Enumerations.Elements.Electric 
                || EntityProperties.HasStatus(Enumerations.StatusTypes.Paralyzed) == true)
            {
                //Remove disguise through a Battle Event
                
            }
        }

        /// <summary>
        /// Removes the Duplighost's disguise.
        /// </summary>
        public void RemoveDisguise()
        {
            PartnerTypeDisguise = Enumerations.PartnerTypes.None;
            FlippableBehavior = null;
        }

        #region Tattle Information

        public bool CanBeTattled { get; set; } = true;

        public string[] GetTattleDescription()
        {
            return new string[] { "N/A" };
        }

        public virtual string[] GetTattleLogEntry()
        {
            //Return a different Tattle for each Partner the Duplighost is disguised as
            switch (PartnerTypeDisguise)
            {
                default:
                    return new string[]
                    {
                        "This is a Duplighost. Duplighosts disguise themselves as members of our party.",
                        "Max HP: 15, Attack Power: 4, Defense Power: 0",
                        "They love to attack in disguise. It seems like they're generally stronger fighters when they're not disguised, though."
                    };
                case Enumerations.PartnerTypes.Goombario:
                    return new string[]
                    {
                        "It's a Duplighost disguised as me.",
                        "Max HP: 15 It'll do Headbonk and Tattle. That looks nothing like me.",
                        "Does it? No! Seriously, though, does it? Mario? Hello?",
                        "Look, I've been working out like crazy trying to get in shape for adventuring. There's just no way I'm that pudgy!"
                    };
                case Enumerations.PartnerTypes.Kooper:
                    return new string[]
                    {
                        "It's a Duplighost disguised as Kooper.",
                        "Max HP: 15 This imposter almost looks better than the original! Ha! Its disguise is pretty much perfect.",
                        "Its attack power is exactly the same as the character it's imitating. It's probably not very difficult for this Duplighost",
                        "to disguise itself as Kooper. After all, our pal is pretty unusual looking. It'll attack us with its shell, just like the regular Kooper.",
                        "You'll have the best luck beating it if you flip it."
                    };
                case Enumerations.PartnerTypes.Bombette:
                    return new string[]
                    {
                        "It's a Duplighost disguised as Bombette.",
                        "Max HP: 15 Its disguise is pretty much perfect. Its attack power is exactly the same as Bombette's.",
                        "You should be careful even though she looks cute."
                    };
                case Enumerations.PartnerTypes.Parakarry:
                    return new string[]
                    {
                        "It's a Duplighost disguised as Parakarry.",
                        "Max HP: 15 He looks even more intelligent as an enemy. Its disguise is pretty much perfect.",
                        "Its attack power is exactly the same as Parakarry's. Don't underestimate him, Mario. He looks pretty serious."
                    };
                case Enumerations.PartnerTypes.Bow:
                    return new string[]
                    {
                        "It's a Duplighost disguised as Bow.",
                        "Max HP: 15 Its disguise is pretty much perfect. Its attack power is exactly the same as Bow's.",
                        "Look, you've seen her in action. Do you really want to know what a Smack attack feels like?"
                    };
                case Enumerations.PartnerTypes.Watt:
                    return new string[]
                    {
                        "It's a Duplighost disguised as Watt.",
                        "Max HP: 15 Its disguise is pretty much perfect. Its attack power is exactly the same as the character it's imitating.",
                        "You know how Watt is shocking? Same goes for this Duplighost."
                    };
                case Enumerations.PartnerTypes.Sushie:
                    return new string[]
                    {
                        "It's a Duplighost disguised as Sushie.",
                        "Max HP: 15 Its face looks even grouchier than the real thing! Its disguise is pretty much perfect.",
                        "Its attack power is exactly the same as Sushie's. I'm glad we have Sushie on our side. She's intimidating!",
                        "I wonder if this imposter is meddlesome as well..."
                    };
                case Enumerations.PartnerTypes.Lakilester:
                    return new string[]
                    {
                        "It's a Duplighost disguised as Lakilester.",
                        "Max HP: 15 It's hard to take him seriously. Its disguise is pretty much perfect.",
                        "Its attack power is exactly the same as Lakilester's. It's tough to dodge his Spiny Flip, so pay attention."
                    };
            }
        }

        #endregion
    }
}
