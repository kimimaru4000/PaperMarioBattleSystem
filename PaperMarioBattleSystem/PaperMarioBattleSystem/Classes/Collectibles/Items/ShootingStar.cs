using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PaperMarioBattleSystem.Enumerations;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Shooting Star item. It deals 6 Star damage to all enemies.
    /// </summary>
    public sealed class ShootingStar : Item, IDamagingItem
    {
        public int Damage { get; private set; }

        public Elements Element { get; private set; }

        public ShootingStar()
        {
            Name = "Shooting Star";
            Description = "Deals 6 HP of damage to all enemies.";

            ItemType = ItemTypes.Damage;

            Damage = 6;
            Element = Elements.Star;
        }
    }
}
