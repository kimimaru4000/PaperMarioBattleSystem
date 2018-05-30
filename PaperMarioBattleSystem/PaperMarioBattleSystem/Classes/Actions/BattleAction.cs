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
    public abstract class BattleAction : INameable
    {
        #region Fields/Properties

        /// <summary>
        /// The name of the action
        /// </summary>
        public string Name { get; set; } = "Action";

        /// <summary>
        /// The user of this action.
        /// Aside from Defensive Actions, it will often be the entity whose turn it currently is.
        /// </summary>
        public BattleEntity User { get; private set; } = null;

        #endregion

        protected BattleAction(BattleEntity user)
        {
            SetUser(user);
        }

        /// <summary>
        /// Sets the BattleEntity performing the BattleAction.
        /// </summary>
        /// <param name="user">The BattleEntity performing the BattleAction.</param>
        public void SetUser(BattleEntity user)
        {
            User = user;
        }

        public virtual void Update()
        {
            
        }

        public virtual void Draw()
        {
            
        }
    }
}
