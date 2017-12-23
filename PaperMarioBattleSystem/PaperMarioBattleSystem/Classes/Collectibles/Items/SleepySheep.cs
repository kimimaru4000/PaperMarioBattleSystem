using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Sleepy Sheep item. It inflicts Sleep on all enemies.
    /// </summary>
    public sealed class SleepySheep : BattleItem, IStatusInflictingItem
    {
        public StatusChanceHolder[] StatusesInflicted { get; private set; }

        public SleepySheep()
        {
            Name = "Sleepy Sheep";
            Description = "Targets all enemies and may cause them to fall asleep.";

            Icon = new CroppedTexture2D(AssetManager.Instance.LoadRawTexture2D($"{ContentGlobals.SpriteRoot}/Items.png"),
                new Rectangle(329, 163, 25, 25));
            ItemType = ItemTypes.Damage | ItemTypes.Status;

            StatusesInflicted = new StatusChanceHolder[] { new StatusChanceHolder(100d, new SleepStatus(2)) };

            SelectionType = TargetSelectionMenu.EntitySelectionType.All;
            MoveAffectionType = Enumerations.MoveAffectionTypes.Other;
            OtherEntTypes = new Enumerations.EntityTypes[] { Enumerations.EntityTypes.Enemy };
        }
    }
}
