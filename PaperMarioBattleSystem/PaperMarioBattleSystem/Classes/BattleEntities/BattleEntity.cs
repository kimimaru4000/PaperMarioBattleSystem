using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static PaperMarioBattleSystem.Enumerations;
using static PaperMarioBattleSystem.AnimationGlobals;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// Any fighter that takes part in battle
    /// </summary>
    public abstract class BattleEntity
    {
        /// <summary>
        /// Weaknesses an entity has to particular elements.
        /// Each weakness can be handled differently by each entity
        /// </summary>
        public Dictionary<Elements, bool> Weaknesses { get; private set; } = new Dictionary<Elements, bool>();

        /// <summary>
        /// The animations, referred to by string, of the BattleEntity
        /// </summary>
        public Dictionary<string, Animation> Animations { get; private set; } = new Dictionary<string, Animation>();

        /// <summary>
        /// The current animation being played
        /// </summary>
        protected Animation CurrentAnim = null;

        protected Stats BStats;

        public Stats BattleStats { get { return BStats; } }
        public int CurHP => BattleStats.HP;
        public int CurFP => BattleStats.FP;

        public string Name { get; protected set; } = "Entity";
        
        /// <summary>
        /// The entity's current position
        /// </summary>
        public Vector2 Position { get; set; } = Vector2.Zero;

        /// <summary>
        /// The entity's battle position. The entity goes back to this after each action
        /// </summary>
        public Vector2 BattlePosition { get; protected set; } = Vector2.Zero;

        public float Rotation { get; set; } = 0f;
        public float Scale { get; set; } = 1f;

        public EntityTypes EntityType { get; protected set; } = EntityTypes.Enemy;

        /// <summary>
        /// The previous BattleAction the entity used
        /// </summary>
        public BattleAction PreviousAction { get; protected set; } = null;

        public bool IsDead => CurHP <= 0;

        //TEMPORARY
        public bool UsedTurn = false;

        protected BattleEntity()
        {
            
        }

        protected BattleEntity(Stats stats) : this()
        {
            BStats = stats;
        }

        #region Stat Manipulations

        public virtual void TakeDamage(Elements element, int damage)
        {
            int newDamage = HandleWeakness(element, damage);
            newDamage = HelperGlobals.Clamp(newDamage - BattleStats.Defense, BattleGlobals.MinDamage, BattleGlobals.MaxDamage);

            LoseHP(newDamage);
        }

        public virtual void HealHP(int hp)
        {
            BStats.HP = HelperGlobals.Clamp(BStats.HP + hp, 0, BStats.MaxHP);

            Debug.Log($"{Name} healed {hp} HP!");
        }

        public void HealFP(int fp)
        {
            BStats.FP = HelperGlobals.Clamp(BStats.FP + fp, 0, BStats.MaxFP);
        }

        public virtual void LoseHP(int hp)
        {
            //TEMPORARY: Fix for removing enemies multiple times during overkill
            if (BStats.HP == 0) return;

            BStats.HP = HelperGlobals.Clamp(BStats.HP - hp, 0, BStats.MaxHP);
            if (BStats.HP <= 0)
            {
                Die();
            }

            Debug.Log($"{Name} took {hp} points of damage!");
        }

        public void LoseFP(int fp)
        {
            BStats.FP = HelperGlobals.Clamp(BStats.FP - fp, 0, BStats.MaxFP);
        }

        public void RaiseAttack(int attack)
        {

        }
        
        public void RaiseDefense(int defense)
        {
            
        }

        public void LowerAttack(int attack)
        {

        }

        public void LowerDefense(int defense)
        {

        }

        /// <summary>
        /// Kills the entity instantly
        /// </summary>
        public void Die()
        {
            BStats.HP = 0;

            OnDeath();
        }

        /// <summary>
        /// Performs entity-specific logic on death
        /// </summary>
        public virtual void OnDeath()
        {
            Debug.Log($"{Name} has been defeated!");
        }

        /// <summary>
        /// Entity-specific handling of being damaged by a source the entity is weak to.
        /// The default behavior is to double the damage dealt.
        /// <para>The weakness can still apply even if the total damage dealt is 0.
        /// An example of handling this behavior is when Clefts are turned on their backs when hit by explosive damage.</para>
        /// </summary>
        /// <param name="element">The element used to damage the entity</param>
        /// <param name="damage">The damage dealt to the entity</param>
        /// <returns>The new damage dealt if the weakness handles damage, otherwise the original damage</returns>
        protected virtual int HandleWeakness(Elements element, int damage)
        {
            if (Weaknesses.ContainsKey(element) == false) return damage;

            return damage * 2;
        }

        #endregion

        #region Damage Calculations

        

        #endregion

        #region Turn Methods

        /// <summary>
        /// What happens when the entity's turn starts
        /// </summary>
        public virtual void OnTurnStart()
        {
            Debug.LogWarning($"Started {Name}'s turn!");
        }

        /// <summary>
        /// What happens when the entity's turn ends
        /// </summary>
        public virtual void OnTurnEnd()
        {

        }

        public void EndTurn()
        {
            if (this != BattleManager.Instance.EntityTurn)
            {
                Debug.LogError($"Attempting to end the turn of {Name} when it's not their turn!");
                return;
            }

            BattleManager.Instance.TurnEnd();
        }

        /// <summary>
        /// What happens during the entity's turn (choosing action commmands, etc.)
        /// </summary>
        public virtual void TurnUpdate()
        {
            PreviousAction?.Update();
        }

        #endregion

        public void SetBattlePosition(Vector2 battlePos)
        {
            BattlePosition = battlePos;
        }

        public void StartAction(BattleAction action, params BattleEntity[] targets)
        {
            PreviousAction = action;
            PreviousAction.StartSequence(targets);
        }

        protected void AddAnimation(string animName, Animation anim)
        {
            if (Animations.ContainsKey(animName) == true)
            {
                Debug.LogError($"Entity {Name} already has an animation called \"{animName}\"");
                return;
            }

            Animations.Add(animName, anim);

            //Assume the first animation that gets added is the default, and play it
            //This allows us to safely have a valid animation reference at all times, provided at least one is added
            if (CurrentAnim == null && Animations.Count == 1)
            {
                PlayAnimation(animName);
            }
        }

        public void PlayAnimation(string animName)
        {
            string animToPlay = animName;

            //If animation cannot be found
            if (Animations.ContainsKey(animName) == false)
            {
                Debug.LogError($"Cannot find animation called \"{animName}\" for entity {Name}. Reverting to \"{IdleName}\" anim");

                if (Animations.ContainsKey(IdleName) == true)
                {
                    //Use idle animation
                    animToPlay = IdleName;
                }
                else
                {
                    Debug.LogError($"Cannot find \"{IdleName}\" anim for entity {Name}. Each entity MUST have an \"{IdleName}\" anim!");
                    return;
                }
            }

            //Play animation
            CurrentAnim = Animations[animToPlay];
            CurrentAnim.Play();
        }

        /// <summary>
        /// Used for update logic that applies to the entity regardless of whether it is its turn or not
        /// </summary>
        public void Update()
        {
            CurrentAnim?.Update();
        }

        public virtual void Draw()
        {
            CurrentAnim?.Draw(Position, Color.White, EntityType != EntityTypes.Enemy, .1f);
            PreviousAction?.Draw();
        }
    }
}
