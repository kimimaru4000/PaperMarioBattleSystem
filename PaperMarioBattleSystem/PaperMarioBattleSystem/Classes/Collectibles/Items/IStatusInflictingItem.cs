using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// An interface for any item that inflicts Status Effects.
    /// </summary>
    public interface IStatusInflictingItem
    {
        StatusChanceHolder[] StatusesInflicted { get; }
    }
}
