using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// A Battle Message.
    /// It shows up to mention important battle-related information, such as not being able to flee or the consequences of a Status Effect.
    /// It disappears after a certain amount of time.
    /// </summary>
    public sealed class MessageBattleEvent : WaitBattleEvent
    {
        /// <summary>
        /// The default duration of a Battle Message.
        /// </summary>
        public const double DefaultWaitDuration = 2000d;

        private TextBox BattleTextBox = null;
        private string BattleMessage = string.Empty;

        private BattleUIManager BUIManager = null;

        public MessageBattleEvent(BattleUIManager bUIManager, string battleMessage, double waitDuration) : base(waitDuration)
        {
            BUIManager = bUIManager;

            BattleMessage = battleMessage;
            BattleTextBox = new TextBox(SpriteRenderer.Instance.WindowCenter, new Vector2(100, 50), BattleMessage);
            BattleTextBox.ScaleToText(AssetManager.Instance.TTYDFont);

            IsUnique = true;
        }

        protected override void OnStart()
        {
            base.OnStart();

            BUIManager.SuppressMenus();
            BUIManager.AddUIElement(BattleTextBox);
        }

        protected override void OnEnd()
        {
            base.OnEnd();

            BUIManager.UnsuppressMenus();
            BUIManager.RemoveUIElement(BattleTextBox);

            BUIManager = null;
        }

        public override bool AreContentsEqual(BattleEvent other)
        {
            if (base.AreContentsEqual(other) == true) return true;

            MessageBattleEvent messageEvent = other as MessageBattleEvent;

            return (messageEvent != null && messageEvent.BattleMessage == BattleMessage);
        }
    }
}
