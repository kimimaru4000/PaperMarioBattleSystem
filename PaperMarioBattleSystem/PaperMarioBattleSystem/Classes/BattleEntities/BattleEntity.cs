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
        /// The animations, referred to by string, of the BattleEntity
        /// </summary>
        protected readonly Dictionary<string, Animation> Animations = new Dictionary<string, Animation>();

        /// <summary>
        /// The physical attributes the entity possesses
        /// </summary>
        protected readonly Dictionary<PhysicalAttributes, bool> PhysAttributes = new Dictionary<PhysicalAttributes, bool>();

        /// <summary>
        /// The Weaknesses the entity has
        /// </summary>
        protected readonly Dictionary<Elements, WeaknessHolder> Weaknesses = new Dictionary<Elements, WeaknessHolder>();

        /// <summary>
        /// The Resistances the entity has
        /// </summary>
        protected readonly Dictionary<Elements, ResistanceHolder> Resistances = new Dictionary<Elements, ResistanceHolder>();

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

        public Stats BattleStats { get; set; } = Stats.Default;
        public int CurHP => BattleStats.HP;
        public int CurFP => BattleStats.FP;

        //NOTE: Unused right now
        public int TurnsUsed { get; protected set; } = 0;
        public int MaxTurns { get; protected set; } = 1;

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

        public bool IsDead => HealthState == HealthStates.Dead;

        //TEMPORARY
        public bool UsedTurn = false;

        protected BattleEntity()
        {
            
        }

        protected BattleEntity(Stats stats) : this()
        {
            BattleStats = stats;
        }

        #region Stat Manipulations

        /// <summary>
        /// Makes the entity take damage from an attack, factoring in stats such as defense, weaknesses, and resistances
        /// </summary>
        /// <param name="element">The element to damage the entity with</param>
        /// <param name="damage">The damage to deal to the entity</param>
        public virtual void TakeDamage(Elements element, int damage)
        {
            int totalDamage = damage;

            totalDamage = UtilityGlobals.Clamp(totalDamage - BattleStats.Defense, BattleGlobals.MinDamage, BattleGlobals.MaxDamage);

            //Get weaknesses and resistances
            ElementInteractionResult result = HandleElementDamage(element, ref totalDamage);

            //NOTE: We do this because we still need to show the correct amount of damage being done on an instant KO
            if (result == ElementInteractionResult.Damage || result == ElementInteractionResult.KO)
            {
                //Lose HP
                LoseHP(totalDamage);

                //If this damage doesn't kill the entity on an instant KO, kill the entity now
                if (result == ElementInteractionResult.KO && IsDead == false)
                {
                    Die();
                }
            }
            //Heal the entity
            else if (result == ElementInteractionResult.Heal)
            {
                Debug.Log($"{Name} is being healed because it has a {nameof(ResistanceTypes.Heal)} resistance to Element {element}");

                //Heal the damage
                HealHP(totalDamage);
            }
        }

        public virtual void HealHP(int hp)
        {
            BattleStats.HP = UtilityGlobals.Clamp(BattleStats.HP + hp, 0, BattleStats.MaxHP);

            UpdateHealthState();
            Debug.Log($"{Name} healed {hp} HP!");
        }

        public void HealFP(int fp)
        {
            BattleStats.FP = UtilityGlobals.Clamp(BattleStats.FP + fp, 0, BattleStats.MaxFP);
        }

        public virtual void LoseHP(int hp)
        {
            BattleStats.HP = UtilityGlobals.Clamp(BattleStats.HP - hp, 0, BattleStats.MaxHP);
            UpdateHealthState();
            if (IsDead == true)
            {
                Die();
            }

            Debug.Log($"{Name} took {hp} points of damage!");
        }

        public void LoseFP(int fp)
        {
            BattleStats.FP = UtilityGlobals.Clamp(BattleStats.FP - fp, 0, BattleStats.MaxFP);
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
            BattleStats.HP = 0;
            HealthState = HealthStates.Dead;
            PlayAnimation(AnimationGlobals.DeathName, true);

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
            HealthStates newHealthState = HealthState;
            if (CurHP > BattleGlobals.MaxDangerHP)
            {
                newHealthState = HealthStates.Normal;
            }
            else if (CurHP >= BattleGlobals.MinDangerHP)
            {
                newHealthState = HealthStates.Danger;
            }
            else if (CurHP == BattleGlobals.PerilHP)
            {
                newHealthState = HealthStates.Peril;
            }
            else
            {
                newHealthState = HealthStates.Dead;
            }

            //Change health states if they're no longer the same
            if (newHealthState != HealthState)
            {
                OnHealthStateChange(newHealthState);

                //Change to the new health state
                HealthState = newHealthState;
            }
        }

        /// <summary>
        /// What occurs when an entity changes HealthStates
        /// </summary>
        /// <param name="newHealthState">The new HealthState of the entity</param>
        protected virtual void OnHealthStateChange(HealthStates newHealthState)
        {

        }

        /// <summary>
        /// Calculates the final result of elemental damage on this entity, based on its weaknesses and resistances to that element
        /// </summary>
        /// <param name="element">The element the entity is attacked with</param>
        /// <param name="damage">The damage of the attack. It's passed by ref, as it can be modified</param>
        /// <returns>An ElementInteractionResult stating the final result of the elemental damage dealt to this entity</returns>
        private ElementInteractionResult HandleElementDamage(Elements element, ref int damage)
        {
            ElementInteractionResult result = ElementInteractionResult.Damage;
            int newDamage = damage;

            //NOTE: If an entity is both resistant and weak to a particular element, they cancel out.
            //I decided to go with this approach because it's the simplest for this situation, which
            //doesn't seem desirable to begin with but could be interesting in its application
            WeaknessHolder weakness = GetWeakness(element);
            ResistanceHolder resistance = GetResistance(element);

            //If there's both a weakness and resistance, return
            if (weakness.WeaknessType == WeaknessTypes.None && resistance.ResistanceType == ResistanceTypes.None) return result;

            //Handle weaknesses
            if (weakness.WeaknessType == WeaknessTypes.PlusDamage)
            {
                newDamage = UtilityGlobals.Clamp(newDamage + weakness.Value, BattleGlobals.MinDamage, BattleGlobals.MaxDamage);
            }
            else if (weakness.WeaknessType == WeaknessTypes.KO)
            {
                result = ElementInteractionResult.KO;
            }

            //Handle resistances
            if (resistance.ResistanceType == ResistanceTypes.MinusDamage)
            {
                newDamage = UtilityGlobals.Clamp(newDamage - resistance.Value, BattleGlobals.MinDamage, BattleGlobals.MaxDamage);
            }
            else if (resistance.ResistanceType == ResistanceTypes.NoDamage)
            {
                newDamage = BattleGlobals.MinDamage;
            }
            else if (resistance.ResistanceType == ResistanceTypes.Heal)
            {
                result = ElementInteractionResult.Heal;
            }

            //Set the damage value
            damage = newDamage;

            return result;
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
            PreviousAction?.PostUpdate();
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

            Debug.Log($"Added the physical attribute {physicalAttribute} to {Name}'s existing attributes!");

            PhysAttributes.Add(physicalAttribute, true);
        }

        /// <summary>
        /// Removes a physical attribute from the entity
        /// </summary>
        /// <param name="physicalAttribute">The physical attribute to remove</param>
        /// <returns>true if the physical attribute was successfully found and removed, false otherwise</returns>
        public bool RemovePhysAttribute(PhysicalAttributes physicalAttribute)
        {
            bool removed = PhysAttributes.Remove(physicalAttribute);

            if (removed == true)
                Debug.Log($"Removed the physical attribute {physicalAttribute} from {Name}'s existing attributes!");

            return removed;
        }

        /// <summary>
        /// Tells whether the entity has a set of physical attributes or not
        /// </summary>
        /// <param name="checkAny">If true, checks the entity has any of the physical attributes rather than all</param>
        /// <param name="attributes">The set of physical attributes to check the entity has</param>
        /// <returns>true if the entity has any or all, based on the value of checkAny, of the physical attributes in the set, otherwise false</returns>
        public bool HasPhysAttributes(bool checkAny, params PhysicalAttributes[] attributes)
        {
            if (attributes == null) return false;

            //Loop through and look at each attribute
            //If we're looking for all attributes, return false if one is not found
            //If we're looking for any attribute, return true if one is found
            for (int i = 0; i < attributes.Length; i++)
            {
                if (PhysAttributes.ContainsKey(attributes[i]) == checkAny) return checkAny;
            }

            return !checkAny;
        }

        /// <summary>
        /// Determines the result of contact, based on the type of contact made, when it's made with this entity
        /// </summary>
        /// <param name="contactType">The type of contact made with this entity</param>
        /// <returns>The result of the interaction</returns>
        public ContactResult GetContactResult(ContactTypes contactType)
        {
            return Interactions.GetContactResult(contactType, PhysAttributes.Keys.ToArray());
        }

        /// <summary>
        /// Gets this entity's weakness to a particular Element
        /// </summary>
        /// <param name="element">The Element to test a weakness for</param>
        /// <returns>A copy of the WeaknessHolder associated with the element if found, otherwise default weakness data</returns>
        public WeaknessHolder GetWeakness(Elements element)
        {
            if (Weaknesses.ContainsKey(element) == false)
            {
                //Debug.Log($"{Name} does not have a weakness for {element}");
                return WeaknessHolder.Default;
            }

            return Weaknesses[element];
        }

        /// <summary>
        /// Gets this entity's resistance to a particular Element
        /// </summary>
        /// <param name="element">The element to test a resistance towards</param>
        /// <returns>A copy of the ResistanceHolder associated with the element if found, otherwise default resistance data</returns>
        public ResistanceHolder GetResistance(Elements element)
        {
            if (Resistances.ContainsKey(element) == false)
            {
                //Debug.Log($"{Name} does not have a resistance for {element}");
                return ResistanceHolder.Default;
            }

            return Resistances[element];
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
