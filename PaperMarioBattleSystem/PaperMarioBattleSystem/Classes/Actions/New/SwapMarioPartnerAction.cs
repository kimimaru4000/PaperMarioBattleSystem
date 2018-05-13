using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// A move that deals damage and swaps out Mario's Partner. This can be used by an enemy.
    /// </summary>
    public sealed class SwapMarioPartnerAction : MoveAction
    {
        public SwapMarioPartnerAction()
        {
            Name = "Swap Partner Attack";

            MoveInfo = new MoveActionData(null, "Swaps out Mario's Partner if possible.", Enumerations.MoveResourceTypes.FP,
                0f, Enumerations.CostDisplayTypes.Shown, Enumerations.MoveAffectionTypes.Other, TargetSelectionMenu.EntitySelectionType.Single,
                false, null, new Enumerations.EntityTypes[] { Enumerations.EntityTypes.Enemy });
            DamageInfo = new DamageData(1, Enumerations.Elements.Normal, false, Enumerations.ContactTypes.None, Enumerations.ContactProperties.Ranged,
                null, Enumerations.DamageEffects.None);

            SetMoveSequence(new SwapMarioPartnerSequence(this));
            actionCommand = null;
        }
    }
}
