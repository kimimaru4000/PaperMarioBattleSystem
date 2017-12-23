using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The most basic FP restoring item. It restores 5 FP.
    /// </summary>
    public class HoneySyrup : BattleItem, IFPHealingItem
    {
        public int FPRestored { get; protected set; }

        public HoneySyrup()
        {
            Name = "Honey Syrup";
            Description = "Restores 5 FP.";
            Icon = new CroppedTexture2D(AssetManager.Instance.LoadRawTexture2D($"{ContentGlobals.SpriteRoot}/Items.png"),
                new Rectangle(373, 57, 25, 25));
            ItemType = ItemTypes.Healing;

            FPRestored = 5;

            SelectionType = TargetSelectionMenu.EntitySelectionType.Single;
            MoveAffectionType = Enumerations.MoveAffectionTypes.Self | Enumerations.MoveAffectionTypes.Ally;
        }
    }
}
