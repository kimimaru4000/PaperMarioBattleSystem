using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using static PaperMarioBattleSystem.Enumerations;
using static PaperMarioBattleSystem.BattleGlobals;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// Handles turns in battle
    /// <para>This is a Singleton</para>
    /// </summary>
    public class BattleManager : IUpdateable, /*IDrawable,*/ ICleanup
    {
        #region Singleton Fields

        public static BattleManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new BattleManager();
                }

                return instance;
            }
        }

        public static bool HasInstance => (instance != null);

        private static BattleManager instance = null;

        #endregion

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

        #region Enumerations

        public enum BattleState
        {
            Init, Turn, TurnEnd, Done
        }

        #endregion

        /// <summary>
        /// The BattleManager's event manager.
        /// </summary>
        public BattleEventManager battleEventManager { get; private set; } = new BattleEventManager();

        /// <summary>
        /// The battle's properties.
        /// </summary>
        public BattleProperties Properties { get; private set; } = default(BattleProperties);

        //Starting positions
        public readonly Vector2 MarioPos = new Vector2(-150, 100);
        public readonly Vector2 PartnerPos = new Vector2(-200, 100);
        public readonly Vector2 EnemyStartPos = new Vector2(150, 125);
        public readonly int PositionXDiff = 50;
        
        //These are general values used by most entities in the air or on the ceiling
        //The entity can still configure how high it goes on its own if needed
        //In that case, make sure to update the entity's BattlePosition as well
        public readonly int AirborneY = 40;
        public readonly int CeilingY = 100;

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
        private BattleEntity FrontPlayer => FindEntityFromBattleIndex(EntityTypes.Player, 0, true);

        /// <summary>
        /// The player in the Back.
        /// </summary>
        private BattleEntity BackPlayer => FindEntityFromBattleIndex(EntityTypes.Player, 1, false);

        /// <summary>
        /// Mario reference.
        /// </summary>
        private BattleMario Mario = null;

        /// <summary>
        /// Partner reference.
        /// </summary>
        private BattlePartner Partner = null;

        /// <summary>
        /// The number of enemies alive.
        /// </summary>
        private int EnemiesAlive
        {
            get
            {
                List<BattleEntity> enemies = GetEntitiesList(EntityTypes.Enemy);
                if (enemies != null)
                    return enemies.Count;

                return 0;
            }
        }

        private BattleManager()
        {
            
        }

        public void CleanUp()
        {
            State = BattleState.Done;

            //Remove and cleanup all BattleEntities in battle
            List<BattleEntity> removedEntities = GetAllEntitiesList();

            //Remove all entities
            RemoveEntities(removedEntities, true);

            Mario = null;
            Partner = null;

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
            Mario = mario;
            Partner = partner;

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

            //Initialize helper objects
            //Check for the battle setting and add darkness if so
            if (Properties.BattleSetting == BattleSettings.Dark)
            {
                BattleDarknessObj battleDarkness = new BattleDarknessObj();
                LightingManager.Instance.Initialize(battleDarkness);
                BattleObjManager.Instance.AddBattleObject(battleDarkness);
            }

            //Add the HP bar manager
            BattleObjManager.Instance.AddBattleObject(new HPBarManagerObj());

            //If you can't run from battle, show a message at the start saying so
            if (Properties.Runnable == false)
            {
                battleEventManager.QueueBattleEvent((int)BattleGlobals.BattleEventPriorities.Message, new BattleState[] { BattleState.Turn },
                    new MessageBattleEvent(BattleGlobals.NoRunMessage, MessageBattleEvent.DefaultWaitDuration));
            }

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

            if (State == BattleState.Turn)
            {
                EntityTurn.TurnUpdate();
            }

            //Update all BattleEntities
            UpdateEntities();
        }

        /*public void Draw()
        {
            //Draw all BattleEntities
            DrawEntities();
            
            //Draw the action the current BattleEntity is performing
            if (EntityTurn != null)
            {
                EntityTurn.PreviousAction?.Draw();
            }
            
            SpriteRenderer.Instance.DrawUIText(AssetManager.Instance.TTYDFont, $"Current turn: {EntityTurn.Name}", new Vector2(250, 10), Color.White, 0f, Vector2.Zero, 1.3f, .2f);
        }*/

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

        private void DrawEntities()
        {
            foreach (KeyValuePair<EntityTypes, List<BattleEntity>> entityPairs in AllEntities)
            {
                List<BattleEntity> entityList = entityPairs.Value;
                for (int i = 0; i < entityList.Count; i++)
                {
                    entityList[i].Draw();
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
        /// Switches Mario and his Partner's battle positions and battle indices.
        /// <para>The actual players' positions are not changed here but in a Battle Event.
        /// See <see cref="SwapPositionBattleEvent"/>.</para>
        /// </summary>
        /// <param name="frontPlayer"></param>
        /// <param name="backPlayer"></param>
        private void SwitchPlayers(BattlePlayer frontPlayer, BattlePlayer backPlayer)
        {
            BattleEntity curFront = FrontPlayer;
            BattleEntity curBack = BackPlayer;

            Vector2 frontBattlePosition = curFront.BattlePosition;
            Vector2 backBattlePosition = curBack.BattlePosition;

            int frontBattleIndex = curFront.BattleIndex;
            int backBattleIndex = curBack.BattleIndex;

            //Swap positions
            curFront.SetBattlePosition(new Vector2(backBattlePosition.X, frontBattlePosition.Y));
            curBack.SetBattlePosition(new Vector2(frontBattlePosition.X, backBattlePosition.Y));

            //Swap BattleIndex; the lists will be automatically sorted
            curFront.SetBattleIndex(backBattleIndex);
            curBack.SetBattleIndex(frontBattleIndex);
        }

        /// <summary>
        /// Switches Mario and his Partner's positions in battle.
        /// </summary>
        /// <param name="playerType">The PlayerTypes to switch to - either Mario or the Partner.</param>
        public void SwitchToTurn(PlayerTypes playerType)
        {
            if (playerType == PlayerTypes.Partner)
            {
                //Put the Partner in front and Mario in the back
                SwitchPlayers(Partner, Mario);
            }
            else
            {
                //Put Mario in front and the Partner in the back
                SwitchPlayers(Mario, Partner);
            }

            SoundManager.Instance.PlaySound(SoundManager.Sound.SwitchPartner);
        }

        /// <summary>
        /// Swaps out Mario's current Partner for a different one.
        /// </summary>
        /// <param name="newPartner">The new BattlePartner to take part in battle.</param>
        public void SwapPartner(BattlePartner newPartner)
        {
            BattlePartner oldPartner = Partner;

            Partner = newPartner;

            Vector2 offset = Vector2.Zero;

            //If the old Partner was airborne and the new one isn't, move the new one down
            if (oldPartner.HeightState == HeightStates.Airborne && Partner.HeightState != HeightStates.Airborne)
            {
                offset.Y += AirborneY;
            }
            //Otherwise, if the old Partner wasn't airborne and the new one is, move the new one up
            else if (oldPartner.HeightState != HeightStates.Airborne && Partner.HeightState == HeightStates.Airborne)
            {
                offset.Y -= AirborneY;
            }

            //Set positions to the old ones
            Partner.Position = oldPartner.Position;
            Partner.SetBattleIndex(oldPartner.BattleIndex);
            Partner.SetBattlePosition(oldPartner.BattlePosition + offset);

            //State the old Partner is out of battle
            oldPartner.SetBattleIndex(BattleGlobals.InvalidBattleIndex);

            //Set flip state
            Partner.SpriteFlip = oldPartner.SpriteFlip;

            //Remove the old partner from the entity dictionary and add the new one
            RemoveEntities(new BattleEntity[] { oldPartner }, false);
            AddEntities(new BattleEntity[] { Partner }, null, false);

            //Set the new Partner to use the same max number of turns all Partners have this phase cycle
            //The only exceptions are if the new partner doesn't move at all (Ex. Goompa) or is immobile
            //In this case, set its max turn count to 0
            if (Partner.BaseTurns > 0 && Partner.IsImmobile() == false)
            {
                Partner.SetMaxTurns(BattlePartner.PartnerMaxTurns);
            }
            else
            {
                Partner.SetMaxTurns(0);
            }

            //If the entity swapping out partners is the old one increment the turn count for the new partner,
            //as the old one's turn count will be incremented after the action is finished
            if (EntityTurn == oldPartner)
            {
                Partner.SetTurnsUsed(oldPartner.TurnsUsed + 1);
            }
            //Otherwise, the entity swapping out partners must be Mario, so set the new Partner's turn count to the old one's
            //(or an enemy via an attack, but none of those attacks exist in the PM games...I'm hinting at a new attack idea :P)
            else
            {
                Partner.SetTurnsUsed(oldPartner.TurnsUsed);
            }

            //Swap Partner badges with the new Partner
            BattlePartner.SwapPartnerBadges(oldPartner, Partner);
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
                    //Queue the event to switch Mario with his Partner
                    battleEventManager.QueueBattleEvent((int)BattleGlobals.BattleEventPriorities.Stage, new BattleState[] { BattleState.Turn, BattleState.TurnEnd },
                        new SwapPositionBattleEvent(FrontPlayer, BackPlayer,
                        new Vector2(BackPlayer.BattlePosition.X, FrontPlayer.BattlePosition.Y), new Vector2(FrontPlayer.BattlePosition.X, BackPlayer.BattlePosition.Y), 500f));

                    //Switch Mario in front
                    SwitchToTurn(PlayerTypes.Mario);
                }

            }

            //Players don't get removed from battle through normal death
            //Partners can be removed via a Status Effect that forces them out of battle, such as Fright or Gale Force
            //Mario's death results in a Game Over, which is checked when updating the battle state
            if (deadEntity.EntityType == EntityTypes.Player) return;

            //Remove this BattleEntity from battle
            RemoveEntities(new BattleEntity[] { deadEntity }, true);
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
        /// Adds a set of BattleEntities to battle.
        /// </summary>
        /// <param name="battleEntities">An <see cref="IList{T}"/> of BattleEntities to add.</param>
        /// <param name="battleIndices">An <see cref="IList{T}"/> of ints containing the BattleIndex to add each BattleEntity at.
        /// <para>If the IList is null, the value at the IList index is less than 0, or the IList index is out of the <paramref name="battleEntities"/>
        /// IList range, the BattleManager will assign the lowest available BattleIndex.</para></param>
        /// <param name="initialize">Whether to initialize the entities or not. Regardless of the value, it assigns a BattleIndex.
        /// If the entities have first joined battle, this should be true.
        /// This should be false in cases where entities have been removed and re-added to battle, such as switching Partners.</param>
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
                BattleEntity entity = battleEntities[i];

                //Don't add null BattleEntities
                if (entity == null)
                {
                    Debug.LogError($"Not adding null BattleEntity at index {i}");
                    continue;
                }

                EntityTypes entityType = entity.EntityType;

                //If no entry exists for this EntityType, add one
                if (AllEntities.ContainsKey(entityType) == false)
                {
                    AllEntities.Add(entityType, new List<BattleEntity>());
                }

                AllEntities[entityType].Add(entity);

                //Check to assign the BattleIndex from the list passed in
                int battleIndex = -1;
                if (battleIndices != null && i < battleIndices.Count)
                {
                    battleIndex = battleIndices[i];
                }
                //If the BattleIndex isn't valid, find the lowest available one for this EntityType
                if (BattleGlobals.IsValidBattleIndex(battleIndex) == false)
                {
                    battleIndex = FindLowestAvailableBattleIndex(entityType);
                }

                //Set battle index and start battle for the entity if it should
                entity.SetBattleIndex(battleIndex, false);

                if (initialize == true)
                {
                    entity.OnBattleStart();
                }

                //Increment BattleEntity count when added
                TotalEntityCount++;

                //Sort the list
                SortEntityList(entityType);

                //Invoke the entity added event
                EntityAddedEvent?.Invoke(entity);
            }
        }

        /// <summary>
        /// Removes a set of BattleEntities from battle.
        /// </summary>
        /// <param name="battleEntities">An <see cref="IList{T}"/> of BattleEntities to remove.</param>
        /// <param name="cleanUp">Whether to clean up the entities or not when removed from battle.
        /// This should be true if the entities are permanently removed from battle.
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
                BattleEntity entity = battleEntities[i];

                //Check for null entries
                if (entity == null)
                {
                    Debug.LogError($"Not removing null BattleEntity at index {i}");
                    continue;
                }

                EntityTypes entityType = entity.EntityType;

                //If no entities of this type are present, there's nothing to remove
                if (AllEntities.ContainsKey(entityType) == false)
                {
                    Debug.LogError($"Can't remove BattleEntities of type {entityType} since they're not in battle!");
                    continue;
                }

                bool removed = AllEntities[entityType].Remove(entity);

                //If there are no more entities left for this EntityType, remove the entry
                if (AllEntities[entityType].Count == 0)
                {
                    AllEntities.Remove(entityType);
                }

                //Clean up the entity if it should be cleaned up
                if (cleanUp == true)
                {
                    entity.CleanUp();

                    //Clear these references if the BattleEntity removed is Mario or his Partner
                    if (entity == Mario)
                    {
                        Mario = null;
                    }
                    if (entity == Partner)
                    {
                        Partner = null;
                    }
                }

                //Decrement BattleEntity count when removed
                if (removed == true)
                    TotalEntityCount--;

                //Invoke the entity removed event
                EntityRemovedEvent?.Invoke(entity);
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
        /// Returns all entities of a specified EntityType in a list.
        /// The returned list is the same reference found in the <see cref="AllEntities"/> dictionary.
        /// This method is used internally in the BattleManager.
        /// <para>NOTE: Be very careful with this list; modifying it outside of the appropriate methods may result in mismatched data.</para>
        /// </summary>
        /// <param name="entityType">The EntityType of entities to return.</param>
        /// <returns>Entities matching the EntityType specified. If none exist, null is returned.</returns>
        private List<BattleEntity> GetEntitiesList(EntityTypes entityType)
        {
            List<BattleEntity> entities = null;

            //Check if we have any BattleEntities of this type available
            if (AllEntities.ContainsKey(entityType) == true)
            {
                //Get the current list
                entities = AllEntities[entityType];
            }

            return entities;
        }

        /// <summary>
        /// Returns all entities of a specified EntityType in a new list.
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
            FilterEntitiesByHeights(entities, heightStates);

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
                List<BattleEntity> existingList = GetEntitiesList(entityTypes[i]);
                if (existingList != null)
                {
                    count += existingList.Count;
                }
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
        public BattleEntity[] GetAllEntities(params HeightStates[] heightStates)
        {
            List<BattleEntity> allentities = GetAllEntitiesList();

            //Filter by height states
            FilterEntitiesByHeights(allentities, heightStates);

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

            FilterEntitiesByHeights(entityList, heightStates);
        }

        /// <summary>
        /// Returns the number of BattleEntities of the specified EntityType are participating in battle.
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
        /// Returns all entities of a specified EntityType in an array.
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

            FilterEntitiesByHeights(entityList, heightStates);
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

        public BattleMario GetMario()
        {
            return Mario;
        }

        public BattlePartner GetPartner()
        {
            return Partner;
        }

        public BattleEntity GetFrontPlayer()
        {
            return FrontPlayer;
        }

        public BattleEntity GetBackPlayer()
        {
            return BackPlayer;
        }

        /// <summary>
        /// Filters a set of entities by specified height states. This method is called internally by the BattleManager.
        /// </summary>
        /// <param name="entities">The list of entities to filter. This list is modified directly.</param>
        /// <param name="heightStates">The height states to filter entities by. Entities with any of the state will be included.
        /// If null or empty, will return the entities passed in</param>
        public void FilterEntitiesByHeights(List<BattleEntity> entities, params HeightStates[] heightStates)
        {
            //Return immediately if either input is null
            if (entities == null || heightStates == null || heightStates.Length == 0) return;

            for (int i = 0; i < entities.Count; i++)
            {
                BattleEntity entity = entities[i];

                //Remove the entity if it wasn't in any of the height states passed in
                if (heightStates.Contains(entity.HeightState) == false)
                {
                    entities.RemoveAt(i);
                    i--;
                }
            }
        }

        /// <summary>
        /// Filters a set of entities by specified height states
        /// </summary>
        /// <param name="entities">The array of entities to filter</param>
        /// <param name="heightStates">The height states to filter entities by. Entities with any of the state will be included.
        /// If null or empty, will return the entities passed in</param>
        /// <returns>An array of BattleEntities filtered by HeightStates</returns>
        public BattleEntity[] FilterEntitiesByHeights(BattleEntity[] entities, params HeightStates[] heightStates)
        {
            if (entities == null || entities.Length == 0 || heightStates == null || heightStates.Length == 0) return entities;

            List<BattleEntity> filteredEntities = new List<BattleEntity>(entities);
            FilterEntitiesByHeights(filteredEntities, heightStates);

            return filteredEntities.ToArray();
        }

        /// <summary>
        /// Filters out BattleEntities marked as Untargetable from a set of BattleEntities.
        /// This method is called internally by the BattleManager.
        /// </summary>
        /// <param name="entities">The list of BattleEntities to filter. The list is modified directly.</param>
        public void FilterEntitiesByTargetable(List<BattleEntity> entities)
        {
            //Return if the list is null
            if (entities == null) return;

            for (int i = 0; i < entities.Count; i++)
            {
                BattleEntity entity = entities[i];

                //Check if the entity has the Untargetable additional property
                bool untargetable = entity.EntityProperties.HasAdditionalProperty(AdditionalProperty.Untargetable);

                //If it's untargetable, remove it
                if (untargetable == true)
                {
                    entities.RemoveAt(i);
                    i--;
                }
            }
        }

        /// <summary>
        /// Filters out BattleEntities marked as Untargetable from a set of BattleEntities.
        /// </summary>
        /// <param name="entities">The array of BattleEntities to filter.</param>
        /// <returns>An array of BattleEntities filtered by untargetable.</returns>
        public BattleEntity[] FilterEntitiesByTargetable(BattleEntity[] entities)
        {
            if (entities == null || entities.Length == 0) return entities;

            List<BattleEntity> filteredEntities = new List<BattleEntity>(entities);
            FilterEntitiesByTargetable(filteredEntities);

            return filteredEntities.ToArray();
        }

        /// <summary>
        /// Filters out dead BattleEntities from a set.
        /// </summary>
        /// <param name="entities">The BattleEntities to filter.</param>
        /// <returns>An array of all the alive BattleEntities.</returns>
        public BattleEntity[] FilterDeadEntities(BattleEntity[] entities)
        {
            if (entities == null || entities.Length == 0) return entities;

            List<BattleEntity> aliveEntities = new List<BattleEntity>(entities);
            FilterDeadEntities(aliveEntities);

            //for (int i = 0; i < aliveEntities.Count; i++)
            //{
            //    if (aliveEntities[i].IsDead == true)
            //    {
            //        aliveEntities.RemoveAt(i);
            //        i--;
            //    }
            //}

            return aliveEntities.ToArray();
        }

        /// <summary>
        /// Filters out dead BattleEntities from a set.
        /// </summary>
        /// <param name="entities">The BattleEntities to filter.</param>
        public void FilterDeadEntities(List<BattleEntity> entities)
        {
            if (entities == null || entities.Count == 0) return;

            for (int i = 0; i < entities.Count; i++)
            {
                if (entities[i].IsDead == true)
                {
                    entities.RemoveAt(i);
                    i--;
                }
            }
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
        private int[] FindBattleIndexGaps(EntityTypes entityType)
        {
            //No BattleEntities are in this list, so nothing can be returned
            if (AllEntities.ContainsKey(entityType) == false)
            {
                return Array.Empty<int>();
            }

            List<int> gaps = new List<int>();
            int prevIndex = -1;

            List<BattleEntity> entities = AllEntities[entityType];
            for (int i = 0; i < entities.Count; i++)
            {
                int battleIndex = entities[i].BattleIndex;

                //Look for gaps in the index; if the difference is 2 or greater, then
                //the values in between prevIndex and battleIndex are gaps
                int diffIndex = battleIndex - prevIndex;

                for (int j = 1; j < diffIndex; j++)
                {
                    gaps.Add(prevIndex + j);
                }

                //Set previous index for comparison with the next BattleEntity
                prevIndex = battleIndex;
            }

            return gaps.ToArray();
        }

        /// <summary>
        /// Finds the lowest available Battle Index for a given EntityType.
        /// </summary>
        /// <param name="entityType">The type of BattleEntities to find a Battle Index for.</param>
        /// <returns>An integer representing the next available Battle Index.
        /// If no BattleEntities of the specified type are in battle, -1 will be returned.</returns>
        private int FindLowestAvailableBattleIndex(EntityTypes entityType)
        {
            //No BattleEntities are in this list, so nothing can be returned
            if (AllEntities.ContainsKey(entityType) == false)
            {
                return -1;
            }

            int prevIndex = -1;
            int highestIndex = -1;

            List<BattleEntity> entities = AllEntities[entityType];
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
            //No BattleEntities are in this list, so nothing can be returned
            if (AllEntities.ContainsKey(entityType) == false)
            {
                return null;
            }

            List<BattleEntity> entities = GetEntitiesList(entityType);

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
        /// Gets the position in front of an entity's battle position
        /// </summary>
        /// <param name="entity">The entity to get the position in front of</param>
        /// <param name="fromLeftSide">Whether the front refers to the left side of the BattleEntity.</param>
        /// <returns>A Vector2 with the position in front of the entity</returns>
        public Vector2 GetPositionInFront(BattleEntity entity, bool fromLeftSide)
        {
            Vector2 xdiff = new Vector2(PositionXDiff, 0f);
            if (fromLeftSide == true) xdiff.X = -xdiff.X;

            return (entity.BattlePosition + xdiff);
        }

        /// <summary>
        /// Tells whether one BattleEntity is in front of another.
        /// </summary>
        /// <param name="behindEntity">The BattleEntity that is supposedly behind <paramref name="frontEntity"/>.</param>
        /// <param name="frontEntity">The BattleEntity that is supposedly in front of <paramref name="behindEntity"/>.</param>
        /// <returns></returns>
        public bool IsEntityInFrontOf(BattleEntity behindEntity, BattleEntity frontEntity)
        {
            //Compare BattleIndex - the entity behind will have a higher BattleIndex
            return (behindEntity.BattleIndex > frontEntity.BattleIndex);
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
