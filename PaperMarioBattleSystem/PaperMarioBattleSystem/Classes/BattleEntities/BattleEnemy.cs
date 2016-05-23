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
        public int BattleIndex { get; private set; } = -1;

        protected BattleEnemy(Stats stats) : base(stats)
        {
            Name = "Partner";

            EntityType = Enumerations.EntityTypes.Enemy;
        }

        public void SetBattleIndex(int battleIndex)
        {
            BattleIndex = battleIndex;
        }
    }
}
