using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// A Super Shroom. It restores 10 HP.
    /// </summary>
    public sealed class SuperShroom : Mushroom
    {
        public SuperShroom()
        {
            Name = "Super Shroom";
            Description = "A feel-super mushroom. Replenishes 10 HP.";

            Icon.SetRect(new Rectangle(615, 7, 25, 23));

            HPRestored = 10;
        }
    }
}
