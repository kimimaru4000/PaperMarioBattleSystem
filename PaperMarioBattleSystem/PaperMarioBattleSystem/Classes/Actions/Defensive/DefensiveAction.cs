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
    public abstract class DefensiveAction : BattleAction, IActionCommand, IActionCommandHandler
    {
        public ActionCommand actionCommand { get; set; } = null;
        public bool DisableActionCommand { get; set; } = false;

        public override BattleEntity User => actionUser;

        public bool CommandEnabled => (actionCommand != null && UserImmobile == false && DisableActionCommand == false);
        public bool IsSuccessful => (PrevCommandTimer >= Time.ActiveMilliseconds);

        //NOTE: Ideally, check the CommandResult instead of the timer - not super important right now, though
        public ActionCommand.CommandResults CommandResult { get; set; } = ActionCommand.CommandResults.Failure;

        /// <summary>
        /// The type of DefensiveAction this is.
        /// This is used to help moves determine whether they can override this or not.
        /// </summary>
        public DefensiveActionTypes DefensiveActionType { get; protected set; } = DefensiveActionTypes.None;

        /// <summary>
        /// Tells if the user of the DefensiveAction is Immobile.
        /// </summary>
        protected bool UserImmobile => (User != null && User.EntityProperties.HasAdditionalProperty(AdditionalProperty.Immobile) == true);

        protected BattleEntity actionUser = null;

        /// <summary>
        /// The amount of time a successful ActionCommand is valid for
        /// </summary>
        protected double CommandSuccessTimer = 0f;

        /// <summary>
        /// The time since the last successful ActionCommand
        /// </summary>
        protected double PrevCommandTimer = 0f;

        /// <summary>
        /// StatusEffects that the DefensiveAction doesn't defend against.
        /// If this is null, it defends against all of them.
        /// </summary>
        protected StatusTypes[] AllowedStatuses = null;

        protected DefensiveAction(BattleEntity user)
        {
            actionUser = user;
        }

        public virtual void OnCommandSuccess()
        {
            PrevCommandTimer = Time.ActiveMilliseconds + CommandSuccessTimer;

            CommandResult = ActionCommand.CommandResults.Success;
        }
        
        public virtual void OnCommandFailed()
        {
            CommandResult = ActionCommand.CommandResults.Failure;
        }

        public virtual void OnCommandRankResult(ActionCommand.CommandRank commandRank)
        {

        }

        public virtual void OnCommandResponse(in object response)
        {
            
        }

        /// <summary>
        /// What happens when the Defensive Action is successfully performed.
        /// <para>This can be called for only one Defensive Action at a time (Ex. You can't both Guard and Superguard).
        /// Whichever is successful first in the BattleEntity's DefensiveAction list is the one that gets called.</para>
        /// </summary>
        /// <param name="damage">The original damage that would be dealt to the BattleEntity</param>
        /// <param name="statusEffects">The original StatusEffects that would be inflicted on the BattleEntity</param>
        /// <param name="damageEffects">The original DamageEffects that would affect the BattleEntity.</param>
        /// <returns>A DefensiveActionHolder containing the modified damage dealt and a filtered set of StatusEffects inflicted</returns>
        public abstract BattleGlobals.DefensiveActionHolder HandleSuccess(int damage, StatusChanceHolder[] statusEffects, DamageEffects damageEffects);

        /// <summary>
        /// Filters out StatusEffects that the DefensiveAction defends against
        /// </summary>
        /// <param name="statusesInflicted">The original set of StatusEffects inflicted</param>
        /// <returns>A set of StatusEffects that the DefensiveAction doesn't defend against.
        /// null if the DefensiveAction defends against all StatusEffects</returns>
        protected virtual StatusChanceHolder[] FilterStatuses(StatusChanceHolder[] statusesInflicted)
        {
            if (AllowedStatuses == null || AllowedStatuses.Length == 0 || statusesInflicted == null || statusesInflicted.Length == 0)
                return null;

            List<StatusChanceHolder> filteredStatuses = new List<StatusChanceHolder>(statusesInflicted);
            filteredStatuses.RemoveAll((statuseffect) => AllowedStatuses.Contains(statuseffect.Status.StatusType) == false);
            return filteredStatuses.ToArray();
        }

        public override void Update()
        {
            if (CommandEnabled == true && actionCommand.AcceptingInput == true)
            {
                actionCommand.Update();
            }
        }
    }
}
