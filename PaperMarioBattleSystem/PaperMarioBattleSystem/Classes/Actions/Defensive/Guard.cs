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
    /// Mario's Guard action when being attacked.
    /// It's unique in that it reduces the damage Piercing attacks do
    /// </summary>
    public class Guard : DefensiveAction
    {
        public Guard(BattleEntity user) : base(user)
        {
            Name = "Guard";

            actionCommand = new GuardCommand(this);

            CommandSuccessTimer = (8d / 60d) * Time.MsPerS;

            AllowedStatuses = null;
        }

        public override BattleGlobals.DefensiveActionHolder HandleSuccess(int damage, StatusChanceHolder[] statusEffects, DamageEffects damageEffects)
        {
            int newDamage = damage - 1;
            StatusChanceHolder[] newStatuses = FilterStatuses(statusEffects);

            BattleEventManager.Instance.QueueBattleEvent((int)BattleGlobals.StartEventPriorities.Damage,
                new BattleManager.BattleState[] { BattleManager.BattleState.Turn },
                new WaitForAnimBattleEvent(User, AnimationGlobals.PlayerBattleAnimations.GuardName, true));

            return new BattleGlobals.DefensiveActionHolder(newDamage, newStatuses, Enumerations.DamageEffects.None);
        }
    }
}
