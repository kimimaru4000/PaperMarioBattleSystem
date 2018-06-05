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
    /// The Fire Flower item. It deals 3 Fire damage to all enemies.
    /// </summary>
    public sealed class FireFlower : BattleItem, IDamagingItem
    {
        public int Damage { get; private set; }

        public Elements Element { get; private set; }

        public FireFlower()
        {
            Name = "Fire Flower";
            Description = "Attacks all enemies with fireballs\nand burns them.";

            Icon = new CroppedTexture2D(AssetManager.Instance.LoadRawTexture2D($"{ContentGlobals.SpriteRoot}/Items.png"),
                new Rectangle(643, 31, 25, 25));
            ItemType = ItemTypes.Damage;

            Damage = 3;
            Element = Elements.Fire;

            SelectionType = Enumerations.EntitySelectionType.All;
            MoveAffectionType = MoveAffectionTypes.Other;
            OtherEntTypes = new EntityTypes[] { EntityTypes.Enemy };
        }
    }
}
