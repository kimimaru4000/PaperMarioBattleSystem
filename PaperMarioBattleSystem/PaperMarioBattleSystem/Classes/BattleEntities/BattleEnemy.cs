using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// Enemies in battle
    /// </summary>
    public abstract class BattleEnemy : BattleEntity
    {
        protected BattleEnemy(Stats stats) : base(stats)
        {
            Name = "Partner";

            EntityType = Enumerations.EntityTypes.Enemy;
        }

        public override void OnDeath()
        {
            base.OnDeath();

            BattleManager.Instance.RemoveEnemies(new List<BattleEnemy>() { this });
        }
    }
}
