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
        protected readonly Dictionary<PhysicalAttributes, int> PhysAttributes = new Dictionary<PhysicalAttributes, int>();

        /// <summary>
        /// The exceptions this entity has for certain types of contact against certain PhysicalAttributes.
        /// <para>This is used for Badges such as Spike Shield and Ice Power.</para>
        /// </summary>
        protected readonly Dictionary<ContactTypes, List<PhysicalAttributes>> ContactExceptions = new Dictionary<ContactTypes, List<PhysicalAttributes>>();

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
        /// Properties relating to how the entity is affected by each StatusEffect.
        /// Examples include the likelihood of being inflicted by a StatusEffect-inducing attack and
        /// how many additional turns the entity is affected by the StatusEffect
        /// </summary>
        protected readonly Dictionary<StatusTypes, StatusPropertyHolder> StatusProperties = new Dictionary<StatusTypes, StatusPropertyHolder>();

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
        /*This is how Paper Mario: The Thousand Year Door calculates damage:
        1. Start with base attack
        2. Subtract damage from Defend Plus, Defend Command, and any additional Defense
        3. Subtract or Add from P-Down D-up and P-Up D-Down
        4. Reduce damage to 0 if superguarded. Reduce by 1 + each Damage Dodge if guarded
        5. Multiply by the number of Double Pains + 1
        6. Divide by the number of Last Stands + 1 (if in danger)
        
        Therefore, two Double Pains = Triple Pain.
        Max Damage is 99.*/

        public void TakeDamage(InteractionHolder damageResult)
        {
            Elements element = damageResult.DamageElement;
            int damage = damageResult.TotalDamage;
            bool piercing = damageResult.Piercing;
            StatusEffect[] statusesInflicted = damageResult.StatusesInflicted;

            //Subtract Defense on non-piercing damage
            //NOTE: Don't do this here, do it in Interactions according to the order in the comments above (Step 2)
            if (piercing == false)
            {
                damage = UtilityGlobals.Clamp(damage - BattleStats.Defense, BattleGlobals.MinDamage, BattleGlobals.MaxDamage);
            }

            //Check for a damage received multiplier on the entity. We need to check if it has one since the default value is 0
            //NOTE: Don't do this here, do it in Interactions according to the order in the comments above (Steps 5/6)
            if (HasMiscProperty(MiscProperty.DamageReceivedMultiplier) == true)
            {
                damage *= GetMiscProperty(MiscProperty.DamageReceivedMultiplier).IntValue;
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
                    Debug.Log($"{Name} was hit with {damage} {element} " + (piercing ? "piercing" : "non-piercing") + " damage!");

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
                Debug.Log($"{Name} was healed for {damage} HP because it has a {nameof(ResistanceTypes.Heal)} resistance to Element {element}");

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
            BattleStats.Accuracy = UtilityGlobals.Clamp(BattleStats.Accuracy + accuracy, int.MinValue, int.MaxValue);
        }

        public void ModifyEvasion(int evasion)
        {
            BattleStats.Evasion = UtilityGlobals.Clamp(BattleStats.Evasion + evasion, int.MinValue, int.MaxValue);
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

        /// <summary>
        /// What occurs when the battle is started for the entity.
        /// </summary>
        public virtual void OnBattleStart()
        {
            
        }

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

        #region Animation Methods

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

        #endregion

        #region Phys Attribute Methods

        /// <summary>
        /// Adds a physical attribute to the entity
        /// </summary>
        /// <param name="physicalAttribute">The physical attribute to add</param>
        public void AddPhysAttribute(PhysicalAttributes physicalAttribute)
        {
            if (PhysAttributes.ContainsKey(physicalAttribute) == true)
            {
                PhysAttributes[physicalAttribute]++;
                Debug.Log($"Incremented the physical attribute {physicalAttribute} for {Name}!");
                return;
            }
            else
            {
                Debug.Log($"Added the physical attribute {physicalAttribute} to {Name}'s existing attributes!");
                PhysAttributes.Add(physicalAttribute, 1);
            }
        }

        /// <summary>
        /// Removes a physical attribute from the entity
        /// </summary>
        /// <param name="physicalAttribute">The physical attribute to remove</param>
        public void RemovePhysAttribute(PhysicalAttributes physicalAttribute)
        {
            if (PhysAttributes.ContainsKey(physicalAttribute) == false)
            {
                Debug.LogWarning($"Cannot remove physical attribute {physicalAttribute} because {Name} does not have it!");
                return;
            }

            

            PhysAttributes[physicalAttribute]--;
            if (PhysAttributes[physicalAttribute] <= 0)
            {
                PhysAttributes.Remove(physicalAttribute);
                Debug.Log($"Removed the physical attribute {physicalAttribute} from {Name}'s existing attributes!");
            }
            else
            {
                Debug.Log($"Decremented the physical attribute {physicalAttribute} for {Name}!");
            }
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
            return Interactions.GetContactResult(attacker, contactType, PhysAttributes.Keys.ToArray(), attacker.GetContactExceptions(contactType));
        }

        #endregion

        #region Contact Exception Methods

        /// <summary>
        /// Adds a contact exception to the entity
        /// </summary>
        /// <param name="contactType"></param>
        /// <param name="physAttribute"></param>
        public void AddContactException(ContactTypes contactType, PhysicalAttributes physAttribute)
        {
            //Add a new key if one doesn't exist
            if (ContactExceptions.ContainsKey(contactType) == false)
            {
                ContactExceptions.Add(contactType, new List<PhysicalAttributes>());
            }

            //Add to the list
            ContactExceptions[contactType].Add(physAttribute);

            Debug.Log($"Added contact exception on {Name} for the {physAttribute} PhysicalAttribute during {contactType} contact!");
        }

        public void RemoveContactException(ContactTypes contactType, PhysicalAttributes physAttribute)
        {
            if (ContactExceptions.ContainsKey(contactType) == false)
            {
                Debug.LogError($"Cannot remove {physAttribute} from the exception list on {Name} for {contactType} because no list exists!");
                return;
            }

            bool removed = ContactExceptions[contactType].Remove(physAttribute);
            if (removed == true)
            {
                Debug.Log($"Removed {physAttribute} attribute exception on {Name} for {contactType} contact!");
            }

            //If there are no PhysicalAttributes in the exceptions list for this ContactType, remove the key
            if (ContactExceptions[contactType].Count == 0)
            {
                ContactExceptions.Remove(contactType);
            }
        }

        /// <summary>
        /// Returns a set of PhysicalAttributes to ignore when the BattleEntity makes contact
        /// </summary>
        /// <param name="contactType">The type of contact this BattleEntity made</param>
        /// <returns>An array of PhysicalAttributes this BattleEntity can ignore when making contact, otherwise an empty array</returns>
        public PhysicalAttributes[] GetContactExceptions(ContactTypes contactType)
        {
            //Return an empty array if no exceptions exist for this type of contact
            if (ContactExceptions.ContainsKey(contactType) == false)
            {
                return new PhysicalAttributes[0];
            }

            return ContactExceptions[contactType].ToArray();
        }

        #endregion

        #region Weakness/Resistance Methods

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

        #endregion

        #region Status Effect Methods

        /// <summary>
        /// Attempts to afflict the entity with a StatusEffect, based on its properties and status percentage for the StatusEffect
        /// </summary>
        /// <param name="status">The StatusEffect to afflict the entity with</param>
        /// <returns>true if the StatusEffect was successfully afflicted, false otherwise</returns>
        public bool TryAfflictStatus(StatusEffect status)
        {
            //Test for StatusEffect immunity - if the entity is immune to a particular alignment, don't allow the StatusEffect to be inflicted
            bool positiveStatusImmune = GetMiscProperty(MiscProperty.PositiveStatusImmune).BoolValue;
            bool negativeStatusImmune = GetMiscProperty(MiscProperty.NegativeStatusImmune).BoolValue;
            if ((status.Alignment == StatusEffect.StatusAlignments.Positive && positiveStatusImmune == true)
                || (status.Alignment == StatusEffect.StatusAlignments.Negative && negativeStatusImmune == true))
            {
                return false;
            }

            StatusPropertyHolder statusProperty = GetStatusProperty(status.StatusType);

            //Test the percentage
            int percentage = statusProperty.StatusPercentage;
            int valueTest = GeneralGlobals.Randomizer.Next(1, 101);

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

            Debug.LogWarning($"Afflicted {Name} with the {newStatus.StatusType} Status for {newStatus.TotalDuration} turns!");
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

            Debug.LogWarning($"Removed the {statusType} Status on {Name} after being inflicted for {status.TotalDuration} turns!");
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
        /// Adds a StatusProperty for a particular StatusEffect to the entity.
        /// If a StatusProperty already exists for a StatusEffect, it will be replaced.
        /// </summary>
        /// <param name="statusType">The StatusType of the StatusEffect.</param>
        /// <param name="statusProperty">The StatusPropertyHolder associated with the StatusEffect.</param>
        protected void AddStatusProperty(StatusTypes statusType, StatusPropertyHolder statusProperty)
        {
            if (StatusProperties.ContainsKey(statusType) == true)
            {
                Debug.Log($"Replacing {nameof(StatusPropertyHolder)} for the {statusType} Status as {Name} already has one!");
                StatusProperties.Remove(statusType);
            }

            StatusProperties.Add(statusType, statusProperty);
        }

        /// <summary>
        /// Tells if the entity has a StatusPropertyHolder associated with a particular StatusEffect or not
        /// </summary>
        /// <param name="statusType">The StatusType of the StatusEffect</param>
        /// <returns>true if a StatusPropertyHolder can be found for the specified StatusType, false otherwise</returns>
        protected bool HasStatusProperty(StatusTypes statusType)
        {
            return StatusProperties.ContainsKey(statusType);
        }

        /// <summary>
        /// Retrieves the StatusPropertyHolder associated with a particular StatusEffect
        /// </summary>
        /// <param name="statusType">The StatusType of the StatusEffect</param>
        /// <returns>The StatusPropertyHolder corresponding to the specified StatusType. If there is no entry, returns a default one</returns>
        public StatusPropertyHolder GetStatusProperty(StatusTypes statusType)
        {
            if (HasStatusProperty(statusType) == false)
            {
                return StatusPropertyHolder.Default;
            }

            return StatusProperties[statusType];
        }

        #endregion

        #region Misc Property Methods

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

        #endregion

        #region Equipment Methods

        

        #endregion

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
