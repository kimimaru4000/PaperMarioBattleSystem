using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PaperMarioBattleSystem.Enumerations;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The MoveAction for the Mystery Item.
    /// </summary>
    public class MysteryAction : ItemAction
    {
        public MysteryAction(BattleItem item) : base(item)
        {
            
        }

        protected override void SetActionProperties()
        {
            Name = ItemUsed.Name;

            MoveInfo = new MoveActionData(ItemUsed.Icon, ItemUsed.Description, MoveResourceTypes.FP, 0, CostDisplayTypes.Hidden,
                MoveAffectionTypes.Self, ItemUsed.SelectionType, false, ItemUsed.HeightsAffected);
        }
    }
}
