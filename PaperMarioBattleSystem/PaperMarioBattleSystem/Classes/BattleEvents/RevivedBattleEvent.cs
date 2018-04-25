using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

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

        private UICroppedTexture2D RevivalItemShown = null;

        public RevivedBattleEvent(double waitDuration, BattleEntity revivedEntity, Item revivalItem) : base(waitDuration)
        {
            RevivedEntity = revivedEntity;
            RevivalItem = revivalItem;

            IsUnique = true;
        }

        protected override void OnStart()
        {
            base.OnStart();

            //Show the item over the BattleEntity's head
            if (RevivalItem.Icon != null || RevivalItem.Icon.Tex != null)
            {
                RevivalItemShown = new UICroppedTexture2D(RevivalItem.Icon.Copy());
                RevivalItemShown.Position = Camera.Instance.SpriteToUIPos(RevivedEntity.Position + new Vector2(0, -20));

                BattleUIManager.Instance.AddUIElement(RevivalItemShown);
            }
        }

        protected override void OnEnd()
        {
            base.OnEnd();

            //Remove the item over the BattleEntity's head
            if (RevivalItemShown != null)
            {
                BattleUIManager.Instance.RemoveUIElement(RevivalItemShown);
                RevivalItemShown = null;
            }

            //Get revival data from the item
            IRevivalItem revivalData = RevivalItem as IRevivalItem;
            if (revivalData != null)
            {
                //If the revival item heals 0 or fewer HP, log a warning
                if (revivalData.RevivalHPRestored <= 0)
                {
                    Debug.LogWarning($"{RevivalItem.Name} heals 0 or fewer HP, so the BattleEntity won't actually be revived!");
                }

                //Heal HP
                RevivedEntity.HealHP(revivalData.RevivalHPRestored);

                //Play the idle animation
                RevivedEntity.AnimManager.PlayAnimation(RevivedEntity.GetIdleAnim());

                //Remove the item
                //For players, remove it from the inventory
                if (RevivedEntity.EntityType == Enumerations.EntityTypes.Player)
                {
                    Inventory.Instance.RemoveItem(RevivalItem);
                }
                //It has to be an enemy, so remove its held item
                else
                {
                    BattleEnemy revivedEnemy = (BattleEnemy)RevivedEntity;
                    revivedEnemy.SetHeldCollectible(null);
                }
            }
            else
            {
                Debug.LogError($"{RevivalItem.Name} does not implement {nameof(IRevivalItem)}, so the BattleEntity can't be revived!");
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
