using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The action performed by Bobbery's Bombs to indicate that they're closer to detonating.
    /// It speeds up their idle animations.
    /// </summary>
    public sealed class BlinkFasterAction : MoveAction
    {
        public BlinkFasterAction(string animName, float speedVal)
        {
            Name = "Blink Faster";

            MoveInfo = new MoveActionData(null, "Blink faster to indicate you're detonating!", Enumerations.MoveResourceTypes.FP, 0,
                Enumerations.CostDisplayTypes.Shown, Enumerations.MoveAffectionTypes.None, TargetSelectionMenu.EntitySelectionType.Single,
                false, null, null);

            SetMoveSequence(new BlinkFasterSequence(this, animName, speedVal));
        }
    }
}
