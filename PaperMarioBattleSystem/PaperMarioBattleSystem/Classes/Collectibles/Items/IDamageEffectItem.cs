using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PaperMarioBattleSystem.Enumerations;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// An interface for any Item that has <see cref="DamageEffects"/>.
    /// </summary>
    public interface IDamageEffectItem
    {
        DamageEffects InducedDamageEffects { get; }
    }
}
