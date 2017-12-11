using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PaperMarioBattleSystem.Enumerations;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// An interface for any item that heals Status Effects.
    /// </summary>
    public interface IStatusHealingItem
    {
        StatusTypes[] StatusesHealed { get; }
    }
}
