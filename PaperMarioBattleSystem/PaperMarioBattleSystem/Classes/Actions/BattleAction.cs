using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static PaperMarioBattleSystem.TargetSelectionMenu;

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
        /// The icon representing the action
        /// </summary>
        public Texture2D Icon = null;

        /// <summary>
        /// How much FP it costs to use the action
        /// </summary>
        public int FPCost = 0;

        /// <summary>
        /// The base damage of the action
        /// </summary>
        public int BaseDamage = 0;

        /// <summary>
        /// The description of the action
        /// </summary>
        public string Description = "Error";

        /// <summary>
        /// The amount of entities this action can select
        /// </summary>
        public EntitySelectionType SelectionType = EntitySelectionType.Single;

        /// <summary>
        /// The ActionCommand associated with the BattleAction
        /// </summary>
        protected ActionCommand Command = null;

        /// <summary>
        /// Whether the action's sequence is being performed or not
        /// </summary>
        public bool InSequence = false;

        public float Length = 1000f;

        private BattleEntity[] EntitiesAffected = null;

        protected BattleAction()
        {
            
        }

        /// <summary>
        /// Starts the action sequence
        /// </summary>
        /// <param name="targets">The targets to perform the sequence on</param>
        public void StartSequence(params BattleEntity[] targets)
        {
            InSequence = true;

            EntitiesAffected = targets;

            if (Command == null)
            {
                OnActionCommandFinish(0);
            }
            else
            {
                Command.StartInput();
            }
        }
        
        /// <summary>
        /// Ends the action sequence
        /// </summary>
        public void EndSequence()
        {
            InSequence = false;
            EntitiesAffected = null;

            BattleManager.Instance.EntityTurn.UsedTurn = true;
            BattleManager.Instance.EntityTurn.EndTurn();
        }

        /// <summary>
        /// What occurs when an ActionCommand is finished.
        /// A successRate of 0 is passed in if the ActionCommand was failed completely
        /// </summary>
        /// <param name="successRate">0 if the ActionCommand was completely failed, otherwise a number of varying success</param>
        public void OnActionCommandFinish(int successRate)
        {
            OnActionCompleted(successRate, EntitiesAffected);
            EndSequence();
        }

        /// <summary>
        /// What occurs when the action is completed
        /// </summary>
        protected abstract void OnActionCompleted(int successRate, BattleEntity[] targets);

        /// <summary>
        /// What happens when the BattleAction is selected on the menu
        /// </summary>
        public virtual void OnMenuSelected()
        {
            
        }

        public void Update()
        {
            //Perform sequence
            if (InSequence == true)
            {
                Command.Update();
            }
        }

        public void Draw()
        {
            if (InSequence == true)
            {
                Command.Draw();
            }
        }
    }
}
