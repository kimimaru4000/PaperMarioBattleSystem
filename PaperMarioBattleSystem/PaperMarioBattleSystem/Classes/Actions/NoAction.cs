using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// A BattleAction that causes the BattleEntity to do nothing.
    /// <para>This is used when Mario or his Partner take the "Do Nothing" action and when entities are unable to attack an ally
    /// because it has no allies remaining or the move can't reach the ally.</para>
    /// </summary>
    public sealed class NoAction : MoveAction
    {
        public NoAction()
        {
            Name = "Do Nothing";
            MoveInfo = new MoveActionData(null, 0, "Do nothing this turn.", TargetSelectionMenu.EntitySelectionType.Single,
                Enumerations.EntityTypes.Player, false, null);

            SetMoveSequence(new NoSequence(this));
        }
    }
}
