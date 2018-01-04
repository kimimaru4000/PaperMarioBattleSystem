using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// An interface for any object that can be rotated.
    /// </summary>
    public interface IRotatable
    {
        float Rotation { get; set; }
    }
}
