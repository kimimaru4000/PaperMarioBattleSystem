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
        /// The StatusEffects the entity is afflicted with
        /// </summary>
        protected readonly Dictionary<StatusTypes, StatusEffect> Statuses = new Dictionary<StatusTypes, StatusEffect>();

        /// <summary>
        /// The likelihood of the entity being affected by each StatusEffect. Empty entries are treated as a 0% chance
        /// </summary>
        //NOTE: Each entity needs its own StatusEffect durations.
        protected readonly Dictionary<StatusTypes, int> StatusPercentages = new Dictionary<StatusTypes, int>();

        /// <summary>
        /// Miscellaneous properties of the entity
        /// </summary>
        protected readonly Dictionary<MiscProperty, MiscValueHolder> MiscProperties = new Dictionary<MiscProperty, MiscValueHolder>();

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
        /// The previous animation that has been played
        /// </summary>
        protected Animation PreviousAnim = null;

        /// <summary>
        /// The current animation being played
        /// </summary>
        protected Animation CurrentAnim = null;

        public Stats BattleStats { get; set; } = Stats.Default;
        public int CurHP => BattleStats.HP;
        public int CurFP => BattleStats.FP;

        /// <summary>
        /// The base number of turns the BattleEntity has in each of its phases
        /// </summary>
        public int BaseTurns { get; protected set; } = BattleGlobals.DefaultTurnCount;

        /// <summary>
        /// The number of turns the BattleEntity used in this phase
        /// </summary>
        public int TurnsUsed { get; protected set; } = 0;

        /// <summary>
        /// The current max number of turns the BattleEntity has in this phase
        /// </summary>
        public int MaxTurns { get; protected set; } = BattleGlobals.DefaultTurnCount;

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
        public bool IsTurn => BattleManager.Instance.EntityTurn == this;

        public bool UsedTurn => (TurnsUsed >= MaxTurns || IsDead == true);

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
        /// <param name="damageResult">The InteractionHolder containing the result of a damage interaction</param>
        public void TakeDamage(InteractionHolder damageResult)
        {
            Elements element = damageResult.DamageElement;
            int damage = damageResult.TotalDamage;
            bool piercing = damageResult.Piercing;
            StatusEffect[] statusesInflicted = damageResult.StatusesInflicted;

            //Subtract Defense on non-piercing damage
            if (piercing == false)
            {
                damage = UtilityGlobals.Clamp(damage - BattleStats.Defense, BattleGlobals.MinDamage, BattleGlobals.MaxDamage);
            }

            //Handle the elemental interaction results
            ElementInteractionResult elementResult = damageResult.ElementResult;

            //Check for Invincibility
            bool invincible = GetMiscProperty(MiscProperty.Invincible).BoolValue;
            //If the entity is invincible, don't deal any damage and negate all weaknesses and resistances
            //The entity wouldn't heal if it should because invincibility means it can't get hit
            if (invincible == true)
            {
                damage = 0;
                elementResult = ElementInteractionResult.Damage;
            }

            if (elementResult == ElementInteractionResult.Damage || elementResult == ElementInteractionResult.KO)
            {
                if (elementResult == ElementInteractionResult.Damage)
                {
                    Debug.Log($"{Name} was hit with {element} " + (piercing ? "piercing" : "non-piercing") + " damage!");

                    //Lose HP
                    LoseHP(damage);
                }
                //Kill the entity now on an instant KO
                else if (elementResult == ElementInteractionResult.KO)
                {
                    Debug.Log($"{Name} was instantly KO'd from {element} because it has a {nameof(WeaknessTypes.KO)} weakness");

                    Die();
                }
            }
            //Heal the entity
            else if (elementResult == ElementInteractionResult.Heal)
            {
                Debug.Log($"{Name} was healed because it has a {nameof(ResistanceTypes.Heal)} resistance to Element {element}");

                //Heal the damage
                HealHP(damage);
            }

            //Inflict Statuses
            if (statusesInflicted != null)
            {
                for (int i = 0; i < statusesInflicted.Length; i++)
                {
                    AfflictStatus(statusesInflicted[i]);
                }
            }

            //If this entity received damage during its action sequence, it has been interrupted
            //The null check is necessary in the event that a StatusEffect that deals damage at the start of the phase, such as Poison,
            //is inflicted at the start of the battle before any entity has moved
            if (IsTurn == true && PreviousAction?.InSequence == true)
            {
                PreviousAction.StartInterruption(element);
            }
        }

        /// <summary>
        /// Makes the entity take damage from an attack, factoring in stats such as defense, weaknesses, and resistances.
        /// <para>This method is a shorthand for inflicting simple damage such as Poison damage every turn from a Status Effect</para>
        /// </summary>
        /// <param name="element">The element to damage the entity with</param>
        /// <param name="damage">The damage to deal to the entity</param>
        /// <param name="piercing">Whether the attack penetrates Defense or not</param>
        public void TakeDamage(Elements element, int damage, bool piercing)
        {
            TakeDamage(new InteractionHolder(null, damage, element, ElementInteractionResult.Damage, ContactTypes.None, piercing, null, true));
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

        public void ModifyAccuracy(int accuracy)
        {
            BattleStats.Accuracy = UtilityGlobals.Clamp(BattleStats.Accuracy + accuracy, 0, int.MaxValue);
        }

        public void ModifyEvasion(int evasion)
        {
            BattleStats.Evasion = UtilityGlobals.Clamp(BattleStats.Evasion + evasion, 0, int.MaxValue);
        }

        /// <summary>
        /// Kills the entity instantly
        /// </summary>
        public void Die()
        {
            BattleStats.HP = 0;
            UpdateHealthState();
            PlayAnimation(AnimationGlobals.DeathName, true);

            //Remove all StatusEffects on the entity
            RemoveAllStatuses();

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

        #endregion

        #region Damage Related

        /// <summary>
        /// Checks if the entity's attempt to hit another entity is successful based on the entity's Accuracy and the victim's Evasion
        /// </summary>
        /// <param name="victim">The entity trying to evade</param>
        /// <returns>true if the entity hits and the victim doesn't evade, false otherwise</returns>
        public bool AttemptHitEntity(BattleEntity victim)
        {
            return (AttemptHit() == true && victim.AttemptEvade() == false);
        }

        /// <summary>
        /// Performs a check to see if the entity hit based on its Accuracy stat
        /// </summary>
        /// <returns>true if the entity successfully hit, false if the entity fails</returns>
        private bool AttemptHit()
        {
            int valueTest = GeneralGlobals.Randomizer.Next(0, 100);
            return (valueTest < BattleStats.Accuracy);
        }

        /// <summary>
        /// Makes the entity attempt to evade an attack, returning a value indicating the result
        /// </summary>
        /// <returns>true if the entity successful evaded the attack, false if the attack hits</returns>
        //NOTE: When dealing with Badges such as Close Call, we should compare the entity's Evasion first, then perform
        //the test again with the Badges' Evasion added in. If the Badges' Evasion bonus allows the entity to evade the attack,
        //that's when we'd play the "LUCKY" animation
        private bool AttemptEvade()
        {
            int valueTest = GeneralGlobals.Randomizer.Next(0, 100);
            return (valueTest < BattleStats.Evasion);
        }

        #endregion

        #region Turn Methods

        /// <summary>
        /// What happens at the start of the phase cycle
        /// </summary>
        public virtual void OnPhaseCycleStart()
        {
            TurnsUsed = 0;
            MaxTurns = BaseTurns;

            StatusEffect[] statuses = GetStatuses();
            for (int i = 0; i < statuses.Length; i++)
            {
                statuses[i].PhaseCycleStart();
            }
        }

        /// <summary>
        /// What happens at the start of the entity's phase
        /// </summary>
        public virtual void OnPhaseStart()
        {
            Debug.Log($"Started phase for {Name}!");
        }

        /// <summary>
        /// What happens at the end of the entity's phase
        /// </summary>
        public virtual void OnPhaseEnd()
        {
            Debug.Log($"Ended phase for {Name}");
        }

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

            TurnsUsed++;
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

        /// <summary>
        /// Sets the max number of turns the entity has during this phase
        /// </summary>
        /// <param name="maxTurns">The new number of turns the entity has for this phase</param>
        public void SetMaxTurns(int maxTurns)
        {
            MaxTurns = maxTurns;
        }

        /// <summary>
        /// Sets the number of turns used by the entity during this phase.
        /// This is primarily used by StatusEffects
        /// </summary>
        /// <param name="turnsUsed">The new number of turns the entity used in this phase</param>
        public void SetTurnsUsed(int turnsUsed)
        {
            TurnsUsed = turnsUsed;
        }

        /// <summary>
        /// Makes the entity brace for an attack against it.
        /// The common use-case for this is starting the inputs for Guard and Superguard
        /// </summary>
        /// <param name="attacker">The entity attacking this one</param>
        public virtual void BraceAttack(BattleEntity attacker)
        {
            
        }

        /// <summary>
        /// Stops the entity from bracing for an attack, as the attack is now over
        /// </summary>
        public virtual void StopBracing()
        {

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

            anim.SetKey(animName);
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
        public void PlayAnimation(string animName, bool resetPrevious = false, Animation.AnimFinish onFinish = null)
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

            //Set previous animation
            PreviousAnim = CurrentAnim;

            //Play animation
            CurrentAnim = animToPlay;
            CurrentAnim.Play(onFinish);
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
        /// <param name="attacker">The entity attacking this one</param>
        /// <param name="contactType">The type of contact made with this entity</param>
        /// <returns>A ContactResultInfo containing the result of the interaction</returns>
        public ContactResultInfo GetContactResult(BattleEntity attacker, ContactTypes contactType)
        {
            return Interactions.GetContactResult(attacker, contactType, PhysAttributes.Keys.ToArray());
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
        /// Attempts to afflict the entity with a StatusEffect, based on its status percentage for the StatusEffect
        /// </summary>
        /// <param name="status">The StatusEffect to afflict the entity with</param>
        /// <returns>true if the StatusEffect was successfully afflicted, false otherwise</returns>
        public bool TryAfflictStatus(StatusEffect status)
        {
            int percentage = GetStatusPercentage(status.StatusType);
            int valueTest = GeneralGlobals.Randomizer.Next(1, 101);

            //Test for StatusEffect immunity - if the entity is immune, don't allow the StatusEffect to be inflicted
            bool statusImmune = GetMiscProperty(MiscProperty.StatusImmune).BoolValue;
            if (statusImmune == true) percentage = 0;

            return (valueTest <= percentage);
        }

        /// <summary>
        /// Directly afflicts the entity with a StatusEffect
        /// </summary>
        /// <param name="status">The StatusEffect to afflict the entity with</param>
        public void AfflictStatus(StatusEffect status)
        {
            //Don't do anything if the entity already has this StatusEffect
            if (HasStatus(status.StatusType) == true)
            {
                Debug.Log($"{Name} is already afflicted with the {status.StatusType} Status!");
                return;
            }

            StatusEffect newStatus = status.Copy();

            //Add the status then afflict it
            Statuses.Add(newStatus.StatusType, newStatus);
            newStatus.SetEntity(this);
            newStatus.Afflict();

            Debug.LogWarning($"Afflicted {Name} with the {newStatus.StatusType} Status!");
        }

        /// <summary>
        /// Ends and removes a StatusEffect on the entity
        /// </summary>
        /// <param name="statusType">The StatusTypes of the StatusEffect to remove</param>
        public void RemoveStatus(StatusTypes statusType)
        {
            //Don't do anything if the entity doesn't have this status
            if (HasStatus(statusType) == false)
            {
                Debug.Log($"{Name} is not currently afflicted with the {statusType} Status!");
                return;
            }

            StatusEffect status = Statuses[statusType];

            //End the status then remove it
            status.End();
            status.ClearEntity();
            Statuses.Remove(statusType);

            Debug.LogWarning($"Removed the {statusType} Status on {Name}!");
        }

        /// <summary>
        /// Ends and removes all StatusEffects on the entity
        /// </summary>
        private void RemoveAllStatuses()
        {
            StatusEffect[] statusEffects = GetStatuses();
            for (int i = 0; i < statusEffects.Length; i++)
            {
                RemoveStatus(statusEffects[i].StatusType);
            }
        }

        /// <summary>
        /// Tells if the entity is currently afflicted with a particular StatusEffect
        /// </summary>
        /// <param name="statusType">The StatusTypes of the StatusEffect to check</param>
        /// <returns>true if the entity is afflicted with the StatusEffect, otherwise false</returns>
        public bool HasStatus(StatusTypes statusType)
        {
            return Statuses.ContainsKey(statusType);
        }

        /// <summary>
        /// Returns all StatusEffects the entity is afflicted with, sorted by their Priority
        /// </summary>
        /// <returns>An array of StatusEffects sorted by their Priority. If no StatusEffects are on the entity, it'll return an empty array</returns>
        public StatusEffect[] GetStatuses()
        {
            //Get the values in a list, then sort them
            List<StatusEffect> statusList = Statuses.Values.ToList();
            statusList.Sort(StatusEffect.StatusPrioritySort);

            return statusList.ToArray();
        }

        /// <summary>
        /// Adds a percentage chance of the entity being inflicted with a StatusEffect.
        /// If a percentage already exists for a StatusEffect, it will be replaced
        /// </summary>
        /// <param name="statusType">The StatusType of the StatusEffect</param>
        /// <param name="percentage">The percentage of being inflicted with the StatusEffect. This value cannot be lower than 0.</param>
        protected void AddStatusPercentage(StatusTypes statusType, int percentage)
        {
            if (StatusPercentages.ContainsKey(statusType) == true)
            {
                Debug.Log($"Replacing percentage for the {statusType} Status as {Name} already has one!");
            }

            //Clamp the percentage value
            percentage = UtilityGlobals.Clamp(percentage, 0, int.MaxValue);

            StatusPercentages.Add(statusType, percentage);
        }

        /// <summary>
        /// Tells if the entity has a percentage value for being inflicted with a particular StatusEffect or not
        /// </summary>
        /// <param name="statusType">The StatusType of the StatusEffect</param>
        /// <returns>true if a percentage can be found for the specified StatusType, false otherwise</returns>
        protected bool HasStatusPercentage(StatusTypes statusType)
        {
            return StatusPercentages.ContainsKey(statusType);
        }

        /// <summary>
        /// Retrieves the percentage chance of the entity being afflicted with a particular StatusEffect
        /// </summary>
        /// <param name="statusType">The StatusType of the StatusEffect</param>
        /// <returns>The percentage corresponding to the specified StatusType, or 0 if there is no percentage value</returns>
        public int GetStatusPercentage(StatusTypes statusType)
        {
            //Initialize to 0
            int percentage = 0;

            //Set the percentage to the correct value if one can be found
            if (HasStatusPercentage(statusType) == true)
            {
                percentage = StatusPercentages[statusType];
            }

            return percentage;
        }

        /// <summary>
        /// Adds a MiscProperty to the entity if it doesn't already have it
        /// </summary>
        /// <param name="property">The MiscProperty to add</param>
        /// <param name="value">The value of the MiscProperty</param>
        public void AddMiscProperty(MiscProperty property, MiscValueHolder value)
        {
            //Return if the entity already has it
            if (HasMiscProperty(property) == true)
            {
                Debug.LogWarning($"{Name} already has the {property} property!");
                return;
            }

            MiscProperties.Add(property, value);
            Debug.Log($"Added the {property} property to {Name}!");
        }

        /// <summary>
        /// Removes a MiscProperty from the entity
        /// </summary>
        /// <param name="property">The MiscProperty to remove</param>
        public void RemoveMiscProperty(MiscProperty property)
        {
            if (HasMiscProperty(property) == true)
            {
                Debug.Log($"Removed the {property} property on {Name}!");
            }
            
            MiscProperties.Remove(property);
        }

        /// <summary>
        /// Checks if the entity has a MiscProperty
        /// </summary>
        /// <param name="property">The MiscProperty to check</param>
        /// <returns>true if the entity has the MiscProperty, otherwise false</returns>
        public bool HasMiscProperty(MiscProperty property)
        {
            return MiscProperties.ContainsKey(property);
        }

        /// <summary>
        /// Gets the value of a MiscProperty the entity has.
        /// </summary>
        /// <param name="property">The MiscProperty to get the value for</param>
        /// <returns>A MiscValueHolder corresponding to the MiscProperty if it has an entry, otherwise a default MiscValueHolder</returns>
        public MiscValueHolder GetMiscProperty(MiscProperty property)
        {
            if (HasMiscProperty(property) == false)
            {
                return new MiscValueHolder();
            }

            return MiscProperties[property];
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
