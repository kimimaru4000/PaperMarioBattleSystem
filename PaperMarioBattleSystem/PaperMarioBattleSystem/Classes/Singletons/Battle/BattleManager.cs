using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using static PaperMarioBattleSystem.Enumerations;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// Handles turns in battle
    /// <para>This is a Singleton</para>
    /// </summary>
    public class BattleManager
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
        private readonly Vector2 EnemyStartPos = new Vector2(150, 125);
        private readonly int EnemyXDiff = 30;

        /// <summary>
        /// Unless scripted, the battle always starts on the player phase, with Mario always going first
        /// </summary>
        private BattlePhase Phase = BattlePhase.Player;

        private BattleState State = BattleState.Init;

        /// <summary>
        /// The current entity going
        /// </summary>
        public BattleEntity EntityTurn = null;
        private int EnemyTurn = 0;

        /// <summary>
        /// Mario reference
        /// </summary>
        private BattleMario Mario = null;

        /// <summary>
        /// Partner reference
        /// </summary>
        private BattlePartner Partner = null;

        /// <summary>
        /// Enemy array. Enemies are displayed in the array order
        /// </summary>
        private BattleEnemy[] Enemies = new BattleEnemy[5];

        /// <summary>
        /// The number of enemies alive
        /// </summary>
        private int EnemiesAlive = 0;

        /// <summary>
        /// Helper property showing the max number of enemies
        /// </summary>
        private int MaxEnemies => Enemies.Length;

        /// <summary>
        /// Helper property telling whether enemy spots are available or not
        /// </summary>
        private bool EnemySpotsAvailable => (EnemiesAlive < MaxEnemies);

        private BattleManager()
        {
        
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

            Mario.Position = MarioPos;
            Mario.SetBattlePosition(MarioPos);
            if (Partner != null)
            {
                Partner.Position = PartnerPos;
                Partner.SetBattlePosition(PartnerPos);
            }

            //Add and initialize enemies
            AddEnemies(enemies);

            StartBattle();
        }

        public void Update()
        {
            if (State == BattleState.TurnEnd)
            {
                TurnStart();
            }

            if (State == BattleState.Turn)
            {
                EntityTurn.TurnUpdate();
            }

            Mario.Update();
            Partner?.Update();

            for (int i = 0; i < Enemies.Length; i++)
            {
                Enemies[i]?.Update();
            }
        }

        public void Draw()
        {
            Mario.Draw();
            Partner?.Draw();

            for (int i = 0; i < Enemies.Length; i++)
            {
                Enemies[i]?.Draw();
            }

            SpriteRenderer.Instance.DrawText(AssetManager.Instance.Font, $"Current turn: {EntityTurn.Name}", new Vector2(250, 10), Color.White, 0f, Vector2.Zero, 1.3f, .2f);
        }

        /// <summary>
        /// Starts the Battle
        /// </summary>
        public void StartBattle()
        {
            State = BattleState.TurnEnd;

            EntityTurn = Mario;
        }

        /// <summary>
        /// Ends the Battle
        /// </summary>
        public void EndBattle()
        {
            State = BattleState.Done;
        }

        private void SwitchPhase(BattlePhase phase)
        {
            Phase = phase;

            if (Phase == BattlePhase.Player)
            {
                EntityTurn = Mario;
            }
            else if (Phase == BattlePhase.Enemy)
            {
                Mario.UsedTurn = false;
                if (Partner != null) Partner.UsedTurn = false;

                EnemyTurn = FindOccupiedEnemyIndex(0);
                EntityTurn = Enemies[EnemyTurn];
            }
        }

        /// <summary>
        /// Switches Mario and his Partner's turn. This cannot be performed if either have already used their turn
        /// </summary>
        /// <param name="partner"></param>
        public void SwitchToTurn(bool partner)
        {
            if (partner == true)
            {
                if (Partner.UsedTurn == true)
                {
                    Debug.LogError($"Cannot swap turns with {Partner.Name} because he/she already used his/her turn!");
                    return;
                }

                //Perform Mario-specific turn end logic
                EntityTurn.OnTurnEnd();
                EntityTurn = Partner;
            }
            else
            {
                if (Mario.UsedTurn == true)
                {
                    Debug.LogError($"Cannot swap turns with Mario because he already used his turn!");
                    return;
                }

                //Perform Partner-specific turn end logic
                EntityTurn.OnTurnEnd();
                EntityTurn = Mario;
            }

            //Perform Mario or Partner-specific turn start logic
            EntityTurn.OnTurnStart();
        }

        public void TurnStart()
        {
            if (State == BattleState.Done)
            {
                Debug.LogError($"Attemping to START turn when the battle is over!");
                return;
            }

            State = BattleState.Turn;

            EntityTurn.OnTurnStart();
        }

        public void TurnEnd()
        {
            EntityTurn.OnTurnEnd();

            UpdateBattleState();

            if (State == BattleState.Done)
            {
                Debug.LogError($"Attemping to END turn when the battle is over!");
                return;
            }

            State = BattleState.TurnEnd;

            if (Phase == BattlePhase.Enemy)
            {
                int nextEnemy = FindOccupiedEnemyIndex(EnemyTurn + 1);
                if (nextEnemy < 0)
                {
                    SwitchPhase(BattlePhase.Player);
                }
                else
                {
                    EnemyTurn = nextEnemy;
                    EntityTurn = Enemies[nextEnemy];
                }
            }
            else
            {
                if (Mario.UsedTurn)
                {
                    if (Partner != null && Partner.UsedTurn == false)
                    {
                        EntityTurn = Partner;
                    }
                    else SwitchPhase(BattlePhase.Enemy);
                }
                else
                {
                    if (Mario.UsedTurn == false)
                        EntityTurn = Mario;
                    else SwitchPhase(BattlePhase.Enemy);
                }
            }
        }

        /// <summary>
        /// Updates the battle state, checking if the battle should be over.
        /// It's game over if Mario has 0 HP, otherwise it's victory
        /// </summary>
        private void UpdateBattleState()
        {
            if (Mario.IsDead == true)
            {
                State = BattleState.Done;
                Debug.Log("GAME OVER");
            }
            else if (EnemiesAlive <= 0)
            {
                State = BattleState.Done;
                Debug.Log("VICTORY");
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
                    Debug.LogError($"Cannot add enemy {enemies[i].Name} because there is no available spots left in battle! Exiting loop!");
                    break;
                }
                int index = FindAvailableEnemyIndex(0);

                BattleEnemy enemy = enemies[i];

                //Set reference and position, then increment the number alive
                Enemies[index] = enemy;

                Vector2 battlepos = EnemyStartPos + new Vector2(EnemyXDiff * index, 0);
                enemy.Position = battlepos;
                enemy.SetBattlePosition(battlepos);

                IncrementEnemiesAlive();
            }
        }

        /// <summary>
        /// Removes enemies from battle
        /// </summary>
        /// <param name="enemies">A list containing the enemies to remove from battle</param>
        public void RemoveEnemies(List<BattleEnemy> enemies)
        {
            //Go through all the enemies and remove them from battle
            for (int i = 0; i < enemies.Count; i++)
            {
                if (EnemiesAlive == 0)
                {
                    Debug.LogError($"No enemies currently alive in battle so removing is impossible!");
                    return;
                }

                int enemyIndex = FindIndexFromEnemy(enemies[i]);
                if (enemyIndex < 0)
                {
                    Debug.LogError($"Enemy {enemies[i].Name} cannot be removed from battle because it isn't in battle!");
                    continue;
                }

                //Set to null and decrease number alive
                Enemies[enemyIndex] = null;
                DecrementEnemiesAlive();
            }
        }

        /// <summary>
        /// Returns all entities of a specified type in an array
        /// </summary>
        /// <param name="entityType">The type of entities to return</param>
        /// <returns>Mario and his Partner if entityType is Player, otherwise all alive enemies</returns>
        public BattleEntity[] GetEntities(EntityTypes entityType)
        {
            List<BattleEntity> entities = new List<BattleEntity>();

            if (entityType == EntityTypes.Enemy)
            {
                return GetAliveEnemies();
            }
            else if (entityType == EntityTypes.Player)
            {
                entities.Add(Mario);
                entities.Add(Partner);
            }

            return entities.ToArray();
        }

        /// <summary>
        /// Returns all alive enemies in an array
        /// </summary>
        /// <returns>An array of all alive enemies. An empty array is returned if no enemies are alive</returns>
        private BattleEnemy[] GetAliveEnemies()
        {
            List<BattleEnemy> aliveEnemies = new List<BattleEnemy>();

            for (int i = 0; i < Enemies.Length; i++)
            {
                if (Enemies[i] != null)
                {
                    aliveEnemies.Add(Enemies[i]);
                }
            }

            return aliveEnemies.ToArray();
        }

        /// <summary>
        /// Finds the next available enemy index
        /// </summary>
        /// <param name="start">The index to start searching from</param>
        /// <returns>The next available enemy index if found, otherwise -1</returns>
        private int FindAvailableEnemyIndex(int start)
        {
            for (int i = start; i < MaxEnemies; i++)
            {
                if (Enemies[i] == null)
                    return i;
            }

            return -1;
        }

        /// <summary>
        /// Finds the next occupied enemy index
        /// </summary>
        /// <param name="start">The index to start searching from</param>
        /// <returns>The next occupied enemy index if found, otherwise -1</returns>
        private int FindOccupiedEnemyIndex(int start)
        {
            for (int i = start; i < MaxEnemies; i++)
            {
                if (Enemies[i] != null)
                    return i;
            }

            return -1;
        }

        /// <summary>
        /// Finds the index an enemy is located at
        /// </summary>
        /// <param name="enemy">The enemy in question</param>
        /// <returns>The index of the enemy if it's in battle, otherwise -1</returns>
        private int FindIndexFromEnemy(BattleEnemy enemy)
        {
            for (int i = 0; i < Enemies.Length; i++)
            {
                BattleEnemy Enemy = Enemies[i];
                if (Enemy != null && Enemy == enemy)
                    return i;
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

        #endregion
    }
}
