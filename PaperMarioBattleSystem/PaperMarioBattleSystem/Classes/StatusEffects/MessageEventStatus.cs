using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// A StatusEffect that handles showing messages when afflicted or removed and queuing a <see cref="StatusEndedBattleEvent"/> when appropriate.
    /// </summary>
    public abstract class MessageEventStatus : StatusEffect
    {
        /// <summary>
        /// The Battle Message shown when a BattleEntity is afflicted with the StatusEffect.
        /// </summary>
        public string AfflictedMessage { get; protected set; } = string.Empty;

        /// <summary>
        /// The Battle Message shown when the StatusEffect is removed from the BattleEntity.
        /// </summary>
        public string RemovedMessage { get; protected set; } = string.Empty;

        /// <summary>
        /// Says whether this Status Effect should tell the BattleEntity afflicted with it to queue
        /// the <see cref="StatusEndedBattleEvent"/> when it ends by turn count. 
        /// </summary>
        protected bool ShouldQueueEndEvent = true;

        public override void Refresh(StatusEffect newStatus)
        {
            base.Refresh(newStatus);
            ShowAfflictedMessage();
        }

        protected override void OnAfflict()
        {
            ShowAfflictedMessage();
        }

        protected override void OnEnd()
        {
            //In the PM games, Status Effects don't show removed messages or play the ending event when ended prematurely
            //Examples include healing via Tasty Tonic and Bowser's Shockwave Drain move
            if (IsTurnFinished == false) return;

            if (string.IsNullOrEmpty(RemovedMessage) == false)
            {
                EntityAfflicted.BManager.battleEventManager.QueueBattleEvent((int)BattleGlobals.BattleEventPriorities.Message + Priority,
                    new BattleGlobals.BattleState[] { BattleGlobals.BattleState.TurnEnd }, new MessageBattleEvent(EntityAfflicted.BManager.battleUIManager, RemovedMessage, 2000d));
            }

            //Queue a battle event for the status removal
            if (ShouldQueueEndEvent == true)
            {
                EntityAfflicted.BManager.battleEventManager.QueueBattleEvent((int)BattleGlobals.BattleEventPriorities.Status + Priority,
                    new BattleGlobals.BattleState[] { BattleGlobals.BattleState.TurnEnd }, new StatusEndedBattleEvent(EntityAfflicted));
            }
        }

        protected void ShowAfflictedMessage()
        {
            if (string.IsNullOrEmpty(AfflictedMessage) == false)
            {
                EntityAfflicted.BManager.battleEventManager.QueueBattleEvent((int)BattleGlobals.BattleEventPriorities.Message + Priority,
                    new BattleGlobals.BattleState[] { BattleGlobals.BattleState.TurnEnd }, new MessageBattleEvent(EntityAfflicted.BManager.battleUIManager, AfflictedMessage, 2000d));
            }
        }
    }
}
