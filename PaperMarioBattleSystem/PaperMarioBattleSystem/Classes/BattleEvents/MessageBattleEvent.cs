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
        public static readonly double DefaultWaitDuration = 2000d;

        private TextBox BattleTextBox = null;
        private string BattleMessage = string.Empty;

        public MessageBattleEvent(string battleMessage, double waitDuration) : base(waitDuration)
        {
            BattleMessage = battleMessage;
            BattleTextBox = new TextBox(SpriteRenderer.Instance.WindowCenter, new Vector2(100, 50), BattleMessage);
            BattleTextBox.ScaleToText(AssetManager.Instance.TTYDFont);

            IsUnique = true;
        }

        protected override void OnStart()
        {
            base.OnStart();

            BattleUIManager.Instance.SuppressMenus();
            BattleUIManager.Instance.AddUIElement(BattleTextBox);
        }

        protected override void OnEnd()
        {
            base.OnEnd();

            BattleUIManager.Instance.UnsuppressMenus();
            BattleUIManager.Instance.RemoveUIElement(BattleTextBox);
        }

        public override bool AreContentsEqual(BattleEvent other)
        {
            if (base.AreContentsEqual(other) == true) return true;

            MessageBattleEvent messageEvent = other as MessageBattleEvent;

            return (messageEvent != null && messageEvent.BattleMessage == BattleMessage);
        }
    }
}
