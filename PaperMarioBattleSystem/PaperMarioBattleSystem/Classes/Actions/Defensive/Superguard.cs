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
            actionCommand = new SuperguardCommand(this);

            CommandSuccessTimer = (3d / 60d) * Time.MsPerS;

            AllowedStatuses = null;
        }

        public override BattleGlobals.DefensiveActionHolder HandleSuccess(int damage, StatusEffect[] statusEffects)
        {
            int newDamage = UtilityGlobals.Clamp(0, BattleGlobals.MinDamage, BattleGlobals.MaxDamage);
            StatusEffect[] newStatuses = FilterStatuses(statusEffects);

            User.PlayAnimation(AnimationGlobals.PlayerBattleAnimations.SuperguardName, true);

            return new BattleGlobals.DefensiveActionHolder(newDamage, newStatuses, new ElementDamageHolder(1, Enumerations.Elements.Normal));
        }
    }
}
