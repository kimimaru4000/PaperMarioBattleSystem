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
    public abstract class BattleEntity : INameable, IUpdateable, IDrawable, ITintable
    {
        #region Delegates and Events

        public delegate void HealthStateChanged(HealthStates newHealthState);
        public event HealthStateChanged HealthStateChangedEvent = null;

        public delegate void PhaseCycleStarted();
        /// <summary>
        /// The event invoked at the start of each phase cycle. This is invoked after all Statuses take effect.
        /// <para>Use this for anything that doesn't need to occur before or after anything else.
        /// Status Effects don't use this event since they need to take effect in a specific order.</para>
        /// </summary>
        public event PhaseCycleStarted PhaseCycleStartEvent = null;

        #endregion

        /// <summary>
        /// Various unique properties belonging to the BattleEntity
        /// </summary>
        public readonly BattleEntityProperties EntityProperties = null;

        /// <summary>
        /// The BattleEntity's animation manager.
        /// </summary>
        public readonly ObjAnimManager AnimManager = null;

        /// <summary>
        /// The HeightState of the entity
        /// </summary>
        public HeightStates HeightState { get; private set; } = HeightStates.Grounded;

        /// <summary>
        /// The HealthState of the entity.
        /// This can apply to any entity, but only Mario and his Partners utilize Danger and Peril
        /// </summary>
        public HealthStates HealthState { get; private set; } = HealthStates.Normal;

        public Stats BattleStats { get; protected set; } = Stats.Default;
        public int CurHP => BattleStats.HP;
        public virtual int CurFP => BattleStats.FP;

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

        public Color TintColor { get; } = Color.White;

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
        public MoveAction PreviousAction { get; protected set; } = null;

        /// <summary>
        /// The BattleEntity targeting this one.
        /// </summary>
        public BattleEntity Targeter { get; private set; } = null;

        /// <summary>
        /// Tells whether the BattleEntity is being targeted.
        /// </summary>
        public bool IsTargeted => (Targeter != null);

        /// <summary>
        /// Tells whether the BattleEntity is in Danger or Peril
        /// </summary>
        public bool IsInDanger => (HealthState == HealthStates.Danger || HealthState == HealthStates.Peril);
        public bool IsDead => HealthState == HealthStates.Dead;
        public bool IsTurn => BattleManager.Instance.EntityTurn == this;

        public bool UsedTurn => (TurnsUsed >= MaxTurns || IsDead == true);

        protected readonly List<DefensiveAction> DefensiveActions = new List<DefensiveAction>();

        protected BattleEntity()
        {
            EntityProperties = new BattleEntityProperties(this);
            AnimManager = new ObjAnimManager(this);
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
            StatusChanceHolder[] statusesInflicted = damageResult.StatusesInflicted;

            //Handle the elemental interaction results
            ElementInteractionResult elementResult = damageResult.ElementResult;

            if (elementResult == ElementInteractionResult.Damage || elementResult == ElementInteractionResult.KO)
            {
                if (elementResult == ElementInteractionResult.Damage)
                {
                    Debug.Log($"{Name} was hit with {damage} {element} " + (piercing ? "piercing" : "non-piercing") + " damage!");

                    //If the entity took damage during their sequence, it's an interruption, and this event should not occur
                    if (damage > 0 && (IsTurn == false || PreviousAction?.MoveSequence.InSequence == false))
                    {
                        BattleEventManager.Instance.QueueBattleEvent((int)BattleGlobals.StartEventPriorities.Damage,
                            new BattleManager.BattleState[] { BattleManager.BattleState.Turn, BattleManager.BattleState.TurnEnd },
                            new DamagedBattleEvent(this));
                    }

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

            //Inflict Statuses if the entity isn't dead
            if (HealthState != HealthStates.Dead && statusesInflicted != null)
            {
                for (int i = 0; i < statusesInflicted.Length; i++)
                {
                    EntityProperties.AfflictStatus(statusesInflicted[i].Status);
                }
            }

            //Handle DamageEffects
            HandleDamageEffects(damageResult.DamageEffect);

            //If this entity received damage during its action sequence, it has been interrupted
            //The null check is necessary in the event that a StatusEffect that deals damage at the start of the phase, such as Poison,
            //is inflicted at the start of the battle before any entity has moved
            if (damage > 0 && IsTurn == true && PreviousAction?.MoveSequence.InSequence == true)
            {
                PreviousAction.MoveSequence.StartInterruption(element);
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
            TakeDamage(new InteractionHolder(null, damage, element, ElementInteractionResult.Damage, ContactTypes.None, piercing, null, true, DamageEffects.None));
        }

        public void HealHP(int hp)
        {
            BattleStats.HP = UtilityGlobals.Clamp(BattleStats.HP + hp, 0, BattleStats.MaxHP);

            UpdateHealthState();
            Debug.Log($"{Name} healed {hp} HP!");
        }

        public virtual void HealFP(int fp)
        {
            BattleStats.FP = UtilityGlobals.Clamp(BattleStats.FP + fp, 0, BattleStats.MaxFP);
            Debug.Log($"{Name} healed {fp} FP!");
        }

        public void LoseHP(int hp)
        {
            BattleStats.HP = UtilityGlobals.Clamp(BattleStats.HP - hp, 0, BattleStats.MaxHP);
            UpdateHealthState();

            if (IsDead == true)
            {
                Die();
            }

            Debug.Log($"{Name} took {hp} points of damage!");
        }

        public virtual void LoseFP(int fp)
        {
            BattleStats.FP = UtilityGlobals.Clamp(BattleStats.FP - fp, 0, BattleStats.MaxFP);
            Debug.Log($"{Name} lost {fp} FP!");
        }

        public void RaiseMaxHP(int hp)
        {
            BattleStats.MaxHP += hp;
            HealHP(0);
        }

        public void LowerMaxHP(int hp)
        {
            BattleStats.MaxHP -= hp;
            HealHP(0);
        }

        public virtual void RaiseMaxFP(int fp)
        {
            BattleStats.MaxFP += fp;
            HealFP(0);
        }

        public virtual void LowerMaxFP(int fp)
        {
            BattleStats.MaxFP -= fp;
            HealFP(0);
        }

        public void RaiseAttack(int attack)
        {
            BattleStats.Attack += attack;
        }

        public void RaiseDefense(int defense)
        {
            BattleStats.Defense += defense;
        }

        public void LowerAttack(int attack)
        {
            BattleStats.Attack -= attack;
        }

        public void LowerDefense(int defense)
        {
            BattleStats.Defense -= defense;
        }

        /// <summary>
        /// Adds an Accuracy modifier. 
        /// </summary>
        /// <param name="accuracyMod">The percentage change in decimal form. For example, .6 would indicate a 60% decrease in Accuracy.</param>
        public void AddAccuracyMod(double accuracyMod)
        {
            BattleStats.AccuracyModifiers.Add(accuracyMod);
            BattleStats.CalculateTotalAccuracyMod();

            Debug.Log($"Added accuracy mod of {accuracyMod}. Total accuracy mod is now: {BattleStats.AccuracyMod}");
        }

        /// <summary>
        /// Removes an Accuracy modifier.
        /// </summary>
        /// <param name="accuracyMod">The percentage change in decimal form. For example, .6 would indicate a 60% decrease in Accuracy.</param>
        public void RemoveAccuracyMod(double accuracyMod)
        {
            BattleStats.AccuracyModifiers.Remove(accuracyMod);
            BattleStats.CalculateTotalAccuracyMod();

            Debug.Log($"Removed accuracy mod of {accuracyMod}. Total accuracy mod is now: {BattleStats.AccuracyMod}");
        }

        /// <summary>
        /// Adds an Evasion modifier.
        /// </summary>
        /// <param name="evasionMod">The percentage change in decimal form. For example, .6 would indicate a 60% decrease in Evasion.</param>
        public void AddEvasionMod(double evasionMod)
        {
            BattleStats.EvasionModifiers.Add(evasionMod);
            BattleStats.CalculateTotalEvasionMod();

            Debug.Log($"Added evasion mod of {evasionMod}. Total evasion mod is now: {BattleStats.EvasionMod}");
        }

        /// <summary>
        /// Removes an Evasion modifier.
        /// </summary>
        /// <param name="evasionMod">The percentage change in decimal form. For example, .6 would indicate a 60% decrease in Evasion.</param>
        public void RemoveEvasionMod(double evasionMod)
        {
            BattleStats.EvasionModifiers.Remove(evasionMod);
            BattleStats.CalculateTotalEvasionMod();

            Debug.Log($"Removed evasion mod of {evasionMod}. Total evasion mod is now: {BattleStats.EvasionMod}");
        }

        /// <summary>
        /// Kills the entity instantly
        /// </summary>
        public void Die()
        {
            BattleStats.HP = 0;
            UpdateHealthState();
            //AnimManager.PlayAnimation(AnimationGlobals.DeathName, true);

            //NOTE: The death event occurs for standard enemies like Goombas during their sequence if it was interrupted
            //I'm not sure about bosses yet, so that'll need to be tested

            BattleEventManager.Instance.QueueBattleEvent((int)BattleGlobals.StartEventPriorities.Death,
                new BattleManager.BattleState[] { BattleManager.BattleState.Turn, BattleManager.BattleState.TurnEnd },
                new DeathBattleEvent(this));

            //Remove all StatusEffects on the entity
            EntityProperties.RemoveAllStatuses();

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
                HealthStateChangedEvent?.Invoke(newHealthState);

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
        /// Changes the HeightState of the BattleEntity.
        /// </summary>
        /// <param name="newHeightState">The new HeightState.</param>
        public void ChangeHeightState(HeightStates newHeightState)
        {
            HeightState = newHeightState;
        }

        /// <summary>
        /// Entity-specific logic for handling DamageEffects.
        /// </summary>
        /// <param name="damageEffects">The bit field of DamageEffects.</param>
        protected virtual void HandleDamageEffects(DamageEffects damageEffects)
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
        /// Checks if the entity's attempt to hit another entity is successful based on the entity's Accuracy and the victim's Evasion.
        /// </summary>
        /// <param name="victim">The entity trying to evade.</param>
        /// <returns>true if the entity hits and the victim doesn't evade, otherwise false.</returns>
        //NOTE: When dealing with Badges such as Close Call, we should compare the entity's Evasion first, then perform
        //the test again with the Badges' Evasion added in. If the Badges' Evasion bonus allows the entity to evade the attack,
        //that's when we'd play the "LUCKY" animation
        public bool AttemptHitEntity(BattleEntity victim)
        {
            return UtilityGlobals.TestRandomCondition(BattleStats.GetTotalAccuracy(), victim.BattleStats.GetTotalEvasion());
        }

        /// <summary>
        /// Gets the result of the first successful Defensive Action performed.
        /// </summary>
        /// <param name="damage">The original damage of the attack.</param>
        /// <param name="statusesInflicted">The original set of StatusEffects inflicted.</param>
        /// <param name="damageEffects">The original DamageEffects that would affect the BattleEntity.</param>
        /// <returns>A nullable DefensiveActionHolder? with a DefensiveAction's result if successful, otherwise null.</returns>
        public BattleGlobals.DefensiveActionHolder? GetDefensiveActionResult(int damage, StatusChanceHolder[] statusesInflicted, DamageEffects damageEffects)
        {
            //Handle Defensive Actions
            for (int i = 0; i < DefensiveActions.Count; i++)
            {
                if (DefensiveActions[i].IsSuccessful == true)
                {
                    BattleGlobals.DefensiveActionHolder holder = DefensiveActions[i].HandleSuccess(damage, statusesInflicted, damageEffects);
                    return holder;
                }
            }

            return null;
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

            StatusEffect[] statuses = EntityProperties.GetStatuses();
            for (int i = 0; i < statuses.Length; i++)
            {
                statuses[i].PhaseCycleStart();
            }

            //Invoke the event
            PhaseCycleStartEvent?.Invoke();
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

            //End all DefensiveAction inputs
            for (int i = 0; i < DefensiveActions.Count; i++)
            {
                DefensiveActions[i].actionCommand.EndInput();
            }
        }

        /// <summary>
        /// What happens when the entity's turn ends
        /// </summary>
        public virtual void OnTurnEnd()
        {
            //Start all DefensiveAction inputs
            for (int i = 0; i < DefensiveActions.Count; i++)
            {
                DefensiveActions[i].actionCommand.StartInput();
            }
        }

        /// <summary>
        /// Notifies the BattleManager to officially end the BattleEntity's turn
        /// </summary>
        public void EndTurn()
        {
            if (this != BattleManager.Instance.EntityTurn)
            {
                Debug.LogError($"Attempting to end the turn of {Name} when it's not their turn!");
                return;
            }

            //End the action
            PreviousAction?.OnActionEnded();

            TurnsUsed++;
            BattleManager.Instance.TurnEnd();
        }

        /// <summary>
        /// What happens during the entity's turn (choosing action commands, etc.)
        /// </summary>
        public virtual void TurnUpdate()
        {
            PreviousAction?.Update();
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

        #endregion

        public void SetBattlePosition(Vector2 battlePos)
        {
            BattlePosition = battlePos;
        }

        /// <summary>
        /// Determines if the BattleEntity gets affected by the Confused status, if it has it.
        /// If the BattleEntity is affected, it may perform a different action or target different entities with its original one.
        /// </summary>
        /// <param name="action">The BattleAction originally used.</param>
        /// <param name="targets">The original set of BattleEntities to target</param>
        /// <returns>An ActionHolder with a new BattleAction to perform or a different target list if the entity was affected by Confused.
        /// If not affected, the originals of each will be returned.</returns>
        private BattleGlobals.ActionHolder HandleConfusion(MoveAction action, params BattleEntity[] targets)
        {
            MoveAction actualAction = action;
            BattleEntity[] actualTargets = targets;

            //Check for Confusion's effects and change actions or targets depending on what happens
            int percent = EntityProperties.GetAdditionalProperty<int>(AdditionalProperty.ConfusionPercent);

            //See if Confusion should take effect
            if (UtilityGlobals.TestRandomCondition(percent) == true)
            {
                Debug.Log($"{Name} is affected by Confusion and will do something unpredictable!");

                //int changeAction = 0;
                //Check if the action can target entities to see if we should change targets
                //if (actualAction.MoveProperties.TargetsEntity == true)
                //{
                //    //Get the opposite type of entities to target
                //    //Items targets enemies, but attacks target allies
                //    EntityTypes oppositeType = actualAction.MoveProperties.EntityType;
                //    if (oppositeType == EntityTypes.Player) oppositeType = EntityTypes.Enemy;
                //    else if (oppositeType == EntityTypes.Enemy) oppositeType = EntityTypes.Player;
                //}

                int changeTargets = 0;

                //Don't give a chance to change targets if the action doesn't have targets
                //NOTE: This means we'll need to handle changing actions first, as not all actions target something
                if (targets != null && targets.Length > 0) changeTargets = GeneralGlobals.Randomizer.Next(0, 2);

                //NOTE: Find a way to make it so we can control what happens based on the type of action.
                //For example, if Tattle was used, it doesn't target anyone else.
                //If you use a Healing item, it uses it on the opposing side, whereas if you use an attack it uses it on an ally.

                //Change to an ally
                /*Steps:
                  1. If the action hits the first entity, find the adjacent allies. If not, find all allies
                  2. Filter by heights based on what the action can hit
                  3. Filter out dead allies
                  4. If the action hits everyone, go with the remaining list. Otherwise, choose a random ally to attack
                  5. If there are no allies to attack after all the filtering, make the entity do nothing*/
                if (changeTargets == 1)
                {
                    BattleEntity[] allies = null;

                    //If this action targets only the first player/enemy, look for adjacent allies
                    if (actualAction.MoveProperties.SelectionType == TargetSelectionMenu.EntitySelectionType.First)
                    {
                        Debug.Log($"{Name} is looking for valid adjacent allies to attack!");

                        //Find adjacent allies and filter out all non-ally entities
                        List<BattleEntity> adjacentEntities = new List<BattleEntity>(BattleManager.Instance.GetAdjacentEntities(this));
                        adjacentEntities.RemoveAll((adjacent) => adjacent.EntityType != EntityType);
                        allies = adjacentEntities.ToArray();
                    }
                    else
                    {
                        //Find all allies
                        allies = BattleManager.Instance.GetEntityAllies(this);
                    }

                    //Filter by heights
                    allies = BattleManager.Instance.FilterEntitiesByHeights(allies, actualAction.MoveProperties.HeightsAffected);

                    //Filter dead entities
                    allies = BattleManager.Instance.FilterDeadEntities(allies);

                    //Choose a random ally to attack if the action only targets one entity
                    if (allies.Length > 0 && actualAction.MoveProperties.SelectionType != TargetSelectionMenu.EntitySelectionType.All)
                    {
                        int randTarget = GeneralGlobals.Randomizer.Next(0, allies.Length);
                        allies = new BattleEntity[] { allies[randTarget] };

                        Debug.Log($"{Name} is choosing to attack ally {allies[0].Name} in Confusion!");
                    }

                    //Set the actual targets to be the set of allies
                    actualTargets = allies;

                    //If you can't attack any allies, do nothing
                    if (actualTargets.Length == 0)
                    {
                        actualAction = new NoAction();

                        Debug.Log($"{Name} did nothing as there either are no allies to attack or they're not in range!");
                    }
                    else
                    {
                        //Disable action commands when attacking allies from Confusion, if the action has an Action Command
                        if (actualAction.HasActionCommand == true)
                            actualAction.DisableActionCommand = true;
                    }
                }
            }

            return new BattleGlobals.ActionHolder(actualAction, actualTargets);
        }

        /// <summary>
        /// Makes the BattleEntity perform a MoveAction.
        /// The actual action performed may not be the one passed in if the BattleEntity is affected by Confusion.
        /// </summary>
        /// <param name="action">The MoveAction to perform.</param>
        /// <param name="targets">The BattleEntities the MoveAction targets.</param>
        public void StartAction(MoveAction action, params BattleEntity[] targets)
        {
            MoveAction actualAction = action;
            BattleEntity[] actualTargets = targets;

            //Check for Confused and handle it appropriately
            if (EntityProperties.HasAdditionalProperty(AdditionalProperty.ConfusionPercent) == true)
            {
                BattleGlobals.ActionHolder holder = HandleConfusion(action, targets);
                actualAction = holder.Action;
                actualTargets = holder.Targets;
            }

            PreviousAction = actualAction;

            //Start the action
            PreviousAction.OnActionStarted();
            PreviousAction.StartSequence(actualTargets);
        }

        /// <summary>
        /// Gets the idle animation of the BattleEntity based on its HealthState and the Status Effects it's inflicted with.
        /// </summary>
        /// <returns>A string with the idle animation name the BattleEntity has.</returns>
        public virtual string GetIdleAnim()
        {
            switch (HealthState)
            {
                case HealthStates.Dead: return AnimationGlobals.DeathName;
                default: return AnimationGlobals.IdleName;
            }
        }

        #region Equipment Methods

        /// <summary>
        /// Gets the number of Badges of a particular BadgeType that the BattleEntity has equipped.
        /// </summary>
        /// <param name="badgeType">The BadgeType to check for.</param>
        /// <returns>The number of Badges of the BadgeType that the BattleEntity has equipped.</returns>
        public abstract int GetEquippedBadgeCount(BadgeGlobals.BadgeTypes badgeType);

        #endregion

        /// <summary>
        /// Targets this BattleEntity for a move.
        /// This prevents it from doing certain things, like playing the death animation, until it is no longer targeted.
        /// </summary>
        /// <param name="attacker">The BattleEntity targeting this one for an attack.</param>
        public void TargetForMove(BattleEntity attacker)
        {
            Targeter = attacker;
        }

        /// <summary>
        /// Tells the BattleEntity that it is no longer being targeted.
        /// </summary>
        public void StopTarget()
        {
            Targeter = null;
        }

        /// <summary>
        /// Used for update logic that applies to the entity regardless of whether it is its turn or not
        /// </summary>
        public void Update()
        {
            AnimManager.CurrentAnim?.Update();

            //Update Defensive actions
            for (int i = 0; i < DefensiveActions.Count; i++)
            {
                DefensiveActions[i].Update();
            }
        }

        public virtual void Draw()
        {
            AnimManager.CurrentAnim?.Draw(Position, TintColor, EntityType != EntityTypes.Enemy, .1f);
            PreviousAction?.Draw();

            //Draw Status Effect icons on the BattleEntity
            //You can't see the icons unless it's Mario or his Partner's turn and they're not in a Sequence
            if (BattleManager.Instance.EntityTurn.EntityType == EntityTypes.Player
                && BattleManager.Instance.EntityTurn.PreviousAction?.MoveSequence.InSequence != true)
            {
                Vector2 statusIconPos = new Vector2(Position.X + 10, Position.Y - 40);
                StatusEffect[] statuses = EntityProperties.GetStatuses();
                int index = 0;

                for (int i = 0; i < statuses.Length; i++)
                {
                    StatusEffect status = statuses[i];
                    CroppedTexture2D texture = status.StatusIcon;

                    //Don't draw the status if it doesn't have an icon
                    if (texture == null || texture.Tex == null)
                    {
                        continue;
                    }

                    float yOffset = ((index + 1) * StatusGlobals.IconYOffset);
                    Vector2 iconPos = Camera.Instance.SpriteToUIPos(new Vector2(statusIconPos.X, statusIconPos.Y - yOffset));

                    float depth = .35f - (index * .01f);
                    float turnStringDepth = depth + .0001f;

                    status.DrawStatusInfo(iconPos, depth, turnStringDepth);

                    index++;
                }
            }
        }
    }
}
