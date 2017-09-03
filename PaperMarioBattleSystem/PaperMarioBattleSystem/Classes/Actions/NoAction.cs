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
    /// <para>This is also the recommended method of skipping turns in many circumstances (Ex. a flipped Koopa).
    /// Instead of ending the turn directly, have the BattleEntity perform a NoAction.</para>
    /// </summary>
    public sealed class NoAction : MoveAction
    {
        public NoAction()
        {
            Name = "Do Nothing";
            MoveInfo = new MoveActionData(null, "Do nothing this turn.", Enumerations.MoveResourceTypes.FP, 0,
                Enumerations.CostDisplayTypes.Hidden, Enumerations.MoveAffectionTypes.None, TargetSelectionMenu.EntitySelectionType.Single,
                false, null);

            SetMoveSequence(new NoSequence(this));
        }
    }
}
