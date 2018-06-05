using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Volt Shroom item. It inflicts Electrified for 5 turns on an ally.
    /// </summary>
    public sealed class VoltShroom : BattleItem, IStatusInflictingItem
    {
        public StatusChanceHolder[] StatusesInflicted { get; private set; }
        private const int ElectrifiedTurns = 5;

        public VoltShroom()
        {
            Name = "Volt Shroom";
            Description = "Electrifies you to damage\ndirect-attackers.";

            Icon = new CroppedTexture2D(AssetManager.Instance.LoadRawTexture2D($"{ContentGlobals.SpriteRoot}/Items.png"),
                new Rectangle(589, 7, 25, 23));
            ItemType = ItemTypes.Damage | ItemTypes.Status;

            StatusesInflicted = new StatusChanceHolder[] { new StatusChanceHolder(100d, new ElectrifiedStatus(ElectrifiedTurns)) };

            SelectionType = Enumerations.EntitySelectionType.Single;
            MoveAffectionType = Enumerations.MoveAffectionTypes.Self | Enumerations.MoveAffectionTypes.Ally;
        }
    }
}
