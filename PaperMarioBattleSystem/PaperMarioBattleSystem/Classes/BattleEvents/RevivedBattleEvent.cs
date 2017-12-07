using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// A BattleEvent that plays when a BattleEntity is being revived.
    /// </summary>
    public class RevivedBattleEvent : WaitBattleEvent
    {
        /// <summary>
        /// The BattleEntity revived.
        /// </summary>
        private BattleEntity RevivedEntity = null;

        /// <summary>
        /// The item that revives the BattleEntity. It must implement <see cref="IRevivalItem"/> to know how much HP to restore upon revival.
        /// </summary>
        private Item RevivalItem = null;

        public RevivedBattleEvent(double waitDuration, BattleEntity revivedEntity, Item revivalItem) : base(waitDuration)
        {
            RevivedEntity = revivedEntity;
            RevivalItem = revivalItem;

            IsUnique = true;
        }

        protected override void OnEnd()
        {
            base.OnEnd();

            //Get revival data from the item
            IRevivalItem revivalData = RevivalItem as IRevivalItem;
            if (revivalData != null && revivalData.RevivalHPRestored > 0)
            {
                RevivedEntity.HealHP(revivalData.RevivalHPRestored);

                //Play the idle animation
                RevivedEntity.AnimManager.PlayAnimation(RevivedEntity.GetIdleAnim());

                //Remove the item
                //NOTE: For now just handle players and remove from the inventory - enemies will need to remove their held items
                Inventory.Instance.RemoveItem(RevivalItem);
            }
            else
            {
                Debug.LogError($"{RevivalItem.Name} does not implement {nameof(IRevivalItem)} or heals 0 or less HP, so the BattleEntity can't be revived!");
            }

            //Clear references
            RevivedEntity = null;
            RevivalItem = null;
        }

        public override bool AreContentsEqual(BattleEvent other)
        {
            if (base.AreContentsEqual(other) == true) return true;

            RevivedBattleEvent revivedEvent = other as RevivedBattleEvent;

            return (revivedEvent != null && revivedEvent.RevivedEntity == RevivedEntity);
        }
    }
}
