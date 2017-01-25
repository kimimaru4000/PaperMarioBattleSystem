using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Lullaby action. Mamar puts all enemies to sleep at the cost of 1 SP.
    /// </summary>
    public sealed class Lullaby : SpecialMoveAction
    {
        public Lullaby()
        {
            Name = "Lullaby";

            SPCost = 100;

            MoveInfo = new MoveActionData(null, 0, "Lull enemies to sleep with\na tender lullaby.",
                false, TargetSelectionMenu.EntitySelectionType.All, Enumerations.EntityTypes.Enemy, null);
            DamageInfo = new InteractionParamHolder(null, null, 0, Enumerations.Elements.Star, true, Enumerations.ContactTypes.None,
                new StatusEffect[] { new SleepStatus(3) });
            SetMoveSequence(new LullabySequence(this));
        }
    }
}
