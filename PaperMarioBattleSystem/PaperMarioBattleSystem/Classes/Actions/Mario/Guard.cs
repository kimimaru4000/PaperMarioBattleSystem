using System;
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
            CommandSuccessTimer = (8d / 60d) * 1000d;

            Command = new GuardCommand(this);
            Command.StartInput();
        }

        public override BattleGlobals.DefensiveActionHolder HandleSuccess(int damage, StatusEffect[] statusEffects)
        {
            int newDamage = UtilityGlobals.Clamp(damage - 1, BattleGlobals.MinDamage, BattleGlobals.MaxDamage);
            StatusEffect[] newStatuses = null;
            if (statusEffects != null && statusEffects.Length != 0)
            {
                List<StatusEffect> filteredStatuses = new List<StatusEffect>(statusEffects);
                for (int i = 0; i < filteredStatuses.Count; i++)
                {
                    //NOTE: Hardcoded and temporary for now
                    if (filteredStatuses[i].StatusType != Enumerations.StatusTypes.Immobilized)
                    {
                        filteredStatuses.RemoveAt(i);
                        i--;
                    }
                }

                newStatuses = filteredStatuses.ToArray();
            }

            return new BattleGlobals.DefensiveActionHolder(newDamage, newStatuses);
        }
    }
}
