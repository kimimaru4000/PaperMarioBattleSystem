using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Mystery item. It uses a random item.
    /// <para>This chooses the set of random items from PM:
    /// { Mushroom,
    /// Super Shroom,
    /// Fire Flower,
    /// Stone Cap,
    /// Dizzy Dial,
    /// Thunder Rage,
    /// Pebble (falls down on Mario) }
    /// </para>
    /// <para>If an item in question cannot be used (Ex. dark battles), it is removed from the roulette!</para>
    /// </summary>
    public sealed class Mystery : BattleItem
    {
        public override ItemAction ActionAssociated => new MysteryAction(this);

        public override Sequence SequencePerformed => new MysterySequence(null);

        /// <summary>
        /// The set of battle items that Mystery can choose from.
        /// <para>This returns a new array with new instances of each battle item.</para>
        /// </summary>
        public BattleItem[] GetItemSet()
        {
            //NOTE: Not all of these items are in the PM set; this is just for testing until we get them in
            return new BattleItem[] { new Mushroom(), new SuperShroom(), new FireFlower(), new StoneCap(), new DizzyDial(), new ThunderRage(), new HurtPebble() };
        }

        public Mystery()
        {
            Name = "Mystery";
            Description = "Who knows what it does... Take a\nchance to find out!";

            Texture2D icon = AssetManager.Instance.LoadRawTexture2D($"{ContentGlobals.SpriteRoot}/Items.png");
            Icon = new CroppedTexture2D(icon, new Rectangle(639, 83, 25, 25));

            //Classify Mystery as any type of battle item so it shows up in the item menu
            ItemType = ItemTypes.Damage | ItemTypes.Healing | ItemTypes.Status;
            
            SelectionType = TargetSelectionMenu.EntitySelectionType.Single;
            MoveAffectionType = Enumerations.MoveAffectionTypes.Self;
        }
    }
}
