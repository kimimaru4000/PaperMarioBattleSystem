using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Dizzy Dial item. It inflicts enemies with Dizzy.
    /// </summary>
    public sealed class DizzyDial : BattleItem, IStatusInflictingItem
    {
        public StatusChanceHolder[] StatusesInflicted { get; private set; }

        public DizzyDial()
        {
            Name = "Dizzy Dial";
            Description = "If it works, dazes and\nparalyzes all enemies briefly.";

            Icon = new CroppedTexture2D(AssetManager.Instance.LoadRawTexture2D($"{ContentGlobals.SpriteRoot}/Items.png"),
                new Rectangle(355, 165, 23, 23));
            ItemType = ItemTypes.Status;

            StatusesInflicted = new StatusChanceHolder[] { new StatusChanceHolder(100d, new DizzyStatus(3)) };

            MoveAffectionType = Enumerations.MoveAffectionTypes.Other;
            OtherEntTypes = new Enumerations.EntityTypes[] { Enumerations.EntityTypes.Enemy };
        }
    }
}
