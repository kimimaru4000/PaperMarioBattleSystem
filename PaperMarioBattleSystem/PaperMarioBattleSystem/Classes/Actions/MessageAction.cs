using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// A MoveAction that queues a Battle Message when selected.
    /// <para>This is often used for temporary moves placed in menus if no valid moves exist in that menu.
    /// An example includes the "No Items" move in the Item menu if the entity has no items.</para>
    /// </summary>
    public sealed class MessageAction : MoveAction
    {
        /// <summary>
        /// The priority of the message event sent out.
        /// </summary>
        private int MessagePriority = (int)BattleGlobals.BattleEventPriorities.Message;

        /// <summary>
        /// The message sent out.
        /// </summary>
        private string Message = string.Empty;

        /// <summary>
        /// The duration of the message.
        /// </summary>
        private double MessageDuration = MessageBattleEvent.DefaultWaitDuration;

        public MessageAction(BattleEntity user, string name, Texture2D icon, string description, int messagePriority, string message)
            : this(user, name, icon, description, messagePriority, message, MessageBattleEvent.DefaultWaitDuration)
        {

        }

        public MessageAction(BattleEntity user, string name, Texture2D icon, string description, int messagePriority, string message, double messageDuration)
            : base(user)
        {
            Name = name;

            MessagePriority = messagePriority;
            Message = message;
            MessageDuration = messageDuration;

            MoveInfo = new MoveActionData(null, description, Enumerations.MoveResourceTypes.FP, 0,
                Enumerations.CostDisplayTypes.Hidden, Enumerations.MoveAffectionTypes.None,
                Enumerations.EntitySelectionType.Single, false, null);

            //This MoveSequence isn't ever used, but one needs to be set
            SetMoveSequence(new NoSequence(this));
        }

        public override void OnMenuSelected()
        {
            User.BManager.battleEventManager.QueueBattleEvent(MessagePriority, new BattleGlobals.BattleState[] { BattleGlobals.BattleState.Turn, BattleGlobals.BattleState.TurnEnd },
                new MessageBattleEvent(Message, MessageDuration));
        }
    }
}
