using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The most basic item. It restores 5 HP.
    /// </summary>
    public class Mushroom : BattleItem, IHPHealingItem
    {
        public int HPRestored { get; protected set; }

        public Mushroom()
        {
            Name = "Mushroom";
            Description = "Heals 5 HP.";

            Icon = new CroppedTexture2D(AssetManager.Instance.LoadRawTexture2D($"{ContentGlobals.SpriteRoot}/Items.png"),
                new Rectangle(537, 7, 25, 23));
            ItemType = ItemTypes.Healing;

            HPRestored = 5;

            SelectionType = TargetSelectionMenu.EntitySelectionType.Single;
            MoveAffectionType = Enumerations.MoveAffectionTypes.Self | Enumerations.MoveAffectionTypes.Ally;
        }
    }
}
