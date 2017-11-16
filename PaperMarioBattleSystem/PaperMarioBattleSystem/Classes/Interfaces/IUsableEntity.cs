using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// An interface for a BattleEntity that is usable by another BattleEntity in some way.
    /// <para>Examples include Gulpits' Rocks being usable by Gulpits.</para>
    /// </summary>
    public interface IUsableEntity
    {
        int UsableValue { get; }
    }
}
