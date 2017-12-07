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

            if (ItemUsed == null || ItemUsed.ItemType == Item.ItemTypes.None)
            {
                Debug.LogError($"Invalid item with {nameof(Item.ItemType)} of {ItemUsed.ItemType} passed into the ItemAction!");
            }

            SetActionProperties();

            SetMoveSequence(item.SequencePerformed);
        }

        public sealed override void OnMenuSelected()
        {
            BattleEntity[] entities = GetEntitiesMoveAffects();

            int startIndex = 0;

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
            MoveInfo.ResourceCost = fp;
            MoveInfo.CostDisplayType = CostDisplayTypes.Hidden;
        }

        /// <summary>
        /// Sets the ItemUsedDelegate to call when the item is used. This is used if the ItemAction is in the Double or Triple Dip menu.
        /// </summary>
        /// <param name="onItemUsed"></param>
        public void SetOnItemUsed(ItemUsedDelegate onItemUsed)
        {
            OnItemUsed = onItemUsed;
        }

        /// <summary>
        /// Sets the ItemAction's properties based on the item it has.
        /// <para>The default behavior is to check the item's properties and set its MoveInfo, DamageInfo, and HealingInfo accordingly.</para>
        /// </summary>
        protected virtual void SetActionProperties()
        {
            Name = ItemUsed.Name;

            //NOTE: Refactor and make cleaner in some way

            IHPHealingItem hpHealing = ItemUsed as IHPHealingItem;
            IFPHealingItem fpHealing = ItemUsed as IFPHealingItem;
            IDamagingItem damageItem = ItemUsed as IDamagingItem;
            IStatusHealingItem statusHealing = ItemUsed as IStatusHealingItem;
            IStatusInflictingItem statusInflicting = ItemUsed as IStatusInflictingItem;
            IDamageEffectItem damageEffectItem = ItemUsed as IDamageEffectItem;

            EntityTypes[] otherEntityTypes = ItemUsed.OtherEntTypes;

            //Check if we should replace all instances of Enemy EntityTypes with opposing ones
            if (ItemUsed.GetOpposingIfEnemy == true && otherEntityTypes != null && otherEntityTypes.Length > 0)
            {
                //Create a new array so we don't modify the item's information
                otherEntityTypes = new EntityTypes[ItemUsed.OtherEntTypes.Length];

                for (int i = 0; i < ItemUsed.OtherEntTypes.Length; i++)
                {
                    EntityTypes entitytype = ItemUsed.OtherEntTypes[i];

                    //If we found Enemy, change it
                    if (entitytype == EntityTypes.Enemy)
                    {
                        entitytype = User.GetOpposingEntityType();
                    }

                    //Set the value
                    otherEntityTypes[i] = entitytype;
                }
            }

            MoveInfo = new MoveActionData(null, ItemUsed.Description, MoveResourceTypes.FP, 0, CostDisplayTypes.Hidden,
                ItemUsed.MoveAffectionType, ItemUsed.SelectionType, false, ItemUsed.HeightsAffected, otherEntityTypes);

            //Set the damage data
            if (damageItem != null || statusInflicting != null)
            {
                int damage = damageItem != null ? damageItem.Damage : 0;
                Elements element = damageItem != null ? damageItem.Element : Elements.Normal;
                StatusChanceHolder[] statuses = statusInflicting != null ? statusInflicting.StatusesInflicted : null;
                DamageEffects damageEffects = damageEffectItem != null ? damageEffectItem.InducedDamageEffects : DamageEffects.None;

                DamageInfo = new DamageData(damage, element, true, ContactTypes.None, statuses, damageEffects);
            }

            //Set the healing data
            if (hpHealing != null || fpHealing != null || statusHealing != null)
            {
                int hpHealed = hpHealing != null ? hpHealing.HPRestored : 0;
                int fpHealed = fpHealing != null ? fpHealing.FPRestored : 0;
                StatusTypes[] statusesHealed = statusHealing != null ? statusHealing.StatusesHealed : null;

                HealingInfo = new HealingData(hpHealed, fpHealed, statusesHealed);
            }
        }
    }
}
