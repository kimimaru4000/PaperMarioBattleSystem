using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// The base class for MoveActions involving Items.
/// </summary>
namespace PaperMarioBattleSystem
{
    public class ItemAction : MoveAction
    {
        /// <summary>
        /// The Item used.
        /// </summary>
        public BattleItem ItemUsed { get; protected set; } = null;

        public ItemAction(BattleItem item)
        {
            ItemUsed = item;

            if (ItemUsed == null || ItemUsed.ItemType == Item.ItemTypes.None || ItemUsed.ItemType == Item.ItemTypes.KeyItem)
            {
                Debug.LogError($"Invalid item passed into the ItemAction!");
            }

            //NOTE: Right now it's not possible to both heal yourself and hurt enemies with any item (Ex. Meteor Meal)
            //In fact, it's not possible to heal at all without deriving. We need this to work for all items with only
            //the Sequences being different.
            //Add a HealingInfo field as a new struct that takes in HP, FP, and StatusEffects healed

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

        public override void OnMenuSelected()
        {
            BattleEntity[] entities = null;

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
            BattleUIManager.Instance.StartTargetSelection(ActionStart, MoveProperties.SelectionType, entities);
        }
    }
}
