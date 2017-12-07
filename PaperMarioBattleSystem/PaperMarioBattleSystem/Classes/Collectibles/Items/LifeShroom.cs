using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// A Life Shroom. It revives a BattleEntity that dies, healing 10 HP.
    /// </summary>
    public class LifeShroom : Mushroom, IRevivalItem
    {
        public int RevivalHPRestored { get; protected set; }

        public LifeShroom()
        {
            Name = "Life Shroom";
            Description = "Restores 10 HP when Mario or his partner falls.";

            HPRestored = 10;
            RevivalHPRestored = 10;

            ItemType |= ItemTypes.Revival;
        }
    }
}
