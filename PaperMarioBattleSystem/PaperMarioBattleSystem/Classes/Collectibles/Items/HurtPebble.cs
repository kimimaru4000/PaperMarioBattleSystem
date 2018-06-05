using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// A special Pebble item that deals 1 damage to the user. This appears only in PM's Mystery? list.
    /// </summary>
    public sealed class HurtPebble : BattleItem, IDamagingItem
    {
        public int Damage { get; private set; }

        public Enumerations.Elements Element { get; private set; }

        public HurtPebble()
        {
            Name = "Hurt Pebble";
            Description = "Damages the user for 1 damage.\nTHIS DESCRIPTION SHOULD NOT APPEAR!!";

            Icon = new CroppedTexture2D(AssetManager.Instance.LoadRawTexture2D($"{ContentGlobals.SpriteRoot}/Items.png"),
                new Rectangle(615, 87, 23, 21));
            ItemType = ItemTypes.Damage;

            Damage = 1;
            Element = Enumerations.Elements.Normal;

            SelectionType = Enumerations.EntitySelectionType.First;
            MoveAffectionType = Enumerations.MoveAffectionTypes.Self;
        }
    }
}
