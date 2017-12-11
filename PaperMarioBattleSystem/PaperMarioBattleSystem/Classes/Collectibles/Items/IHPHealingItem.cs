using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// An interface for any Item that can restore HP.
    /// </summary>
    public interface IHPHealingItem
    {
        int HPRestored { get; }
    }
}
