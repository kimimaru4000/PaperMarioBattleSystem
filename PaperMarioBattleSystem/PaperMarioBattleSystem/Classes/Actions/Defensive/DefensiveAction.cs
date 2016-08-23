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
    public abstract class DefensiveAction : BattleAction, ICommandAction
    {
        protected BattleEntity actionUser { get; set; } = null;

        public ActionCommand actionCommand { get; set; } = null;
        public bool DisableActionCommand { get; set; } = false;

        protected double CommandSuccessTimer = 0f;
        protected double PrevCommandTimer = 0f;

        public override BattleEntity User => actionUser;

        public bool CommandEnabled => (actionCommand != null);
        public bool IsSuccessful => (PrevCommandTimer >= Time.ActiveMilliseconds);

        protected DefensiveAction(BattleEntity user)
        {
            actionUser = user;
        }

        public virtual void OnCommandSuccess()
        {
            PrevCommandTimer = Time.ActiveMilliseconds + CommandSuccessTimer;
        }
        
        public virtual void OnCommandFailed()
        {

        }

        public virtual void OnCommandResponse(int response)
        {
            
        }

        /// <summary>
        /// What happens when the Defensive Action is successfully performed.
        /// <para>This can be called for only one Defensive Action at a time (Ex. You can't both Guard and Superguard).
        /// Whichever is successful first in the BattleEntity's DefensiveAction list is the one that gets called.</para>
        /// </summary>
        /// <param name="damage">The original damage that would be dealt to the BattleEntity</param>
        /// <param name="statusEffects">The original StatusEffects that would be inflicted on the BattleEntity</param>
        /// <returns>A DefensiveActionHolder containing the modified damage dealt and a filtered set of StatusEffects inflicted</returns>
        public abstract BattleGlobals.DefensiveActionHolder HandleSuccess(int damage, StatusEffect[] statusEffects);

        public override void Update()
        {
            if (CommandEnabled == true)
            {
                actionCommand.Update();
            }
        }
    }
}
