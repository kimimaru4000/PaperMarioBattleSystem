using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static PaperMarioBattleSystem.Enumerations;
using static PaperMarioBattleSystem.AnimationGlobals;
using PaperMarioBattleSystem.Utilities;
using PaperMarioBattleSystem.Extensions;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// Any fighter that takes part in battle.
    /// </summary>
    public abstract class BattleEntity : INameable, IUpdateable, IDrawable, IPosition, IRotatable, IScalable, ITintable, ICleanup
    {
        #region Delegates and Events

        public delegate void HealthStateChanged(HealthStates newHealthState);
        /// <summary>
        /// The event invoked when the BattleEntity's HealthState is changed. This occurs immediately before actually changing the HealthState.
        /// <para>This event won't be invoked if the BattleEntity's HealthState is set to the same one it currently has.</para>
        /// </summary>
        public event HealthStateChanged HealthStateChangedEvent = null;

        public delegate void PhaseCycleStarted();
        /// <summary>
        /// The event invoked at the start of each phase cycle. This is invoked after all Statuses take effect.
        /// <para>Use this for anything that doesn't need to occur before or after anything else.
        /// Status Effects don't use this event since they need to take effect in a specific order.</para>
        /// </summary>
        public event PhaseCycleStarted PhaseCycleStartEvent = null;

        public delegate void TurnStarted();
        /// <summary>
        /// The event invoked at the start of the BattleEntity's turn. This is invoked after all other logic when the turn starts.
        /// </summary>
        public event TurnStarted TurnStartEvent = null;

        public delegate void TurnEnded();
        /// <summary>
        /// The event invoked at the end of the BattleEntity's turn.
        /// </summary>
        public event TurnEnded TurnEndEvent = null;

        public delegate void DamageTaken(in InteractionHolder damageInfo);
        /// <summary>
        /// The event invoked when the BattleEntity takes damage. This is invoked after all other logic when taking damage.
        /// </summary>
        public event DamageTaken DamageTakenEvent = null;

        public delegate void DealtDamage(in InteractionHolder damageInfo);
        /// <summary>
        /// The event invoked when the BattleEntity damages another BattleEntity.
        /// This is invoked after all other logic when damaging the BattleEntity.
        /// </summary>
        public event DealtDamage DealtDamageEvent = null;

        public delegate void ChangedBattleManager(in BattleManager prevBattleManager, in BattleManager newBattleManager);
        /// <summary>
        /// The event invoked when the BattleEntity changes its BattleManager.
        /// This is invoked after the new BattleManager is set.
        /// </summary>
        public event ChangedBattleManager ChangedBattleManagerEvent = null;

        #endregion

        #region Confusion Handlers

        /// <summary>
        /// A delegate handling how the BattleEntity gets affected by the Confused status.
        /// The BattleEntity may perform a different action and/or target different entities.
        /// </summary>
        /// <param name="action">The MoveAction originally used.</param>
        /// <param name="targets">The original set of BattleEntities to target.</param>
        /// <returns>An ActionHolder with a new BattleAction to perform and/or a different target list.</returns>
        protected delegate BattleGlobals.ActionHolder ConfusionDelegate(MoveAction action, BattleEntity[] targets);

        /// <summary>
        /// The handler to use when determining Confusion's effects.
        /// This allows BattleEntities to handle Confusion however they want.
        /// It defaults to <see cref="BaseConfusionHandler(MoveAction, BattleEntity[])"/>.
        /// </summary>
        protected ConfusionDelegate ConfusionHandler = null;

        #endregion

        /// <summary>
        /// The BattleManager the BattleEntity belongs to.
        /// </summary>
        public BattleManager BManager { get; private set; } = null;

        /// <summary>
        /// Various unique properties belonging to the BattleEntity.
        /// </summary>
        public BattleEntityProperties EntityProperties { get; private set; } = null;

        /// <summary>
        /// The BattleEntity's animation manager.
        /// </summary>
        public ObjAnimManager AnimManager { get; private set; } = null;

        /// <summary>
        /// The HeightState of the BattleEntity.
        /// </summary>
        public HeightStates HeightState { get; private set; } = HeightStates.Grounded;

        /// <summary>
        /// The HealthState of the BattleEntity.
        /// This can apply to any BattleEntity, but only Mario and his Partners utilize Danger and Peril.
        /// </summary>
        public HealthStates HealthState { get; private set; } = HealthStates.Normal;

        public Stats BattleStats { get; protected set; } = Stats.Default;
        public int CurHP => BattleStats.HP;
        public virtual int CurFP => BattleStats.FP;

        /// <summary>
        /// The base number of turns the BattleEntity has in each of its phases.
        /// </summary>
        public int BaseTurns { get; protected set; } = BattleGlobals.DefaultTurnCount;

        /// <summary>
        /// The number of turns the BattleEntity used in this phase.
        /// </summary>
        public int TurnsUsed { get; protected set; } = 0;

        /// <summary>
        /// The current max number of turns the BattleEntity has in this phase.
        /// </summary>
        public int MaxTurns { get; protected set; } = BattleGlobals.DefaultTurnCount;

        public string Name { get; set; } = "Entity";

        public Color TintColor { get; set;  } = Color.White;

        /// <summary>
        /// The BattleEntity's battle index, assigned from the BattleManager.
        /// If it's less than or equal to <see cref="BattleGlobals.InvalidBattleIndex"/>, the BattleEntity is not considered in battle.
        /// <para>This indicates its relation to BattleEntities of the same EntityType.
        /// BattleEntities with higher battle indices are behind ones with lower battle indices.</para>
        /// <para>In most cases, it also corresponds to its position from the left side of the stage and
        /// its reference index in its entity list.</para>
        /// </summary>
        public int BattleIndex { get; private set; } = BattleGlobals.InvalidBattleIndex;

        /// <summary>
        /// The BattleEntity's current position.
        /// </summary>
        public Vector2 Position { get; set; } = Vector2.Zero;

        /// <summary>
        /// The BattleEntity's battle position. The BattleEntity goes back to this after each action.
        /// </summary>
        public Vector2 BattlePosition { get; protected set; } = Vector2.Zero;

        public float Rotation { get; set; } = 0f;
        public Vector2 Scale { get; set; } = Vector2.One;
        public bool SpriteFlip { get; set; } = false;
        public float Layer { get; set; } = .1f;
        public readonly Vector2 Origin = new Vector2(.5f, .5f);

        public EntityTypes EntityType { get; protected set; } = EntityTypes.Enemy;

        /// <summary>
        /// The last MoveAction the BattleEntity performed.
        /// </summary>
        public MoveAction LastAction { get; protected set; } = null;

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
        public bool IsTurn => (BManager.EntityTurn == this);

        public bool UsedTurn => (TurnsUsed >= MaxTurns || IsDead == true);

        /// <summary>
        /// Tells if the BattleEntity is currently in battle.
        /// </summary>
        public bool IsInBattle => (BManager != null && BattleGlobals.IsValidBattleIndex(BattleIndex));

        protected readonly List<DefensiveAction> DefensiveActions = new List<DefensiveAction>();

        private BattleEntity()
        {
            SetEntityProperties(new BattleEntityProperties(this));
            AnimManager = new ObjAnimManager(this);
            ConfusionHandler = BaseConfusionHandler;
        }

        protected BattleEntity(Stats stats) : this()
        {
            BattleStats = stats;
            UpdateHealthState();
        }

        public virtual void CleanUp()
        {
            SetBattleIndex(BattleGlobals.InvalidBattleIndex, false);

            LastAction?.SetUser(null);
            LastAction = null;

            EntityProperties.CleanUp();

            SetBattleManager(null);

            HealthStateChangedEvent = null;
            PhaseCycleStartEvent = null;
            TurnStartEvent = null;
            TurnEndEvent = null;
            DamageTakenEvent = null;
            DealtDamageEvent = null;
            ChangedBattleManagerEvent = null;
        }

        /// <summary>
        /// Loads a BattleEntity's animations.
        /// <para>This is public to allow for easier debugging through reloading animations.</para>
        /// </summary>
        public virtual void LoadAnimations()
        {

        }

        /// <summary>
        /// Sets the properties of this BattleEntity.
        /// </summary>
        /// <remarks>By default, all BattleEntities have a new set of properties, which suffices for the vast majority of cases.
        /// However, there may be some situations that require referencing another set of properties.
        /// For example, target points sharing properties with the main entity, or swapping among sets of properties without losing information.
        /// </remarks>
        /// <param name="entityProperties">The BattleEntityProperties to set.</param>
        protected void SetEntityProperties(BattleEntityProperties entityProperties)
        {
            EntityProperties = entityProperties;
        }

        #region Damage Handling

        /*
         * Damage handling methods:
         * -The virtual methods are called in order in TakeDamage()
         * -BattleEntities can override any steps of receiving damage on their end
         * -The base behavior should work just fine for the large majority of BattleEntities
         */

        /// <summary>
        /// Tells this BattleEntity to damage another one.
        /// This method is a wrapper that calls the <see cref="DealtDamageEvent"/> for this BattleEntity.
        /// This should only be called if damage actually hits.
        /// <para>This also is called when dealing Payback damage.</para>
        /// </summary>
        /// <param name="damageInfo">The InteractionHolder containing the BattleEntity to damage and all other interaction data.</param>
        public void DamageEntity(in InteractionHolder damageInfo)
        {
            damageInfo.Entity.TakeDamage(damageInfo);

            //Invoke the event
            DealtDamageEvent?.Invoke(damageInfo);
        }

        /// <summary>
        /// Handles the <see cref="ElementInteractionResult"/> received from being damaged.
        /// <para>The base behavior deals damage for <see cref="ElementInteractionResult.Damage"/>,
        /// KOs for <see cref="ElementInteractionResult.KO"/>,
        /// and heals for <see cref="ElementInteractionResult.Heal"/>.</para>
        /// </summary>
        /// <param name="damageResult">The InteractionHolder containing the result of a damage interaction.</param>
        protected virtual void HandleDamageResult(in InteractionHolder damageResult)
        {
            Elements element = damageResult.DamageElement;
            int damage = damageResult.TotalDamage;
            bool piercing = damageResult.Piercing;

            //Handle the elemental interaction results
            ElementInteractionResult elementResult = damageResult.ElementResult;

            if (elementResult == ElementInteractionResult.Damage || elementResult == ElementInteractionResult.KO)
            {
                if (elementResult == ElementInteractionResult.Damage)
                {
                    Debug.Log($"{Name} was hit with {damage} {element} " + (piercing ? "piercing" : "non-piercing") + " damage!");

                    //If the entity took damage during their sequence, it's an interruption, and this event should not occur
                    if (damage > 0 && (IsTurn == false || LastAction?.MoveSequence.InSequence == false))
                    {
                        BManager.battleEventManager.QueueBattleEvent((int)BattleGlobals.BattleEventPriorities.Damage,
                            new BattleGlobals.BattleState[] { BattleGlobals.BattleState.Turn, BattleGlobals.BattleState.TurnEnd },
                            new DamagedBattleEvent(this));

                        //Play the damaged sound
                        SoundManager.Instance.PlaySound(SoundManager.Sound.Damaged);
                    }
                    else if (damage <= 0)
                    {
                        //Play the immune sound
                        SoundManager.Instance.PlaySound(SoundManager.Sound.Immune);
                    }

                    //Show the star indicating damage
                    BManager.battleObjManager.AddBattleObject(new DamageStarVFX(damage, Position + (EntityType == EntityTypes.Player ? new Vector2(-40, -35) : new Vector2(50, -35))));

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
        }

        /// <summary>
        /// Handles inflicting Status Effects on the BattleEntity after receiving damage.
        /// </summary>
        /// <param name="damageResult">The InteractionHolder containing the result of a damage interaction.</param>
        protected virtual void HandleStatusAffliction(in InteractionHolder damageResult)
        {
            StatusChanceHolder[] statusesInflicted = damageResult.StatusesInflicted;

            //Inflict Statuses if the entity isn't dead
            if (IsDead == false && statusesInflicted != null)
            {
                for (int i = 0; i < statusesInflicted.Length; i++)
                {
                    EntityProperties.AfflictStatus(statusesInflicted[i].Status);
                }
            }
        }

        /// <summary>
        /// BattleEntity-specific logic for handling DamageEffects.
        /// </summary>
        /// <param name="damageEffects">The bit field of DamageEffects.</param>
        protected virtual void HandleDamageEffects(DamageEffects damageEffects)
        {

        }

        /// <summary>
        /// Makes the BattleEntity take damage from an attack.
        /// </summary>
        /// <param name="damageResult">The InteractionHolder containing the result of a damage interaction.</param>
        public void TakeDamage(in InteractionHolder damageResult)
        {
            int damage = damageResult.TotalDamage;
            Elements element = damageResult.DamageElement;

            //Handle being damaged
            HandleDamageResult(damageResult);

            //Handle afflicting statuses
            HandleStatusAffliction(damageResult);

            //Handle DamageEffects
            HandleDamageEffects(damageResult.DamageEffect);

            //If this entity received damage during its action sequence, it has been interrupted
            //The null check is necessary in the event that a StatusEffect that deals damage at the start of the phase, such as Poison,
            //is inflicted at the start of the battle before any entity has moved
            if (damage > 0 && IsTurn == true && LastAction?.MoveSequence.InSequence == true)
            {
                LastAction.MoveSequence.StartInterruption(element);
            }

            //Perform entity-specific logic to react to taking damage
            OnTakeDamage(damageResult);

            //Invoke the damage taken event
            DamageTakenEvent?.Invoke(damageResult);
        }

        /// <summary>
        /// Makes the BattleEntity take damage from an attack.
        /// <para>This method is a shorthand for inflicting simple damage such as Poison damage every turn from a StatusEffect.</para>
        /// </summary>
        /// <param name="element">The element to damage the entity with</param>
        /// <param name="damage">The damage to deal to the entity</param>
        /// <param name="piercing">Whether the attack penetrates Defense or not</param>
        public void TakeDamage(Elements element, int damage, bool piercing)
        {
            TakeDamage(new InteractionHolder(this, damage, element, ElementInteractionResult.Damage, ContactTypes.None, ContactProperties.Ranged, piercing, null, true, DamageEffects.None));
        }

        /// <summary>
        /// Performs BattleEntity-specific logic when taking damage.
        /// This is called after damage has been dealt but before the damage taken event.
        /// </summary>
        /// <param name="damageInfo">The InteractionHolder containing the BattleEntity to damage and all other interaction data.</param>
        protected virtual void OnTakeDamage(in InteractionHolder damageInfo)
        {

        }

        #endregion

        #region Stat Manipulations

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
        /// Kills the BattleEntity instantly.
        /// </summary>
        public void Die()
        {
            BattleStats.HP = 0;
            UpdateHealthState();

            //Remove all StatusEffects on the entity
            EntityProperties.RemoveAllStatuses();

            OnDeath();

            //NOTE: The death event occurs for standard enemies like Goombas during their sequence if it was interrupted
            //I'm not sure about bosses yet, so that'll need to be tested

            //Start the event only if the BattleEntity is in battle
            //If the BattleEntity isn't in battle, there's no way to start the event, so ignore it
            if (IsInBattle == true)
            {
                BManager.battleEventManager.QueueBattleEvent((int)BattleGlobals.BattleEventPriorities.Death,
                    new BattleGlobals.BattleState[] { BattleGlobals.BattleState.Turn, BattleGlobals.BattleState.TurnEnd },
                    new DeathBattleEvent(this, IsInBattle == false, EntityType == EntityTypes.Enemy));
            }
        }

        /// <summary>
        /// Performs BattleEntity-specific logic on death.
        /// </summary>
        protected virtual void OnDeath()
        {
            Debug.Log($"{Name} has been defeated!");
        }

        /// <summary>
        /// Updates the BattleEntity's health state based on its current HP.
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
        /// What occurs when an BattleEntity changes HealthStates
        /// </summary>
        /// <param name="newHealthState">The new HealthState of the BattleEntity.</param>
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

        #endregion

        /// <summary>
        /// What occurs when the BattleEntity enters a battle.
        /// <para>This is called when the BattleEntity is added to a battle.</para>
        /// </summary>
        public virtual void OnEnteredBattle()
        {
            //By default, set the direction the BattleEntity is facing to be right for players and left for everything else
            SpriteFlip = (EntityType == EntityTypes.Player);
        }

        #region Damage Related

        /// <summary>
        /// Checks if the BattleEntity's attempt to hit another BattleEntity is successful based on the BattleEntity's Accuracy and the victim's Evasion.
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
        /// <param name="defensiveOverrides">The types of Defensive Actions to override.</param>
        /// <returns>A nullable DefensiveActionHolder? with a DefensiveAction's result if successful, otherwise null.</returns>
        public BattleGlobals.DefensiveActionHolder? GetDefensiveActionResult(int damage, StatusChanceHolder[] statusesInflicted, DamageEffects damageEffects,
            DefensiveActionTypes defensiveOverrides)
        {
            //Handle Defensive Actions
            for (int i = 0; i < DefensiveActions.Count; i++)
            {
                //Check if there are any overrides for this type of Defensive Action
                if (defensiveOverrides != DefensiveActionTypes.None
                    && UtilityGlobals.DefensiveActionTypesHasFlag(defensiveOverrides, DefensiveActions[i].DefensiveActionType))
                {
                    Debug.Log($"{defensiveOverrides} overrode {DefensiveActions[i].DefensiveActionType}!");
                    continue;
                }

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
        /// What happens at the start of the phase cycle.
        /// </summary>
        public virtual void OnPhaseCycleStart()
        {
            TurnsUsed = 0;
            MaxTurns = BaseTurns;

            //If the BattleEntity dies before the last Status Effect and clears all statuses, the ones that follow will have a null entity reference
            //Status Effects that alter turn count are unaffected, so if the BattleEntity gets revived with a Life Shroom it will still move this turn
            //This is how it works in TTYD. For our purposes, simply break when encountering a null reference to replicate this behavior
            StatusEffect[] statuses = EntityProperties.GetStatuses();
            for (int i = 0; i < statuses.Length; i++)
            {
                //Break on null; this indicates the BattleEntity died and all Status Effects on it were removed partway into this loop
                if (statuses[i].EntityAfflicted == null) break;

                statuses[i].PhaseCycleStart();
            }

            //Invoke the event
            PhaseCycleStartEvent?.Invoke();
        }

        /// <summary>
        /// What happens at the start of the BattleEntity's phase.
        /// </summary>
        public virtual void OnPhaseStart()
        {
            Debug.Log($"Started phase for {Name}!");
        }

        /// <summary>
        /// What happens at the end of the BattleEntity's phase.
        /// </summary>
        public virtual void OnPhaseEnd()
        {
            Debug.Log($"Ended phase for {Name}");

            //Start all DefensiveAction inputs when the BattleEntity's phase ends
            for (int i = 0; i < DefensiveActions.Count; i++)
            {
                DefensiveActions[i].actionCommand.StartInput();
            }
        }

        /// <summary>
        /// Officially starts the BattleEntity's turn.
        /// </summary>
        public void StartTurn()
        {
            OnTurnStart();

            //Invoke the event
            TurnStartEvent?.Invoke();
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
        /// Officially ends the BattleEntity's turn.
        /// </summary>
        public void EndTurn()
        {
            if (IsTurn == false)
            {
                Debug.LogError($"Attempting to end the turn of {Name} when it's not their turn!");
                return;
            }

            //End the action
            LastAction?.OnActionEnded();

            OnTurnEnd();

            //Invoke the event
            TurnEndEvent?.Invoke();

            //Increment the number of turns
            TurnsUsed++;
        }

        /// <summary>
        /// What happens during the entity's turn (choosing action commands, etc.)
        /// </summary>
        public virtual void TurnUpdate()
        {
            LastAction?.Update();
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

        /// <summary>
        /// Sets the BattleManager the BattleEntity belongs to.
        /// This is typically only called by BattleManagers themselves when adding and removing BattleEntities.
        /// <para>When transferring BattleEntities between battles, make sure to remove them from one battle before adding them to the other.
        /// If a BattleEntity technically stays in battle but is inactive (Ex. Partners), it's recommended to keep the BattleManager reference.</para>
        /// </summary>
        /// <param name="bManager">The BattleManager the BattleEntity is apart of.</param>
        public void SetBattleManager(BattleManager bManager)
        {
            BattleManager prevBManager = BManager;
            BManager = bManager;

            ChangedBattleManagerEvent?.Invoke(prevBManager, BManager);
        }

        /// <summary>
        /// Sets the BattleIndex for the BattleEntity.
        /// </summary>
        /// <param name="battleIndex">The new BattleIndex of the BattleEntity.</param>
        /// <param name="autoSort">If true, will tell the BattleManager to sort all BattleEntities of this type by BattleIndex.
        /// If false, make sure to sort manually to prevent mismatched data.</param>
        public void SetBattleIndex(int battleIndex, bool autoSort = true)
        {
            BattleIndex = battleIndex;

            //Tell the BattleManager to sort the list for this type of BattleEntity
            if (autoSort == true)
            {
                BManager.SortEntityList(EntityType);
            }
        }

        public void SetBattlePosition(Vector2 battlePos)
        {
            BattlePosition = battlePos;
        }

        /// <summary>
        /// The base confusion handler.
        /// It covers a broad set of actions, targeting allies if the action deals damage and targeting enemies if the action heals.
        /// </summary>
        /// <param name="action">The BattleAction originally used.</param>
        /// <param name="targets">The original set of BattleEntities to target</param>
        /// <returns>An ActionHolder with a new BattleAction to perform or a different target list if the BattleEntity was affected by Confused.
        /// If not affected, the originals of each will be returned.</returns>
        protected BattleGlobals.ActionHolder BaseConfusionHandler(MoveAction action, BattleEntity[] targets)
        {
            MoveAction actualAction = action;
            BattleEntity[] actualTargets = targets;

            int changeTargets = 0;

            //Don't give a chance to change targets if the action doesn't have targets
            if (targets != null && targets.Length > 0)
            {
                changeTargets = RandomGlobals.Randomizer.Next(0, 2);
            }
            
            //Custom can target anything and may have a wide range of effects, so simply do nothing
            //This puts us on the safe side since targeting an incorrect BattleEntity with a Custom move may cause unintended behavior
            //For example, Tattle expects an ITattleableEntity, so it would throw an exception if it targeted anything else
            if (UtilityGlobals.MoveAffectionTypesHasFlag(actualAction.MoveProperties.MoveAffectionType, MoveAffectionTypes.Custom))
            {
                changeTargets = 0;
                actualAction = new NoAction(this);
            }

            //Change to an ally
            /*Steps:
              1. If the action deals damage, mark a flag to attack an ally. If it heals, say to target an enemy
              2. If the action targets the first entity, find the adjacent entities. If not, find all entities
              3. Filter by heights based on who the action can target
              4. Filter out dead entities
              5. If the action targets everyone, go with the remaining list. Otherwise, choose a random entity to target
              6. If there are no entities to target after all the filtering, make the entity do nothing*/
            if (changeTargets == 1)
            {
                bool targetAlly = true;

                //If the action deals damage, attempt to target an ally
                if (actualAction.DealsDamage == true)
                {
                    targetAlly = true;
                }
                //Tell it to not to target an ally if the action heals
                else if (actualAction.Heals == true)
                {
                    targetAlly = false;
                }

                List<BattleEntity> newTargets = new List<BattleEntity>();

                //If this action targets only the first player/enemy, look for adjacent entities
                if (actualAction.MoveProperties.SelectionType == EntitySelectionType.First)
                {
                    Debug.Log($"{Name} is looking for valid adjacent allies to attack!");

                    //Find adjacent allies and filter out all non-ally entities
                    BManager.GetAdjacentEntities(newTargets, this);

                    if (targetAlly == true)
                        newTargets.RemoveAll((adjacent) => adjacent.EntityType != EntityType);
                    else
                        newTargets.RemoveAll((adjacent) => adjacent.EntityType == EntityType);
                }
                else
                {
                    //Neutral BattleEntities should target allies regardless, as they have no enemies
                    if (targetAlly == true || EntityType == EntityTypes.Neutral)
                    {
                        //Find all allies
                        BManager.GetEntityAllies(newTargets, this);
                    }
                    else
                    {
                        BManager.GetEntities(newTargets, this.GetOpposingEntityType(), null);
                    }
                }

                //Filter by heights
                BattleManagerUtils.FilterEntitiesByHeights(newTargets, actualAction.MoveProperties.HeightsAffected);

                //Filter dead entities
                BattleManagerUtils.FilterDeadEntities(newTargets);

                //Choose a random target to attack if the action only targets one entity
                if (newTargets.Count > 0 && actualAction.MoveProperties.SelectionType != EntitySelectionType.All)
                {
                    int randTarget = RandomGlobals.Randomizer.Next(0, newTargets.Count);
                    BattleEntity target = newTargets[randTarget];
                    newTargets.Clear();
                    newTargets.Add(target);

                    if (targetAlly == true)
                        Debug.Log($"{Name} will attack {newTargets[0].Name} in Confusion!");
                    else
                        Debug.Log($"{Name} will heal {newTargets[0].Name} in Confusion!");
                }

                //If you can't target anyone, do nothing
                if (newTargets.Count == 0)
                {
                    actualAction = new NoAction(this);

                    Debug.Log($"{Name} did nothing as there's either no one to attack or they're not in range!");
                }
                else
                {
                    //Set the actual targets to be the set of new targets
                    actualTargets = newTargets.ToArray();

                    //Disable action commands when attacking allies from Confusion, if the action has an Action Command
                    if (actualAction.HasActionCommand == true)
                        actualAction.EnableActionCommand = false;
                }
            }

            return new BattleGlobals.ActionHolder(actualAction, actualTargets);
        }

        /// <summary>
        /// Makes the BattleEntity perform a MoveAction.
        /// The actual action performed may not be the one passed in if the BattleEntity is affected by Confusion.
        /// </summary>
        /// <param name="action">The MoveAction to perform.</param>
        /// <param name="ignoreConfusion">Whether to ignore the effects of Confusion for this action or not.
        /// This is often true when a certain action must be performed at a certain time (Ex. Koopas doing nothing when flipped, 
        /// Veil's second part).</param>
        /// <param name="targets">The BattleEntities the MoveAction targets.</param>
        public void StartAction(MoveAction action, bool ignoreConfusion, params BattleEntity[] targets)
        {
            MoveAction actualAction = action;
            BattleEntity[] actualTargets = targets;

            //Check for Confused and handle it appropriately
            if (ignoreConfusion == false && EntityProperties.HasAdditionalProperty(AdditionalProperty.ConfusionPercent) == true)
            {
                //Test if we should enter confusion
                int percent = EntityProperties.GetAdditionalProperty<int>(AdditionalProperty.ConfusionPercent);

                //See if Confusion should take effect
                if (UtilityGlobals.TestRandomCondition(percent) == true)
                {
                    Debug.Log($"{Name} is affected by Confusion and will do something unpredictable!");

                    BattleGlobals.ActionHolder holder = ConfusionHandler(action, targets);
                    actualAction = holder.Action;
                    actualTargets = holder.Targets;
                }
            }

            LastAction = actualAction;

            //Start the action
            LastAction.OnActionStarted();
            LastAction.StartSequence(actualTargets);
        }

        /// <summary>
        /// Gets the idle animation of the BattleEntity based on its HealthState and the Status Effects it's inflicted with.
        /// </summary>
        /// <returns>A string with the idle animation name the BattleEntity has.</returns>
        public virtual string GetIdleAnim()
        {
            //Quick fix until there's a more concrete way of defining the current idle animation to play
            if (EntityProperties.HasStatus(StatusTypes.Stone) == true)
                return AnimationGlobals.StatusBattleAnimations.StoneName;
            else if (EntityProperties.HasStatus(StatusTypes.Injured) == true)
                return AnimationGlobals.StatusBattleAnimations.InjuredName;
            else if (EntityProperties.HasStatus(StatusTypes.Stop) == true || EntityProperties.HasStatus(StatusTypes.Frozen) == true)
                return AnimationGlobals.HurtName;
            else if (EntityProperties.HasStatus(StatusTypes.Sleep) == true)
                return AnimationGlobals.StatusBattleAnimations.SleepName;
            else if (EntityProperties.HasStatus(StatusTypes.Dizzy) == true)
                return AnimationGlobals.StatusBattleAnimations.DizzyName;
            else if (EntityProperties.HasStatus(StatusTypes.Confused) == true)
                return AnimationGlobals.StatusBattleAnimations.ConfusedName;
            else if (EntityProperties.HasStatus(StatusTypes.Poison) == true)
                return AnimationGlobals.PlayerBattleAnimations.DangerName;
            else if (EntityProperties.HasStatus(StatusTypes.Paralyzed) == true)
                return AnimationGlobals.IdleName;

            switch (HealthState)
            {
                case HealthStates.Dead: return AnimationGlobals.DeathName;
                default: return AnimationGlobals.IdleName;
            }
        }

        #region Equipment Methods

        /// <summary>
        /// Gets the first item of a particular ItemType that the BattleEntity has.
        /// </summary>
        /// <param name="itemTypes">The ItemType enum value. If an item has any of these values, it will be returned.</param>
        /// <returns>An Item that the BattleEntity has matching the ItemType enum value, otherwise null.</returns>
        public abstract Item GetItemOfType(Item.ItemTypes itemTypes);

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
        /// Used for update logic that applies to the BattleEntity regardless of whether it is its turn or not.
        /// </summary>
        public virtual void Update()
        {
            AnimManager.CurrentAnim?.Update();

            //Update Defensive actions
            for (int i = 0; i < DefensiveActions.Count; i++)
            {
                DefensiveActions[i].Update();
            }
        }

        public void Draw()
        {
            //Draw the entity itself
            DrawEntity();

            //Draw anything else, such as Status Effect icons
            DrawOther();
        }

        /// <summary>
        /// Draws the BattleEntity itself.
        /// </summary>
        protected virtual void DrawEntity()
        {
            AnimManager.GetCurrentAnim<Animation>()?.Draw(Position, TintColor, Rotation, Origin, Scale, SpriteFlip, Layer);
        }

        /// <summary>
        /// Draws anything else related to the BattleEntity.
        /// </summary>
        protected virtual void DrawOther()
        {
            
        }
    }
}
