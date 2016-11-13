using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PaperMarioBattleSystem.Enumerations;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// An interface for any item that can damage enemies.
    /// All item damage is Piercing and doesn't make any contact.
    /// </summary>
    public interface IDamagingItem
    {
        int Damage { get; }

        Elements Element { get; }
    }
}
