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
    /// The Thunder Bolt item. It deals 5 Electric damage to an enemy.
    /// </summary>
    public class ThunderBolt : BattleItem, IDamagingItem
    {
        public int Damage { get; protected set; }

        public Elements Element { get; protected set; }

        public ThunderBolt()
        {
            Name = "Thunder Bolt";
            Description = "Deals 5 HP of damage to a single enemy.";

            Icon = new CroppedTexture2D(AssetManager.Instance.LoadRawTexture2D($"{ContentGlobals.SpriteRoot}/Items.png"),
                new Rectangle(429, 136, 25, 25));
            ItemType = ItemTypes.Damage;

            Damage = 5;
            Element = Elements.Electric;

            SelectionType = TargetSelectionMenu.EntitySelectionType.Single;
            MoveAffectionType = MoveAffectionTypes.Other;
            OtherEntTypes = new EntityTypes[] { EntityTypes.Enemy };
        }
    }
}
