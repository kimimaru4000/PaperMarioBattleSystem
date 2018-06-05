using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using static PaperMarioBattleSystem.Enumerations;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// Used by Koopatrols to summon more Koopatrols.
    /// </summary>
    public sealed class SummonKoopatrolAction : MoveAction
    {
        public SummonKoopatrolAction(BattleEntity user) : base(user)
        {
            Name = "Summon Koopatrol";

            MoveInfo = new MoveActionData(null, "Summon an ally Koopatrol.", MoveResourceTypes.FP, 0, CostDisplayTypes.Shown,
                MoveAffectionTypes.None, Enumerations.EntitySelectionType.First, false, null, null);

            SetMoveSequence(new SummonKoopatrolSequence(this));
        }
    }
}
