using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Stone Cap item. It inflicts Stone for 3 turns on an ally.
    /// </summary>
    public sealed class StoneCap : BattleItem, IStatusInflictingItem
    {
        public StatusChanceHolder[] StatusesInflicted { get; private set; }
        private const int StoneTurns = 3;

        public override Sequence SequencePerformed => new StoneCapSequence(null);

        public StoneCap()
        {
            Name = "Stone Cap";
            Description = "Turns Mario to stone and makes\nhim unable to move for a while.";

            Icon = new CroppedTexture2D(AssetManager.Instance.LoadRawTexture2D($"{ContentGlobals.SpriteRoot}/Items.png"),
                new Rectangle(507, 142, 25, 19));
            ItemType = ItemTypes.Damage | ItemTypes.Status;

            StatusesInflicted = new StatusChanceHolder[] { new StatusChanceHolder(100d, new StoneStatus(StoneTurns)) };

            SelectionType = Enumerations.EntitySelectionType.Single;
            MoveAffectionType = Enumerations.MoveAffectionTypes.Self | Enumerations.MoveAffectionTypes.Ally;
        }
    }
}
