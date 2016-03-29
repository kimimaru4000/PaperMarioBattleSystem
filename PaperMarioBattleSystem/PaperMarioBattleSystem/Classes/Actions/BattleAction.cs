using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// An action that is performed by an entity in battle
    /// </summary>
    public abstract class BattleAction
    {
        /// <summary>
        /// The name of the action
        /// </summary>
        public string Name = "Action";

        /// <summary>
        /// How much FP it costs to use the action
        /// </summary>
        public int FPCost = 0;

        /// <summary>
        /// The base damage of the action
        /// </summary>
        public int BaseDamage = 0;

        /// <summary>
        /// The user of the action
        /// </summary>
        public BattleEntity User { get; protected set; } = null;

        protected BattleAction()
        {
            
        }

        public void SetUser(BattleEntity user)
        {
            User = user;
        }
    }
}
