using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using static PaperMarioBattleSystem.Enumerations;

namespace PaperMarioBattleSystem
{
    public sealed class Superguard : DefensiveAction
    {
        public Superguard(BattleEntity user) : base(user)
        {
            Name = "Superguard";
            DefensiveActionType = DefensiveActionTypes.Superguard;

            actionCommand = new SuperguardCommand(this);

            CommandSuccessTimer = (3d / 60d) * Time.MsPerS;

            AllowedStatuses = null;
        }

        public override BattleGlobals.DefensiveActionHolder HandleSuccess(int damage, StatusChanceHolder[] statusEffects, DamageEffects damageEffects)
        {
            int newDamage = 0;
            StatusChanceHolder[] newStatuses = FilterStatuses(statusEffects);

            BattleEventManager.Instance.QueueBattleEvent((int)BattleGlobals.StartEventPriorities.Damage,
                new BattleManager.BattleState[] { BattleManager.BattleState.Turn },
                new WaitForAnimBattleEvent(User, AnimationGlobals.PlayerBattleAnimations.SuperguardName, true));

            BattleObjManager.Instance.AddBattleObject(new ActionCommandVFX(ActionCommand.CommandRank.Great, User.Position, new Vector2(-15, -15)));
            SoundManager.Instance.PlaySound(SoundManager.Sound.ActionCommandSuccess);

            return new BattleGlobals.DefensiveActionHolder(newDamage, newStatuses, Enumerations.DamageEffects.None,
                new ElementDamageHolder(1, Enumerations.Elements.Normal));
        }
    }
}
