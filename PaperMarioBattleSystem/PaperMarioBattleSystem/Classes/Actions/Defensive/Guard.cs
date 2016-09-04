﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

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
            actionCommand = new GuardCommand(this);

            CommandSuccessTimer = (8d / 60d) * 1000d;

            AllowedStatuses = new Enumerations.StatusTypes[] { Enumerations.StatusTypes.Immobilized };
        }

        public override BattleGlobals.DefensiveActionHolder HandleSuccess(int damage, StatusEffect[] statusEffects)
        {
            int newDamage = UtilityGlobals.Clamp(damage - 1, BattleGlobals.MinDamage, BattleGlobals.MaxDamage);
            StatusEffect[] newStatuses = FilterStatuses(statusEffects);

            User.PlayAnimation(AnimationGlobals.PlayerBattleAnimations.GuardName, true);

            return new BattleGlobals.DefensiveActionHolder(newDamage, newStatuses);
        }
    }
}