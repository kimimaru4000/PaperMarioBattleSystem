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
    /// The Tasty Tonic item. It removes a lot of statuses such as Poison and Tiny on Mario.
    /// </summary>
    public sealed class TastyTonic : BattleItem, IStatusHealingItem
    {
        public StatusTypes[] StatusesHealed { get; private set; }

        public TastyTonic()
        {
            Name = "Tasty Tonic";
            Description = "A very tasty tonic. Cures\npoisoning and shrinking.";

            Icon = new CroppedTexture2D(AssetManager.Instance.LoadRawTexture2D($"{ContentGlobals.SpriteRoot}/Items.png"),
                new Rectangle(325, 57, 21, 25));
            ItemType = ItemTypes.Healing;

            StatusesHealed = new StatusTypes[] { StatusTypes.Poison, StatusTypes.Tiny, StatusTypes.Allergic, StatusTypes.DEFDown,
                                                 StatusTypes.Dizzy, StatusTypes.Confused, StatusTypes.Frozen, StatusTypes.Burn,
                                                 StatusTypes.Slow, StatusTypes.Sleep, StatusTypes.Stop };

            SelectionType = TargetSelectionMenu.EntitySelectionType.Single;
            MoveAffectionType = Enumerations.MoveAffectionTypes.Self | Enumerations.MoveAffectionTypes.Ally;
        }
    }
}
