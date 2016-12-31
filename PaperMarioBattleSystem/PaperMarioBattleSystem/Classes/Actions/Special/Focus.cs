using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Focus action. Mario regains some Star Spirit Star Power.
    /// <para>The amount regained can be increased with Deep Focus Badges.
    /// Partners can also use this move with the Group Focus Badge, but Deep Focus doesn't increase the amount they gain when using it.</para>
    /// </summary>
    public sealed class Focus : SpecialMoveAction
    {
        public Focus()
        {
            Name = "Focus";

            MoveInfo = new MoveActionData(null, 0, "Charge Star Energy.", TargetSelectionMenu.EntitySelectionType.First, Enumerations.EntityTypes.Player, false, null);
            SetMoveSequence(new FocusSequence(this));
        }
    }
}
