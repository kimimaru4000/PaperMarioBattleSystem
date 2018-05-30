using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using static PaperMarioBattleSystem.StarPowerGlobals;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The base class for Special Moves, which use up Star Power.
    /// <para>Special Moves are unaffected by All Or Nothing; make sure AllOrNothingAffected is false when defining damage data.</para>
    /// <para>Note that all Crystal Star Special Moves can hit entities with the Invisible Status Effect.</para>
    /// </summary>
    public class SpecialMoveAction : MoveAction
    {
        protected SpecialMoveAction(BattleEntity user) : base(user)
        {
            
        }

        public SpecialMoveAction(BattleEntity user, string name, MoveActionData moveProperties, Sequence moveSequence)
            : base(user, name, moveProperties, moveSequence)
        {

        }

        public SpecialMoveAction(BattleEntity user, string name, MoveActionData moveProperties, Sequence moveSequence, HealingData healingData)
            : base(user, name, moveProperties, moveSequence, healingData)
        {
            
        }

        public SpecialMoveAction(BattleEntity user, string name, MoveActionData moveProperties, Sequence moveSequence, ActionCommand actionCommand, HealingData healingData)
            : base(user, name, moveProperties, moveSequence, actionCommand, healingData)
        {
            
        }

        public SpecialMoveAction(BattleEntity user, string name, MoveActionData moveProperties, Sequence moveSequence, DamageData damageInfo)
            : base(user, name, moveProperties, moveSequence, damageInfo)
        {
          
        }

        public SpecialMoveAction(BattleEntity user, string name, MoveActionData moveProperties, Sequence moveSequence, ActionCommand actionCommand, DamageData damageInfo)
            : base(user, name, moveProperties, moveSequence, actionCommand, damageInfo)
        {
            
        }
    }
}
