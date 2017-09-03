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
    public class BattleManager : IUpdateable, IDrawable, IDisposable
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

        private static BattleManager instance = null;

        #endregion

        #region Events

        public delegate void OnEnemyAdded(BattleEnemy battleEnemy);
        /// <summary>
        /// The event invoked after a <see cref="BattleEnemy"/> has been added to battle.
        /// <para>This is invoked after the enemy has been added to battle and the enemy count has been incremented.</para>
        /// </summary>
        public event OnEnemyAdded EnemyAddedEvent = null;

        #endregion

        #region Enumerations

        public enum BattlePhase
        {
            Player, Enemy
        }

        public enum BattleState
        {
            Init, Turn, TurnEnd, Done
        }

        #endregion

        //Starting positions
        private readonly Vector2 MarioPos = new Vector2(-150, 100);
        private readonly Vector2 PartnerPos = new Vector2(-190, 120);
        public readonly Vector2 EnemyStartPos = new Vector2(150, 125);
        public readonly int PositionXDiff = 30;
        
        //These are general values used by most entities in the air or on the ceiling
        //The entity can still configure how high it goes on its own if needed
        //In that case, make sure to update the entity's BattlePosition as well
        public readonly int AirborneY = 40;
        public readonly int CeilingY = 100;

        /// <summary>
        /// How many phase cycles (Player and Enemy turns) passed
        /// </summary>
        public int PhaseCycleCount { get; private set; } = 0;

        /// <summary>
        /// Whether certain UI, such as Status Effect icons and enemy HP, should show up or not.
        /// This UI shows up only when the Player is choosing an action.
        /// </summary>
        public bool ShouldShowPlayerTurnUI => (EntityTurn.EntityType == EntityTypes.Player && EntityTurn.PreviousAction?.MoveSequence.InSequence != true);

        /// <summary>
        /// The BattlePhase the battle starts on.
        /// </summary>
        //private const BattlePhase StartingPhase = BattlePhase.Player;

        /// <summary>
        /// Unless scripted, the battle always starts on the player phase, with Mario always going first
        /// </summary>
        private BattlePhase Phase = BattlePhase.Player;

        /// <summary>
        /// The current state of the battle
        /// </summary>
        public BattleState State { get; private set; } = BattleState.Init;

        /// <summary>
        /// The current entity going
        /// </summary>
        public BattleEntity EntityTurn { get; private set; } = null;
        private int EnemyTurn = 0;

        /// <summary>
        /// The BattlePlayer in the Front
        /// </summary>
        private BattlePlayer FrontPlayer = null;

        /// <summary>
        /// The BattlePlayer in the Back
        /// </summary>
        private BattlePlayer BackPlayer = null;

        /// <summary>
        /// Mario reference
        /// </summary>
        private BattleMario Mario = null;

        /// <summary>
        /// Partner reference
        /// </summary>
        private BattlePartner Partner = null;

        /// <summary>
        /// Enemy list. Enemies are displayed in order
        /// </summary>
        private List<BattleEnemy> Enemies = new List<BattleEnemy>(BattleGlobals.MaxEnemies);

        /// <summary>
        /// The number of enemies alive
        /// </summary>
        private int EnemiesAlive = 0;

        /// <summary>
        /// Helper property showing the max number of enemies
        /// </summary>
        private int MaxEnemies => Enemies.Capacity;

        /// <summary>
        /// Helper property telling whether enemy spots are available or not
        /// </summary>
        private bool EnemySpotsAvailable => (EnemiesAlive < MaxEnemies);

        private BattleManager()
        {
            SoundManager.Instance.SoundVolume = 0f;

            //TEMPORARY: For compatibility with the old array system until we migrate over completely
            for (int i = 0; i < MaxEnemies; i++)
            {
                Enemies.Add(null);
            }
        }

        public void Dispose()
        {
            EnemyAddedEvent = null;
        }

        /// <summary>
        /// Initializes the battle
        /// </summary>
        /// <param name="mario">Mario</param>
        /// <param name="partner">Mario's partner</param>
        /// <param name="enemies">The enemies, in order</param>
        public void Initialize(BattleMario mario, BattlePartner partner, List<BattleEnemy> enemies)
        {
            Mario = mario;
            Partner = partner;

            //Mario always starts out in the front, and the Partner always starts out in the back
            FrontPlayer = Mario;
            BackPlayer = Partner;

            Mario.Position = MarioPos;
            Mario.SetBattlePosition(MarioPos);

            //Start battle for Mario
            Mario.OnBattleStart();

            if (Partner != null)
            {
                Partner.Position = PartnerPos;
                Partner.SetBattlePosition(PartnerPos);

                //Start battle for the Partner
                Partner.OnBattleStart();
            }

            //Add and initialize enemies
            AddEnemies(enemies);

            StartBattle();
        }

        public void Update()
        {
            //NOTE: Create a general way to halt turns in battle in place of these hardcoded event check

            //Update battle events if there are any
            if (BattleEventManager.Instance.HasBattleEvents == true)
            {
                BattleEventManager.Instance.UpdateBattleEvents();
            }

            //If a turn just ended, update the current state
            if (State == BattleState.TurnEnd)
            {
                //Don't start the next turn until all Battle Events are finished
                if (BattleEventManager.Instance.HasBattleEvents == false)
                    TurnStart();
            }

            if (State == BattleState.Turn)
            {
                EntityTurn.TurnUpdate();
            }

            Mario.Update();
            Partner?.Update();

            for (int i = 0; i < MaxEnemies; i++)
            {
                Enemies[i]?.Update();
            }
        }

        public void Draw()
        {
            Mario.Draw();
            Partner?.Draw();

            for (int i = 0; i < MaxEnemies; i++)
            {
                Enemies[i]?.Draw();
            }

            SpriteRenderer.Instance.DrawText(AssetManager.Instance.TTYDFont, $"Current turn: {EntityTurn.Name}", new Vector2(250, 10), Color.White, 0f, Vector2.Zero, 1.3f, .2f);
        }

        /// <summary>
        /// Starts the Battle
        /// </summary>
        public void StartBattle()
        {
            ChangeBattleState(BattleState.TurnEnd);
            SwitchPhase(BattlePhase.Player);
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

            BattleEventManager.Instance.AddPendingEvents();
        }

        private void SwitchPhase(BattlePhase phase)
        {
            Phase = phase;

            if (Phase == BattlePhase.Player)
            {
                //Increment the phase cycles when switching to the Player phase
                //This is because the cycle always starts with the Player phase in the Paper Mario games
                PhaseCycleCount++;

                Debug.Log($"Started new phase cycle. Current cycle count: {PhaseCycleCount}");

                Mario.OnPhaseCycleStart();
                Partner.OnPhaseCycleStart();

                Mario.OnPhaseStart();
                Partner.OnPhaseStart();

                for (int i = 0; i < MaxEnemies; i++)
                {
                    Enemies[i]?.OnPhaseEnd();
                    Enemies[i]?.OnPhaseCycleStart();
                }

                //Reset the enemy that should go next
                EnemyTurn = 0;
            }
            else if (Phase == BattlePhase.Enemy)
            {                
                Mario.OnPhaseEnd();
                Partner.OnPhaseEnd();

                for (int i = 0; i < MaxEnemies; i++)
                {
                    Enemies[i]?.OnPhaseStart();
                }

                //Reset the enemy that should go next
                EnemyTurn = 0;
            }

            //NOTE: There's a bug: if all players and enemies have no turns, all BattleEvents will be delayed until one of them
            //has a turn. This is because it never gets to Update() since it searches for a turn, switches phases, then searches
            //for a turn again, switches phases again, and repeats.
            //This is easiest to replicate by inflicting everyone with Immobilized or a derived status (Ex. Frozen)

            FindNextEntityTurn();
        }

        /// <summary>
        /// Switches Mario and his Partner's battle positions and updates the Front and Back player references.
        /// <para>The actual players' positions are not changed here but in a Battle Event.</para>
        /// <see cref="SwapPositionBattleEvent"/>
        /// </summary>
        /// <param name="frontPlayer"></param>
        /// <param name="backPlayer"></param>
        private void SwitchPlayers(BattlePlayer frontPlayer, BattlePlayer backPlayer)
        {
            Vector2 frontBattlePosition = FrontPlayer.BattlePosition;
            Vector2 backBattlePosition = BackPlayer.BattlePosition;

            FrontPlayer.SetBattlePosition(backBattlePosition);
            BackPlayer.SetBattlePosition(frontBattlePosition);

            FrontPlayer = frontPlayer;
            BackPlayer = backPlayer;
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
            Partner.Position = oldPartner.Position;
            Partner.SetBattlePosition(oldPartner.BattlePosition);

            //Set the new Partner to use the same max number of turns all Partners have this phase cycle
            Partner.SetMaxTurns(BattlePartner.PartnerMaxTurns);

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

            //Check if the Partner is in the front or back and set the correct reference
            if (oldPartner == FrontPlayer)
            {
                FrontPlayer = Partner;
            }
            else if (oldPartner == BackPlayer)
            {
                BackPlayer = Partner;
            }
        }

        public void TurnStart()
        {
            if (State == BattleState.Done)
            {
                Debug.LogError($"Attemping to START turn when the battle is over!");
                return;
            }

            ChangeBattleState(BattleState.Turn);

            EntityTurn.StartTurn();
        }

        public void TurnEnd()
        {
            if (State == BattleState.Done)
            {
                Debug.LogError($"Attemping to END turn when the battle is over!");
                return;
            }

            EntityTurn.OnTurnEnd();

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
        /// Called when an entity dies to update the battle state and remove dead entities.
        /// Called in <see cref="DeathBattleEvent"/>.
        /// </summary>
        public void HandleEntityDeaths()
        {
            CheckDeadEntities();
            UpdateBattleState();
        }

        /// <summary>
        /// Updates the battle state, checking if the battle should be over.
        /// It's game over if Mario has 0 HP, otherwise it's victory if no enemies are alive
        /// </summary>
        private void UpdateBattleState()
        {
            if (Mario.IsDead == true)
            {
                ChangeBattleState(BattleState.Done);
                Debug.Log("GAME OVER");
            }
            else if (EnemiesAlive <= 0)
            {
                ChangeBattleState(BattleState.Done);
                Mario.AnimManager.PlayAnimation(AnimationGlobals.VictoryName);
                Partner.AnimManager.PlayAnimation(AnimationGlobals.VictoryName);
                Debug.Log("VICTORY");
            }
        }

        /// <summary>
        /// Checks for dead entities and handles them accordingly
        /// </summary>
        private void CheckDeadEntities()
        {
            //if (Mario.IsDead == true)
            //{
            //    Mario.PlayAnimation(AnimationGlobals.DeathName);
            //}
            if (Partner.IsDead == true)
            {
                //If the Partner died and is in front, switch places with Mario
                if (Partner == FrontPlayer)
                {
                    //Queue the event to switch Mario with his Partner
                    BattleEventManager.Instance.QueueBattleEvent((int)BattleGlobals.StartEventPriorities.Stage, new BattleState[] { BattleState.Turn, BattleState.TurnEnd },
                        new SwapPositionBattleEvent(FrontPlayer, BackPlayer, BackPlayer.BattlePosition, FrontPlayer.BattlePosition, 500f));

                    //Switch Mario in front
                    SwitchToTurn(PlayerTypes.Mario);
                }
                
                //NOTE: Don't play this animation again if the Partner is still dead
                //Partner.PlayAnimation(AnimationGlobals.DeathName);
            }

            List<BattleEnemy> deadEnemies = new List<BattleEnemy>();

            for (int i = 0; i < MaxEnemies; i++)
            {
                if (Enemies[i] != null && Enemies[i].IsDead == true)
                {
                    deadEnemies.Add(Enemies[i]);
                }
            }

            //Remove enemies from battle here
            RemoveEnemies(deadEnemies, true);
        }

        /// <summary>
        /// Finds the next BattleEntity that should go.
        /// </summary>
        private void FindNextEntityTurn()
        {
            //Enemy phase
            if (Phase == BattlePhase.Enemy)
            {
                //Look through all enemies, starting from the current
                //Find the first one that has turns remaining
                int nextEnemy = -1;
                for (int i = EnemyTurn; i < Enemies.Count; i++)
                {
                    if (Enemies[i] != null && Enemies[i].UsedTurn == false && Enemies[i].IsDead == false)
                    {
                        nextEnemy = EnemyTurn = i;
                        EntityTurn = Enemies[nextEnemy];
                        break;
                    }
                }

                //If all enemies are done with their turns, go to the player phase
                if (nextEnemy < 0)
                {
                    SwitchPhase(BattlePhase.Player);
                }
            }
            //Player phase
            else
            {
                //If the front player has turns remaining, it goes up next
                if (FrontPlayer.UsedTurn == false)
                {
                    EntityTurn = FrontPlayer;
                }
                //Next check the back player - if it has turns remaining, it goes up
                //The dead check is only for the BackPlayer because any dead Partners
                //get moved to the back. If Mario dies, it shouldn't get here because
                //the battle would be over
                else if (BackPlayer.UsedTurn == false && BackPlayer.IsDead == false)
                {
                    EntityTurn = BackPlayer;
                }
                //Neither player has turns remaining, so go to the enemy phase
                else
                {
                    SwitchPhase(BattlePhase.Enemy);
                }
            }
        }

        #region Helper Methods

        /// <summary>
        /// Adds enemies to battle
        /// </summary>
        /// <param name="enemies">A list containing the enemies to add to battle</param>
        public void AddEnemies(List<BattleEnemy> enemies)
        {
            //Look through all enemies and add one to the specified position
            for (int i = 0; i < enemies.Count; i++)
            {
                if (EnemySpotsAvailable == false)
                {
                    Debug.LogError($"Cannot add enemy {enemies[i].Name} because there are no available spots left in battle! Exiting loop!");
                    break;
                }
                int index = FindAvailableEnemyIndex(0);

                BattleEnemy enemy = enemies[i];

                //Set reference and position, then increment the number alive
                Enemies[index] = enemy;

                Vector2 battlepos = EnemyStartPos + new Vector2(PositionXDiff * index, 0);
                if (enemy.HeightState == HeightStates.Airborne) battlepos.Y -= AirborneY;
                else if (enemy.HeightState == HeightStates.Ceiling) battlepos.Y -= CeilingY;

                enemy.Position = battlepos;
                enemy.SetBattlePosition(battlepos);
                enemy.SetBattleIndex(index);

                //Start battle for the enemy
                enemy.OnBattleStart();

                IncrementEnemiesAlive();

                //Call the enemy added event
                EnemyAddedEvent?.Invoke(enemy);
            }
        }

        /// <summary>
        /// Removes enemies from battle
        /// </summary>
        /// <param name="enemies">A list containing the enemies to remove from battle</param>
        /// <param name="removedFromDeath">Whether the enemies are removed because they died in battle. If true, will play the death sound.</param>
        public void RemoveEnemies(List<BattleEnemy> enemies, bool removedFromDeath)
        {
            //Go through all the enemies and remove them from battle
            for (int i = 0; i < enemies.Count; i++)
            {
                if (EnemiesAlive == 0)
                {
                    Debug.LogError($"No enemies currently alive in battle so removing is impossible!");
                    return;
                }

                int enemyIndex = enemies[i].BattleIndex;
                if (enemyIndex < 0)
                {
                    Debug.LogError($"Enemy {enemies[i].Name} cannot be removed from battle because it isn't in battle!");
                    continue;
                }

                //Set to null and decrease number alive
                Enemies[enemyIndex] = null;
                DecrementEnemiesAlive();

                if (removedFromDeath)
                {
                    SoundManager.Instance.PlaySound(SoundManager.Sound.EnemyDeath);
                }
            }
        }

        /// <summary>
        /// Returns all entities of a specified type in a list.
        /// This method is used internally in the BattleManager to allow for easy manipulation of the returned list.
        /// </summary>
        /// <param name="entityType">The type of entities to return</param>
        /// <param name="heightStates">The height states to filter entities by. Entities with any of the state will be included.
        /// If null, will include entities of all height states</param>
        /// <returns>Entities matching the type and height states specified</returns>
        private List<BattleEntity> GetEntitiesList(EntityTypes entityType, params HeightStates[] heightStates)
        {
            List<BattleEntity> entities = new List<BattleEntity>();

            if (entityType == EntityTypes.Enemy)
            {
                entities.AddRange(GetAliveEnemies());
            }
            else if (entityType == EntityTypes.Player)
            {
                //To be consistent, go from left to right
                entities.Add(BackPlayer);
                entities.Add(FrontPlayer);
            }

            //Filter by height states
            FilterEntitiesByHeights(entities, heightStates);

            return entities;
        }

        /// <summary>
        /// Returns all entities of a specified type in an array
        /// </summary>
        /// <param name="entityType">The type of entities to return</param>
        /// <param name="heightStates">The height states to filter entities by. Entities with any of the state will be included.
        /// If null, will include entities of all height states</param>
        /// <returns>Entities matching the type and height states specified</returns>
        public BattleEntity[] GetEntities(EntityTypes entityType, params HeightStates[] heightStates)
        {
            List<BattleEntity> entities = GetEntitiesList(entityType, heightStates);
            return entities.ToArray();
        }

        /// <summary>
        /// Returns all entities of a specified type in an array.
        /// The entities are returned in reverse order. Entities in the back are the first elements in the array.
        /// </summary>
        /// <param name="entityType">The type of entities to return.</param>
        /// <param name="heightStates">The height states to filter entities by. Entities with any of the state will be included.
        /// If null, will include entities of all height states.</param>
        /// <returns>Entities matching the type and height states specified, in reverse.</returns>
        public BattleEntity[] GetEntitiesReversed(EntityTypes entityType, params HeightStates[] heightStates)
        {
            List<BattleEntity> entities = GetEntitiesList(entityType, heightStates);
            entities.Reverse();
            return entities.ToArray();
        }

        /// <summary>
        /// Returns all alive enemies in an array
        /// </summary>
        /// <returns>An array of all alive enemies. An empty array is returned if no enemies are alive</returns>
        private BattleEnemy[] GetAliveEnemies()
        {
            List<BattleEnemy> aliveEnemies = new List<BattleEnemy>();

            for (int i = 0; i < MaxEnemies; i++)
            {
                if (Enemies[i] != null)
                {
                    aliveEnemies.Add(Enemies[i]);
                }
            }

            return aliveEnemies.ToArray();
        }

        public BattleMario GetMario()
        {
            return Mario;
        }

        public BattlePartner GetPartner()
        {
            return Partner;
        }

        public BattlePlayer GetFrontPlayer()
        {
            return FrontPlayer;
        }

        public BattlePlayer GetBackPlayer()
        {
            return BackPlayer;
        }

        /// <summary>
        /// Filters a set of entities by specified height states. This method is called internally by the BattleManager.
        /// </summary>
        /// <param name="entities">The list of entities to filter. This list is modified directly.</param>
        /// <param name="heightStates">The height states to filter entities by. Entities with any of the state will be included.
        /// If null or empty, will return the entities passed in</param>
        private void FilterEntitiesByHeights(List<BattleEntity> entities, params HeightStates[] heightStates)
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
        /// Filters out dead BattleEntities from a set
        /// </summary>
        /// <param name="entities">The BattleEntities to filter</param>
        /// <returns>An array of all the alive BattleEntities</returns>
        public BattleEntity[] FilterDeadEntities(BattleEntity[] entities)
        {
            if (entities == null || entities.Length == 0) return entities;

            List<BattleEntity> aliveEntities = new List<BattleEntity>(entities);

            for (int i = 0; i < aliveEntities.Count; i++)
            {
                if (aliveEntities[i].IsDead == true)
                {
                    aliveEntities.RemoveAt(i);
                    i--;
                }
            }

            return aliveEntities.ToArray();
        }

        /// <summary>
        /// Gets all allies of a particular BattleEntity.
        /// </summary>
        /// <param name="entity">The BattleEntity whose allies to get</param>
        /// <param name="heightStates">The height states to filter entities by. Entities with any of the state will be included.
        /// If null or empty, will return the entities passed in</param>
        /// <returns>An array of allies the BattleEntity has</returns>
        public BattleEntity[] GetEntityAllies(BattleEntity entity, params HeightStates[] heightStates)
        {
            List<BattleEntity> allies = GetEntitiesList(entity.EntityType, heightStates);
            allies.Remove(entity);

            //Return all allies
            return allies.ToArray();
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

            //If the entity is an enemy, it can either be two Enemies or the front Player and another Enemy
            if (entity.EntityType == EntityTypes.Enemy)
            {
                BattleEnemy enemy = entity as BattleEnemy;

                int enemyIndex = enemy.BattleIndex;
                int prevEnemyIndex = FindOccupiedEnemyIndex(enemyIndex - 1, true);
                int nextEnemyIndex = FindOccupiedEnemyIndex(enemyIndex + 1);

                //Check if there's an Enemy before this one
                if (prevEnemyIndex >= 0) adjacentEntities.Add(Enemies[prevEnemyIndex]);
                //There's no Enemy, so target the Front Player
                else adjacentEntities.Add(FrontPlayer);

                //Check if there's an Enemy after this one
                if (nextEnemyIndex >= 0) adjacentEntities.Add(Enemies[nextEnemyIndex]);
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
                    //NOTE: The dead check is being removed for consistency with other methods
                    //if (BackPlayer.IsDead == false)
                    //{
                        adjacentEntities.Add(BackPlayer);
                    //}

                    //Add the next enemy
                    int nextEnemy = FindOccupiedEnemyIndex(0);
                    if (nextEnemy >= 0) adjacentEntities.Add(Enemies[nextEnemy]);
                }
            }

            return adjacentEntities.ToArray();
        }

        /// <summary>
        /// Gets the BattleEntities behind a particular BattleEntity.
        /// </summary>
        /// <param name="entity">The BattleEntity to find entities behind</param>
        /// <returns>An array of BattleEntities behind the given one. If none are behind, an empty array.</returns>
        public BattleEntity[] GetEntitiesBehind(BattleEntity entity)
        {
            List<BattleEntity> behindEntities = new List<BattleEntity>();

            //If it's a Player, check if the entity is in the front or the back
            if (entity.EntityType == EntityTypes.Player)
            {
                //If the entity is in the front, return the Back player
                if (entity == FrontPlayer)
                    behindEntities.Add(BackPlayer);
            }
            else
            {
                BattleEnemy battleEnemy = entity as BattleEnemy;

                //Get this enemy's BattleIndex
                int enemyIndex = battleEnemy.BattleIndex;

                //Look for all enemies with a BattleIndex greater than this one, which indicates it's behind
                for (int i = 0; i < Enemies.Count; i++)
                {
                    BattleEnemy enemy = Enemies[i];
                    if (enemy != null && enemy.BattleIndex > enemyIndex)
                    {
                        behindEntities.Add(enemy);
                    }
                }
            }

            return behindEntities.ToArray();
        }

        /// <summary>
        /// Finds the next available enemy index
        /// </summary>
        /// <param name="start">The index to start searching from</param>
        /// <param name="backwards">Whether to search backwards or not</param>
        /// <returns>The next available enemy index if found, otherwise -1</returns>
        private int FindAvailableEnemyIndex(int start, bool backwards = false)
        {
            //More code, but more readable too
            if (backwards == false)
            {
                for (int i = start; i < MaxEnemies; i++)
                {
                    if (Enemies[i] == null)
                        return i;
                }
            }
            else
            {
                for (int i = start; i >= 0; i--)
                {
                    if (Enemies[i] == null)
                        return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// Finds the next occupied enemy index
        /// </summary>
        /// <param name="start">The index to start searching from</param>
        /// <param name="backwards">Whether to search backwards or not</param>
        /// <returns>The next occupied enemy index if found, otherwise -1</returns>
        private int FindOccupiedEnemyIndex(int start, bool backwards = false)
        {
            //More code, but more readable too
            if (backwards == false)
            {
                for (int i = start; i < MaxEnemies; i++)
                {
                    if (Enemies[i] != null)
                        return i;
                }
            }
            else
            {
                for (int i = start; i >= 0; i--)
                {
                    if (Enemies[i] != null)
                        return i;
                }
            }

            return -1;
        }

        private void IncrementEnemiesAlive()
        {
            EnemiesAlive++;
            if (EnemiesAlive > MaxEnemies) EnemiesAlive = MaxEnemies;
        }

        private void DecrementEnemiesAlive()
        {
            EnemiesAlive--;
            if (EnemiesAlive < 0) EnemiesAlive = 0;
        }

        /// <summary>
        /// Gets the position in front of an entity's battle position
        /// </summary>
        /// <param name="entity">The entity to get the position in front of</param>
        /// <returns>A Vector2 with the position in front of the entity</returns>
        public Vector2 GetPositionInFront(BattleEntity entity)
        {
            Vector2 xdiff = new Vector2(PositionXDiff, 0f);
            if (entity.EntityType == EntityTypes.Enemy) xdiff.X = -xdiff.X;

            return (entity.BattlePosition + xdiff);
        }

        /// <summary>
        /// Sorts enemies by battle indices, with lower indices appearing first
        /// </summary>
        private int SortEnemiesByBattleIndex(BattleEnemy enemy1, BattleEnemy enemy2)
        {
            if (enemy1 == null)
                return 1;
            else if (enemy2 == null)
                return -1;

            //Compare battle indices
            if (enemy1.BattleIndex < enemy2.BattleIndex)
                return -1;
            else if (enemy1.BattleIndex > enemy2.BattleIndex)
                return 1;

            return 0;
        }

        /// <summary>
        /// Tells whether one BattleEntity is in front of another.
        /// </summary>
        /// <param name="behindEntity">The BattleEntity that is supposedly behind <paramref name="frontEntity"/>.</param>
        /// <param name="frontEntity">The BattleEntity that is supposedly in front of <paramref name="behindEntity"/>.</param>
        /// <returns></returns>
        public bool IsEntityInFrontOf(BattleEntity behindEntity, BattleEntity frontEntity)
        {
            //If the entities aren't the same type of BattleEntity or if they're the same BattleEntity, then ignore
            if (behindEntity.EntityType != frontEntity.EntityType || behindEntity == frontEntity)
                return false;
            
            //For players, check if the front entity is the front player
            if (behindEntity.EntityType == EntityTypes.Player)
            {
                if (frontEntity == FrontPlayer)
                {
                    return true;
                }
            }
            else
            {
                //For enemies, compare the front entity's BattleIndex with the one behind
                //If the front entity's BattleIndex is lower, then it's in front
                BattleEnemy behindEnemy = (BattleEnemy)behindEntity;
                BattleEnemy frontEnemy = (BattleEnemy)frontEntity;

                return (frontEnemy.BattleIndex < behindEnemy.BattleIndex);
            }

            //For all other cases, return false
            return false;
        }

        /// <summary>
        /// Returns the closest BattleEntity in front of a specified one.
        /// <para>For players this would return whoever is in front if the entity specified is in the back.
        /// For enemies this would return the closest enemy in front of the specified one.</para>
        /// </summary>
        /// <param name="battleEntity">The BattleEntity to find the entity in front of.</param>
        /// <returns>The closest BattleEntity in front of the specified one. null if no BattleEntity is in front of the specified one.</returns>
        public BattleEntity GetEntityInFrontOf(BattleEntity battleEntity)
        {
            //Players
            if (battleEntity.EntityType == EntityTypes.Player)
            {
                //If this player is in the back, return the player in the front
                if (battleEntity == BackPlayer)
                {
                    return FrontPlayer;
                }
            }
            //Enemies
            else if (battleEntity.EntityType == EntityTypes.Enemy)
            {
                //Check for an enemy with a lower BattleIndex than this one
                BattleEnemy enemy = (BattleEnemy)battleEntity;
                int inFrontEnemy = FindOccupiedEnemyIndex(enemy.BattleIndex - 1, true);

                //We found the enemy in front of this one; return it
                if (inFrontEnemy >= 0)
                {
                    return Enemies[inFrontEnemy];
                }
            }

            //There is no BattleEntity in front of this one, so return null
            return null;
        }

        #endregion
    }
}
