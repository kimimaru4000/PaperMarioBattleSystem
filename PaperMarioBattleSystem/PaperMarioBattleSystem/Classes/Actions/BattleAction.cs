using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static PaperMarioBattleSystem.TargetSelectionMenu;
using static PaperMarioBattleSystem.Enumerations;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// An action that is performed by a BattleEntity in battle
    /// </summary>
    public abstract class BattleAction
    {
        #region Fields/Properties

        /// <summary>
        /// The name of the action
        /// </summary>
        public string Name { get; protected set; } = "Action";

        /// <summary>
        /// The user of this action.
        /// Aside from Defensive Actions, it will be the entity whose turn it currently is
        /// </summary>
        public virtual BattleEntity User => BattleManager.Instance.EntityTurn;

        #endregion

        protected BattleAction()
        {
            
        }

        public virtual void Update()
        {
            
        }

        public virtual void Draw()
        {
            
        }
    }
}
