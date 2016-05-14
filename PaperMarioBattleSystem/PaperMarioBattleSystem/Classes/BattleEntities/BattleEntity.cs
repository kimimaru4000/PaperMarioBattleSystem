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
        /// The physical attributes the entity possesses
        /// </summary>
        protected Dictionary<PhysicalAttributes, bool> PhysAttributes { get; private set; } = new Dictionary<PhysicalAttributes, bool>();

        /// <summary>
        /// Weaknesses an entity has to particular elements.
        /// Each weakness can be handled differently by each entity
        /// </summary>
        protected Dictionary<Elements, bool> Weaknesses { get; private set; } = new Dictionary<Elements, bool>();

        /// <summary>
        /// The animations, referred to by string, of the BattleEntity
        /// </summary>
        protected Dictionary<string, Animation> Animations { get; private set; } = new Dictionary<string, Animation>();

        /// <summary>
        /// The HeightState of the entity
        /// </summary>
        public HeightStates HeightState { get; protected set; } = HeightStates.Grounded;

        /// <summary>
        /// The HealthState of the entity.
        /// This can apply to any entity, but only Mario and his Partners utilize Danger and Peril
        /// </summary>
        public HealthStates HealthState { get; private set; } = HealthStates.Normal;

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
            int newDamage = damage;
            newDamage = UtilityGlobals.Clamp(newDamage - BattleStats.Defense, BattleGlobals.MinDamage, BattleGlobals.MaxDamage);

            LoseHP(newDamage);
        }

        public virtual void HealHP(int hp)
        {
            BStats.HP = UtilityGlobals.Clamp(BStats.HP + hp, 0, BStats.MaxHP);

            UpdateHealthState();
            Debug.Log($"{Name} healed {hp} HP!");
        }

        public void HealFP(int fp)
        {
            BStats.FP = UtilityGlobals.Clamp(BStats.FP + fp, 0, BStats.MaxFP);
        }

        public virtual void LoseHP(int hp)
        {
            //TEMPORARY: Fix for removing enemies multiple times during overkill
            if (BStats.HP == 0) return;

            BStats.HP = UtilityGlobals.Clamp(BStats.HP - hp, 0, BStats.MaxHP);
            UpdateHealthState();
            if (HealthState == HealthStates.Dead)
            {
                Die();
            }

            Debug.Log($"{Name} took {hp} points of damage!");
        }

        public void LoseFP(int fp)
        {
            BStats.FP = UtilityGlobals.Clamp(BStats.FP - fp, 0, BStats.MaxFP);
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
        /// Updates the entity's health state based on its current HP
        /// </summary>
        protected void UpdateHealthState()
        {
            if (CurHP > BattleGlobals.MaxDangerHP)
            {
                HealthState = HealthStates.Normal;
            }
            else if (CurHP >= BattleGlobals.MinDangerHP)
            {
                HealthState = HealthStates.Danger;
            }
            else if (CurHP == BattleGlobals.PerilHP)
            {
                HealthState = HealthStates.Peril;
            }
            else
            {
                HealthState = HealthStates.Dead;
            }
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
        //protected virtual int HandleWeakness(Elements element, int damage)
        //{
        //    if (Weaknesses.ContainsKey(element) == false) return damage;
        //
        //    return damage * 2;
        //}

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

        /// <summary>
        /// Adds an animation for the entity.
        /// If an animation already exists, it will be replaced.
        /// </summary>
        /// <param name="animName">The name of the animation</param>
        /// <param name="anim">The animation reference</param>
        protected void AddAnimation(string animName, Animation anim)
        {
            //Return if trying to add null animation
            if (anim == null)
            {
                Debug.LogError($"Trying to add null animation called \"{animName}\" to entity {Name}, so it won't be added");
                return;
            }

            if (Animations.ContainsKey(animName) == true)
            {
                Debug.LogWarning($"Entity {Name} already has an animation called \"{animName}\" and will be replaced");

                //Clear the current animation reference if it is the animation being removed
                Animation prevAnim = Animations[animName];
                if (CurrentAnim == prevAnim)
                { 
                    CurrentAnim = null;
                }

                Animations.Remove(animName);
            }

            Animations.Add(animName, anim);

            //Play the most recent animation that gets added is the default, and play it
            //This allows us to safely have a valid animation reference at all times, provided at least one is added
            if (CurrentAnim == null)
            {
                PlayAnimation(animName);
            }
        }

        public Animation GetAnimation(string animName)
        {
            //If animation cannot be found
            if (Animations.ContainsKey(animName) == false)
            {
                Debug.LogError($"Cannot find animation called \"{animName}\" for entity {Name} to play");
                return null;
            }

            return Animations[animName];
        }

        /// <summary>
        /// Plays an animation that the entity has, specified by its name. If the animation does not have the specified animation,
        /// nothing happens
        /// </summary>
        /// <param name="animName">The name of the animation to play</param>
        /// <param name="resetPrevious">If true, resets the previous animation that was playing, if any.
        /// This will also reset its speed</param>
        public void PlayAnimation(string animName, bool resetPrevious = false)
        {
            Animation animToPlay = GetAnimation(animName);

            //If animation cannot be found, return
            if (animToPlay == null)
            {
                return;
            }

            //Reset the previous animation if specified
            if (resetPrevious == true)
            {
                CurrentAnim?.Reset(true);
            }

            //Play animation
            CurrentAnim = animToPlay;
            CurrentAnim.Play();
        }

        /// <summary>
        /// Adds a physical attribute to the entity
        /// </summary>
        /// <param name="physicalAttribute">The physical attribute to add</param>
        public void AddPhysAttribute(PhysicalAttributes physicalAttribute)
        {
            if (PhysAttributes.ContainsKey(physicalAttribute) == true)
            {
                Debug.LogError($"{Name} already has the {physicalAttribute} {nameof(PhysicalAttributes)}!");
                return;
            }

            PhysAttributes.Add(physicalAttribute, true);
        }

        /// <summary>
        /// Removes a physical attribute from the entity
        /// </summary>
        /// <param name="physicalAttribute">The physical attribute to remove</param>
        /// <returns>true if the physical attribute was successfully found and removed, false otherwise</returns>
        public bool RemovePhysAttribute(PhysicalAttributes physicalAttribute)
        {
            return PhysAttributes.Remove(physicalAttribute);
        }

        /// <summary>
        /// Tells whether the entity has a set of physical attributes or not
        /// </summary>
        /// <param name="attributes">The set of physical attributes to check the entity has</param>
        /// <returns>true if the entity has all of the physical attributes in the set, otherwise false</returns>
        public bool HasPhysAttributes(params PhysicalAttributes[] attributes)
        {
            if (attributes == null) return false;

            for (int i = 0; i < attributes.Length; i++)
            {
                if (PhysAttributes.ContainsKey(attributes[i]) == false) return false;
            }

            return true;
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
