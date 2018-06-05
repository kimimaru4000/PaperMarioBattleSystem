using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using static PaperMarioBattleSystem.Enumerations;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Shooting Star item. It deals 6 Star damage to all enemies.
    /// </summary>
    public sealed class ShootingStar : BattleItem, IDamagingItem
    {
        public int Damage { get; private set; }

        public Elements Element { get; private set; }

        public ShootingStar()
        {
            Name = "Shooting Star";
            Description = "Deals 6 HP of damage to all enemies.";

            Icon = new CroppedTexture2D(AssetManager.Instance.LoadRawTexture2D($"{ContentGlobals.SpriteRoot}/Items.png"),
                new Rectangle(377, 139, 25, 23));
            ItemType = ItemTypes.Damage;

            Damage = 6;
            Element = Elements.Star;

            SelectionType = Enumerations.EntitySelectionType.All;
            MoveAffectionType = MoveAffectionTypes.Other;
            OtherEntTypes = new EntityTypes[] { EntityTypes.Enemy };
        }
    }
}
