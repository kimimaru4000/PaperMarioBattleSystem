using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static PaperMarioBattleSystem.Enumerations;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// Any fighter that takes part in battle
    /// </summary>
    public abstract class BattleEntity
    {
        protected Stats BStats;

        public Stats BattleStats { get { return BStats; } }
        public int CurHP => BattleStats.HP;
        public int CurFP => BattleStats.FP;

        public string Name { get; protected set; } = "Entity";
        public Vector2 Position { get; set; } = Vector2.Zero;
        public float Rotation { get; set; } = 0f;
        public float Scale { get; set; } = 1f;

        public EntityTypes EntityType { get; protected set; } = EntityTypes.Enemy;

        public Texture2D SpriteSheet = null;
        
        public bool IsDead => CurHP <= 0;

        //TEMPORARY
        public bool UsedTurn = false;

        protected BattleEntity()
        {

        }

        protected BattleEntity(Stats stats)
        {
            BStats = stats;
        }

        #region Stat Manipulations

        public virtual void HealHP(int hp)
        {
            BStats.HP = HelperGlobals.Clamp(BStats.HP + hp, 0, BStats.MaxHP);
        }

        public void HealFP(int fp)
        {
            BStats.FP = HelperGlobals.Clamp(BStats.FP + fp, 0, BStats.MaxFP);
        }

        public virtual void LoseHP(int hp)
        {
            BStats.HP = HelperGlobals.Clamp(BStats.HP - hp, 0, BStats.MaxHP);
            if (BStats.HP <= 0)
            {
                Die();
            }
        }

        public void LoseFP(int fp)
        {
            BStats.FP = HelperGlobals.Clamp(BStats.FP - fp, 0, BStats.MaxFP);
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
            BattleManager.Instance.TurnEnd();
        }

        /// <summary>
        /// What happens during the entity's turn (choosing action commmands, etc.)
        /// </summary>
        public virtual void TurnUpdate()
        {
            
        }

        #endregion

        /// <summary>
        /// Used for update logic that applies to the entity regardless of whether it is its turn or not
        /// </summary>
        public void Update()
        {
            
        }

        public virtual void Draw()
        {
            
        }
    }
}
