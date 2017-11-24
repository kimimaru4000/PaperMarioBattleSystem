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
    /// Bobbery's Bombs created in his Bomb Squad move.
    /// They're untargetable neutral entities that explode after a number of phase cycles, dealing explosive damage to those in their range.
    /// </summary>
    public sealed class BobberyBomb : BattleEntity
    {
        /// <summary>
        /// The number of turns the Bobbery Bomb has taken.
        /// </summary>
        private int TurnsTaken = 0;

        private LoopAnimation SparkAnimation = null;

        /// <summary>
        /// Whether the Bobbery Bomb detonated or not.
        /// </summary>
        private bool Detonated = false;

        private Rectangle GetHitbox => new Rectangle((int)Position.X - 15, (int)Position.Y - 60, 80, 140);

        public BobberyBomb(int damage) : base(new Stats(0, 1, 0, damage, 0))
        {
            Name = "Bobbery Bomb";

            EntityType = EntityTypes.Neutral;

            //Explosions cause the bombs to detonate (only possible from other Bobbery Bombs in the actual games)
            EntityProperties.AddWeakness(Elements.Explosion, new WeaknessHolder(WeaknessTypes.KO, 0));

            //15 frame color change normal, 6 frame color change faster
            //2 frame spark change for normal and faster
            //Bomb starts brown, turns red 1 frame after fully stopping, then starts the spark
            //Spark order is red, orange, grey. It cycles back to red after grey
            Texture2D spriteSheet = AssetManager.Instance.LoadAsset<Texture2D>($"{ContentGlobals.SpriteRoot}/Neutral/BobberyBomb");

            double bombFrameRate = (1d / 15d) * 1000d;
            double sparkFrameRate = (1d / 30d) * 1000d;

            AnimManager.AddAnimation(AnimationGlobals.IdleName, new LoopAnimation(spriteSheet, AnimationGlobals.InfiniteLoop,
                new Animation.Frame(new Rectangle(62, 1, 65, 68), bombFrameRate),
                new Animation.Frame(new Rectangle(62, 72, 65, 68), bombFrameRate)));

            //Offset the spark animation so we can play it in the same position as the bombs
            Vector2 sparkOffset = new Vector2(16, -68 / 4);

            SparkAnimation = new LoopAnimation(spriteSheet, AnimationGlobals.InfiniteLoop,
                new Animation.Frame(new Rectangle(0, 0, 32, 24), sparkFrameRate, sparkOffset),
                new Animation.Frame(new Rectangle(0, 24, 32, 24), sparkFrameRate, sparkOffset),
                new Animation.Frame(new Rectangle(0, 48, 32, 24), sparkFrameRate, sparkOffset));

            //Pause the spark animation until we're ready to play it
            SparkAnimation.Pause();
        }

        public override void OnBattleStart()
        {
            base.OnBattleStart();
        }

        protected override void OnTakeDamage(InteractionHolder damageInfo)
        {
            base.OnTakeDamage(damageInfo);
        
            //When taking explosive damage, the bombs explode
            //If they're already in the process of exploding, don't do anything
            if (damageInfo.DamageElement == Elements.Explosion)
            {
                TintColor = Color.Black;

                //Detonate if the bomb hasn't upon taking explosive damage
                if (Detonated == false)
                {
                    //Explode with a battle event and do nothing on this turn
                    DetonateBobberyBombBattleEvent detonateEvent = new DetonateBobberyBombBattleEvent(this, GetDamageData(),
                        GetHitbox, HeightStates.Grounded, HeightStates.Hovering, HeightStates.Airborne);

                    //Queue the event
                    BattleEventManager.Instance.QueueBattleEvent((int)BattleGlobals.StartEventPriorities.BobberyBomb,
                        new BattleManager.BattleState[] { BattleManager.BattleState.Turn, BattleManager.BattleState.TurnEnd },
                        detonateEvent);

                    Detonated = true;
                }
            }
        }

        private DamageData GetDamageData()
        {
            return new DamageData(BattleStats.TotalAttack, Elements.Explosion, false, ContactTypes.None, null, DamageEffects.None);
        }

        public override void OnTurnStart()
        {
            base.OnTurnStart();

            //This is handled for turns instead of Phase Cycles in the event that the bomb's turn count is modified
            //This way, if it is somehow inflicted with Fast, it will blow up in one turn instead of two
            TurnsTaken++;

            //Blink faster on the first turn
            if (TurnsTaken <= 1)
            {
                StartAction(new BlinkFasterAction(AnimationGlobals.IdleName, 1.6f), true, null);
            }
            //Explode on the second turn
            else
            {
                //Explode with a battle event and do nothing on this turn
                DetonateBobberyBombBattleEvent detonateEvent = new DetonateBobberyBombBattleEvent(this, GetDamageData(),
                    GetHitbox, HeightStates.Grounded, HeightStates.Hovering, HeightStates.Airborne);

                //Queue the event
                BattleEventManager.Instance.QueueBattleEvent((int)BattleGlobals.StartEventPriorities.BobberyBomb,
                    new BattleManager.BattleState[] { BattleManager.BattleState.TurnEnd },
                    detonateEvent);

                Detonated = true;

                StartAction(new NoAction(), true, null);
            }
        }

        public override int GetEquippedBadgeCount(BadgeGlobals.BadgeTypes badgeType)
        {
            //Bomb Squad Bombs don't have any held items or badges
            return 0;
        }

        /// <summary>
        /// Initializes the Bobbery Bomb when it comes to rest after being thrown.
        /// It sets its BattlePosition to its current position and starts playing the spark animation.
        /// </summary>
        public void InitializeBomb()
        {
            SetBattlePosition(Position);

            //Play the spark animation
            SparkAnimation.Play();
        }

        public override void Update()
        {
            base.Update();

            SparkAnimation.Update();
        }

        public override void Draw()
        {
            base.Draw();

            //Draw the spark animation if it's playing
            if (SparkAnimation.IsPlaying == true)
                SparkAnimation.Draw(Position, TintColor, false, .11f);

            //DrawHitbox();
        }

        private void DrawHitbox()
        {
            Rectangle hitRect = GetHitbox;

            Debug.DebugDrawHollowRect(hitRect, Color.White, .7f, 2, false);
        }
    }
}
