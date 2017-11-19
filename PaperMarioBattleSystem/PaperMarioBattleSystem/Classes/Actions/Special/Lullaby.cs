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

            MoveInfo = new MoveActionData(null, "Lull enemies to sleep with\na tender lullaby.", Enumerations.MoveResourceTypes.SP,
                100, Enumerations.CostDisplayTypes.Shown, Enumerations.MoveAffectionTypes.Other,
                TargetSelectionMenu.EntitySelectionType.All, false, null, Enumerations.EntityTypes.Enemy);
                
            DamageInfo = new DamageData(0, Enumerations.Elements.Star, true, Enumerations.ContactTypes.None,
                new StatusChanceHolder[] { new StatusChanceHolder(100d, new SleepStatus(3)) }, Enumerations.DamageEffects.None);

            SetMoveSequence(new LullabySequence(this));
        }
    }
}
