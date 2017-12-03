using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// A BattleEvent that tells the BattleManager to sort the list corresponding to a particular EntityType.
    /// </summary>
    public sealed class SortEntityListBattleEvent : BattleEvent
    {
        private Enumerations.EntityTypes EntityTypeToSort;

        public SortEntityListBattleEvent(Enumerations.EntityTypes entityType)
        {
            EntityTypeToSort = entityType;
        }

        protected override void OnEnd()
        {
            base.OnEnd();

            //Sort the list for this EntityType
            BattleManager.Instance.SortEntityList(EntityTypeToSort);
        }

        protected override void OnUpdate()
        {
            End();
        }
    }
}
