using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PaperMarioBattleSystem.Enumerations;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The base class for Defensive Actions BattleEntities can perform in battle.
    /// <para>A Defensive Action is defined as one that can be done when it's not the entities turn.
    /// In the first two Paper Mario games, the only Defensive Actions are Guard and Superguard.</para>
    /// </summary>
    public abstract class DefensiveAction
    {
        protected BattleEntity User = null;

        protected ActionCommand Command { get; set; } = null;

        protected bool CommandEnabled => (Command != null);

        protected float CommandSuccessTimer = 0f;
        protected float PrevCommandTimer = 0f;

        protected DefensiveAction()
        {
            
        }

        public void Update()
        {
            if (CommandEnabled == true)
            {
                Command.Update();
            }
        }
    }
}
