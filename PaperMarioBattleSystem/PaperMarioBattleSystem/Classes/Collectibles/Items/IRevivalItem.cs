using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// An interface for any Item that can revive.
    /// </summary>
    public interface IRevivalItem
    {
        /// <summary>
        /// How much HP to restore when being revived.
        /// </summary>
        int RevivalHPRestored { get; }
    }
}
