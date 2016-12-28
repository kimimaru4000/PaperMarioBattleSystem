using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PaperMarioBattleSystem.Enumerations;

/// <summary>
/// The base class for MoveActions involving Items.
/// </summary>
namespace PaperMarioBattleSystem
{
    public class ItemAction : MoveAction
    {
        /// <summary>
        /// Delegate for determining what happens if an item is used.
        /// </summary>
        public delegate void ItemUsedDelegate();

        /// <summary>
        /// What happens when the item is used before the Sequence starts.
        /// </summary>
        public ItemUsedDelegate OnItemUsed { get; private set; }

        /// <summary>
        /// The Item used.
        /// </summary>
        public BattleItem ItemUsed { get; protected set; } = null;

        public ItemAction(BattleItem item)
        {
            ItemUsed = item;

            if (ItemUsed == null || ItemUsed.ItemType == Item.ItemTypes.None || ItemUsed.ItemType == Item.ItemTypes.KeyItem)
            {
                Debug.LogError($"Invalid item with {nameof(Item.ItemType)} of {ItemUsed.ItemType} passed into the ItemAction!");
            }

            Name = item.Name;

            IHPHealingItem hpHealing = item as IHPHealingItem;
            IFPHealingItem fpHealing = item as IFPHealingItem;
            IDamagingItem damageItem = item as IDamagingItem;
            IStatusHealingItem statusHealing = item as IStatusHealingItem;
            IStatusInflictingItem statusInflicting = item as IStatusInflictingItem;

            MoveInfo = new MoveActionData(null, 0, item.Description, item.SelectionType, item.EntityType, item.HeightsAffected);

            //Set the damage
            if (damageItem != null || statusInflicting != null)
            {
                int damage = damageItem != null ? damageItem.Damage : 0;
                Enumerations.Elements element = damageItem != null ? damageItem.Element : Enumerations.Elements.Normal;
                StatusEffect[] statuses = statusInflicting != null ? statusInflicting.StatusesInflicted : null;

                DamageInfo = new InteractionParamHolder(null, null, damage, element, true, Enumerations.ContactTypes.None, statuses);
            }

            //Set the healing data
            if (hpHealing != null || fpHealing != null || statusHealing != null)
            {
                int hpHealed = hpHealing != null ? hpHealing.HPRestored : 0;
                int fpHealed = fpHealing != null ? fpHealing.FPRestored : 0;
                Enumerations.StatusTypes[] statusesHealed = statusHealing != null ? statusHealing.StatusesHealed : null;

                HealingInfo = new HealingData(hpHealed, fpHealed, statusesHealed);
            }

            SetMoveSequence(new ItemSequence(this));
        }

        public sealed override void OnMenuSelected()
        {
            BattleEntity[] entities = null;

            int startIndex = 0;

            //If the item targets the user, only choose the user as the target
            if (ItemUsed.TargetsSelf == true)
            {
                entities = new BattleEntity[] { User };
            }
            else
            {
                entities = BattleManager.Instance.GetEntities(MoveProperties.EntityType, MoveProperties.HeightsAffected);
            }

            //Bring up the target selection menu
            BattleUIManager.Instance.StartTargetSelection(ActionStart, MoveProperties.SelectionType, startIndex, entities);
        }

        public sealed override void OnActionStarted()
        {
            //Call the item used delegate
            //If this is set to perform Double/Triple Dip, this will occur after getting the number of Item turns
            //This allows the correct number of item turns to 
            OnItemUsed?.Invoke();

            //Get remaining item turn count minus 1
            //Get this here, as the property will automatically be removed by the base behavior for safety
            int dipTurns = User.EntityProperties.GetAdditionalProperty<int>(AdditionalProperty.DipItemTurns) - 1;

            //Perform base behavior
            base.OnActionStarted();

            //If there are items left to use, subtract 1 from the number of turns used and adjust the dip count
            //Subtracting preserves the turn count and works well with other methods of changing turn count (Fast Status, etc.)
            if (dipTurns > 0)
            {
                User.SetTurnsUsed(User.TurnsUsed - 1);
                User.EntityProperties.AddAdditionalProperty(AdditionalProperty.DipItemTurns, dipTurns);
            }
        }

        /// <summary>
        /// Sets the FP cost of this item. This is only used if the ItemAction is in the Double or Triple Dip menu.
        /// </summary>
        /// <param name="fp"></param>
        public void SetDipFPCost(int fp)
        {
            MoveInfo.FPCost = fp;
        }

        /// <summary>
        /// Sets the ItemUsedDelegate to call when the item is used. This is used if the ItemAction is in the Double or Triple Dip menu.
        /// </summary>
        /// <param name="onItemUsed"></param>
        public void SetOnItemUsed(ItemUsedDelegate onItemUsed)
        {
            OnItemUsed = onItemUsed;
        }
    }
}
