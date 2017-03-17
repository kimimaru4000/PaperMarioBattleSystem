using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    public sealed class Superguard : DefensiveAction
    {
        public Superguard(BattleEntity user) : base(user)
        {
            Name = "Superguard";

            actionCommand = new SuperguardCommand(this);

            CommandSuccessTimer = (3d / 60d) * Time.MsPerS;

            AllowedStatuses = null;
        }

        public override BattleGlobals.DefensiveActionHolder HandleSuccess(int damage, StatusChanceHolder[] statusEffects)
        {
            int newDamage = 0;
            StatusChanceHolder[] newStatuses = FilterStatuses(statusEffects);

            BattleEventManager.Instance.QueueBattleEvent((int)BattleGlobals.StartEventPriorities.Damage,
                new BattleManager.BattleState[] { BattleManager.BattleState.Turn },
                new WaitForAnimBattleEvent(User, AnimationGlobals.PlayerBattleAnimations.SuperguardName, true));

            return new BattleGlobals.DefensiveActionHolder(newDamage, newStatuses, new ElementDamageHolder(1, Enumerations.Elements.Normal));
        }
    }
}
