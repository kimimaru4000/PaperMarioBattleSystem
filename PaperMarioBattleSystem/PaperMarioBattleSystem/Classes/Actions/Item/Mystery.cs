using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The MoveAction for the Mystery Item.
    /// </summary>
    public class Mystery : ItemAction
    {
        public Mystery(BattleItem item) : base(item)
        {
            
        }

        protected override void SetActionProperties()
        {
            Name = ItemUsed.Name;
        }
    }
}
