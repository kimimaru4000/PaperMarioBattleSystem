using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using static PaperMarioBattleSystem.Enumerations;
using static PaperMarioBattleSystem.BattleGlobals;
using PaperMarioBattleSystem.Utilities;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// Handles all aspects regarding turn-based battles. It manages the BattleEntities taking part in battle.
    /// </summary>
    public class BattleManager : IUpdateable, ICleanup
    {
        #region Events

        public delegate void OnEntityAdded(BattleEntity battleEntity);
        /// <summary>
        /// The event invoked after a <see cref="BattleEntity"/> has been added to battle.
        /// <para>This is invoked after the entity has been added to battle and initialized.</para>
        /// </summary>
        public event OnEntityAdded EntityAddedEvent = null;

        public delegate void OnEntityRemoved(BattleEntity battleEntity);
        /// <summary>
        /// The event invoked after a <see cref="BattleEntity"/> has been removed from battle.
        /// <para>This is invoked after the entity has been entirely removed from battle.</para>
        /// </summary>
        public event OnEntityRemoved EntityRemovedEvent = null;

        public delegate void OnBattleTurnStart(BattleEntity battleEntity);
        /// <summary>
        /// The event invoked when any BattleEntity's turn is started.
        /// <para>This is invoked right before the BattleEntity is notified to start its turn.</para>
        /// </summary>
        public event OnBattleTurnStart BattleTurnStartedEvent = null;

        public delegate void OnBattleTurnEnd(BattleEntity battleEntity);
        /// <summary>
        /// The event invoked when any BattleEntity's turn is ended.
        /// <para>This is invoked right before the BattleEntity is notified to end its turn.</para>
        /// </summary>
        public event OnBattleTurnEnd BattleTurnEndedEvent = null;

        #endregion

        /// <summary>
        /// The BattleManager's event manager.
        /// </summary>
        public BattleEventManager battleEventManager { get; private set; } = null;

        /// <summary>
        /// The battle's properties.
        /// </summary>
        public BattleProperties Properties { get; private set; } = default(BattleProperties);

        /// <summary>
        /// How many phase cycles (finished the phase order) passed.
        /// </summary>
        public int PhaseCycleCount { get; private set; } = -1;

        /// <summary>
        /// Whether certain UI, such as Status Effect icons and enemy HP, should show up or not.
        /// This UI shows up only when the Player is choosing an action.
        /// </summary>
        public bool ShouldShowPlayerTurnUI => (EntityTurn?.EntityType == EntityTypes.Player && EntityTurn.PreviousAction?.MoveSequence.InSequence != true);

        /// <summary>
        /// The phase order in battle.
        /// BattleEntities of these types go in this order.
        /// </summary>
        private readonly EntityTypes[] PhaseOrder = new EntityTypes[] { EntityTypes.Player, EntityTypes.Enemy, EntityTypes.Neutral };

        /// <summary>
        /// The phase the battle starts on.
        /// </summary>
        private int StartingPhase => 0;

        /// <summary>
        /// The current phase, represented as an integer. The battle starts on the first phase in <see cref="PhaseOrder"/>.
        /// </summary>
        private int Phase = 0;

        /// <summary>
        /// Tells whether the BattleManager is currently switching phases or not.
        /// </summary>
        private bool SwitchingPhase = false;

        /// <summary>
        /// The current phase, represented as an <see cref="EntityTypes"/>.
        /// This is a property that references the <see cref="PhaseOrder"/>.
        /// </summary>
        public EntityTypes CurEntityPhase => PhaseOrder[Phase];

        /// <summary>
        /// The current state of the battle.
        /// </summary>
        public BattleState State { get; private set; } = BattleState.Init;

        /// <summary>
        /// The BattleEntity whose turn it currently is.
        /// </summary>
        public BattleEntity EntityTurn { get; private set; } = null;

        /// <summary>
        /// Tells whether the current BattleEntity's turn is halted from being updated or not.
        /// <para>This has several use-cases. For example, Merlee's spell starts after Mario selects his move, and Mario's turn is halted until the animation is done.</para>
        /// </summary>
        public bool HaltedTurn { get; private set; } = false;

        /// <summary>
        /// The total number of BattleEntities in battle.
        /// </summary>
        public int TotalEntityCount { get; private set; } = 0;

        /// <summary>
        /// All the BattleEntities taking part in the battle.
        /// <para>The key is the EntityType and the value is the list of BattleEntities.</para>
        /// </summary>
        private readonly Dictionary<EntityTypes, List<BattleEntity>> AllEntities = new Dictionary<EntityTypes, List<BattleEntity>>();

        /// <summary>
        /// The player in the Front.
        /// </summary>
        public BattleEntity FrontPlayer => FindEntityFromBattleIndex(EntityTypes.Player, 0, true);

        /// <summary>
        /// The player in the Back.
        /// </summary>
        public BattleEntity BackPlayer => FindEntityFromBattleIndex(EntityTypes.Player, 1, false);

        /// <summary>
        /// Mario reference.
        /// </summary>
        public BattleMario Mario { get; private set; } = null;

        /// <summary>
        /// Partner reference.
        /// </summary>
        public BattlePartner Partner { get; private set; } = null;

        /// <summary>
        /// The number of enemies alive.
        /// </summary>
        private int EnemiesAlive => GetEntitiesCount(EntityTypes.Enemy);

        public BattleManager()
        {
            battleEventManager = new BattleEventManager(this);
        }

        public void CleanUp()
        {
            State = BattleState.Done;

            //Remove and cleanup all BattleEntities in battle
            List<BattleEntity> removedEntities = GetAllEntitiesList();

            //Remove all entities
            RemoveEntities(removedEntities, true);

            EntityAddedEvent = null;
            EntityRemovedEvent = null;
            BattleTurnStartedEvent = null;
            BattleTurnEndedEvent = null;
        }

        /// <summary>
        /// Initializes the battle.
        /// </summary>
        /// <param name="properties">The battle's properties.</param>
        /// <param name="mario">Mario.</param>
        /// <param name="partner">Mario's partner.</param>
        /// <param name="otherEntities">The BattleEntities to add, in order. This includes enemies.</param>
        public void Initialize(BattleProperties properties, BattleMario mario, BattlePartner partner, List<BattleEntity> otherEntities)
        {
            Properties = properties;

            //Mario always starts out in the front, and the Partner always starts out in the back
            SetMario(mario);
            SetPartner(partner);

            //Add all entities at once
            List<BattleEntity> addedEntities = new List<BattleEntity>();

            //Add Mario before the Partner so his Battle Index is set to 0
            addedEntities.Add(mario);
            addedEntities.Add(partner);

            //Add others
            if (otherEntities != null)
            {
                addedEntities.AddRange(otherEntities);
            }

            //Add and initialize all entities
            AddEntities(addedEntities, null, true);

            Phase = StartingPhase;

            //Calculate the total number of BattleEntities
            TotalEntityCount = CalculateEntityCount();
        }

        public void Update()
        {
            //Don't do anything until the battle starts
            if (State == BattleState.Init) return;

            //NOTE: Create a general way to halt turns in battle in place of these hardcoded event checks

            //Update battle events if there are any
            if (battleEventManager.HasBattleEvents == true)
            {
                battleEventManager.UpdateBattleEvents();
            }

            //If a turn just ended, update the current state
            if (State == BattleState.TurnEnd)
            {
                //Don't start the next turn until all Battle Events are finished
                if (battleEventManager.HasBattleEvents == false)
                {
                    //If we should switch phases, switch to the next phase here
                    //Putting this here fixes all Battle Events being delayed until a BattleEntity has a turn
                    if (SwitchingPhase == true)
                    {
                        //All of the entities on this phase are done with their turns, so go to the next phase
                        int nextPhase = UtilityGlobals.Wrap(Phase + 1, 0, PhaseOrder.Length - 1);
                        SwitchPhase(nextPhase);
                    }
                    //Otherwise, start the current turn
                    else
                    {
                        TurnStart();
                    }
                }
            }

            //Update the current turn if we should
            if (State == BattleState.Turn && HaltedTurn == false)
            {
                EntityTurn.TurnUpdate();
            }

            //Update all BattleEntities
            UpdateEntities();
        }

        private void UpdateEntities()
        {
            foreach (KeyValuePair<EntityTypes, List<BattleEntity>> entityPairs in AllEntities)
            {
                List<BattleEntity> entityList = entityPairs.Value;
                for (int i = 0; i < entityList.Count; i++)
                {
                    entityList[i].Update();
                }
            }
        }

        /// <summary>
        /// Starts the Battle
        /// </summary>
        public void StartBattle()
        {
            ChangeBattleState(BattleState.TurnEnd);
            SwitchingPhase = true;
            SwitchPhase(Phase);
        }

        /// <summary>
        /// Ends the Battle
        /// </summary>
        public void EndBattle()
        {
            ChangeBattleState(BattleState.Done);
        }

        /// <summary>
        /// Changes the current state of the battle.
        /// </summary>
        /// <param name="state">The new BattleState the battle is in.</param>
        private void ChangeBattleState(BattleState state)
        {
            State = state;

            battleEventManager.AddPendingEvents();
        }

        private void SwitchPhase(int phase)
        {
            SwitchingPhase = false;

            EntityTypes prevPhase = PhaseOrder[Phase];
            Phase = phase;

            //Call OnPhaseEnd() for the previous entities
            //Use a new list in case it's modified
            List<BattleEntity> entities = GetEntitiesList(prevPhase, null);
            if (entities != null)
            {
                for (int i = 0; i < entities.Count; i++)
                {
                    entities[i].OnPhaseEnd();
                }
            }

            //Check if we wrapped around to the starting phase
            if (Phase == StartingPhase)
            {
                //Increment the phase cycles when switching to the starting phase
                PhaseCycleCount++;

                Debug.Log($"Started new phase cycle. Current cycle count: {PhaseCycleCount}");

                //Call OnPhaseCycleStart() for every entity
                foreach (KeyValuePair<EntityTypes, List<BattleEntity>> entityDict in AllEntities)
                {
                    List<BattleEntity> entityList = entityDict.Value;
                    for (int i = 0; i < entityList.Count; i++)
                    {
                        entityList[i].OnPhaseCycleStart();
                    }
                }
            }

            //Call OnPhaseStart() for the new entities going
            //Use a new list in case it's modified
            entities = GetEntitiesList(CurEntityPhase, null);
            if (entities != null)
            {
                for (int i = 0; i < entities.Count; i++)
                {
                    entities[i].OnPhaseStart();
                }
            }

            //Find out who should go now
            FindNextEntityTurn();
        }

        /// <summary>
        /// Halts the current BattleEntity's turn from updating.
        /// </summary>
        public void HaltTurn()
        {
            HaltedTurn = true;
        }

        /// <summary>
        /// Resumes the current BattleEntity's turn.
        /// </summary>
        public void ResumeTurn()
        {
            HaltedTurn = false;
        }

        /// <summary>
        /// Sets the Mario reference. Make sure to remove the old one from battle first if it exists.
        /// </summary>
        /// <param name="mario">A BattleMario to act as the new Mario.</param>
        public void SetMario(BattleMario mario)
        {
            Mario = mario;
        }

        /// <summary>
        /// Sets the Partner reference. Make sure to remove the old one from battle first if it exists.
        /// </summary>
        /// <param name="partner">A BattlePartner to act as the new Partner.</param>
        public void SetPartner(BattlePartner partner)
        {
            Partner = partner;
        }

        public void TurnStart()
        {
            if (State == BattleState.Done)
            {
                Debug.LogError($"Attemping to START turn when the battle is over!");
                return;
            }

            ChangeBattleState(BattleState.Turn);

            //Invoke the event for starting the turn
            BattleTurnStartedEvent?.Invoke(EntityTurn);

            EntityTurn.StartTurn();
        }

        public void TurnEnd()
        {
            if (State == BattleState.Done)
            {
                Debug.LogError($"Attemping to END turn when the battle is over!");
                return;
            }

            //Invoke the event for ending the turn
            BattleTurnEndedEvent?.Invoke(EntityTurn);

            EntityTurn.EndTurn();

            //The battle is finished
            if (State == BattleState.Done)
            {
                return;
            }

            ChangeBattleState(BattleState.TurnEnd);

            //Find the next entity to go
            FindNextEntityTurn();
        }

        /// <summary>
        /// Called when a BattleEntity dies to remove it and update the battle state.
        /// Called in <see cref="DeathBattleEvent"/>.
        /// </summary>
        public void HandleEntityDeath(BattleEntity deadEntity)
        {
            HandleDeadEntity(deadEntity);
            UpdateBattleState();
        }

        /// <summary>
        /// Updates the battle state, checking if the battle should be over.
        /// It's game over if Mario has 0 HP, otherwise it's victory if no enemies are alive
        /// </summary>
        private void UpdateBattleState()
        {
            if (Mario == null || Mario.IsDead == true)
            {
                EndBattle();
                Debug.Log("GAME OVER");

                BattleUIManager.Instance.ClearMenuStack();
            }
            else if (EnemiesAlive <= 0)
            {
                EndBattle();
                Mario?.AnimManager.PlayAnimation(AnimationGlobals.VictoryName);
                Partner?.AnimManager.PlayAnimation(AnimationGlobals.VictoryName);
                Debug.Log("VICTORY");

                BattleUIManager.Instance.ClearMenuStack();
            }
        }

        /// <summary>
        /// Handles a dead BattleEntity.
        /// Non-players are removed from battle, and Players are handled differently.
        /// </summary>
        private void HandleDeadEntity(BattleEntity deadEntity)
        {
            //Check if the dead BattleEntity is the Partner
            if (deadEntity == Partner)
            {
                //If the Partner died and is in front, switch places with Mario
                if (Partner == FrontPlayer)
                {                    
                    //Switch Mario in front
                    BattleManagerUtils.SwapEntityBattlePosAndIndex(Mario, Partner, true);

                    //Queue the event to switch Mario with his Partner
                    battleEventManager.QueueBattleEvent((int)BattleGlobals.BattleEventPriorities.Stage, new BattleState[] { BattleState.Turn, BattleState.TurnEnd },
                        new SwapPositionBattleEvent(FrontPlayer, BackPlayer, FrontPlayer.BattlePosition, BackPlayer.BattlePosition, 500f));
                }

            }

            //Players don't get removed from battle through normal death
            //Partners can be removed via a Status Effect that forces them out of battle, such as Fright or Gale Force
            //Mario's death results in a Game Over, which is checked when updating the battle state
            if (deadEntity.EntityType == EntityTypes.Player) return;

            //Remove this BattleEntity from battle
            RemoveEntity(deadEntity, true);
        }

        /// <summary>
        /// Finds the next BattleEntity that should go.
        /// </summary>
        private void FindNextEntityTurn()
        {
            //Get the list of entities going on the current phase
            List<BattleEntity> entities = GetEntitiesList(CurEntityPhase);
            if (entities != null)
            {
                for (int i = 0; i < entities.Count; i++)
                {
                    //If the entity isn't dead and has turns left, it should go next
                    if (entities[i].UsedTurn == false && entities[i].IsDead == false)
                    {
                        EntityTurn = entities[i];
                        return;
                    }
                }
            }

            SwitchingPhase = true;
        }

        /// <summary>
        /// Adds a BattleEntity to battle.
        /// </summary>
        /// <param name="battleEntity">The BattleEntity to add.</param>
        /// <param name="battleIndex">An int containing the BattleIndex to add the BattleEntity at.
        /// <para>If the value is null, the BattleManager will assign the lowest available BattleIndex.</para></param>
        /// <param name="initialize">Whether to initialize the BattleEntity or not. Regardless of the value, it assigns a BattleIndex.
        /// If the BattleEntity has first joined battle, this should be true.
        /// This should be false in cases where the BattleEntities have been removed and re-added to battle, such as switching Partners.</param>
        public void AddEntity(BattleEntity battleEntity, int? battleIndex, bool initialize)
        {
            //Check for a null BattleEntity
            if (battleEntity == null)
            {
                Debug.LogError($"Can't add BattleEntity to battle because it is null!");
                return;
            }

            EntityTypes entityType = battleEntity.EntityType;

            //Get the existing list to add this BattleEntity to
            List<BattleEntity> entityList = GetEntitiesList(entityType);

            //If no list exists for this EntityType, add one
            if (entityList == null)
            {
                entityList = new List<BattleEntity>();
                AllEntities.Add(entityType, entityList);
            }

            //Add the BattleEntity to the list
            entityList.Add(battleEntity);

            //Check to assign the BattleIndex from the value passed in
            int finalBattleIndex = battleIndex ?? BattleGlobals.InvalidBattleIndex;

            //If the BattleIndex isn't valid, find the lowest available one for this EntityType
            if (BattleGlobals.IsValidBattleIndex(finalBattleIndex) == false)
            {
                finalBattleIndex = FindLowestAvailableBattleIndex(entityType);
            }

            //Set the Battle Manager associated with the BattleEntity to this one
            battleEntity.SetBattleManager(this);

            //Set the battle index and start battle for the BattleEntity if it should
            battleEntity.SetBattleIndex(finalBattleIndex, false);

            if (initialize == true)
            {
                battleEntity.OnBattleStart();
            }

            //Increment BattleEntity count when added
            TotalEntityCount++;

            //Sort the list
            SortEntityList(entityType);

            //Invoke the entity added event
            EntityAddedEvent?.Invoke(battleEntity);
        }

        /// <summary>
        /// Adds a set of BattleEntities to battle.
        /// </summary>
        /// <param name="battleEntities">An <see cref="IList{T}"/> of BattleEntities to add.</param>
        /// <param name="battleIndices">An <see cref="IList{T}"/> of ints containing the BattleIndex to add each BattleEntity at.
        /// <para>If the IList is null, the value at the IList index is less than 0, or the IList index is out of the <paramref name="battleEntities"/>
        /// IList range, the BattleManager will assign the lowest available BattleIndex.</para></param>
        /// <param name="initialize">Whether to initialize the entities or not. Regardless of the value, it assigns a BattleIndex.
        /// If the BattleEntities have first joined battle, this should be true.
        /// This should be false in cases where BattleEntities have been removed and re-added to battle, such as switching Partners.</param>
        public void AddEntities(IList<BattleEntity> battleEntities, IList<int> battleIndices, bool initialize)
        {
            //Don't add a null or empty list
            if (battleEntities == null || battleEntities.Count == 0)
            {
                Debug.LogError($"Not adding null or empty BattleEntities IList!");
                return;
            }

            //Add the BattleEntities
            for (int i = 0; i < battleEntities.Count; i++)
            {
                //Check for a valid index
                int? index = null;
                if (battleIndices != null && i < battleIndices.Count)
                    index = battleIndices[i];

                AddEntity(battleEntities[i], index, initialize);
            }
        }

        /// <summary>
        /// Removes a BattleEntity from battle.
        /// </summary>
        /// <param name="battleEntity">The BattleEntity to remove.</param>
        /// <param name="cleanUp">Whether to clean up the BattleEntity or not when removed from battle.
        /// This should be true if the BattleEntity is permanently removed from battle.
        /// In temporary cases, such as switching Partners, it would be preferable to set this to false.</param>
        public void RemoveEntity(BattleEntity battleEntity, bool cleanUp)
        {
            //Check for a null BattleEntity
            if (battleEntity == null)
            {
                Debug.LogError($"Can't remove BattleEntity from battle because it is null!");
                return;
            }

            EntityTypes entityType = battleEntity.EntityType;

            //Get the existing list to remove this BattleEntity from
            List<BattleEntity> entityList = GetEntitiesList(entityType);

            //If no BattleEntities of this type are present, there's nothing to remove
            if (entityList == null)
            {
                Debug.LogError($"Can't remove BattleEntity of type {entityType} since it's not in battle!");
                return;
            }

            bool removed = entityList.Remove(battleEntity);

            //If there are no more BattleEntities left for this EntityType, remove the entry
            if (entityList.Count == 0)
            {
                AllEntities.Remove(entityType);
            }

            //Clean up the BattleEntity if it should be cleaned up
            if (cleanUp == true)
            {
                battleEntity.CleanUp();

                //Clear these references if the BattleEntity removed is Mario or his Partner
                if (battleEntity == Mario)
                {
                    SetMario(null);
                }
                if (battleEntity == Partner)
                {
                    SetPartner(null);
                }
            }

            //Decrement BattleEntity count when removed
            if (removed == true)
                TotalEntityCount--;

            //Invoke the entity removed event
            EntityRemovedEvent?.Invoke(battleEntity);
        }

        /// <summary>
        /// Removes a set of BattleEntities from battle.
        /// </summary>
        /// <param name="battleEntities">An <see cref="IList{T}"/> of BattleEntities to remove.</param>
        /// <param name="cleanUp">Whether to clean up the BattleEntities or not when removed from battle.
        /// This should be true if the BattleEntities are permanently removed from battle.
        /// In temporary cases, such as switching Partners, it would be preferable to set this to false.</param>
        public void RemoveEntities(IList<BattleEntity> battleEntities, bool cleanUp)
        {
            //Don't remove a null or empty list
            if (battleEntities == null || battleEntities.Count == 0)
            {
                Debug.LogError($"Not removing null or empty BattleEntities!");
                return;
            }

            //Remove all specified entities from the list
            for (int i = 0; i < battleEntities.Count; i++)
            {
                RemoveEntity(battleEntities[i], cleanUp);
            }
        }

        /// <summary>
        /// Sorts a list of BattleEntities of a specific type based on BattleIndex.
        /// </summary>
        /// <param name="entityType">The type of BattleEntities to sort.</param>
        public void SortEntityList(EntityTypes entityType)
        {
            List<BattleEntity> entityList = GetEntitiesList(entityType);
            if (entityList != null && entityList.Count > 0)
            {
                entityList.Sort(BattleGlobals.EntityBattleIndexSort);
            }
        }

        #region Helper Methods

        #region Internal

        /// <summary>
        /// Returns all BattleEntities in battle in a new list.
        /// This method is used internally in the BattleManager.
        /// </summary>
        /// <returns>A list of all BattleEntities in battle. An empty list if none are in battle.</returns>
        private List<BattleEntity> GetAllEntitiesList()
        {
            List<BattleEntity> allEntities = new List<BattleEntity>();

            //Add all BattleEntities
            EntityTypes[] entityTypes = UtilityGlobals.GetEnumValues<EntityTypes>();
            for (int i = 0; i < entityTypes.Length; i++)
            {
                List<BattleEntity> existingList = GetEntitiesList(entityTypes[i]);
                if (existingList != null)
                {
                    allEntities.AddRange(existingList);
                }
            }

            return allEntities;
        }

        /// <summary>
        /// Returns all BattleEntities of a specified EntityType in a list.
        /// The returned list is the same reference found in the <see cref="AllEntities"/> dictionary.
        /// This method is used internally in the BattleManager.
        /// <para>NOTE: Be very careful with this list; modifying it outside of the appropriate methods may result in mismatched data.</para>
        /// </summary>
        /// <param name="entityType">The EntityType of entities to return.</param>
        /// <returns>Entities matching the EntityType specified. If none exist, null is returned.</returns>
        private List<BattleEntity> GetEntitiesList(EntityTypes entityType)
        {
            List<BattleEntity> entities = null;

            //Get the current list
            //It will be null if there are no BattleEntities of this EntityType in the dictionary
            AllEntities.TryGetValue(entityType, out entities);

            return entities;
        }

        /// <summary>
        /// Returns all BattleEntities of a specified EntityType in a new list.
        /// This method is used internally in the BattleManager to allow for easy manipulation of the returned list.
        /// </summary>
        /// <param name="entityType">The EntityType of entities to return.</param>
        /// <param name="heightStates">The height states to filter entities by. Entities with any of the state will be included.
        /// If null, will include entities of all height states.</param>
        /// <returns>Entities matching the EntityType and height states specified. If none are found, an empty list.</returns>
        private List<BattleEntity> GetEntitiesList(EntityTypes entityType, params HeightStates[] heightStates)
        {
            //Get the internal list
            List<BattleEntity> entities = new List<BattleEntity>();
            List<BattleEntity> entitiesOfType = GetEntitiesList(entityType);

            //Add the BattleEntities in the internal list into the new list
            if (entitiesOfType != null)
            {
                entities.AddRange(entitiesOfType);
            }

            //Filter by height states
            BattleManagerUtils.FilterEntitiesByHeights(entities, heightStates);

            return entities;
        }

        /// <summary>
        /// Calculates and returns the total number of BattleEntities participating in battle. This method is internally used by the BattleManager.
        /// </summary>
        /// <returns>The number of BattleEntities in battle.</returns>
        private int CalculateEntityCount()
        {
            int count = 0;

            //Get all BattleEntities
            EntityTypes[] entityTypes = UtilityGlobals.GetEnumValues<EntityTypes>();
            for (int i = 0; i < entityTypes.Length; i++)
            {
                count += GetEntitiesCount(entityTypes[i]);
            }

            return count;
        }

        #endregion

        /// <summary>
        /// Returns all BattleEntities taking part in battle in an array.
        /// </summary>
        /// <param name="heightStates">The height states to filter entities by. Entities with any of the state will be included.
        /// If null, will include entities of all height states.</param>
        /// <returns>All BattleEntities in battle matching the height states specified.</returns>
        public BattleEntity[] GetAllBattleEntities(params HeightStates[] heightStates)
        {
            List<BattleEntity> allentities = GetAllEntitiesList();

            //Filter by height states
            BattleManagerUtils.FilterEntitiesByHeights(allentities, heightStates);

            return allentities.ToArray();
        }

        /// <summary>
        /// Gets all BattleEntities taking part in battle into a supplied list. This method generates no garbage.
        /// </summary>
        /// <param name="entityList">The list to put the BattleEntities into.</param>
        /// <param name="heightStates">The height states to filter entities by. Entities with any of the state will be included.
        /// If null, will include entities of all height states.</param>
        public void GetAllBattleEntities(List<BattleEntity> entityList, params HeightStates[] heightStates)
        {
            //Add all the BattleEntities to the existing list
            foreach(KeyValuePair<EntityTypes, List<BattleEntity>> entities in AllEntities)
            {
                entityList.CopyFromList(entities.Value);
            }

            BattleManagerUtils.FilterEntitiesByHeights(entityList, heightStates);
        }

        /// <summary>
        /// Returns the number of BattleEntities of the specified EntityType participating in battle.
        /// </summary>
        /// <param name="entityType">The EntityType of BattleEntities to return the count for.</param>
        /// <returns>The number of BattleEntities in battle of the particular EntityType.</returns>
        public int GetEntitiesCount(EntityTypes entityType)
        {
            //Get the same list reference and return its count
            List<BattleEntity> entities = GetEntitiesList(entityType);

            if (entities == null) return 0;
            else return entities.Count;
        }

        /// <summary>
        /// Returns all entities of a specified EntityType in a new array.
        /// </summary>
        /// <param name="entityType">The EntityType of BattleEntities to return.</param>
        /// <param name="heightStates">The height states to filter entities by. Entities with any of the state will be included.
        /// If null, will include entities of all height states</param>
        /// <returns>Entities matching the EntityType and height states specified.</returns>
        public BattleEntity[] GetEntities(EntityTypes entityType, params HeightStates[] heightStates)
        {
            List<BattleEntity> entities = GetEntitiesList(entityType, heightStates);
            return entities.ToArray();
        }

        /// <summary>
        /// Adds all entities of a specified EntityType in a supplied List. This method generates no garbage.
        /// </summary>
        /// <param name="entityList">The List to add the BattleEntities to.</param>
        /// <param name="entityType">The EntityType of BattleEntities to return.</param>
        /// <param name="heightStates">The height states to filter entities by. Entities with any of the state will be included.
        /// If null, will include entities of all height states</param>
        public void GetEntities(List<BattleEntity> entityList, EntityTypes entityType, params HeightStates[] heightStates)
        {
            List<BattleEntity> entList = GetEntitiesList(entityType);
            entityList.CopyFromList(entList);

            BattleManagerUtils.FilterEntitiesByHeights(entityList, heightStates);
        }

        /// <summary>
        /// Returns all entities of a specified EntityType in an array.
        /// The entities are returned in reverse order. Entities in the back are the first elements in the List.
        /// </summary>
        /// <param name="entityType">The EntityType of BattleEntities to return.</param>
        /// <param name="heightStates">The height states to filter entities by. Entities with any of the state will be included.
        /// If null, will include entities of all height states.</param>
        /// <returns>Entities matching the EntityType and height states specified, in reverse.</returns>
        public BattleEntity[] GetEntitiesReversed(EntityTypes entityType, params HeightStates[] heightStates)
        {
            List<BattleEntity> entities = GetEntitiesList(entityType, heightStates);
            entities.Reverse();
            return entities.ToArray();
        }

        /// <summary>
        /// Adds all entities of a specified EntityType in a supplied List in reverse order.
        /// Entities in the back are the first elements in the List. This method generates no garbage.
        /// </summary>
        /// <param name="entityList">The List to add the BattleEntities to.</param>
        /// <param name="entityType">The EntityType of BattleEntities to return.</param>
        /// <param name="heightStates">The height states to filter entities by. Entities with any of the state will be included.
        /// If null, will include entities of all height states.</param>
        /// <returns>Entities matching the EntityType and height states specified, in reverse.</returns>
        public void GetEntitiesReversed(List<BattleEntity> entityList, EntityTypes entityType, params HeightStates[] heightStates)
        {
            List<BattleEntity> entList = GetEntitiesList(entityType);
            entityList.CopyFromListReverse(entList);

            BattleManagerUtils.FilterEntitiesByHeights(entityList, heightStates);
        }

        /// <summary>
        /// Gets all allies of a particular BattleEntity.
        /// </summary>
        /// <param name="entity">The BattleEntity whose allies to get</param>
        /// <param name="heightStates">The height states to filter entities by. Entities with any of the state will be included.
        /// If null or empty, will return the entities passed in</param>
        /// <returns>An array of allies the BattleEntity has.</returns>
        public BattleEntity[] GetEntityAllies(BattleEntity entity, params HeightStates[] heightStates)
        {
            List<BattleEntity> allies = new List<BattleEntity>();
            GetEntityAllies(allies, entity, heightStates);

            //Return all allies
            return allies.ToArray();
        }

        /// <summary>
        /// Gets all allies of a particular BattleEntity. This method generates no garbage.
        /// </summary>
        /// <param name="entityList">The list to put the allies into.</param>
        /// <param name="entity">The BattleEntity whose allies to get.</param>
        /// <param name="heightStates">The height states to filter entities by. Entities with any of the state will be included.
        /// If null or empty, will return the entities passed in</param>
        public void GetEntityAllies(List<BattleEntity> entityList, BattleEntity entity, params HeightStates[] heightStates)
        {
            List<BattleEntity> entList = GetEntitiesList(entity.EntityType);

            entityList.CopyFromList(entList);
            entityList.Remove(entity);
        }

        /// <summary>
        /// Gets the BattleEntities adjacent to a particular BattleEntity.
        /// <para>This considers all foreground entities (Ex. Adjacent to Mario would be his Partner and the first Enemy)</para>
        /// </summary>
        /// <param name="entity">The BattleEntity to find entities adjacent to</param>
        /// <param name="getDead">Gets any adjacent entities even if they're dead</param>
        /// <returns>An array of adjacent BattleEntities. If none are adjacent, an empty array.</returns>
        public BattleEntity[] GetAdjacentEntities(BattleEntity entity)
        {
            List<BattleEntity> adjacentEntities = new List<BattleEntity>();
            GetAdjacentEntities(adjacentEntities, entity);

            return adjacentEntities.ToArray();
        }

        /// <summary>
        /// Gets the BattleEntities adjacent to a particular BattleEntity. This method generates no garbage.
        /// <para>This considers all foreground entities (Ex. Adjacent to Mario would be his Partner and the first Enemy).</para>
        /// </summary>
        /// <param name="adjacentEntities">The list to populate with the adjacent BattleEntities.</param>
        /// <param name="entity">The BattleEntity to find entities adjacent to.</param>
        /// <param name="getDead">Gets any adjacent entities, even if they're dead.</param>
        public void GetAdjacentEntities(List<BattleEntity> adjacentEntities, BattleEntity entity)
        {
            //If the entity is an enemy, it can either be two Enemies or the front Player and another Enemy
            if (entity.EntityType == EntityTypes.Enemy)
            {
                int enemyIndex = entity.BattleIndex;
                BattleEntity prevEnemy = FindEntityFromBattleIndex(EntityTypes.Enemy, enemyIndex - 1, true);
                BattleEntity nextEnemy = FindEntityFromBattleIndex(EntityTypes.Enemy, enemyIndex + 1, false);

                //Check if there's an Enemy before this one
                if (prevEnemy != null) adjacentEntities.Add(prevEnemy);
                //There's no Enemy, so target the Front Player
                else adjacentEntities.Add(FrontPlayer);

                //Check if there's an Enemy after this one
                if (nextEnemy != null) adjacentEntities.Add(nextEnemy);
            }
            //If it's a Player, it will be either Mario/Partner and the first enemy
            else if (entity.EntityType == EntityTypes.Player)
            {
                //The previous entity for Players is always Mario or his Partner, unless the latter has 0 HP
                //The back Player has no one else adjacent to him/her aside from the Front player
                if (entity == BackPlayer)
                {
                    adjacentEntities.Add(FrontPlayer);
                }
                else if (entity == FrontPlayer)
                {
                    //Add the player in the back
                    adjacentEntities.Add(BackPlayer);

                    //Add the next enemy
                    BattleEntity nextEnemy = FindEntityFromBattleIndex(EntityTypes.Enemy, 0);
                    if (nextEnemy != null) adjacentEntities.Add(nextEnemy);
                }
            }
        }

        /// <summary>
        /// Gets the BattleEntities behind a particular BattleEntity.
        /// </summary>
        /// <param name="entity">The BattleEntity to find entities behind</param>
        /// <returns>An array of BattleEntities behind the given one. If none are behind, an empty array.</returns>
        public BattleEntity[] GetEntitiesBehind(BattleEntity entity)
        {
            List<BattleEntity> behindEntities = new List<BattleEntity>();
            GetEntitiesBehind(behindEntities, entity);

            return behindEntities.ToArray();
        }

        /// <summary>
        /// Gets the BattleEntities behind a particular BattleEntity. This method generates no garbage.
        /// </summary>
        /// <param name="entity">The BattleEntity to find entities behind</param>
        public void GetEntitiesBehind(List<BattleEntity> entityList, BattleEntity entity)
        {
            List<BattleEntity> entList = GetEntitiesList(entity.EntityType);

            //Copy the BattleEntities into this list
            entityList.CopyFromList(entList);
            entityList.Remove(entity);

            //Compare Battle Index
            //BattleEntities behind have higher Battle Indices
            int battleIndex = entity.BattleIndex;
            for (int i = 0; i < entityList.Count; i++)
            {
                if (entityList[i].BattleIndex <= battleIndex)
                {
                    entityList.RemoveAt(i);
                    i--;
                }
            }
        }

        /// <summary>
        /// Finds all BattleIndex gaps for a particular type of BattleEntity.
        /// </summary>
        /// <param name="entityType">The type of BattleEntity to find the gaps for.</param>
        /// <returns>An int array containing the Battle Indices that have been skipped over. If none are found, an empty array.</returns>
        public int[] FindBattleIndexGaps(EntityTypes entityType)
        {
            return BattleManagerUtils.FindBattleIndexGaps(GetEntitiesList(entityType));
        }

        /// <summary>
        /// Finds the lowest available Battle Index for a given EntityType.
        /// </summary>
        /// <param name="entityType">The type of BattleEntities to find a Battle Index for.</param>
        /// <returns>An integer representing the next available Battle Index.
        /// If no BattleEntities of the specified type are in battle, -1 will be returned.</returns>
        private int FindLowestAvailableBattleIndex(EntityTypes entityType)
        {
            List<BattleEntity> entities = GetEntitiesList(entityType);

            //No BattleEntities are in this list, so nothing can be returned
            if (entities == null || entities.Count == 0)
            {
                return -1;
            }

            int prevIndex = -1;
            int highestIndex = -1;

            for (int i = 0; i < entities.Count; i++)
            {
                int battleIndex = entities[i].BattleIndex;

                //Store the highest index available
                if (battleIndex > highestIndex)
                    highestIndex = battleIndex;

                //Look for gaps in the index; if the difference is 2 or greater, then the index (1 + prevIndex) would
                //be the lowest available one, since the entities are sorted by BattleIndex
                int diffIndex = battleIndex - prevIndex;
                if (diffIndex > 1)
                {
                    return prevIndex + 1;
                }

                //Set previous index for comparison with the next BattleEntity
                prevIndex = battleIndex;
            }

            //The highest index plus one is the next available index if we exhausted the list and didn't find one in between
            //For example, if no BattleEntities were in the list, 0 would be the next available index
            return (highestIndex + 1);
        }

        /// <summary>
        /// Finds a BattleEntity with a BattleIndex greater than or equal to <paramref name="startIndex"/>.
        /// If searching backwards, will find one less than or equal to <paramref name="startIndex"/> instead.
        /// </summary>
        /// <param name="entityType">The type of BattleEntity.</param>
        /// <param name="startIndex">The BattleIndex to start searching from.</param>
        /// <param name="backwards">Whether to search backwards or not.
        /// If true, will find a BattleEntity with a BattleIndex less than or equal to <paramref name="startIndex"/>.</param>
        /// <returns></returns>
        private BattleEntity FindEntityFromBattleIndex(EntityTypes entityType, int startIndex, bool backwards = false)
        {
            List<BattleEntity> entities = GetEntitiesList(entityType);

            //No BattleEntities are in this list, so nothing can be returned
            if (entities == null) return null;

            //Search forwards
            if (backwards == false)
            {
                for (int i = 0; i < entities.Count; i++)
                {
                    int battleIndex = entities[i].BattleIndex;
                    if (battleIndex >= startIndex)
                    {
                        return entities[i];
                    }
                }
            }
            //Search backwards
            else
            {
                for (int i = entities.Count - 1; i >= 0; i--)
                {
                    int battleindex = entities[i].BattleIndex;
                    if (battleindex <= startIndex)
                    {
                        return entities[i];
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Returns the closest BattleEntity in front of a specified one.
        /// </summary>
        /// <param name="battleEntity">The BattleEntity to find the entity in front of.</param>
        /// <returns>The closest BattleEntity in front of the specified one. null if no BattleEntity is in front of the specified one.</returns>
        public BattleEntity GetEntityInFrontOf(BattleEntity battleEntity)
        {
            //Find an entity of the same EntityType with a lower BattleIndex
            return FindEntityFromBattleIndex(battleEntity.EntityType, battleEntity.BattleIndex - 1, true);
        }

        /// <summary>
        /// Gets the frontmost BattleEntity for a specified EntityType.
        /// </summary>
        /// <param name="entityType">The EntityType.</param>
        /// <param name="heightStates">The height states to filter the BattleEntities by.
        /// The frontmost BattleEntity with any of the state will be included.
        /// If null or empty, will consider all height states.</param>
        /// <returns>The frontmost BattleEntity for the EntityType.
        /// If none exist that match the height states specified, then null.</returns>
        public BattleEntity GetFrontmostBattleEntity(EntityTypes entityType, params HeightStates[] heightStates)
        {
            List<BattleEntity> entities = GetEntitiesList(entityType);

            //Return immediately if either input is null
            if (entities == null || entities.Count == 0) return null;

            //Return the first in the list if there's no filter
            if (heightStates == null || heightStates.Length == 0) return entities[0];

            //Search for the first one with any of the HeightStates specified
            for (int i = 0; i < entities.Count; i++)
            {
                BattleEntity entity = entities[i];

                //Check if the BattleEntity has any of the specified HeightStates
                if (heightStates.Contains(entity.HeightState) == true)
                {
                    return entity;
                }
            }

            //A BattleEntity wasn't found
            return null;
        }

        #endregion
    }
}
